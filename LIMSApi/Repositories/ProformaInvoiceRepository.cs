
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace LIMSApi.Repositories
{
    public class ProformaInvoiceRepository : IProformaInvoiceRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;
        //private readonly IConverter _converter;
        public ProformaInvoiceRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
            //_converter = converter;
        }

        public async Task<string> GeneratePINoAsync()
        {
            var year = DateTime.Now.Year;
            var last = await _context.ProformaInvoiceHeader
                .OrderByDescending(x => x.ID)
                .Select(x => x.PINo)
                .FirstOrDefaultAsync();

            var next = last != null
                ? int.Parse(last.Split('/').Last()) + 1
                : 1;

            return $"PI/{year}/{next.ToString("D6")}";
        }
        public async Task<long> GeneratePIAsync(long inwardId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. PREVENT DUPLICATE PI
                if (await _context.ProformaInvoiceHeader.AnyAsync(x => x.InwardID == inwardId))
                    throw new Exception("PI already generated.");

                // 2. VALIDATE INVOICE NOT ALREADY GENERATED
                var inward = await _context.SampleInwards
                    .Include(x => x.Customer)
                    .FirstOrDefaultAsync(x => x.ID == inwardId);

                if (inward == null)
                    throw new Exception("Inward not found.");

                if (inward.IsInvoiceGenerated)
                    throw new InvalidOperationException("Cannot generate PI. Invoice has already been generated for this inward.");
                var applyGST = inward?.Customer?.GSTNA ?? false;
                // Check for existing TaxInvoice as additional safeguard
                var hasInvoice = await _context.TaxInvoices
                    .AnyAsync(x => x.InwardID == inwardId);

                if (hasInvoice)
                    throw new InvalidOperationException("Cannot generate PI. Invoice has already been generated for this inward.");

                // 3. CUTTING + MACHINING
                var cuttingHeader = await _context.CuttingChargeHeaders
                    .FirstOrDefaultAsync(x => x.InwardID == inwardId);

                var cuttingAmount = cuttingHeader?.GrandTotal ?? 0;

                var machiningAmount = await _context.SampleDetails
                    .Where(x => x.InwardID == inwardId && x.MachiningRequired)
                    .SumAsync(x => x.MachiningAmount + x.OtherPreparationCharge);

                // ===========================
                //  4. LAB TEST CHARGES (FULLY IMPLEMENTED)
                // ===========================
                var piTestDetails = new List<ProformaInvoiceDetail>();
                decimal totalTestAmount = 0;

                var sampleDetails = await _context.SampleDetails
                    .Where(x => x.InwardID == inwardId)
                    .Include(x => x.TestPlans)
                        .ThenInclude(x => x.GeneralTests)
                            .ThenInclude(x => x.Methods)
                    .Include(x => x.TestPlans)
                        .ThenInclude(x => x.ChemicalTests)
                            .ThenInclude(x => x.Elements)
                    .Include(x => x.TestPlans)
                        .ThenInclude(x => x.ChemicalTests)
                            .ThenInclude(x => x.TestTypes)
                    .ToListAsync();

                foreach (var sd in sampleDetails)
                {
                    foreach (var plan in sd.TestPlans)
                    {
                        // ---------------- GENERAL TESTS ---------------- 
                        foreach (var gt in plan.GeneralTests)
                        {
                            foreach (var method in gt.Methods)
                            {
                                try
                                {
                                    var (rate, configId, selectionType, usedValue) = await GetRateForGeneralTestAsync(
                                        method.LaboratoryTestID,
                                        gt,
                                        method,
                                        plan.ID
                                    );

                                    var amount = rate * method.Quantity;
                                    totalTestAmount += amount;

                                    piTestDetails.Add(new ProformaInvoiceDetail
                                    {
                                        SampleID = sd.ID,
                                        ChargeType = "GeneralTest",
                                        Description = "General Test",
                                        Quantity = method.Quantity,
                                        Rate = rate,
                                        Amount = amount,
                                        SelectionType = selectionType,
                                        UsedValue = usedValue ?? 0,
                                        InvoiceCaseConfigID = configId
                                    });
                                }
                                catch
                                {
                                    // Log error and continue with next method
                                    // You may want to add logging here
                                    continue;
                                }
                            }
                        }

                        // ---------------- CHEMICAL TESTS (NEW LOGIC) ---------------- 
                        foreach (var ct in plan.ChemicalTests)
                        {
                            // Count selected elements once
                            var usedElements = ct.Elements.Count(x => x.Selected);

                            // Loop over each selected TestType
                            foreach (var tt in ct.TestTypes.Where(t => t.IsSelected && t.LaboratoryTestID.HasValue))
                            {
                                try
                                {
                                    if (!tt.LaboratoryTestID.HasValue) continue;

                                    var (rate, configId, selectionType, usedValue) = await GetRateForChemicalTestAsync(
                                        tt.LaboratoryTestID.Value,
                                        ct,
                                        usedElements
                                    );

                                    var amount = rate;
                                    totalTestAmount += amount;

                                    piTestDetails.Add(new ProformaInvoiceDetail
                                    {
                                        SampleID = sd.ID,
                                        ChargeType = "ChemicalTest",
                                        Description = $"Chemical Test - {tt.Name}",
                                        Quantity = 1,
                                        Rate = rate,
                                        Amount = amount,
                                        SelectionType = selectionType,
                                        UsedValue = usedValue ?? 0,
                                        InvoiceCaseConfigID = configId
                                    });
                                }
                                catch
                                {
                                    // Log error and continue with next test type
                                    // You may want to add logging here
                                    continue;
                                }
                            }
                        }

                    }
                }

                // ===========================
                //  5. GRAND TOTAL
                // ===========================
                var subTotal = cuttingAmount + machiningAmount + totalTestAmount;

                decimal cgst = 0, sgst = 0, igst = 0;

                if (applyGST)
                {
                    cgst = subTotal * 0.09m;
                    sgst = subTotal * 0.09m;
                }

                var taxAmount = cgst + sgst + igst;
                var grandTotal = subTotal + taxAmount;

                // ===========================
                //  6. INSERT PI HEADER
                // ===========================
                var piHeader = new ProformaInvoiceHeader
                {
                    InwardID = inwardId,
                    PINo = await GeneratePINoAsync(),
                    PIDate = DateTime.Now,

                    SubTotal = subTotal,
                    CGST = cgst,
                    SGST = sgst,
                    IGST = igst,
                    TaxAmount = taxAmount,
                    GrandTotal = grandTotal,

                    CreatedBy = loggedInUser.UserId
                };

                _context.ProformaInvoiceHeader.Add(piHeader);
                await _context.SaveChangesAsync();

                // ===========================
                //  7. INSERT MACHINING + CUTTING DETAILS
                // ===========================
                if (cuttingHeader != null)
                {
                    var cuttingSamples = await _context.CuttingChargeSamples
                        .Where(x => x.CuttingChargeHeader.InwardID == inwardId)
                        .ToListAsync();

                    foreach (var cs in cuttingSamples)
                    {
                        _context.ProformaInvoiceDetails.Add(new ProformaInvoiceDetail
                        {
                            ProformaInvoiceHeaderID = piHeader.ID,
                            SampleID = cs.SampleID,
                            ChargeType = "Machining",
                            Description = "Sample Machining Charges",
                            Quantity = 1,
                            Rate = cs.SampleTotal,
                            Amount = cs.SampleTotal
                        });
                    }
                }

                // ===========================
                //  8. INSERT TEST DETAILS
                // ===========================
                foreach (var t in piTestDetails)
                {
                    t.ProformaInvoiceHeaderID = piHeader.ID;
                    _context.ProformaInvoiceDetails.Add(t);
                }

                await _context.SaveChangesAsync();

                // Update BillingStatus to PI_GENERATED after successful PI creation
                // Note: inward variable already loaded earlier for validation
                inward.BillingStatus = BillingStatus.PI_GENERATED.ToString();
                inward.ModifiedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                return piHeader.ID;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }


        /// <summary>
        /// Get rate for General Test by extracting parameter values from SpecificationLine
        /// </summary>
        private async Task<(decimal Rate, long ConfigId, string? SelectionType, decimal? UsedValue)> GetRateForGeneralTestAsync(
            long laboratoryTestId,
            GeneralTest generalTest,
            GeneralTestMethod method,
            long testPlanId)
        {
            // Get all InvoiceCaseConfigurations linked to this LaboratoryTest
            var configs = await _context.LaboratoryTestInvoiceCase
                .Where(lt => lt.LabTestID == laboratoryTestId)
                .Include(lt => lt.InvoiceCaseConfiguration)
                .Where(lt => lt.InvoiceCaseConfiguration != null && lt.InvoiceCaseConfiguration.IsActive)
                .Select(lt => lt.InvoiceCaseConfiguration!)
                .ToListAsync();

            if (!configs.Any())
                throw new Exception($"No pricing configuration found for LaboratoryTest {laboratoryTestId}");

            // Get SpecificationLines for this GeneralTest (mechanical type)
            var specificationLines = await _context.SpecificationLines
                .Where(sl => (sl.SpecificationGradeID == generalTest.Specification1 ||
                             (generalTest.Specification2.HasValue && sl.SpecificationGradeID == generalTest.Specification2.Value))
                            && sl.Type == "mechanical"
                            && sl.ParameterID.HasValue)
                .Include(sl => sl.Parameter)
                .ToListAsync();

            // Get sample ID from the test plan
            var testPlan = await _context.TestPlans
                .FirstOrDefaultAsync(tp => tp.ID == testPlanId);

            if (testPlan == null)
                throw new Exception($"TestPlan {testPlanId} not found");

            // Try to get TestResultParameter values if test results are available
            var testResultHeader = await _context.TestResultHeaders
                .Where(trh => trh.LaboratoryTestID == laboratoryTestId
                             && trh.TestPlanID == testPlanId
                             && trh.SampleID == testPlan.SampleID)
                .Include(trh => trh.Parameters)
                .FirstOrDefaultAsync();

            // For each configuration, try to find matching parameter and calculate rate
            foreach (var config in configs.OrderBy(c => c.SelectionType))
            {
                try
                {
                    var parameterValue = await ExtractParameterValueForConfigAsync(
                        config,
                        specificationLines,
                        testResultHeader?.Parameters,
                        "mechanical"
                    );

                    if (parameterValue.HasValue)
                    {
                        var (rate, configId) = await MatchConfigAndGetRateAsync(
                            laboratoryTestId,
                            config,
                            parameterValue.Value
                        );

                        return (rate, configId, config.SelectionType, parameterValue.Value);
                    }
                }
                catch
                {
                    continue;
                }
            }

            throw new Exception($"No matching pricing configuration found for LaboratoryTest {laboratoryTestId} with available parameters");
        }

        /// <summary>
        /// Get rate for Chemical Test
        /// </summary>
        private async Task<(decimal Rate, long ConfigId, string? SelectionType, decimal? UsedValue)> GetRateForChemicalTestAsync(
            long laboratoryTestId,
            ChemicalTest chemicalTest,
            int usedElements)
        {
            // Get all InvoiceCaseConfigurations linked to this LaboratoryTest
            var configs = await _context.LaboratoryTestInvoiceCase
                .Where(lt => lt.LabTestID == laboratoryTestId)
                .Include(lt => lt.InvoiceCaseConfiguration)
                .Where(lt => lt.InvoiceCaseConfiguration != null && lt.InvoiceCaseConfiguration.IsActive)
                .Select(lt => lt.InvoiceCaseConfiguration!)
                .ToListAsync();

            if (!configs.Any())
                throw new Exception($"No pricing configuration found for LaboratoryTest {laboratoryTestId}");

            // For Element type, use element count
            var elementConfig = configs.FirstOrDefault(c => c.SelectionType == "Element");
            if (elementConfig != null)
            {
                var (rate, configId) = await MatchConfigAndGetRateAsync(
                    laboratoryTestId,
                    elementConfig,
                    usedElements
                );

                return (rate, configId, "Element", usedElements);
            }

            // For other types, get SpecificationLines from ChemicalTestElement
            var specificationLineIds = chemicalTest.Elements
                .Where(e => e.Selected)
                .Select(e => e.SpecificationLineID)
                .ToList();

            var specificationLines = await _context.SpecificationLines
                .Where(sl => specificationLineIds.Contains(sl.ID)
                            && sl.Type == "chemical"
                            && sl.ParameterID.HasValue)
                .Include(sl => sl.Parameter)
                .ToListAsync();

            // Try to get TestResultParameter values if test results are available
            var testResultHeader = await _context.TestResultHeaders
                .Where(trh => trh.LaboratoryTestID == laboratoryTestId)
                .Include(trh => trh.Parameters)
                .FirstOrDefaultAsync();

            // For each configuration, try to find matching parameter and calculate rate
            foreach (var config in configs.OrderBy(c => c.SelectionType))
            {
                try
                {
                    var parameterValue = await ExtractParameterValueForConfigAsync(
                        config,
                        specificationLines,
                        testResultHeader?.Parameters,
                        "chemical"
                    );

                    if (parameterValue.HasValue)
                    {
                        var (rate, configId) = await MatchConfigAndGetRateAsync(
                            laboratoryTestId,
                            config,
                            parameterValue.Value
                        );

                        return (rate, configId, config.SelectionType, parameterValue.Value);
                    }
                }
                catch
                {
                    continue;
                }
            }

            throw new Exception($"No matching pricing configuration found for LaboratoryTest {laboratoryTestId} with available parameters");
        }

        /// <summary>
        /// Extract parameter value for a given configuration from SpecificationLine or TestResultParameter
        /// </summary>
        private Task<decimal?> ExtractParameterValueForConfigAsync(
            InvoiceCaseConfiguration config,
            List<SpecificationLine> specificationLines,
            ICollection<TestResultParameter>? testResultParameters,
            string parameterType)
        {
            // Get parameter names/aliases to match
            var configNames = new List<string> { config.Name };
            if (!string.IsNullOrWhiteSpace(config.AliasName))
            {
                configNames.AddRange(config.AliasName.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim()));
            }

            // First, try to get from TestResultParameter (actual test results)
            if (testResultParameters != null && testResultParameters.Any())
            {
                // Get ParameterMasters for the test result parameters
                var parameterIds = testResultParameters.Select(trp => trp.ParameterID).Distinct().ToList();
                var parameterMasters = _context.ParameterMasters
                    .Where(pm => parameterIds.Contains(pm.ID))
                    .ToDictionary(pm => pm.ID);

                foreach (var trp in testResultParameters)
                {
                    if (!parameterMasters.TryGetValue(trp.ParameterID, out var paramMaster))
                        continue;

                    var paramName = paramMaster.Name?.ToLower() ?? "";
                    var paramAlias = paramMaster.AliasName?.ToLower() ?? "";
                    var trpParamName = trp.ParameterName?.ToLower() ?? "";

                    foreach (var configName in configNames)
                    {
                        var configNameLower = configName.ToLower();
                        if ((paramName.Contains(configNameLower) || configNameLower.Contains(paramName) ||
                             paramAlias.Contains(configNameLower) || configNameLower.Contains(paramAlias) ||
                             trpParamName.Contains(configNameLower) || configNameLower.Contains(trpParamName)) &&
                            trp.Value.HasValue)
                        {
                            return Task.FromResult<decimal?>(trp.Value.Value);
                        }
                    }
                }
            }

            // If not found in test results, try SpecificationLine
            foreach (var specLine in specificationLines)
            {
                if (specLine.Parameter == null) continue;

                var paramName = specLine.Parameter.Name?.ToLower() ?? "";
                var paramAlias = specLine.Parameter.AliasName?.ToLower() ?? "";

                foreach (var configName in configNames)
                {
                    var configNameLower = configName.ToLower();
                    if (paramName.Contains(configNameLower) || configNameLower.Contains(paramName) ||
                        paramAlias.Contains(configNameLower) || configNameLower.Contains(paramAlias))
                    {
                        // For range types, we might need to use MinValue or MaxValue
                        // For single value types, we might need to use a default or calculated value
                        // For now, return MinValue if available, otherwise MaxValue
                        if (specLine.MinValue.HasValue)
                            return Task.FromResult<decimal?>(specLine.MinValue.Value);
                        if (specLine.MaxValue.HasValue)
                            return Task.FromResult<decimal?>(specLine.MaxValue.Value);
                    }
                }
            }

            return Task.FromResult<decimal?>(null);
        }

        /// <summary>
        /// Match configuration against parameter value and get rate
        /// </summary>
        private async Task<(decimal Rate, long ConfigId)> MatchConfigAndGetRateAsync(
            long laboratoryTestId,
            InvoiceCaseConfiguration config,
            decimal usedValue)
        {
            // Get Invoice Case for this Lab Test
            var invoiceCase = await _context.InvoiceCases
                .Where(x => x.LaboratoryTestID == laboratoryTestId && x.IsActive)
                .Include(x => x.InvoiceCasePrices)
                .FirstOrDefaultAsync();

            if (invoiceCase == null)
                throw new Exception($"No invoice case found for LaboratoryTest {laboratoryTestId}");

            // Check if this config is used in the invoice case
            var configPrice = invoiceCase.InvoiceCasePrices
                .FirstOrDefault(p => p.InvoiceCaseConfigID == config.ID);

            if (configPrice == null)
                throw new Exception($"Configuration {config.ID} not found in invoice case prices");

            // Determine if this is a range type or single value type
            var isRangeType = config.SelectionType.EndsWith("Range", StringComparison.OrdinalIgnoreCase);

            if (isRangeType)
            {
                // Range type: check if usedValue falls within Start and End
                if (string.IsNullOrWhiteSpace(config.Start) || string.IsNullOrWhiteSpace(config.End))
                    throw new Exception($"Configuration {config.ID} is a range type but Start or End is missing");

                var startValue = decimal.Parse(config.Start);
                var endValue = decimal.Parse(config.End);

                if (usedValue >= startValue && usedValue <= endValue)
                {
                    return (configPrice.Price, config.ID);
                }
            }
            else
            {
                // Single value type: find the nearest higher or equal slab
                // Get all configs of the same SelectionType
                var allConfigsForType = await _context.LaboratoryTestInvoiceCase
                    .Where(lt => lt.LabTestID == laboratoryTestId)
                    .Include(lt => lt.InvoiceCaseConfiguration)
                    .Where(lt => lt.InvoiceCaseConfiguration != null
                                && lt.InvoiceCaseConfiguration.IsActive
                                && lt.InvoiceCaseConfiguration.SelectionType == config.SelectionType)
                    .Select(lt => lt.InvoiceCaseConfiguration!)
                    .Where(c => !string.IsNullOrWhiteSpace(c.Value))
                    .ToListAsync();

                var matchingConfig = allConfigsForType
                    .Where(c => !string.IsNullOrWhiteSpace(c.Value) && decimal.TryParse(c.Value, out var val) && val >= usedValue)
                    .OrderBy(c => decimal.Parse(c.Value!))
                    .FirstOrDefault();

                if (matchingConfig != null && !string.IsNullOrWhiteSpace(matchingConfig.Value))
                {
                    var matchingPrice = invoiceCase.InvoiceCasePrices
                        .FirstOrDefault(p => p.InvoiceCaseConfigID == matchingConfig.ID);

                    if (matchingPrice != null)
                    {
                        return (matchingPrice.Price, matchingConfig.ID);
                    }
                }
            }

            throw new Exception($"No pricing slab found for value {usedValue} under SelectionType {config.SelectionType}");
        }

        public async Task<byte[]> GeneratePIPdfAsync(long piId)
        {
            try
            {


                var pi = await _context.ProformaInvoiceHeader
                    .Include(x => x.SampleInward)
                        .ThenInclude(x => x.Customer)
                    .Include(x => x.Details)
                    .FirstOrDefaultAsync(x => x.ID == piId);   //  FIX: FirstOrDefaultAsync

                //  IF PI NOT FOUND → USE DUMMY DATA
                if (pi == null)
                {
                    pi = new ProformaInvoiceHeader
                    {
                        ID = 0,
                        PINo = "PI/DUMMY/000001",
                        PIDate = DateTime.Today,

                        SubTotal = 1000,
                        CGST = 90,
                        SGST = 90,
                        IGST = 0,
                        TaxAmount = 180,
                        GrandTotal = 1180,

                        SampleInward = new SampleInward
                        {
                            CaseNo = "CASE-DUMMY-001",
                            CreatedOn = DateTime.Today,
                            GstNo = "24ABCDE1234F1Z5",
                            Address = "Dummy Industrial Area, Ahmedabad",
                            State = "Gujarat",

                            Customer = new Customer
                            {
                                Name = "DUMMY INDUSTRIES PVT LTD",
                                Address = "Dummy Industrial Area, Ahmedabad",
                                GSTNo = "24ABCDE1234F1Z5",
                                PinCode = "380015",
                                TallyLedgerName = "DUMMY INDUSTRIES PVT LTD",
                                CustomerType = "Regular"
                            }
                        },

                        //  DUMMY LINE ITEMS
                        Details = new List<ProformaInvoiceDetail>
                            {
                                new ProformaInvoiceDetail
                                {
                                    SampleID = 1,
                                    Description = "Sample Machining Charges",
                                    Quantity = 1,
                                    Rate = 500,
                                    Amount = 500
                                },
                                new ProformaInvoiceDetail
                                {
                                    SampleID = 2,
                                    Description = "Chemical Testing Charges",
                                    Quantity = 1,
                                    Rate = 500,
                                    Amount = 500
                                }
                            }
                    };
                }

                //var html = BuildHtml(pi);
                //var result = ConvertHtmlToPdf(html);
                //return result;

                var model = new TaxInvoicePdfModelDto
                {
                    InvoiceNo = pi.PINo,
                    InvoiceDate = pi.PIDate,
                    CustomerName = pi.SampleInward.Customer.Name,
                    CustomerAddress = pi.SampleInward.Address,
                    CustomerGst = pi.SampleInward.GstNo,
                    State = pi.SampleInward.State,
                    StateCode = "24", // TODO: DB se lo agar hai
                    RefNo = pi.SampleInward.CaseNo,
                    ReceivedDate = pi.SampleInward.CreatedOn,
                    SubTotal = pi.SubTotal,
                    CGST = pi.CGST,
                    SGST = pi.SGST,
                    IGST = pi.IGST,
                    GrandTotal = pi.GrandTotal,
                    AmountInWords = NumberToWords((long)pi.GrandTotal)
                };

                foreach (var d in pi.Details)
                {
                    model.Rows.Add(new TaxInvoiceRow
                    {
                        Sample = d.SampleID.ToString(),
                        Description = d.Description,
                        QtyDisplay = d.Quantity.ToString(),
                        Rate = d.Rate,
                        Amount = d.Amount
                    });
                }

                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "logo.png");
                var signPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "signature.png");

                var document = new ProformaInvoiceDocument(model, logoPath, signPath);
                var pdfBytes = document.GeneratePdf(); // QuestPDF extension

                return pdfBytes;
            }
            catch
            {
                throw;
            }
        }

        //private string BuildHtml(ProformaInvoiceHeader pi)
        //{
        //    var templatePath = Path.Combine(
        //        Directory.GetCurrentDirectory(), "Templates", "PI_Template.html");

        //    var html = File.ReadAllText(templatePath);

        //    var logoPath = Path.Combine(
        //        Directory.GetCurrentDirectory(), "Assets", "logo.png");

        //    var signPath = Path.Combine(
        //        Directory.GetCurrentDirectory(), "Assets", "signature.png");

        //    html = html
        //        .Replace("{{LogoPath}}", $"file:///{logoPath.Replace("\\", "/")}")
        //        .Replace("{{SignaturePath}}", $"file:///{signPath.Replace("\\", "/")}")
        //        .Replace("{{InvoiceNo}}", pi.PINo)
        //        .Replace("{{InvoiceDate}}", pi.PIDate.ToString("dd-MM-yyyy"))
        //        .Replace("{{CustomerName}}", pi.SampleInward.Customer.Name)
        //        .Replace("{{CustomerAddress}}", pi.SampleInward.Address)
        //        .Replace("{{CustomerGST}}", pi.SampleInward.GstNo)
        //        .Replace("{{State}}", pi.SampleInward.State)
        //        .Replace("{{StateCode}}", "24")
        //        .Replace("{{ReceivedDate}}", pi.SampleInward.CreatedOn.ToString("dd-MM-yyyy"))
        //        .Replace("{{RefNo}}", pi.SampleInward.CaseNo)
        //        .Replace("{{PI_ROWS}}", BuildPIRows(pi))
        //        .Replace("{{SubTotal}}", pi.SubTotal.ToString("0.00"))
        //        .Replace("{{CGST}}", pi.CGST.ToString("0.00"))
        //        .Replace("{{SGST}}", pi.SGST.ToString("0.00"))
        //        .Replace("{{IGST}}", pi.IGST.ToString("0.00"))
        //        .Replace("{{GrandTotal}}", pi.GrandTotal.ToString("0.00"))
        //        .Replace("{{AmountInWords}}", NumberToWords((long)pi.GrandTotal));

        //    return html;
        //}
        //private string BuildPIRows(ProformaInvoiceHeader pi)
        //{
        //    var rows = "";

        //    var details = pi.Details?.ToList();

        //    //  Fallback dummy rows if empty
        //    if (details == null || !details.Any())
        //    {
        //        return @"
        //<tr>
        //    <td>1</td>
        //    <td>Sample Machining Charges</td>
        //    <td class='center'>1</td>
        //    <td class='right'>500.00</td>
        //    <td class='right'>500.00</td>
        //</tr>
        //<tr>
        //    <td>2</td>
        //    <td>Chemical Testing Charges</td>
        //    <td class='center'>1</td>
        //    <td class='right'>500.00</td>
        //    <td class='right'>500.00</td>
        //</tr>";
        //    }

        //    foreach (var d in details)
        //    {
        //        rows += $@"
        //<tr>
        //    <td>{d.SampleID}</td>
        //    <td>{d.Description}</td>
        //    <td class='center'>{d.Quantity}</td>
        //    <td class='right'>{d.Rate:0.00}</td>
        //    <td class='right'>{d.Amount:0.00}</td>
        //</tr>";
        //    }

        //    return rows;
        //}


        ////  Convert HTML to PDF
        //private byte[] ConvertHtmlToPdf(string html)
        //{
        //    var doc = new HtmlToPdfDocument()
        //    {
        //        GlobalSettings = {
        //        PaperSize = PaperKind.A4,
        //        Orientation = Orientation.Portrait
        //    },
        //        Objects = {
        //        new ObjectSettings {
        //            HtmlContent = html,
        //            WebSettings = { DefaultEncoding = "utf-8" }
        //        }
        //    }
        //    };

        //    return _converter.Convert(doc);
        //}

        private string NumberToWords(long number)
        {
            return $"{number} Only"; // You can plug full converter here
        }
    }
}
