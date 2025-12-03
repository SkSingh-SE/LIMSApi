using System.Xml.Linq;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Migrations;
using LIMSApi.Models;
using LIMSApi.Repositories;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;

namespace LIMSApi.Services
{
    public class SampleInwardService : ISampleInwardService
    {
        private readonly ISampleInwardRepository _SampleInwardRepository;
        private readonly ILogger<SampleInwardService> _logger;
        private readonly IFileUploadService _uploadService;
        private readonly IWorkflowService _workflowService;
        private LoggedInUserDTO loggedInUser;
        private readonly ISampleStatusService _sampleStatusService;
        private readonly IProformaInvoiceRepository _proformaInvoiceRepository;

        public SampleInwardService(ISampleInwardRepository SampleInwardRepo, ILogger<SampleInwardService> logger, IFileUploadService uploadService, IWorkflowService workflowService, ISampleStatusService sampleStatusService, IProformaInvoiceRepository proformaInvoiceRepository)
        {
            _SampleInwardRepository = SampleInwardRepo;
            _logger = logger;
            _uploadService = uploadService;
            _workflowService = workflowService;
            loggedInUser = LoggedInUserProvider.CurrentUser;
            _sampleStatusService = sampleStatusService;
            _proformaInvoiceRepository = proformaInvoiceRepository;
        }

        public async Task CreateSampleInward(SampleInwardDto model)
        {

            dynamic caseAndSample = await _SampleInwardRepository.GetCaseNoAndSampleNo();

            if (caseAndSample == null || !caseAndSample.GetType().GetProperty("nextSampleCounter")?.CanRead == true)
            {
                throw new InvalidOperationException("Invalid case and sample data returned.");
            }

            int nextSampleNumber = (int)caseAndSample?.nextSampleCounter;

            var year = DateTime.UtcNow.Year.ToString().Substring(2, 2);
            int testCounter = 1;
            string tcPrefix = "TC5098";
            string labLocation = "0";
            string yearCode = DateTime.UtcNow.Year.ToString().Substring(2, 2);

            // Rest of the method remains unchanged.  
            var entity = new SampleInward
            {
                CaseNo = caseAndSample.caseNo,
                CustomerID = model.CustomerID,
                Address = model.Address,
                Area = model.Area,
                State = model.State,
                City = model.City,
                PinCode = model.PinCode,
                Country = model.Country,
                GstNo = model.GstNo,
                AdvancePayment = model.AdvancePayment,
                BillRequired = model.BillRequired,
                AdvancePIRequired = model.AdvancePIRequired,
                HoldTesting = model.HoldTesting,
                HoldTestingUntilPIApproved = model.HoldTestingUntilPIApproved,
                Urgent = model.Urgent,
                ReturnSample = model.ReturnSample,
                NotDestroyed = model.NotDestroyed,
                SampleReceiptNote = model.SampleReceiptNote,
                RequestFilePath = model.RequestFilePath,
                RequestFileName = model.RequestFileName,
                UploadReferenceID = model.UploadReferenceID,
                File = model.File,

                DispatchModes = model.DispatchModes.Select(d => new SampleDispatchMode
                {
                    ID = d.ID,
                    InwardID = d.InwardID,
                    DispatchModeID = d.DispatchModeID
                }).ToList(),

                Contacts = model.Contacts.Select(c => new SampleInwardContactPerson
                {
                    ContactID = c.ContactID,
                    Name = c.Name,
                    MobileNo = c.MobileNo,
                    EmailId = c.EmailId,
                    SendBill = c.SendBill,
                    SendReport = c.SendReport,
                    Selected = c.Selected
                }).ToList(),

                Addresses = new List<SampleInwardAddressInfo>
               {
                   new SampleInwardAddressInfo
                   {
                       ContactPersonName = model.ReportingTo.ContactPersonName,
                       ContactPersonID = model.ReportingTo.ContactPersonID,
                       Address = model.ReportingTo.Address,
                       PinCode = model.ReportingTo.PinCode,
                       Area = model.ReportingTo.Area,
                       City = model.ReportingTo.City,
                       State = model.ReportingTo.State,
                       Country = model.ReportingTo.Country,
                       Type = model.ReportingTo.Type
                   },
                   new SampleInwardAddressInfo
                   {
                       ContactPersonName = model.BillingTo.ContactPersonName,
                       ContactPersonID = model.BillingTo.ContactPersonID,
                       Address = model.BillingTo.Address,
                       PinCode = model.BillingTo.PinCode,
                       Area = model.BillingTo.Area,
                       City = model.BillingTo.City,
                       State = model.BillingTo.State,
                       Country = model.BillingTo.Country,
                       Type = model.BillingTo.Type
                   }
               },

                SampleDetails = model.SampleDetails.Select((s, index) => new SampleDetail
                {
                    SampleNo = $"{year}-{(nextSampleNumber + index):D6}",
                    Details = s.Details,
                    MetalClassificationID = s.MetalClassificationID,
                    ProductConditionID = s.ProductConditionID,
                    Remarks = s.Remarks,
                    Quantity = s.Quantity,
                    UploadReferenceID = s.UploadReferenceID,
                    SampleFilePath = s.SampleFilePath,
                    FileName = s.FileName,
                    File = s.File,

                    AdditionalDetails = model.SampleAdditionalDetails
                        .Where(a => a.SampleNo == s.SampleNo)
                        .Select(a => new SampleAdditionalDetail
                        {
                            SampleNo = $"{year}-{(nextSampleNumber + index):D6}",
                            Label = a.Label,
                            Value = a.Value
                        }).ToList()
                }).ToList()
            };

            if (entity.File != null)
            {
                var fileUploadResponse = await _uploadService.UploadFileAsync(entity.File, FileType.Other, null, entity.RequestFileName);
                if (fileUploadResponse == null)
                    throw new InvalidOperationException("File upload failed!");
                entity.RequestFilePath = fileUploadResponse.FilePath;
                entity.RequestFileName = fileUploadResponse.OriginalFileName;
                entity.UploadReferenceID = fileUploadResponse.ID;
            }

            var statusJobs = new List<Func<Task>>();

            if (entity.SampleDetails.Any())
            {
                foreach (var sampleDetail in entity.SampleDetails)
                {
                    if (sampleDetail.File != null)
                    {
                        var fileUploadResponse = await _uploadService.UploadFileAsync(sampleDetail.File, FileType.Other, null, sampleDetail.FileName);
                        if (fileUploadResponse == null)
                            throw new InvalidOperationException("File upload failed for sample detail!");
                        sampleDetail.SampleFilePath = fileUploadResponse.FilePath;
                        sampleDetail.FileName = fileUploadResponse.OriginalFileName;
                        sampleDetail.UploadReferenceID = fileUploadResponse.ID;
                    }
                    // Queue status update job
                    statusJobs.Add(async () =>
                    {
                        await _sampleStatusService.ForceAutoStatusAsync(
                            sampleDetail.ID,
                            SampleStatus.SAMPLE_INWARD_REGISTERED,
                            loggedInUser.EmployeeID
                        );
                    });
                }
            }
            await _SampleInwardRepository.AddSampleInward(entity);
            _logger.LogInformation("SampleInward '{Case}' created successfully.", model.CaseNo);

            // Process queued jobs
            foreach (var job in statusJobs)
            {
                await job();
            }
            await _sampleStatusService.UpdateInwardStatus(entity.ID, loggedInUser.EmployeeID);

        }

