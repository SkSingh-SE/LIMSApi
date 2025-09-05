using System.Xml.Linq;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Services
{
    public class SampleInwardService : ISampleInwardService
    {
        private readonly ISampleInwardRepository _SampleInwardRepository;
        private readonly ILogger<SampleInwardService> _logger;
        private readonly IFileUploadService _uploadService;

        public SampleInwardService(ISampleInwardRepository SampleInwardRepo, ILogger<SampleInwardService> logger, IFileUploadService uploadService)
        {
            _SampleInwardRepository = SampleInwardRepo;
            _logger = logger;
            _uploadService = uploadService;
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
                                    Specification2 = g.g.Specification2,
                                    Parameter = g.g.Parameter
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
                                    Specification2 = g.g.Specification2,
                                    Parameter = g.g.Parameter
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



        public async Task ModifySamplePlan(SampleInwardDto model)
        {
            var entity = await _SampleInwardRepository.GetSampleInwardWithPlans(model.ID);
            if (entity == null)
                throw new Exception("Sample Inward not found");

            entity.StatementOfConformity = model.StatementOfConformity;
            entity.DecisionRule = model.DecisionRule;

            string tcPrefix = "TC5098";
            string labLocation = "0";
            string year = DateTime.UtcNow.Year.ToString().Substring(2, 2);

            foreach (var sampleDto in model.SampleDetails)
            {
                var sample = entity.SampleDetails.FirstOrDefault(s => s.SampleNo == sampleDto.SampleNo);
                if (sample == null)
                    throw new Exception($"Sample '{sampleDto.SampleNo}' not found in inward {model.ID}");

                // update sample preparation fields
                sample.CuttingRequired = sampleDto.CuttingRequired;
                sample.MachiningRequired = sampleDto.MachiningRequired;
                sample.MachiningAmount = sampleDto.MachiningAmount;
                sample.OtherPreparation = sampleDto.OtherPreparation;
                sample.OtherPreparationCharge = sampleDto.OtherPreparationCharge;
                sample.TpiRequired = sampleDto.TpiRequired;

                // find or create test plan
                var existingPlan = sample.TestPlans.FirstOrDefault();
                if (existingPlan == null)
                {
                    existingPlan = new SampleTestPlan { SampleNo = sampleDto.SampleNo };
                    sample.TestPlans.Add(existingPlan);
                }
                else
                {
                    // clear existing plan
                    existingPlan.GeneralTests.Clear();
                    existingPlan.ChemicalTests.Clear();
                }

                int ulrCounter = 1;

                // add new general tests
                foreach (var g in sampleDto.TestPlans.SelectMany(p => p.GeneralTests).Select((g, idx) => new { g, idx }))
                {
                    var generalTest = new GeneralTest
                    {
                        Specification1 = g.g.Specification1,
                        Specification2 = g.g.Specification2
                    };

                    foreach (var m in g.g.Methods)
                    {
                        generalTest.Methods.Add(new GeneralTestMethod
                        {
                            TestMethodID = m.TestMethodID ?? 0,
                            StandardID = m.StandardID ?? 0,
                            Quantity = m.Quantity,
                            ReportNo = string.IsNullOrEmpty(m.ReportNo)
                                        ? $"{sample.SampleNo}-{g.idx}"
                                        : m.ReportNo,
                            UlrNo = string.IsNullOrEmpty(m.UlrNo)
                                        ? $"{tcPrefix}{year}{labLocation}{sample.SampleNo.Split('-')[1].PadLeft(8, '0')}{ulrCounter++}F"
                                        : m.UlrNo,
                            Cancel = m.Cancel
                        });
                    }

                    existingPlan.GeneralTests.Add(generalTest);
                }

                // add new chemical tests
                foreach (var c in sampleDto.TestPlans.SelectMany(p => p.ChemicalTests).Select((c, idx) => new { c, idx }))
                {
                    var chemicalTest = new ChemicalTest
                    {
                        ReportNo = string.IsNullOrEmpty(c.c.ReportNo)
                                    ? $"{sample.SampleNo}-{c.idx}"
                                    : c.c.ReportNo,
                        UlrNo = string.IsNullOrEmpty(c.c.UlrNo)
                                    ? $"{tcPrefix}{year}{labLocation}{sample.SampleNo.Split('-')[1].PadLeft(8, '0')}{ulrCounter++}F"
                                    : c.c.UlrNo,
                        MetalClassificationID = c.c.MetalClassificationID,
                        Specification1 = c.c.Specification1,
                        Specification2 = c.c.Specification2,
                        TestMethod = c.c.TestMethod
                    };

                    foreach (var e in c.c.Elements)
                    {
                        chemicalTest.Elements.Add(new ChemicalTestElement
                        {
                            ParameterID = e.ParameterID
                        });
                    }

                    existingPlan.ChemicalTests.Add(chemicalTest);
                }
            }

            await _SampleInwardRepository.UpdateSampleInward(entity);
            _logger.LogInformation("Plans and sample prep updated for Inward '{InwardID}'", model.ID);
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

                // 🔹 NEW: Sample Test Plans
                SampleTestPlans = sampleInward.SampleDetails
                    .Where(s => s.TestPlans.Any())
                    .SelectMany(s => s.TestPlans.Select(tp => new SampleTestPlanDto
                    {
                        SampleNo = tp.SampleNo,
                        GeneralTests = tp.GeneralTests.Select(gt => new GeneralTestDto
                        {
                            Specification1 = gt.Specification1,
                            Specification2 = gt.Specification2,
                            Parameter = gt.Parameter,
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
