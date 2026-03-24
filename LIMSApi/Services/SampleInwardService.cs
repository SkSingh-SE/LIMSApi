using System.Xml.Linq;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Migrations;
using LIMSApi.Models;
using LIMSApi.Repositories;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
        private readonly LIMSContext _context;
        private readonly EmailService _emailService;
        private readonly TemplateService _templateService;
        private readonly IPlanService _planService;

        public SampleInwardService(ISampleInwardRepository SampleInwardRepo, ILogger<SampleInwardService> logger, IFileUploadService uploadService, IWorkflowService workflowService, ISampleStatusService sampleStatusService, IProformaInvoiceRepository proformaInvoiceRepository, ILaboratoryTestRepository laboratoryTestRepository, LIMSContext context, EmailService emailService, TemplateService templateService, IPlanService planService)
        {
            _SampleInwardRepository = SampleInwardRepo;
            _logger = logger;
            _uploadService = uploadService;
            _workflowService = workflowService;
            loggedInUser = LoggedInUserProvider.CurrentUser;
            _sampleStatusService = sampleStatusService;
            _proformaInvoiceRepository = proformaInvoiceRepository;
            _context = context;
            _emailService = emailService;
            _templateService = templateService;
            _planService = planService;
        }

        public async Task<long> CreateSampleInward(SampleInwardDto model)
        {
            try
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

                    DispatchModes = model.DispatchModes.Select(d => new SampleInwardDispatchMode
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
                       Type = model.ReportingTo.Type,
                       MobileNo = model.ReportingTo.MobileNo,
                          EmailId = model.ReportingTo.EmailId
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
                       Type = model.BillingTo.Type,
                          MobileNo = model.BillingTo.MobileNo,
                              EmailId = model.BillingTo.EmailId
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
                // Validate mandatory information
                var isComplete = ValidateMandatoryInformation(entity);
                
                await _SampleInwardRepository.AddSampleInward(entity);
                _logger.LogInformation("SampleInward '{Case}' created successfully.", model.CaseNo);

                // Process queued jobs
                foreach (var job in statusJobs)
                {
                    await job();
                }

                // Set InwardStatus based on validation
                var inwardStatus = isComplete ? InwardStatus.INWARD_COMPLETED : InwardStatus.INWARD_REGISTERED;
                await _sampleStatusService.UpdateInwardStatus(entity.ID, inwardStatus, loggedInUser.EmployeeID);

                // Send acknowledgment email if complete
                //if (isComplete && entity.Contacts.Any())
                //{
                //    var emailBody = await _templateService.GetTemplateAsync(MessageTemplateKey.SAMPLE_INWARD_ACK, NotificationType.Email, new
                //    {
                //        CustomerName = entity.Customer.Name,
                //        CaseNo = entity.CaseNo
                //    });
                //    var contact = entity.Contacts.FirstOrDefault(c => !string.IsNullOrEmpty(c.EmailId));
                //    if (contact != null)
                //    {
                //        //var emailBody = $"<h3>Sample Inward Acknowledgment</h3><p>Your sample case {entity.CaseNo} has been registered successfully. All required information has been received.</p>";
                //        await _emailService.SendEmailAsync(contact.EmailId, $"Sample Inward Acknowledgment - {entity.CaseNo}", emailBody);
                //    }
                //}

                return entity.ID;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating SampleInward");
                throw;
            }
        }

        /// <summary>
        /// Validates mandatory information for sample inward
        /// Returns true if all mandatory fields are complete
        /// </summary>
        private bool ValidateMandatoryInformation(SampleInward inward)
        {
            // Check mandatory fields
            if (string.IsNullOrWhiteSpace(inward.CaseNo) ||
                inward.CustomerID == 0 ||
                string.IsNullOrWhiteSpace(inward.Address) ||
                string.IsNullOrWhiteSpace(inward.City) ||
                string.IsNullOrWhiteSpace(inward.State) ||
                string.IsNullOrWhiteSpace(inward.PinCode) ||
                string.IsNullOrWhiteSpace(inward.Country) ||
                string.IsNullOrWhiteSpace(inward.GstNo))
            {
                return false;
            }

            // Check if contacts exist
            if (!inward.Contacts.Any())
            {
                return false;
            }

            // Check if at least one contact has email
            if (!inward.Contacts.Any(c => !string.IsNullOrWhiteSpace(c.EmailId)))
            {
                return false;
            }

            // Check if sample details exist
            if (!inward.SampleDetails.Any())
            {
                return false;
            }

            return true;
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
                    entity.DispatchModes.Add(new SampleInwardDispatchMode
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
                    Type = model.ReportingTo.Type,
                    MobileNo = model.ReportingTo.MobileNo,
                    EmailId = model.ReportingTo.EmailId
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
                    Type = model.BillingTo.Type,
                    MobileNo = model.BillingTo.MobileNo,
                    EmailId = model.BillingTo.EmailId
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

                // Re-validate and update status
                var isComplete = ValidateMandatoryInformation(entity);
                var inwardStatus = isComplete ? InwardStatus.INWARD_COMPLETED : InwardStatus.INWARD_REGISTERED;
                await _sampleStatusService.UpdateInwardStatus(entity.ID, inwardStatus, loggedInUser.EmployeeID);

                // Send acknowledgment email if just completed
                if (isComplete && entity.InwardStatus != InwardStatus.INWARD_COMPLETED.ToString())
                {
                    var contact = entity.Contacts.FirstOrDefault(c => !string.IsNullOrEmpty(c.EmailId));
                    if (contact != null)
                    {
                        var emailBody = await _templateService.GetTemplateAsync(MessageTemplateKey.SAMPLE_INWARD_UPDATED, NotificationType.Email, new
                        {
                            CustomerName = entity.Customer.Name,
                            CaseNo = entity.CaseNo
                        });
                        await _emailService.SendEmailAsync(contact.EmailId, $"Sample Inward Completed - {entity.CaseNo}", emailBody);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating SampleInward ID {ID}", model.ID);
                throw;
            }
        }





        //public async Task ModifySamplePlan(PlanDto model)
        //{
        //    if (model == null) throw new ArgumentNullException(nameof(model));

        //    const string tcPrefix = "TC5098";
        //    string labLocation = "0";
        //    string year = DateTime.UtcNow.Year.ToString()[^2..];

        //    var statusJobs = new List<Func<Task>>();
        //    await using var trx = await _context.Database.BeginTransactionAsync();
        //    try
        //    {
        //        var entity = await _context.SampleInwards
        //            .Include(i => i.SampleDetails)
        //                .ThenInclude(sd => sd.TestPlans)
        //                    .ThenInclude(tp => tp.GeneralTests)
        //                        .ThenInclude(gt => gt.Methods)
        //            .Include(i => i.SampleDetails)
        //                .ThenInclude(sd => sd.TestPlans)
        //                    .ThenInclude(tp => tp.ChemicalTests)
        //                        .ThenInclude(ct => ct.Elements)
        //            .Include(i => i.SampleDetails)
        //                .ThenInclude(sd => sd.TestPlans)
        //                    .ThenInclude(tp => tp.ChemicalTests)
        //                        .ThenInclude(ct => ct.TestTypes)
        //            .FirstOrDefaultAsync(x => x.ID == model.ID);

        //        if (entity == null)
        //            throw new Exception($"Sample Inward {model.ID} not found.");

        //        entity.StatementOfConformity = model.StatementOfConformity;
        //        entity.DecisionRule = model.DecisionRule;
        //        entity.ModifiedOn = DateTime.UtcNow;

        //        // =====================================================
        //        // STEP 1: BUILD INCOMING ID SETS (AUTHORITATIVE)
        //        // =====================================================
        //        var incomingGeneralIds = model.SampleDetails
        //            .SelectMany(s => s.TestPlans)
        //            .SelectMany(p => p.GeneralTests)
        //            .Where(g => g.ID > 0)
        //            .Select(g => g.ID)
        //            .ToHashSet();

        //        var incomingChemicalIds = model.SampleDetails
        //            .SelectMany(s => s.TestPlans)
        //            .SelectMany(p => p.ChemicalTests)
        //            .Where(c => c.ID > 0)
        //            .Select(c => c.ID)
        //            .ToHashSet();

        //        // =====================================================
        //        // STEP 2: DELETE MISSING GENERAL TESTS (FIRST)
        //        // =====================================================
        //        var generalToDelete = entity.SampleDetails
        //            .SelectMany(s => s.TestPlans)
        //            .SelectMany(p => p.GeneralTests)
        //            .Where(gt => gt.ID > 0 && !incomingGeneralIds.Contains(gt.ID))
        //            .ToList();

        //        foreach (var gt in generalToDelete)
        //        {
        //            _context.GeneralTestMethods.RemoveRange(gt.Methods);
        //            _context.GeneralTests.Remove(gt);
        //        }

        //        // =====================================================
        //        // STEP 3: DELETE MISSING CHEMICAL TESTS
        //        // =====================================================
        //        var chemicalToDelete = entity.SampleDetails
        //            .SelectMany(s => s.TestPlans)
        //            .SelectMany(p => p.ChemicalTests)
        //            .Where(ct => ct.ID > 0 && !incomingChemicalIds.Contains(ct.ID))
        //            .ToList();

        //        foreach (var ct in chemicalToDelete)
        //        {
        //            _context.ChemicalTestElements.RemoveRange(ct.Elements);
        //            _context.ChemicalTestTypes.RemoveRange(ct.TestTypes);
        //            _context.ChemicalTests.Remove(ct);
        //        }


        //        // =====================================================
        //        // STEP 4: UPSERT PER SAMPLE / PER PLAN
        //        // =====================================================
        //        foreach (var sampleDto in model.SampleDetails)
        //        {
        //            var sample = entity.SampleDetails
        //                .First(s => s.SampleNo == sampleDto.SampleNo);

        //            // ---------- SAMPLE PROPERTY UPDATES (UNCHANGED) ----------
        //            sample.MetalClassificationID = sampleDto.MetalClassificationID;
        //            sample.ProductConditionID = sampleDto.ProductConditionID;
        //            sample.PreparationRequired = sampleDto.PreparationRequired;
        //            sample.MachiningRequired = sampleDto.MachiningRequired;
        //            sample.MachiningAmount = sampleDto.MachiningAmount ?? 0;
        //            sample.OtherPreparation = sampleDto.OtherPreparation;
        //            sample.OtherPreparationCharge = sampleDto.OtherPreparationCharge ?? 0;
        //            sample.TpiRequired = sampleDto.TpiRequired;
        //            sample.TpiAgencyID = sampleDto.TpiAgencyID;

        //            statusJobs.Add(async () =>
        //            {
        //                await _sampleStatusService.ForceAutoStatusAsync(
        //                    sample.ID,
        //                    SampleStatus.INWARD_COMPLETED,
        //                    loggedInUser.EmployeeID
        //                );
        //            });

        //            // =====================================================
        //            // 🔥 NEW: DELETE EXISTING PLANS FOR THIS SAMPLE
        //            // =====================================================
        //            //var existingPlans = sample.TestPlans.ToList();

        //            //_context.GeneralTestMethods.RemoveRange(
        //            //    existingPlans.SelectMany(p => p.GeneralTests)
        //            //                 .SelectMany(g => g.Methods));

        //            //_context.GeneralTests.RemoveRange(
        //            //    existingPlans.SelectMany(p => p.GeneralTests));

        //            //_context.ChemicalTestElements.RemoveRange(
        //            //    existingPlans.SelectMany(p => p.ChemicalTests)
        //            //                 .SelectMany(c => c.Elements));

        //            //_context.ChemicalTestTypes.RemoveRange(
        //            //    existingPlans.SelectMany(p => p.ChemicalTests)
        //            //                 .SelectMany(c => c.TestTypes));

        //            //_context.ChemicalTests.RemoveRange(
        //            //    existingPlans.SelectMany(p => p.ChemicalTests));

        //            //_context.TestPlans.RemoveRange(existingPlans);

        //            sample.TestPlans.Clear();

        //            // =====================================================
        //            // CREATE ONE NEW PLAN
        //            // =====================================================
        //            var planDto = sampleDto.TestPlans.FirstOrDefault();
        //            if (planDto == null) continue;

        //            var plan = new SampleTestPlan
        //            {
        //                SampleID = sample.ID,
        //                SampleNo = sample.SampleNo,
        //                GeneralTests = new List<GeneralTest>(),
        //                ChemicalTests = new List<ChemicalTest>()
        //            };

        //            sample.TestPlans.Add(plan);

        //            int ulrCounter = 1;

        //            // ---------- GENERAL TEST (ONLY ONE) ----------
        //            if (planDto.GeneralTests?.Any() == true)
        //            {
        //                var gDto = planDto.GeneralTests.First();

        //                var general = new GeneralTest
        //                {
        //                    Specification1 = gDto.Specification1,
        //                    Specification2 = gDto.Specification2,
        //                    Methods = new List<GeneralTestMethod>()
        //                };

        //                foreach (var mDto in gDto.Methods)
        //                {
        //                    general.Methods.Add(new GeneralTestMethod
        //                    {
        //                        LaboratoryTestID = mDto.TestMethodID ?? 0,
        //                        TestCaseID = mDto.TestCaseID ?? 0,
        //                        StandardID = mDto.StandardID ?? 0,
        //                        Quantity = mDto.Quantity,
        //                        ReportNo = string.IsNullOrWhiteSpace(mDto.ReportNo) ? $"{sample.SampleNo}-{ulrCounter}": mDto.ReportNo,
        //                        UlrNo = GenerateUlr(mDto.UlrNo, tcPrefix, year, labLocation, sample.SampleNo, ref ulrCounter),
        //                        Cancel = mDto.Cancel
        //                    });
        //                }

        //                plan.GeneralTests.Add(general);
        //            }

        //            // ---------- CHEMICAL TEST (ONLY ONE) ----------
        //            if (planDto.ChemicalTests?.Any() == true)
        //            {
        //                var cDto = planDto.ChemicalTests.First();

        //                var chem = new ChemicalTest
        //                {
        //                    Specification1 = cDto.Specification1,
        //                    Specification2 = cDto.Specification2,
        //                    TestMethod = cDto.TestMethod,
        //                    ReportNo = string.IsNullOrWhiteSpace(cDto.ReportNo)? $"{sample.SampleNo}-C": cDto.ReportNo,
        //                    UlrNo = GenerateUlr(cDto.UlrNo, tcPrefix, year, labLocation, sample.SampleNo, ref ulrCounter),
        //                    Elements = new List<ChemicalTestElement>(),
        //                    TestTypes = new List<ChemicalTestType>()
        //                };

        //                foreach (var e in cDto.Elements)
        //                {
        //                    chem.Elements.Add(new ChemicalTestElement
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

        //                foreach (var kvp in cDto.TestTypes)
        //                {
        //                    if (!long.TryParse(kvp.Key, out var labTestId)) continue;

        //                    var labTest = await _context.LaboratoryTests
        //                        .FirstOrDefaultAsync(x => x.ID == labTestId);

        //                    chem.TestTypes.Add(new ChemicalTestType
        //                    {
        //                        LaboratoryTestID = labTestId,
        //                        Name = labTest?.Name ?? "",
        //                        IsSelected = kvp.Value
        //                    });
        //                }

        //                plan.ChemicalTests.Add(chem);
        //            }
        //        }


        //        // =====================================================
        //        // STEP 5: DELETE EMPTY PLANS (LAST, SAFE)
        //        // =====================================================
        //        var emptyPlans = entity.SampleDetails
        //            .SelectMany(s => s.TestPlans)
        //            .Where(p => !p.GeneralTests.Any() && !p.ChemicalTests.Any())
        //            .ToList();

        //        _context.TestPlans.RemoveRange(emptyPlans);

        //        await _context.SaveChangesAsync();
        //        await trx.CommitAsync();

        //        foreach (var job in statusJobs)
        //        {
        //            await job();
        //        }
        //        await _sampleStatusService.UpdateInwardStatus(entity.ID, InwardStatus.INWARD_COMPLETED, loggedInUser.EmployeeID);
        //    }
        //    catch
        //    {
        //        await trx.RollbackAsync();
        //        throw;
        //    }
        //}

        public async Task ModifySamplePlan(PlanDto model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            const string tcPrefix = "TC5098";
            string labLocation = "0";
            string year = DateTime.UtcNow.Year.ToString()[^2..];

            var statusJobs = new List<Func<Task>>();
            await using var trx = await _context.Database.BeginTransactionAsync();

            try
            {
                var entity = await _context.SampleInwards
                    .Include(i => i.SampleDetails)
                        .ThenInclude(sd => sd.TestPlans)
                            .ThenInclude(tp => tp.GeneralTests)
                                .ThenInclude(gt => gt.Methods)
                    .Include(i => i.SampleDetails)
                        .ThenInclude(sd => sd.TestPlans)
                            .ThenInclude(tp => tp.ChemicalTests)
                                .ThenInclude(ct => ct.Elements)
                    .Include(i => i.SampleDetails)
                        .ThenInclude(sd => sd.TestPlans)
                            .ThenInclude(tp => tp.ChemicalTests)
                                .ThenInclude(ct => ct.TestTypes)
                    .FirstOrDefaultAsync(x => x.ID == model.ID);

                if (entity == null)
                    throw new Exception($"Sample Inward {model.ID} not found.");

                entity.StatementOfConformity = model.StatementOfConformity;
                entity.DecisionRule = model.DecisionRule;
                entity.ModifiedOn = DateTime.UtcNow;

                // =====================================================
                // STEP 1: AUTHORITATIVE INCOMING TEST PLAN IDS
                // =====================================================
                var incomingPlanIds = model.SampleDetails
                    .SelectMany(s => s.TestPlans)
                    .Where(p => p.ID > 0)
                    .Select(p => p.ID)
                    .ToHashSet();

                // =====================================================
                // STEP 2: DELETE ONLY NON-MATCHING TEST PLANS
                // =====================================================
                var plansToDelete = entity.SampleDetails
                    .SelectMany(s => s.TestPlans)
                    .Where(p => p.ID > 0 && !incomingPlanIds.Contains(p.ID))
                    .ToList();

                foreach (var plan in plansToDelete)
                {
                    _context.GeneralTestMethods.RemoveRange(
                        plan.GeneralTests.SelectMany(g => g.Methods));

                    _context.GeneralTests.RemoveRange(plan.GeneralTests);

                    _context.ChemicalTestElements.RemoveRange(
                        plan.ChemicalTests.SelectMany(c => c.Elements));

                    _context.ChemicalTestTypes.RemoveRange(
                        plan.ChemicalTests.SelectMany(c => c.TestTypes));

                    _context.ChemicalTests.RemoveRange(plan.ChemicalTests);

                    _context.TestPlans.Remove(plan);
                }

                // =====================================================
                // STEP 3: UPSERT PER SAMPLE
                // =====================================================
                foreach (var sampleDto in model.SampleDetails)
                {
                    var sample = entity.SampleDetails
                        .First(s => s.SampleNo == sampleDto.SampleNo);

                    sample.MetalClassificationID = sampleDto.MetalClassificationID;
                    sample.ProductConditionID = sampleDto.ProductConditionID;
                    sample.PreparationRequired = sampleDto.PreparationRequired;
                    sample.MachiningRequired = sampleDto.MachiningRequired;
                    sample.MachiningAmount = sampleDto.MachiningAmount ?? 0;
                    sample.OtherPreparation = sampleDto.OtherPreparation;
                    sample.OtherPreparationCharge = sampleDto.OtherPreparationCharge ?? 0;
                    sample.TpiRequired = sampleDto.TpiRequired;
                    sample.TpiAgencyID = sampleDto.TpiAgencyID;

                    statusJobs.Add(() =>
                        _sampleStatusService.ForceAutoStatusAsync(
                            sample.ID,
                            SampleStatus.INWARD_COMPLETED,
                            loggedInUser.EmployeeID));


                    var planDto = sampleDto.TestPlans.FirstOrDefault();
                    if (planDto == null) continue;

                    SampleTestPlan plan;

                    bool isNewPlan = false;
                    if (planDto.ID > 0)
                    {
                        plan = sample.TestPlans.First(p => p.ID == planDto.ID);
                    }
                    else
                    {
                        isNewPlan = true;
                        plan = new SampleTestPlan
                        {
                            SampleID = sample.ID,
                            SampleNo = sample.SampleNo,
                            GeneralTests = new List<GeneralTest>(),
                            ChemicalTests = new List<ChemicalTest>(),
                            Version = 1,
                            ReplanCount = 0,
                            PlanStatus = "Draft"
                        };
                        sample.TestPlans.Add(plan);
                    }

                    int ulrCounter = 1;

                    #region GENERAL TEST (UPSERT)

                    if (planDto.GeneralTests?.Any() == true)
                    {
                        var gDto = planDto.GeneralTests.First();

                        GeneralTest general;

                        if (gDto.ID > 0)
                        {
                            general = plan.GeneralTests.FirstOrDefault(g => g.ID == gDto.ID);
                            if (general == null)
                                throw new Exception($"GeneralTest ID {gDto.ID} not found");

                            general.Methods.Clear(); // resync methods
                        }
                        else
                        {
                            general = new GeneralTest
                            {
                                Methods = new List<GeneralTestMethod>()
                            };
                            plan.GeneralTests.Add(general);
                        }

                        general.Specification1 = gDto.Specification1;
                        general.Specification2 = gDto.Specification2;

                        foreach (var m in gDto.Methods)
                        {
                            general.Methods.Add(new GeneralTestMethod
                            {
                                LaboratoryTestID = m.TestMethodID ?? 0,
                                TestCaseID = m.TestCaseID,
                                Quantity = m.Quantity,
                                ReportNo = string.IsNullOrWhiteSpace(m.ReportNo)
                                    ? $"{sample.SampleNo}-{ulrCounter}"
                                    : m.ReportNo,
                                UlrNo = GenerateUlr(
                                    m.UlrNo, tcPrefix, year, labLocation,
                                    sample.SampleNo, ref ulrCounter),
                                Cancel = m.Cancel
                            });
                        }
                    }

                    #endregion

                    #region CHEMICAL TEST (UPSERT)

                    if (planDto.ChemicalTests?.Any() == true)
                    {
                        var cDto = planDto.ChemicalTests.First();

                        ChemicalTest chem;

                        if (cDto.ID > 0)
                        {
                            chem = plan.ChemicalTests.FirstOrDefault(c => c.ID == cDto.ID);
                            if (chem == null)
                                throw new Exception($"ChemicalTest ID {cDto.ID} not found");

                            chem.Elements.Clear();
                            chem.TestTypes.Clear();
                        }
                        else
                        {
                            plan.ChemicalTests.Clear(); // for safe & single chemical test
                            chem = new ChemicalTest
                            {
                                Elements = new List<ChemicalTestElement>(),
                                TestTypes = new List<ChemicalTestType>()
                            };
                            plan.ChemicalTests.Add(chem);
                        }

                        chem.Specification1 = cDto.Specification1;
                        chem.Specification2 = cDto.Specification2;
                        chem.ReportNo = string.IsNullOrWhiteSpace(cDto.ReportNo)
                            ? $"{sample.SampleNo}-C"
                            : cDto.ReportNo;
                        chem.UlrNo = GenerateUlr(
                            cDto.UlrNo, tcPrefix, year, labLocation,
                            sample.SampleNo, ref ulrCounter);

                        foreach (var e in cDto.Elements)
                        {
                            chem.Elements.Add(new ChemicalTestElement
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

                        foreach (var kvp in cDto.TestTypes)
                        {
                            if (!long.TryParse(kvp.Key, out var labTestId)) continue;

                            var labTest = await _context.LaboratoryTests
                                .FirstOrDefaultAsync(x => x.ID == labTestId);

                            chem.TestTypes.Add(new ChemicalTestType
                            {
                                LaboratoryTestID = labTestId,
                                Name = labTest?.Name ?? "",
                                IsSelected = kvp.Value
                            });
                        }
                    }

                    #endregion
                }

                await _context.SaveChangesAsync();

                // Create plan history entries after save (so new plans have IDs)
                foreach (var sampleDto in model.SampleDetails)
                {
                    var sample = entity.SampleDetails.First(s => s.SampleNo == sampleDto.SampleNo);
                    foreach (var tp in sample.TestPlans)
                    {
                        var changeType = tp.Version == 1 && tp.PlanStatus == "Draft" ? "Created" : "Updated";
                        await _planService.CreatePlanHistoryEntry(
                            tp.ID,
                            changeType,
                            null,
                            null,
                            null,
                            $"Plan {changeType.ToLower()} during planning."
                        );
                    }
                }

                await _context.SaveChangesAsync();
                await trx.CommitAsync();

                foreach (var job in statusJobs)
                    await job();

                await _sampleStatusService.UpdateInwardStatus(
                    entity.ID,
                    InwardStatus.UNDER_PLANNING,
                    loggedInUser.EmployeeID);
            }
            catch
            {
                await trx.RollbackAsync();
                throw;
            }
        }

        string GenerateUlr(string reportNo, string tcPrefix, string year, string labLocation, string sampleNo, ref int ulrCounter)
        {
            if (string.IsNullOrWhiteSpace(reportNo))
                return $"{tcPrefix}{year}{labLocation}{sampleNo.Split('-')[1].PadLeft(8, '0')}{ulrCounter++}F";
            else
            {
                ulrCounter++;
                return reportNo;
            }
        }



        public async Task SubmitPlanForReview(PlanDto model)
        {
            try
            {
                // Pre-check: Verify "Request Review" workflow exists BEFORE making any changes.
                // Without this, plan data gets saved but workflow creation fails,
                // leaving the system in an inconsistent state (PlanStatus="Submitted" with nothing to approve).
                var reviewEntityType = WorkFlowEntityTypeExtensions.GetEntityType(WorkFlowEntityType.Request_Review);
                var workflowExists = await _workflowService.WorkflowExistsForEntityType(reviewEntityType);
                if (!workflowExists)
                    throw new InvalidOperationException(
                        "Cannot submit for review: No 'Request Review' workflow is configured. " +
                        "Please ask an administrator to set up the review workflow before submitting plans.");

                await ModifySamplePlan(model);


                var entity = await _SampleInwardRepository.GetSampleInwardWithPlans(model.ID);
                if (entity == null)
                    throw new Exception("Sample Inward not found");


                entity.ReviewStatus = "Pending for Approval";
                entity.ReviewedBy = loggedInUser.EmployeeID;
                entity.ReviewedOn = DateTime.UtcNow;

                // Update PlanStatus on all test plans to Submitted
                foreach (var sample in entity.SampleDetails)
                {
                    foreach (var tp in sample.TestPlans)
                    {
                        tp.PlanStatus = "Submitted";

                        await _planService.CreatePlanHistoryEntry(
                            tp.ID,
                            "Submitted",
                            null,
                            null,
                            JsonSerializer.Serialize(new[]
                            {
                                new { field = "PlanStatus", oldValue = "Draft", newValue = "Submitted" }
                            }),
                            "Plan submitted for review."
                        );
                    }
                }
                await _context.SaveChangesAsync();

                var statusJobs = new List<Func<Task>>();

                foreach (var sample in entity.SampleDetails)
                {
                    statusJobs.Add(() =>
                        _sampleStatusService.ForceAutoStatusAsync(
                            sample.ID,
                            SampleStatus.UNDER_REVIEW_REQUEST,
                            loggedInUser.EmployeeID
                        )
                    );
                }

                await _workflowService.StartWorkflow(entity.ID, WorkFlowEntityTypeExtensions.GetEntityType(WorkFlowEntityType.Request_Review));

                await _SampleInwardRepository.UpdateSampleInward(entity);

                foreach (var job in statusJobs)
                {
                    await job();
                }
                await _sampleStatusService.UpdateInwardStatus(entity.ID, InwardStatus.UNDER_REVIEW, loggedInUser.EmployeeID);
                _logger.LogInformation("Plan submitted for review for inward {ID}", model.ID);
            }
            catch (Exception ex)
            {
                throw;
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
                        Type = a.Type,
                        MobileNo = a.MobileNo,
                        EmailId = a.EmailId
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
                        Type = a.Type,
                        MobileNo = a.MobileNo,
                        EmailId = a.EmailId
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
                //                LaboratoryTestID = m.LaboratoryTestID,
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
                        Type = a.Type,
                        MobileNo = a.MobileNo,
                        EmailId = a.EmailId
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
                        Type = a.Type,
                        MobileNo = a.MobileNo,
                        EmailId = a.EmailId
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
                        Version = tp.Version,
                        ReplanCount = tp.ReplanCount,
                        PlanStatus = tp.PlanStatus,
                        ApprovedById = tp.ApprovedById,
                        ApprovedByName = tp.ApprovedByName,
                        ApprovedAt = tp.ApprovedAt,
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
                                TestMethodID = m.LaboratoryTestID,
                                TestCaseID = m.TestCaseID,
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
                            TestTypes = ct.TestTypes.ToDictionary(tt => tt.LaboratoryTestID?.ToString() ?? tt.Name, tt => tt.IsSelected),
                            Elements = ct.Elements.Select(e => new ChemicalTestElementDto
                            {
                                ID = e.ID,
                                ParameterUnitID = e.ParameterUnitID,
                                ChemicalTestID = e.ChemicalTestID,
                                ParameterID = e.ParameterID,
                                Selected = e.Selected,
                                SpecificationLineID = e.SpecificationLineID
                            }).ToList()
                        }).ToList()
                    }))
                    .ToList()
            };

            var step = await _workflowService.GetCurrentWorkflowStepAsync(sampleInward.ID, WorkFlowEntityTypeExtensions.GetEntityType(WorkFlowEntityType.Request_Review));
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
                        .GetActiveInstanceForEntityAsync(sampleInward.ID, WorkFlowEntityTypeExtensions.GetEntityType(WorkFlowEntityType.Request_Review));

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
