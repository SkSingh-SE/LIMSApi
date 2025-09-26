using System.Xml.Linq;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Migrations;
using LIMSApi.Models;
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

        public SampleInwardService(ISampleInwardRepository SampleInwardRepo, ILogger<SampleInwardService> logger, IFileUploadService uploadService,IWorkflowService workflowService)
        {
            _SampleInwardRepository = SampleInwardRepo;
            _logger = logger;
            _uploadService = uploadService;
            _workflowService = workflowService;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateSampleInward(SampleInwardDto model)
        {

            dynamic caseAndSample = await _SampleInwardRepository.GetCaseNoAndSampleNo();

            if (caseAndSample == null || !caseAndSample.GetType().GetProperty("nextSampleCounter")?.CanRead == true)
            {
                throw new InvalidOperationException("Invalid case and sample data returned.");
            }

            int nextSampleNumber = caseAndSample.nextSampleCounter;

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
                Status = model.Status,
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
                    Nature = s.Nature,
                    Category = s.Category,
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
                }
            }
            await _SampleInwardRepository.AddSampleInward(entity);
            _logger.LogInformation("SampleInward '{Case}' created successfully.", model.CaseNo);
        }

        public async Task ModifySampleInward(SampleInwardDto model)
        {
            if (model.ID == 0)
                throw new ArgumentException("SampleInward ID should not be empty!");

            var entity = await _SampleInwardRepository.GetSampleInwardWithPlans(model.ID);
            if (entity == null)
                throw new Exception("Sample Inward not found");

            //  Update scalar properties
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
            entity.Status = model.Status;

            //  Handle request file update
            if (model.File != null)
            {
                var fileUploadResponse = await _uploadService.UploadFileAsync(model.File, FileType.Other, null, model.RequestFileName);
                if (fileUploadResponse == null)
                    throw new InvalidOperationException("File upload failed!");

                entity.RequestFilePath = fileUploadResponse.FilePath;
                entity.RequestFileName = fileUploadResponse.OriginalFileName;
                entity.UploadReferenceID = fileUploadResponse.ID;
            }

            //  Sync DispatchModes
            entity.DispatchModes.Clear();
            foreach (var d in model.DispatchModes)
            {
                entity.DispatchModes.Add(new SampleDispatchMode
                {
                    InwardID = entity.ID,
                    DispatchModeID = d.DispatchModeID
                });
            }

            //  Sync Contacts
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

            //  Sync Addresses
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

            //  Prepare SampleNo generator
            dynamic caseAndSample = await _SampleInwardRepository.GetCaseNoAndSampleNo();
            int nextSampleNumber = caseAndSample.nextSampleCounter;
            var year = DateTime.UtcNow.Year.ToString().Substring(2, 2);

            //  Sync SampleDetails
            foreach (var s in model.SampleDetails)
            {
                // find existing sample
                var existingSample = entity.SampleDetails.FirstOrDefault(x => x.SampleNo == s.SampleNo);

                if (existingSample != null)
                {
                    // update existing sample
                    existingSample.Details = s.Details;
                    existingSample.Nature = s.Nature;
                    existingSample.Category = s.Category;
                    existingSample.Remarks = s.Remarks;
                    existingSample.Quantity = s.Quantity;
                    existingSample.Specimen = s.Specimen;
                    existingSample.TestInstructions = s.TestInstructions;

                    // handle file update
                    if (s.File != null)
                    {
                        var fileUploadResponse = await _uploadService.UploadFileAsync(s.File, FileType.Other, null, s.FileName);
                        if (fileUploadResponse == null)
                            throw new InvalidOperationException($"File upload failed for sample {existingSample.SampleNo}");

                        existingSample.SampleFilePath = fileUploadResponse.FilePath;
                        existingSample.FileName = fileUploadResponse.OriginalFileName;
                        existingSample.UploadReferenceID = fileUploadResponse.ID;
                    }

                    // update additional details
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

                    //  Update Test Plans if provided
                    if (model.SampleTestPlans.Any(tp => tp.SampleNo == s.SampleNo))
                    {
                        existingSample.TestPlans.Clear();
                        string tcPrefix = "TC5098";
                        string labLocation = "0";
                        int ulrCounter = 1;

                        foreach (var tp in model.SampleTestPlans.Where(tp => tp.SampleNo == s.SampleNo))
                        {
                            var plan = new SampleTestPlan { SampleNo = tp.SampleNo };

                            // general tests
                            foreach (var g in tp.GeneralTests.Select((g, idx) => new { g, idx }))
                            {
                                var general = new GeneralTest
                                {
                                    Specification1 = g.g.Specification1,
                                    Specification2 = g.g.Specification2
                                };

                                foreach (var m in g.g.Methods)
                                {
                                    general.Methods.Add(new GeneralTestMethod
                                    {
                                        TestMethodID = m.TestMethodID ?? 0,
                                        StandardID = m.StandardID ?? 0,
                                        Quantity = m.Quantity,
                                        ReportNo = $"{tp.SampleNo}-{g.idx}",
                                        UlrNo = $"{tcPrefix}{year}{labLocation}{tp.SampleNo.Split('-')[1].PadLeft(8, '0')}{ulrCounter++}F",
                                        Cancel = m.Cancel
                                    });
                                }

                                plan.GeneralTests.Add(general);
                            }

                            // chemical tests
                            foreach (var c in tp.ChemicalTests.Select((c, idx) => new { c, idx }))
                            {
                                var chemical = new ChemicalTest
                                {
                                    ReportNo = $"{tp.SampleNo}-{c.idx}",
                                    UlrNo = $"{tcPrefix}{year}{labLocation}{tp.SampleNo.Split('-')[1].PadLeft(8, '0')}{ulrCounter++}F",
                                    MetalClassificationID = c.c.MetalClassificationID,
                                    Specification1 = c.c.Specification1,
                                    Specification2 = c.c.Specification2,
                                    TestMethod = Convert.ToInt64(c.c.TestMethod)
                                };

                                foreach (var e in c.c.Elements)
                                {
                                    chemical.Elements.Add(new ChemicalTestElement
                                    {
                                        ParameterID = e.ParameterID
                                    });
                                }

                                plan.ChemicalTests.Add(chemical);
                            }

                            existingSample.TestPlans.Add(plan);
                        }
                    }
                }
                else
                {
                    // add new sample
                    var newSample = new SampleDetail
                    {
                        SampleNo = string.IsNullOrEmpty(s.SampleNo)
                                    ? $"{year}-{nextSampleNumber++:D6}"
                                    : s.SampleNo,
                        Details = s.Details,
                        Nature = s.Nature,
                        Category = s.Category,
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
                            SampleNo = newSample.SampleNo,
                            Label = a.Label,
                            Value = a.Value
                        });
                    }

                    //  Add Test Plans if provided
                    if (model.SampleTestPlans.Any(tp => tp.SampleNo == newSample.SampleNo))
                    {
                        string tcPrefix = "TC5098";
                        string labLocation = "0";
                        int ulrCounter = 1;

                        foreach (var tp in model.SampleTestPlans.Where(tp => tp.SampleNo == newSample.SampleNo))
                        {
                            var plan = new SampleTestPlan { SampleNo = tp.SampleNo };

                            // general tests
                            foreach (var g in tp.GeneralTests.Select((g, idx) => new { g, idx }))
                            {
                                var general = new GeneralTest
                                {
                                    Specification1 = g.g.Specification1,
                                    Specification2 = g.g.Specification2
                                };

                                foreach (var m in g.g.Methods)
                                {
                                    general.Methods.Add(new GeneralTestMethod
                                    {
                                        TestMethodID = m.TestMethodID ?? 0,
                                        StandardID = m.StandardID ?? 0,
                                        Quantity = m.Quantity,
                                        ReportNo = $"{tp.SampleNo}-{g.idx}",
                                        UlrNo = $"{tcPrefix}{year}{labLocation}{tp.SampleNo.Split('-')[1].PadLeft(8, '0')}{ulrCounter++}F",
                                        Cancel = m.Cancel
                                    });
                                }

                                plan.GeneralTests.Add(general);
                            }

                            // chemical tests
                            foreach (var c in tp.ChemicalTests.Select((c, idx) => new { c, idx }))
                            {
                                var chemical = new ChemicalTest
                                {
                                    ReportNo = $"{tp.SampleNo}-{c.idx}",
                                    UlrNo = $"{tcPrefix}{year}{labLocation}{tp.SampleNo.Split('-')[1].PadLeft(8, '0')}{ulrCounter++}F",
                                    MetalClassificationID = c.c.MetalClassificationID,
                                    Specification1 = c.c.Specification1,
                                    Specification2 = c.c.Specification2,
                                    TestMethod = Convert.ToInt64(c.c.TestMethod)
                                };

                                foreach (var e in c.c.Elements)
                                {
                                    chemical.Elements.Add(new ChemicalTestElement
                                    {
                                        ParameterID = e.ParameterID
                                    });
                                }

                                plan.ChemicalTests.Add(chemical);
                            }

                            newSample.TestPlans.Add(plan);
                        }
                    }

                    entity.SampleDetails.Add(newSample);
                }
            }

            await _SampleInwardRepository.UpdateSampleInward(entity);
            _logger.LogInformation("SampleInward '{Case}' updated successfully with samples & plans.", entity.CaseNo);
        }



        public async Task ModifySamplePlan(PlanDto model)
        {
            var entity = await _SampleInwardRepository.GetSampleInwardWithPlans(model.ID);
            if (entity == null)
                throw new Exception("Sample Inward not found");

            // update review info
            entity.StatementOfConformity = model.StatementOfConformity;
            entity.DecisionRule = model.DecisionRule;
            entity.ReviewStatus = model.ReviewStatus ?? "Reviewed";
            entity.ReviewedBy = loggedInUser.EmployeeID;
            entity.ReviewedOn = DateTime.UtcNow;

            string tcPrefix = "TC5098";
            string labLocation = "0";
            string year = DateTime.UtcNow.Year.ToString().Substring(2, 2);

            foreach (var sampleDto in model.SampleDetails)
            {
                var sample = entity.SampleDetails.FirstOrDefault(s => s.SampleNo == sampleDto.SampleNo);
                if (sample == null)
                    throw new Exception($"Sample '{sampleDto.SampleNo}' not found in inward {model.ID}");

                // update prep fields
                sample.CuttingRequired = sampleDto.CuttingRequired;
                sample.MachiningRequired = sampleDto.MachiningRequired;
                sample.MachiningAmount = sampleDto.MachiningAmount ?? 0;
                sample.OtherPreparation = sampleDto.OtherPreparation;
                sample.OtherPreparationCharge = sampleDto.OtherPreparationCharge ?? 0;
                sample.TpiRequired = sampleDto.TpiRequired;

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
                            StandardID = m.StandardID ?? 0,
                            Quantity = m.Quantity,
                            ReportNo = string.IsNullOrEmpty(m.ReportNo)
                                        ? existingGeneral.Methods.FirstOrDefault(x => x.TestMethodID == m.TestMethodID)?.ReportNo
                                          ?? $"{sample.SampleNo}-{g.idx}"
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

                    existingChem.MetalClassificationID = c.c.MetalClassificationID;
                    existingChem.Specification1 = c.c.Specification1;
                    existingChem.Specification2 = c.c.Specification2;
                    existingChem.TestMethod = c.c.TestMethod;

                    // sync elements
                    existingChem.Elements.Clear();
                    foreach (var e in c.c.Elements)
                    {
                        existingChem.Elements.Add(new ChemicalTestElement
                        {
                            ParameterID = e.ParameterID
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
            await _workflowService.StartWorkflow(entity.ID, "Request of Review");
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
                Status = sampleInward.Status,
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
                        Nature = s.Nature,
                        Category = s.Category,
                        Remarks = s.Remarks,
                        Quantity = s.Quantity,
                        UploadReferenceID = s.UploadReferenceID,
                        SampleFilePath = s.SampleFilePath,
                        FileName = s.FileName
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
                        SampleNo = tp.SampleNo,
                        GeneralTests = tp.GeneralTests.Select(gt => new GeneralTestDto
                        {
                            Specification1 = gt.Specification1,
                            Specification2 = gt.Specification2,
                            Methods = gt.Methods.Select(m => new GeneralTestMethodDto
                            {
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
                            ReportNo = ct.ReportNo,
                            UlrNo = ct.UlrNo,
                            MetalClassificationID = ct.MetalClassificationID,
                            Specification1 = ct.Specification1,
                            Specification2 = ct.Specification2,
                            TestMethod = ct.TestMethod,
                            Elements = ct.Elements.Select(e => new ChemicalTestElementDto
                            {
                                ParameterID = e.ParameterID
                            }).ToList()
                        }).ToList()
                    }))
                    .ToList()
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
                Status = sampleInward.Status,
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
                        ID= c.ID,
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
                        Nature = s.Nature,
                        Category = s.Category,
                        Remarks = s.Remarks,
                        Quantity = s.Quantity,
                        UploadReferenceID = s.UploadReferenceID,
                        SampleFilePath = s.SampleFilePath,
                        FileName = s.FileName,
                        CuttingRequired = s.CuttingRequired,
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
                            MetalClassificationID = ct.MetalClassificationID,
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

            return dto;
        }


        public async Task<PagedResponse<object>> FetchSampleInwardList(PageFilter filter)
        {
            return await _SampleInwardRepository.GetAllSampleInwards(filter);
        }

        public async Task<object> GetCaseNoAndSampleNo()
        {
            var caseNumber = await _SampleInwardRepository.GetCaseNoAndSampleNo();
            if (caseNumber == null)
                throw new InvalidOperationException("No case number found!");
            return caseNumber;
        }
    }
}
