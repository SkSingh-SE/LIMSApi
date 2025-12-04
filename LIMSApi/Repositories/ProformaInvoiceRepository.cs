
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
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
        public async Task<long> GeneratePIAsync(long inwardId, bool applyGST, bool isInterState)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. PREVENT DUPLICATE PI
                if (await _context.ProformaInvoiceHeader.AnyAsync(x => x.InwardID == inwardId))
                    throw new Exception("PI already generated.");

                // 2. VALIDATE PREPARATION COMPLETED
                var prepRequired = await _context.SampleDetails
                    .CountAsync(x => x.InwardID == inwardId && x.PreparationRequired);

                var completed = await _context.CuttingChargeSamples
                    .Where(x => x.CuttingChargeHeader.InwardID == inwardId)
                    .CountAsync();

                if (prepRequired > completed)
                    throw new Exception("Sample preparation is not fully completed.");

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
                                decimal usedValue = method.Quantity; // HOURS / LOAD / etc

                                var (rate, configId) = await GetRateBySelectionAsync(
                                    method.TestMethodID,
                                    method.SelectionType,
                                    method.Value.Value
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
                                    SelectionType = method.SelectionType,
                                    UsedValue = method.Value.Value,
                                    InvoiceCaseConfigID = configId
                                });
                            }
                        }

                        // ---------------- CHEMICAL TESTS ----------------
                        foreach (var ct in plan.ChemicalTests)
                        {
                            var usedElements = ct.Elements.Count(x => x.Selected);

                            var (rate, configId) = await GetRateBySelectionAsync(
                                ct.TestMethod,
                                "Element",
                                usedElements
                            );

                            var amount = rate;
                            totalTestAmount += amount;

                            piTestDetails.Add(new ProformaInvoiceDetail
                            {
                                SampleID = sd.ID,
                                ChargeType = "ChemicalTest",
                                Description = "Chemical Test",
                                Quantity = 1,
                                Rate = rate,
                                Amount = amount,
                                SelectionType = "Element",
                                UsedValue = usedElements,
                                InvoiceCaseConfigID = configId
                            });
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
                    if (isInterState)
                        igst = subTotal * 0.18m;
                    else
                    {
                        cgst = subTotal * 0.09m;
                        sgst = subTotal * 0.09m;
                    }
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
                await tx.CommitAsync();

                return piHeader.ID;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }


        private async Task<(decimal Rate, long ConfigId)> GetRateBySelectionAsync(long testMethodId, string selectionType, decimal usedValue)
        {
            // 1️ Get Invoice Case for this Lab Test
            var invoiceCase = await _context.InvoiceCases
                .Where(x => x.LaboratoryTestID == testMethodId && x.IsActive)
                .Include(x => x.InvoiceCasePrices)
                .FirstOrDefaultAsync();

            if (invoiceCase == null)
                throw new Exception($"No invoice case found for TestMethodId {testMethodId}");

            // 2️ Get all related Configurations used in this Invoice Case
            var configIds = invoiceCase.InvoiceCasePrices
                .Select(x => x.InvoiceCaseConfigID)
                .ToList();

            var configs = await _context.InvoiceCaseConfigurations
                .Where(c => configIds.Contains(c.ID)
                            && c.SelectionType == selectionType
                            && c.IsActive)
                .ToListAsync();

            if (!configs.Any())
                throw new Exception($"No pricing configuration found for SelectionType {selectionType}");

            // 3️ Select the nearest higher or equal slab
            var selectedConfig = configs
                .Where(c => decimal.Parse(c.Value) >= usedValue)
                .OrderBy(c => decimal.Parse(c.Value))
                .FirstOrDefault();

            if (selectedConfig == null)
                throw new Exception($"No pricing slab found for value {usedValue} under {selectionType}");

            // 4️ Get Price for the selected slab
            var selectedPrice = invoiceCase.InvoiceCasePrices
                .Where(p => p.InvoiceCaseConfigID == selectedConfig.ID)
                .Select(p => p.Price)
                .FirstOrDefault();

            return (selectedPrice, selectedConfig.ID);
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

                var model = new ProformaInvoicePdfModel
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
                    model.Rows.Add(new ProformaInvoicePdfRow
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
            catch (Exception ex)
            {
                throw ex;
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
