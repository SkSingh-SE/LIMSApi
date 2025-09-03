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

            var entity = await _SampleInwardRepository.GetSampleInwardById(model.ID);

            if (entity == null)
                throw new Exception("Sample Inward not found");

            //  Update scalar properties (CaseNo should remain unchanged once generated)
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
                    DispatchModeID = d.DispatchModeID
                });
            }

            //  Sync Contacts
            entity.Contacts.Clear();
            foreach (var c in model.Contacts)
            {
                entity.Contacts.Add(new SampleInwardContactPerson
                {
                    ContactID = c.ContactID,
                    Name = c.Name,
                    MobileNo = c.MobileNo,
                    EmailId = c.EmailId,
                    SendBill = c.SendBill,
                    SendReport = c.SendReport,
                    Selected = c.Selected
                });
            }

            //  Sync Addresses (Reporting + Billing)
            entity.Addresses.Clear();
            entity.Addresses.Add(new SampleInwardAddressInfo
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
            });
            entity.Addresses.Add(new SampleInwardAddressInfo
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
            });

            //  Sync SampleDetails + AdditionalDetails
            entity.SampleDetails.Clear();

            // fetch next sample number for new samples
            dynamic caseAndSample = await _SampleInwardRepository.GetCaseNoAndSampleNo();
            if (caseAndSample == null || !caseAndSample.GetType().GetProperty("nextSampleCounter")?.CanRead == true)
            {
                throw new InvalidOperationException("Invalid case and sample data returned.");
            }

            int nextSampleNumber = caseAndSample.nextSampleCounter;

            var year = DateTime.UtcNow.Year.ToString().Substring(2, 2);

            foreach (var s in model.SampleDetails.Select((s, idx) => new { s, idx }))
            {
                var sampleDetail = new SampleDetail
                {
                    // keep existing SampleNo if passed, else auto-generate
                    SampleNo = string.IsNullOrEmpty(s.s.SampleNo)
                                ? $"{year}-{(nextSampleNumber + s.idx):D6}"
                                : s.s.SampleNo,
                    Details = s.s.Details,
                    Nature = s.s.Nature,
                    Category = s.s.Category,
                    Remarks = s.s.Remarks,
                    Quantity = s.s.Quantity,
                    UploadReferenceID = s.s.UploadReferenceID,
                    SampleFilePath = s.s.SampleFilePath,
                    FileName = s.s.FileName
                };

                //  Handle sample file upload if new file provided
                if (s.s.File != null)
                {
                    var fileUploadResponse = await _uploadService.UploadFileAsync(s.s.File, FileType.Other, null, s.s.FileName);
                    if (fileUploadResponse == null)
                        throw new InvalidOperationException($"File upload failed for sample {sampleDetail.SampleNo}");

                    sampleDetail.SampleFilePath = fileUploadResponse.FilePath;
                    sampleDetail.FileName = fileUploadResponse.OriginalFileName;
                    sampleDetail.UploadReferenceID = fileUploadResponse.ID;
                }

                //  Add Additional Details
                foreach (var a in model.SampleAdditionalDetails.Where(a => a.SampleNo == s.s.SampleNo))
                {
                    sampleDetail.AdditionalDetails.Add(new SampleAdditionalDetail
                    {
                        SampleNo = sampleDetail.SampleNo, // ensure same as assigned
                        Label = a.Label,
                        Value = a.Value
                    });
                }

                entity.SampleDetails.Add(sampleDetail);
            }

            await _SampleInwardRepository.UpdateSampleInward(entity);
            _logger.LogInformation("SampleInward '{Case}' updated successfully.", entity.CaseNo);
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

            List<SampleAdditionalDetailDto> SampleAdditionalDetails = new List<SampleAdditionalDetailDto>();
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
            }).ToList()
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