        public async Task ModifySampleInward(SampleInwardDto model)
        {
            try
            {
                if (model.ID == 0)
                    throw new ArgumentException("SampleInward ID should not be empty!");

                var entity = await _SampleInwardRepository.GetSampleInwardWithPlans(model.ID);
                if (entity == null)
                    throw new Exception("Sample Inward not found");

                // Update scalar fields
                entity.CustomerID = model.CustomerID;
                entity.Address = model.Address;
                entity.Area = model.Area;
                entity.State = model.State;
                entity.City = model.City;
                entity.PinCode = model.PinCode;
                entity.Country = model.Country;
                entity.GstNo = model.GstNo;
                entity.AdvancePayment = model.AdvancePayment;
                entity.BillRequired = model.BillRequired;
                entity.AdvancePIRequired = model.AdvancePIRequired;
                entity.HoldTesting = model.HoldTesting;
                entity.HoldTestingUntilPIApproved = model.HoldTestingUntilPIApproved;
                entity.Urgent = model.Urgent;
                entity.ReturnSample = model.ReturnSample;
                entity.NotDestroyed = model.NotDestroyed;
                entity.SampleReceiptNote = model.SampleReceiptNote;

                // Handle request file update
                if (model.File != null)
                {
                    var fileUploadResponse = await _uploadService.UploadFileAsync(model.File, FileType.Other, null, model.RequestFileName);
                    if (fileUploadResponse == null)
                        throw new InvalidOperationException("File upload failed!");

                    entity.RequestFilePath = fileUploadResponse.FilePath;
                    entity.RequestFileName = fileUploadResponse.OriginalFileName;
                    entity.UploadReferenceID = fileUploadResponse.ID;
                }

                // Sync DispatchModes
                entity.DispatchModes.Clear();
                foreach (var d in model.DispatchModes)
                {
                    entity.DispatchModes.Add(new SampleDispatchMode
                    {
                        InwardID = entity.ID,
                        DispatchModeID = d.DispatchModeID
                    });
                }

                // Sync Contacts
                entity.Contacts.Clear();
                foreach (var c in model.Contacts)
                {
                    entity.Contacts.Add(new SampleInwardContactPerson
                    {
                        InwardID = entity.ID,
                        ContactID = c.ContactID,
                        Name = c.Name,
                        MobileNo = c.MobileNo,
                        EmailId = c.EmailId,
                        SendBill = c.SendBill,
                        SendReport = c.SendReport,
                        Selected = c.Selected
                    });
                }

                // Sync Addresses
                entity.Addresses.Clear();
                entity.Addresses.Add(new SampleInwardAddressInfo
                {
                    InwardID = entity.ID,
                    ContactPersonName = model.ReportingTo.ContactPersonName,
                    ContactPersonID = model.ReportingTo.ContactPersonID,
                    Address = model.ReportingTo.Address,
                    PinCode = model.ReportingTo.PinCode,
                    Area = model.ReportingTo.Area,
                    City = model.ReportingTo.City,
                    State = model.ReportingTo.State,
                    Country = model.ReportingTo.Country,
                    Type = model.ReportingTo.Type
                });
                entity.Addresses.Add(new SampleInwardAddressInfo
                {
                    InwardID = entity.ID,
                    ContactPersonName = model.BillingTo.ContactPersonName,
                    ContactPersonID = model.BillingTo.ContactPersonID,
                    Address = model.BillingTo.Address,
                    PinCode = model.BillingTo.PinCode,
                    Area = model.BillingTo.Area,
                    City = model.BillingTo.City,
                    State = model.BillingTo.State,
                    Country = model.BillingTo.Country,
                    Type = model.BillingTo.Type
                });

                //  Only fetch next sample number if a new sample will be added
                int nextSampleNumber = 0;
                var year = DateTime.UtcNow.Year.ToString().Substring(2, 2);

                // Check if any new sample (SampleNo is empty or not found in DB)
                var newSamples = model.SampleDetails
                    .Where(s => string.IsNullOrEmpty(s.SampleNo) || !entity.SampleDetails.Any(e => e.SampleNo == s.SampleNo))
                    .ToList();

                if (newSamples.Any())
                {
                    dynamic caseAndSample = await _SampleInwardRepository.GetCaseNoAndSampleNo();
                    nextSampleNumber = caseAndSample.nextSampleCounter;
                }
                var statusJobs = new List<Func<Task>>();

                foreach (var s in model.SampleDetails)
                {
                    var existingSample = entity.SampleDetails.FirstOrDefault(x => x.SampleNo == s.SampleNo);

                    if (existingSample != null)
                    {
                        //  Update existing sample, keep original SampleNo
                        existingSample.Details = s.Details;
                        existingSample.MetalClassificationID = s.MetalClassificationID;
                        existingSample.ProductConditionID = s.ProductConditionID;
                        existingSample.TpiAgencyID = s.TpiAgencyID;
                        existingSample.Remarks = s.Remarks;
                        existingSample.Quantity = s.Quantity;
                        existingSample.Specimen = s.Specimen;
                        existingSample.TestInstructions = s.TestInstructions;

                        if (s.File != null)
                        {
                            var fileUploadResponse = await _uploadService.UploadFileAsync(s.File, FileType.Other, null, s.FileName);
                            if (fileUploadResponse == null)
                                throw new InvalidOperationException($"File upload failed for sample {existingSample.SampleNo}");

                            existingSample.SampleFilePath = fileUploadResponse.FilePath;
                            existingSample.FileName = fileUploadResponse.OriginalFileName;
                            existingSample.UploadReferenceID = fileUploadResponse.ID;
                        }

                        existingSample.AdditionalDetails.Clear();
                        foreach (var a in model.SampleAdditionalDetails.Where(a => a.SampleNo == s.SampleNo))
                        {
                            existingSample.AdditionalDetails.Add(new SampleAdditionalDetail
                            {
                                SampleNo = existingSample.SampleNo,
                                Label = a.Label,
                                Value = a.Value
                            });
                        }

                        //  Queue status update job for existing sample
                        statusJobs.Add(async () =>
                        {
                            await _sampleStatusService.ForceAutoStatusAsync(
                                existingSample.ID,
                                SampleStatus.INWARD_COMPLETED,
                                loggedInUser.EmployeeID
                            );
                        });
                    }
                    else
                    {
                        //  Add new sample with new SampleNo only for new entries
                        var newSampleNo = $"{year}-{nextSampleNumber++:D6}";
                        var newSample = new SampleDetail
                        {
                            SampleNo = newSampleNo,
                            Details = s.Details,
                            MetalClassificationID = s.MetalClassificationID,
                            ProductConditionID = s.ProductConditionID,
                            Remarks = s.Remarks,
                            Quantity = s.Quantity
                        };

                        if (s.File != null)
                        {
                            var fileUploadResponse = await _uploadService.UploadFileAsync(s.File, FileType.Other, null, s.FileName);
                            if (fileUploadResponse == null)
                                throw new InvalidOperationException($"File upload failed for sample {newSample.SampleNo}");

                            newSample.SampleFilePath = fileUploadResponse.FilePath;
                            newSample.FileName = fileUploadResponse.OriginalFileName;
                            newSample.UploadReferenceID = fileUploadResponse.ID;
                        }

                        foreach (var a in model.SampleAdditionalDetails.Where(a => a.SampleNo == s.SampleNo))
                        {
                            newSample.AdditionalDetails.Add(new SampleAdditionalDetail
                            {
                                SampleNo = newSampleNo,
                                Label = a.Label,
                                Value = a.Value
                            });
                        }

                        entity.SampleDetails.Add(newSample);
                        //  Queue status update job for new sample
                        statusJobs.Add(async () =>
                        {
                            await _sampleStatusService.ForceAutoStatusAsync(
                                newSample.ID,
                                SampleStatus.INWARD_COMPLETED,
                                loggedInUser.EmployeeID
                            );
                        });
                    }
                }

                await _SampleInwardRepository.UpdateSampleInward(entity);
                _logger.LogInformation("SampleInward '{Case}' updated successfully.", entity.CaseNo);
                foreach (var job in statusJobs)
                {
                    await job();
                }
                await _sampleStatusService.UpdateInwardStatus(entity.ID, loggedInUser.EmployeeID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating SampleInward ID {ID}", model.ID);
                throw;
            }
        }




        //public async Task ModifySamplePlan(PlanDto model)
        //{
        //    try
        //    {
        //        var entity = await _SampleInwardRepository.GetSampleInwardWithPlans(model.ID);
        //        if (entity == null)
        //            throw new Exception("Sample Inward not found");

        //        // Update high-level plan info
        //        entity.StatementOfConformity = model.StatementOfConformity;
        //        entity.DecisionRule = model.DecisionRule;
        //        entity.ReviewedBy = loggedInUser.EmployeeID;
        //        entity.ReviewedOn = DateTime.UtcNow;

        //        string tcPrefix = "TC5098";
        //        string labLocation = "0";
        //        string year = DateTime.UtcNow.Year.ToString().Substring(2, 2);

        //        var statusJobs = new List<Task>();

        //        foreach (var sampleDto in model.SampleDetails)
        //        {
        //            var sample = entity.SampleDetails.FirstOrDefault(s => s.SampleNo == sampleDto.SampleNo);
        //            if (sample == null)
        //                throw new Exception($"Sample '{sampleDto.SampleNo}' not found in inward {model.ID}");

        //            // Queue status update (async)
        //            statusJobs.Add(
        //                _sampleStatusService.ForceAutoStatusAsync(
        //                    sample.ID,
        //                    SampleStatus.SAMPLE_UNDER_PLANNING,
        //                    loggedInUser.EmployeeID
        //                )
        //            );

        //            // Update prep fields
        //            sample.PreparationRequired = sampleDto.PreparationRequired;
        //            sample.MachiningRequired = sampleDto.MachiningRequired;
        //            sample.MachiningAmount = sampleDto.MachiningAmount ?? 0;
        //            sample.OtherPreparation = sampleDto.OtherPreparation;
        //            sample.OtherPreparationCharge = sampleDto.OtherPreparationCharge ?? 0;
        //            sample.TpiRequired = sampleDto.TpiRequired;

        //            var existingPlan = sample.TestPlans.FirstOrDefault()
        //                ?? new SampleTestPlan { SampleNo = sampleDto.SampleNo };

        //            if (!sample.TestPlans.Contains(existingPlan))
        //                sample.TestPlans.Add(existingPlan);

        //            int ulrCounter = 1;

        //            // ========== GENERAL TESTS ==========
        //            var dtoGeneral = sampleDto.TestPlans.SelectMany(p => p.GeneralTests).ToList();

        //            var toRemoveGeneral = existingPlan.GeneralTests
        //                .Where(gt => !dtoGeneral.Any(d => d.ID == gt.ID))
        //                .ToList();

        //            foreach (var rem in toRemoveGeneral)
        //                existingPlan.GeneralTests.Remove(rem);

        //            foreach (var g in dtoGeneral.Select((g, idx) => new { g, idx }))
        //            {
        //                var gt = existingPlan.GeneralTests.FirstOrDefault(x => x.ID == g.g.ID)
        //                    ?? new GeneralTest();

        //                gt.Specification1 = g.g.Specification1;
        //                gt.Specification2 = g.g.Specification2;
        //                gt.Methods.Clear();

        //                foreach (var m in g.g.Methods)
        //                {
        //                    gt.Methods.Add(new GeneralTestMethod
        //                    {
        //                        TestMethodID = m.TestMethodID ?? 0,
        //                        StandardID = m.StandardID ?? 0,
        //                        Quantity = m.Quantity,
        //                        ReportNo = string.IsNullOrEmpty(m.ReportNo)
        //                            ? $"{sample.SampleNo}-{g.idx}"
        //                            : m.ReportNo,
        //                        UlrNo = string.IsNullOrEmpty(m.UlrNo)
        //                            ? $"{tcPrefix}{year}{labLocation}{sample.SampleNo.Split('-')[1].PadLeft(8, '0')}{ulrCounter++}F"
        //                            : m.UlrNo,
        //                        Cancel = m.Cancel
        //                    });
        //                }

        //                if (!existingPlan.GeneralTests.Contains(gt))
        //                    existingPlan.GeneralTests.Add(gt);
        //            }

        //            // ========== CHEMICAL TESTS ==========
        //            var dtoChem = sampleDto.TestPlans.SelectMany(p => p.ChemicalTests).ToList();

        //            var toRemoveChem = existingPlan.ChemicalTests
        //                .Where(ct => !dtoChem.Any(d => d.ID == ct.ID))
        //                .ToList();

        //            foreach (var rem in toRemoveChem)
        //                existingPlan.ChemicalTests.Remove(rem);

        //            foreach (var c in dtoChem.Select((c, idx) => new { c, idx }))
        //            {
        //                var ct = existingPlan.ChemicalTests.FirstOrDefault(x => x.ID == c.c.ID)
        //                    ?? new ChemicalTest();

        //                ct.ReportNo = string.IsNullOrEmpty(c.c.ReportNo)
        //                    ? $"{sample.SampleNo}-{c.idx}"
        //                    : c.c.ReportNo;

        //                ct.UlrNo = string.IsNullOrEmpty(c.c.UlrNo)
        //                    ? $"{tcPrefix}{year}{labLocation}{sample.SampleNo.Split('-')[1].PadLeft(8, '0')}{ulrCounter++}F"
        //                    : c.c.UlrNo;

        //                ct.Specification1 = c.c.Specification1;
        //                ct.Specification2 = c.c.Specification2;
        //                ct.TestMethod = c.c.TestMethod;

        //                // Sync elements
        //                ct.Elements.Clear();
        //                foreach (var e in c.c.Elements)
        //                {
        //                    ct.Elements.Add(new ChemicalTestElement
        //                    {
        //                        ParameterID = e.ParameterID,
        //                        SpecificationLineID = e.SpecificationLineID,
        //                        ParameterUnitID = e.ParameterUnitID,
        //                        ParameterUnit = e.ParameterUnit,
        //                        MinValue = e.MinValue,
        //                        MaxValue = e.MaxValue,
        //                        Selected = e.Selected
        //                    });
        //                }

        //                // Sync TestTypes
        //                ct.TestTypes.Clear();
        //                foreach (var kvp in c.c.TestTypes)
        //                {
        //                    ct.TestTypes.Add(new ChemicalTestType
        //                    {
        //                        Name = kvp.Key,
        //                        IsSelected = kvp.Value
        //                    });
        //                }

        //                if (!existingPlan.ChemicalTests.Contains(ct))
        //                    existingPlan.ChemicalTests.Add(ct);
        //            }
        //        }

        //        await _SampleInwardRepository.UpdateSampleInward(entity);

        //        // Execute all pending status jobs
        //        await Task.WhenAll(statusJobs);

        //        _logger.LogInformation("Plan updated for Inward '{InwardID}'.", model.ID);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "ModifySamplePlan ERROR for inward {ID}", model.ID);
        //        throw;
        //    }
        //}
        public async Task ModifySamplePlan(PlanDto model)
        {
            try
            {

                var entity = await _SampleInwardRepository.GetSampleInwardWithPlans(model.ID);
                if (entity == null)
                    throw new Exception("Sample Inward not found");

                // update review info
                entity.StatementOfConformity = model.StatementOfConformity;
                entity.DecisionRule = model.DecisionRule;

                string tcPrefix = "TC5098";
                string labLocation = "0";
                string year = DateTime.UtcNow.Year.ToString().Substring(2, 2);

                var statusJobs = new List<Func<Task>>();
                foreach (var sampleDto in model.SampleDetails)
                {
                    var sample = entity.SampleDetails.FirstOrDefault(s => s.SampleNo == sampleDto.SampleNo);
                    if (sample == null)
                        throw new Exception($"Sample '{sampleDto.SampleNo}' not found in inward {model.ID}");


                    // Queue status update (async)
                    statusJobs.Add(()=>
                        _sampleStatusService.ForceAutoStatusAsync(
                            sample.ID,
                            SampleStatus.UNDER_PLANNING,
                            loggedInUser.EmployeeID
                        )
                    );

                    // update prep fields
                    sample.PreparationRequired = sampleDto.PreparationRequired;
                    sample.MachiningRequired = sampleDto.MachiningRequired;
                    sample.MachiningAmount = sampleDto.MachiningAmount ?? 0;
                    sample.OtherPreparation = sampleDto.OtherPreparation;
                    sample.OtherPreparationCharge = sampleDto.OtherPreparationCharge ?? 0;
                    sample.TpiRequired = sampleDto.TpiRequired;
                    sample.TpiAgencyID = sampleDto.TpiAgencyID;

                    // ensure plan
                    var existingPlan = sample.TestPlans.FirstOrDefault();
                    if (existingPlan == null)
                    {
                        existingPlan = new SampleTestPlan { SampleNo = sampleDto.SampleNo };
                        sample.TestPlans.Add(existingPlan);
                    }

                    int ulrCounter = 1;

                    //  Sync General Tests
                    var dtoGeneralTests = sampleDto.TestPlans.SelectMany(p => p.GeneralTests).ToList();

                    // remove missing general tests
                    var toRemoveGeneral = existingPlan.GeneralTests
                        .Where(gt => !dtoGeneralTests.Any(d => d.ID == gt.ID))
                        .ToList();
                    foreach (var rem in toRemoveGeneral)
                        existingPlan.GeneralTests.Remove(rem);

                    // add / update general tests
                    foreach (var g in dtoGeneralTests.Select((g, idx) => new { g, idx }))
                    {
                        var existingGeneral = existingPlan.GeneralTests
                            .FirstOrDefault(x => x.ID == g.g.ID);

                        if (existingGeneral == null)
                        {
                            existingGeneral = new GeneralTest
                            {
                                Specification1 = g.g.Specification1,
                                Specification2 = g.g.Specification2,
                                Methods = new List<GeneralTestMethod>()
                            };
                            existingPlan.GeneralTests.Add(existingGeneral);
                        }
                        else
                        {
                            // update fields
                            existingGeneral.Specification1 = g.g.Specification1;
                            existingGeneral.Specification2 = g.g.Specification2;
                            existingGeneral.Methods.Clear(); // re-sync methods
                        }

                        foreach (var m in g.g.Methods)
                        {
                            var newMethod = new GeneralTestMethod
                            {
                                TestMethodID = m.TestMethodID ?? 0,
                                TestCaseID = m.TestCaseID ?? 0,
                                SelectionType = m.SelectionType,
                                Value = m.Value,
                                StandardID = m.StandardID ?? 0,
                                Quantity = m.Quantity,
                                ReportNo = string.IsNullOrEmpty(m.ReportNo)
                                            ? existingGeneral.Methods.FirstOrDefault(x => x.TestMethodID == m.TestMethodID)?.ReportNo
                                              ?? $"{sample.SampleNo}-{g.idx+1}"
                                            : m.ReportNo,
                                UlrNo = string.IsNullOrEmpty(m.UlrNo)
                                            ? existingGeneral.Methods.FirstOrDefault(x => x.TestMethodID == m.TestMethodID)?.UlrNo
                                              ?? $"{tcPrefix}{year}{labLocation}{sample.SampleNo.Split('-')[1].PadLeft(8, '0')}{ulrCounter++}F"
                                            : m.UlrNo,
                                Cancel = m.Cancel
                            };

                            existingGeneral.Methods.Add(newMethod);
                        }
                    }

                    //  Sync Chemical Tests
                    var dtoChemicalTests = sampleDto.TestPlans.SelectMany(p => p.ChemicalTests).ToList();

                    // remove missing chemical tests
                    var toRemoveChem = existingPlan.ChemicalTests
                        .Where(ct => !dtoChemicalTests.Any(d => d.ID == ct.ID))
                        .ToList();
                    foreach (var rem in toRemoveChem)
                        existingPlan.ChemicalTests.Remove(rem);

                    // add / update chemical tests
                    foreach (var c in dtoChemicalTests.Select((c, idx) => new { c, idx }))
                    {
                        var existingChem = existingPlan.ChemicalTests
                            .FirstOrDefault(x => x.ID == c.c.ID);

                        if (existingChem == null)
                        {
                            existingChem = new ChemicalTest();
                            existingPlan.ChemicalTests.Add(existingChem);
                        }

                        // update fields
                        existingChem.ReportNo = string.IsNullOrEmpty(c.c.ReportNo)
                            ? existingChem.ReportNo ?? $"{sample.SampleNo}-{c.idx}"
                            : c.c.ReportNo;

                        existingChem.UlrNo = string.IsNullOrEmpty(c.c.UlrNo)
                            ? existingChem.UlrNo ?? $"{tcPrefix}{year}{labLocation}{sample.SampleNo.Split('-')[1].PadLeft(8, '0')}{ulrCounter++}F"
                            : c.c.UlrNo;

                        existingChem.Specification1 = c.c.Specification1;
                        existingChem.Specification2 = c.c.Specification2;
                        existingChem.TestMethod = c.c.TestMethod;

                        // sync elements
                        existingChem.Elements.Clear();
                        foreach (var e in c.c.Elements)
                        {
                            existingChem.Elements.Add(new ChemicalTestElement
                            {
                                ParameterID = e.ParameterID,
                                SpecificationLineID = e.SpecificationLineID,
                                ParameterUnitID = e.ParameterUnitID,
                                ParameterUnit = e.ParameterUnit,
                                MinValue = e.MinValue,
                                MaxValue = e.MaxValue,
                                Selected = e.Selected
                            });
                        }

                        existingChem.TestTypes.Clear();
                        foreach (var kvp in c.c.TestTypes)
                        {
                            existingChem.TestTypes.Add(new ChemicalTestType
                            {
                                Name = kvp.Key,
                                IsSelected = kvp.Value
                            });
                        }
                    }
                }

                await _SampleInwardRepository.UpdateSampleInward(entity);
                _logger.LogInformation("Plans and sample prep updated for Inward '{InwardID}'", model.ID);

                foreach (var job in statusJobs)
                {
                    await job();
                }
                await _sampleStatusService.UpdateInwardStatus(entity.ID, loggedInUser.EmployeeID);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SubmitPlanForReview(PlanDto model)
        {
            try
            {

                await ModifySamplePlan(model);


                var entity = await _SampleInwardRepository.GetSampleInwardWithPlans(model.ID);
                if (entity == null)
                    throw new Exception("Sample Inward not found");


                entity.ReviewStatus = "Pending for Approval";
                entity.ReviewedBy = loggedInUser.EmployeeID;
                entity.ReviewedOn = DateTime.UtcNow;


                var statusJobs = new List<Func<Task>>();

                foreach (var sample in entity.SampleDetails)
                {
                    statusJobs.Add(()=>
                        _sampleStatusService.ForceAutoStatusAsync(
                            sample.ID,
                            SampleStatus.UNDER_REVIEW_REQUEST,
                            loggedInUser.EmployeeID
                        )
                    );
                }

                await _workflowService.StartWorkflow(entity.ID, "Request of Review");

                await _SampleInwardRepository.UpdateSampleInward(entity);

                foreach (var job in statusJobs)
                {
                    await job();
                }
                await _sampleStatusService.UpdateInwardStatus(entity.ID, loggedInUser.EmployeeID);
                _logger.LogInformation("Plan submitted for review for inward {ID}", model.ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        public async Task RemoveSampleInward(long id)
        {
            var existingSampleInward = await _SampleInwardRepository.GetSampleInwardById(id);
            if (existingSampleInward == null)
                throw new InvalidOperationException("SampleInward not found!");

            existingSampleInward.IsActive = false;
            existingSampleInward.ModifiedOn = DateTime.UtcNow;

            await _SampleInwardRepository.UpdateSampleInward(existingSampleInward);
            _logger.LogInformation("SampleInward with ID '{SampleInwardId}' deleted successfully.", id);
        }

        public async Task<SampleInwardDto> GetSampleInwardDetails(long id)
        {
            var sampleInward = await _SampleInwardRepository.GetSampleInwardById(id);
            if (sampleInward == null)
                throw new InvalidOperationException("SampleInward not found!");

            var dto = new SampleInwardDto
            {
                ID = sampleInward.ID,
                CaseNo = sampleInward.CaseNo,
                CustomerID = sampleInward.CustomerID,
                Address = sampleInward.Address,
                Area = sampleInward.Area,
                State = sampleInward.State,
                City = sampleInward.City,
                PinCode = sampleInward.PinCode,
                Country = sampleInward.Country,
                GstNo = sampleInward.GstNo,
                AdvancePayment = sampleInward.AdvancePayment,
                BillRequired = sampleInward.BillRequired,
                AdvancePIRequired = sampleInward.AdvancePIRequired,
                HoldTesting = sampleInward.HoldTesting,
                HoldTestingUntilPIApproved = sampleInward.HoldTestingUntilPIApproved,
                Urgent = sampleInward.Urgent,
                ReturnSample = sampleInward.ReturnSample,
                NotDestroyed = sampleInward.NotDestroyed,
                SampleReceiptNote = sampleInward.SampleReceiptNote,
                RequestFilePath = sampleInward.RequestFilePath,
                RequestFileName = sampleInward.RequestFileName,
                UploadReferenceID = sampleInward.UploadReferenceID,
                Status = sampleInward.InwardStatus,
                CollectionTime = sampleInward.CollectionTime,

                DispatchModes = sampleInward.DispatchModes
                    .Select(d => new DispatchModeDto
                    {
                        ID = d.ID,
                        InwardID = d.InwardID,
                        DispatchModeID = d.DispatchModeID
                    }).ToList(),

                Contacts = sampleInward.Contacts
                    .Select(c => new ContactDto
                    {
                        ContactID = c.ContactID,
                        Name = c.Name,
                        MobileNo = c.MobileNo,
                        EmailId = c.EmailId,
                        SendBill = c.SendBill,
                        SendReport = c.SendReport,
                        Selected = c.Selected
                    }).ToList(),

                ReportingTo = sampleInward.Addresses
                    .Where(a => a.Type == "reporting")
                    .Select(a => new PartyAddressDto
                    {
                        ID = a.ID,
                        InwardID = a.InwardID,
                        ContactPersonName = a.ContactPersonName,
                        ContactPersonID = a.ContactPersonID,
                        Address = a.Address,
                        PinCode = a.PinCode,
                        Area = a.Area,
                        City = a.City,
                        State = a.State,
                        Country = a.Country,
                        Type = a.Type
                    })
                    .FirstOrDefault(),

                BillingTo = sampleInward.Addresses
                    .Where(a => a.Type == "billing")
                    .Select(a => new PartyAddressDto
                    {
                        ID = a.ID,
                        InwardID = a.InwardID,
                        ContactPersonName = a.ContactPersonName,
                        ContactPersonID = a.ContactPersonID,
                        Address = a.Address,
                        PinCode = a.PinCode,
                        Area = a.Area,
                        City = a.City,
                        State = a.State,
                        Country = a.Country,
                        Type = a.Type
                    })
                    .FirstOrDefault(),

                SampleDetails = sampleInward.SampleDetails
                    .Select(s => new SampleDetailDto
                    {
                        ID = s.ID,
                        SampleNo = s.SampleNo,
                        Details = s.Details,
                        MetalClassificationID = s.MetalClassificationID,
                        ProductConditionID = s.ProductConditionID,
                        Remarks = s.Remarks,
                        Quantity = s.Quantity,
                        UploadReferenceID = s.UploadReferenceID,
                        SampleFilePath = s.SampleFilePath,
                        FileName = s.FileName,
                        PreparationRequired = s.PreparationRequired,
                        MachiningRequired = s.MachiningRequired,
                        MachiningAmount = s.MachiningAmount,
                        OtherPreparation = s.OtherPreparation,
                        OtherPreparationCharge = s.OtherPreparationCharge,
                        TpiRequired = s.TpiRequired,
                        Specimen = s.Specimen,
                        TestInstructions = s.TestInstructions
                    }).ToList(),

                SampleAdditionalDetails = sampleInward.SampleDetails
                    .SelectMany(s => s.AdditionalDetails)
                    .Select(a => new SampleAdditionalDetailDto
                    {
                        ID = a.ID,
                        SampleID = a.SampleID,
                        SampleNo = a.SampleNo,
                        Label = a.Label,
                        Value = a.Value
                    }).ToList(),

                //  NEW: Sample Test Plans
                //SampleTestPlans = sampleInward.SampleDetails
                //    .Where(s => s.TestPlans.Any())
                //    .SelectMany(s => s.TestPlans.Select(tp => new SampleTestPlanDto
                //    {
                //        SampleNo = tp.SampleNo,
                //        GeneralTests = tp.GeneralTests.Select(gt => new GeneralTestDto
                //        {
                //            Specification1 = gt.Specification1,
                //            Specification2 = gt.Specification2,
                //            Methods = gt.Methods.Select(m => new GeneralTestMethodDto
                //            {
                //                TestMethodID = m.TestMethodID,
                //                StandardID = m.StandardID,
                //                Quantity = m.Quantity,
                //                ReportNo = m.ReportNo,
                //                UlrNo = m.UlrNo,
                //                Cancel = m.Cancel
                //            }).ToList()
                //        }).ToList(),
                //        ChemicalTests = tp.ChemicalTests.Select(ct => new ChemicalTestDto
                //        {
                //            ReportNo = ct.ReportNo,
                //            UlrNo = ct.UlrNo,
                //            Specification1 = ct.Specification1,
                //            Specification2 = ct.Specification2,
                //            TestMethod = ct.TestMethod,
                //            Elements = ct.Elements.Select(e => new ChemicalTestElementDto
                //            {
                //                ParameterID = e.ParameterID
                //            }).ToList()
                //        }).ToList()
                //    }))
                //    .ToList()
            };

            return dto;
        }
        public async Task<SampleInwardDto> GetSampleInwardWithPlans(long id)
        {
            var sampleInward = await _SampleInwardRepository.GetSampleInwardWithPlans(id);
            if (sampleInward == null)
                throw new InvalidOperationException("SampleInward not found!");

            var dto = new SampleInwardDto
            {
                ID = sampleInward.ID,
                CaseNo = sampleInward.CaseNo,
                CustomerID = sampleInward.CustomerID,
                CustomerName = sampleInward?.Customer?.Name,
                Address = sampleInward.Address,
                Area = sampleInward.Area,
                State = sampleInward.State,
                City = sampleInward.City,
                PinCode = sampleInward.PinCode,
                Country = sampleInward.Country,
                GstNo = sampleInward.GstNo,
                AdvancePayment = sampleInward.AdvancePayment,
                BillRequired = sampleInward.BillRequired,
                AdvancePIRequired = sampleInward.AdvancePIRequired,
                HoldTesting = sampleInward.HoldTesting,
                HoldTestingUntilPIApproved = sampleInward.HoldTestingUntilPIApproved,
                Urgent = sampleInward.Urgent,
                ReturnSample = sampleInward.ReturnSample,
                NotDestroyed = sampleInward.NotDestroyed,
                SampleReceiptNote = sampleInward.SampleReceiptNote,
                RequestFilePath = sampleInward.RequestFilePath,
                RequestFileName = sampleInward.RequestFileName,
                UploadReferenceID = sampleInward.UploadReferenceID,
                Status = sampleInward.InwardStatus,
                CollectionTime = sampleInward.CollectionTime,
                StatementOfConformity = sampleInward.StatementOfConformity,
                DecisionRule = sampleInward.DecisionRule,
                ReviewStatus = sampleInward.ReviewStatus,
                ReviewedBy = sampleInward.ReviewedBy,
                ReviewedOn = sampleInward.ReviewedOn,

                DispatchModes = sampleInward.DispatchModes
                    .Select(d => new DispatchModeDto
                    {
                        ID = d.ID,
                        InwardID = d.InwardID,
                        DispatchModeID = d.DispatchModeID
                    }).ToList(),

                Contacts = sampleInward.Contacts
                    .Select(c => new ContactDto
                    {
                        ID = c.ID,
                        InwardID = c.InwardID,
                        ContactID = c.ContactID,
                        Name = c.Name,
                        MobileNo = c.MobileNo,
                        EmailId = c.EmailId,
                        SendBill = c.SendBill,
                        SendReport = c.SendReport,
                        Selected = c.Selected
                    }).ToList(),

                ReportingTo = sampleInward.Addresses
                    .Where(a => a.Type == "reporting")
                    .Select(a => new PartyAddressDto
                    {
                        ID = a.ID,
                        InwardID = a.InwardID,
                        ContactPersonName = a.ContactPersonName,
                        ContactPersonID = a.ContactPersonID,
                        Address = a.Address,
                        PinCode = a.PinCode,
                        Area = a.Area,
                        City = a.City,
                        State = a.State,
                        Country = a.Country,
                        Type = a.Type
                    })
                    .FirstOrDefault(),

                BillingTo = sampleInward.Addresses
                    .Where(a => a.Type == "billing")
                    .Select(a => new PartyAddressDto
                    {
                        ID = a.ID,
                        InwardID = a.InwardID,
                        ContactPersonName = a.ContactPersonName,
                        ContactPersonID = a.ContactPersonID,
                        Address = a.Address,
                        PinCode = a.PinCode,
                        Area = a.Area,
                        City = a.City,
                        State = a.State,
                        Country = a.Country,
                        Type = a.Type
                    })
                    .FirstOrDefault(),

                SampleDetails = sampleInward.SampleDetails
                    .Select(s => new SampleDetailDto
                    {
                        ID = s.ID,
                        InwardID = s.InwardID,
                        SampleNo = s.SampleNo,
                        Details = s.Details,
                        MetalClassificationID = s.MetalClassificationID,
                        ProductConditionID = s.ProductConditionID,
                        Remarks = s.Remarks,
                        Quantity = s.Quantity,
                        UploadReferenceID = s.UploadReferenceID,
                        SampleFilePath = s.SampleFilePath,
                        FileName = s.FileName,
                        PreparationRequired = s.PreparationRequired,
                        MachiningRequired = s.MachiningRequired,
                        MachiningAmount = s.MachiningAmount,
                        OtherPreparation = s.OtherPreparation,
                        OtherPreparationCharge = s.OtherPreparationCharge,
                        TpiRequired = s.TpiRequired
                    }).ToList(),

                SampleAdditionalDetails = sampleInward.SampleDetails
                    .SelectMany(s => s.AdditionalDetails)
                    .Select(a => new SampleAdditionalDetailDto
                    {
                        ID = a.ID,
                        SampleID = a.SampleID,
                        SampleNo = a.SampleNo,
                        Label = a.Label,
                        Value = a.Value
                    }).ToList(),

                //  NEW: Sample Test Plans
                SampleTestPlans = sampleInward.SampleDetails
                    .Where(s => s.TestPlans.Any())
                    .SelectMany(s => s.TestPlans.Select(tp => new SampleTestPlanDto
                    {
                        ID = tp.ID,
                        SampleNo = tp.SampleNo,
                        SampleID = tp.SampleID,
                        GeneralTests = tp.GeneralTests.Select(gt => new GeneralTestDto
                        {
                            ID = gt.ID,
                            SampleTestPlanID = gt.SampleTestPlanID,
                            Specification1 = gt.Specification1,
                            Specification2 = gt.Specification2,
                            Methods = gt.Methods.Select(m => new GeneralTestMethodDto
                            {
                                ID = m.ID,
                                GeneralTestID = m.GeneralTestID,
                                TestMethodID = m.TestMethodID,
                                StandardID = m.StandardID,
                                Quantity = m.Quantity,
                                ReportNo = m.ReportNo,
                                UlrNo = m.UlrNo,
                                Cancel = m.Cancel
                            }).ToList()
                        }).ToList(),
                        ChemicalTests = tp.ChemicalTests.Select(ct => new ChemicalTestDto
                        {
                            ID = ct.ID,
                            SampleTestPlanID = ct.SampleTestPlanID,
                            ReportNo = ct.ReportNo,
                            UlrNo = ct.UlrNo,
                            Specification1 = ct.Specification1,
                            Specification2 = ct.Specification2,
                            TestMethod = ct.TestMethod,
                            TestTypes = ct.TestTypes.ToDictionary(tt => tt.Name, tt => tt.IsSelected),
                            Elements = ct.Elements.Select(e => new ChemicalTestElementDto
                            {
                                ID = e.ID,
                                ChemicalTestID = e.ChemicalTestID,
                                ParameterID = e.ParameterID
                            }).ToList()
                        }).ToList()
                    }))
                    .ToList()
            };

            var step =  await _workflowService.GetCurrentWorkflowStepAsync(sampleInward.ID, "Request of Review");
            if (step != null)
            {
                var approverIds = step.AssignedToValue?
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => long.TryParse(x, out var id) ? id : 0)
                    .Where(id => id > 0)
                    .ToList()
                    ?? new List<long>();

                dto.CanTakeAction = approverIds.Contains(loggedInUser.EmployeeID);

               
                if (dto.CanTakeAction)
                {
                    var instance = await _workflowService
                        .GetActiveInstanceForEntityAsync(sampleInward.ID, "Request of Review");

                    dto.Actions = step.Transitions
                        .Where(t => t.IsActive)
                        .Select(t => new ActionDto
                        {
                            Id = instance.ID,
                            Name = t.Alias ?? t.Action,
                            Action = t.Action
                        })
                        .ToList();
                }
            }

            return dto;
        }


        public async Task<PagedResponse<object>> FetchSampleInwardList(PageFilter filter)
        {
            return await _SampleInwardRepository.GetInwardList(filter);
        }
        public async Task<PagedResponse<object>> FetchPlanList(PageFilter filter)
        {
            return await _SampleInwardRepository.GetPlanList(filter);
        }
        public async Task<PagedResponse<object>> FetchReviewList(PageFilter filter)
        {
            return await _SampleInwardRepository.GetReviewList(filter);
        }

        public async Task<object> GetCaseNoAndSampleNo()
        {
            var caseNumber = await _SampleInwardRepository.GetCaseNoAndSampleNo();
            if (caseNumber == null)
                throw new InvalidOperationException("No case number found!");
            return caseNumber;
        }

        public static string GetStatusLabel(SampleWorkflowStatus status)
        {
            return status switch
            {
                SampleWorkflowStatus.INWARD_REGISTERED => "Inward Registered",
                SampleWorkflowStatus.INWARD_VERIFIED => "Sample Verified",
                SampleWorkflowStatus.PLAN_DRAFT => "Plan Draft",
                SampleWorkflowStatus.PLAN_SUBMITTED => "Plan Submitted",
                SampleWorkflowStatus.TECHNICAL_REVIEW => "Technical Review",
                SampleWorkflowStatus.QUALITY_REVIEW => "Quality Review",
                SampleWorkflowStatus.AWAITING_L1_APPROVAL => "Awaiting Level 1 Approval",
                SampleWorkflowStatus.APPROVED_L1 => "Approved Level 1",
                SampleWorkflowStatus.AWAITING_L2_APPROVAL => "Awaiting Level 2 Approval",
                SampleWorkflowStatus.APPROVED_L2 => "Approved Level 2",
                SampleWorkflowStatus.FINAL_APPROVED => "Final Approved",
                SampleWorkflowStatus.REJECTED => "Rejected",
                SampleWorkflowStatus.RETURNED_TO_ORIGIN => "Returned for Correction",
                SampleWorkflowStatus.PI_GENERATION_PENDING => "PI Pending",
                SampleWorkflowStatus.PI_GENERATED => "PI Generated",
                SampleWorkflowStatus.WORK_ASSIGNED => "Work Assigned",
                SampleWorkflowStatus.TESTING_IN_PROGRESS => "Testing In Progress",
                SampleWorkflowStatus.TESTING_COMPLETED => "Testing Completed",
                SampleWorkflowStatus.ARCHIVED => "Archived",
                _ => status.ToString()
            };
        }

        public async Task<List<DropdwonSelector>> GetSampleInwardDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _SampleInwardRepository.GetSampleInwardDropdown(searchTerm, pageNo, pageSize);
        }
        public async Task<List<DropdwonSelector>> GetSamplePreparationInwardDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _SampleInwardRepository.GetSamplePreparationInwardDropdown(searchTerm, pageNo, pageSize);
        }

        public async Task<byte[]> GeneratePIPdfAsync(long piId)
        {
            return await _proformaInvoiceRepository.GeneratePIPdfAsync(piId);
        }
    }
}
