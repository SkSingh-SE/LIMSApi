using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML.Voice;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SampleInwardController : ControllerBase
    {
        private readonly ISampleInwardService _SampleInwardService;

        public SampleInwardController(ISampleInwardService SampleInwardServce)
        {
            _SampleInwardService = SampleInwardServce;
        }

        [HttpPost("list")]
        public async Task<IActionResult> SampleInwardList(PageFilter filter)
        {
            return Ok(await _SampleInwardService.FetchSampleInwardList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<SampleInward>> GetSampleInward(long id)
        {
            var entity = await _SampleInwardService.GetSampleInwardDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutSampleInward(SampleInward model)
        {
            await _SampleInwardService.ModifySampleInward(model);
            return Ok(new
            {
                status = "success",
                message = $"SampleInward updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<SampleInward>> PostSampleInward([FromForm] SampleInwardDto model)
        {
            var entity = new SampleInward
            {
                CaseNo = model.CaseNo,
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
                        ContactPersonID= model.ReportingTo.ContactPersonID,
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
                        ContactPersonID= model.BillingTo.ContactPersonID,
                        Address = model.BillingTo.Address,
                        PinCode = model.BillingTo.PinCode,
                        Area = model.BillingTo.Area,
                        City = model.BillingTo.City,
                        State = model.BillingTo.State,
                        Country = model.BillingTo.Country,
                        Type = model.BillingTo.Type
                    }
                },


                SampleDetails = model.SampleDetails.Select(s => new SampleDetail
                {
                    SampleNo = s.SampleNo,
                    Details = s.Details,
                    Nature = s.Nature,
                    Category = s.Category,
                    Remarks = s.Remarks,
                    Quantity = s.Quantity,
                    UploadReferenceID = s.UploadReferenceID,
                    SampleFilePath = s.SampleFilePath,
                    FileName = s.FileName,
                    File = s.File,

                    // 🔑 Attach AdditionalDetails filtered by SampleNo
                    AdditionalDetails = model.SampleAdditionalDetails
                     .Where(a => a.SampleNo == s.SampleNo)
                     .Select(a => new SampleAdditionalDetail
                     {
                         SampleNo = a.SampleNo,
                         Label = a.Label,
                         Value = a.Value
                     }).ToList()
                }).ToList()

            };


            await _SampleInwardService.CreateSampleInward(entity);
            return Ok(new
            {
                status = "success",
                message = $"SampleInward created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSampleInward(long id)
        {
            var entity = await _SampleInwardService.GetSampleInwardDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("SampleInward not found!");
            }
            await _SampleInwardService.RemoveSampleInward(id);
            return Ok(new
            {
                status = "success",
                message = $"SampleInward deleted successfully."
            });
        }

        [HttpGet("case-number")]
        public async Task<IActionResult> GetCaseNoAndSampleNo()
        {
            var caseNumber = await _SampleInwardService.GetCaseNoAndSampleNo();

            return caseNumber == null ? NoContent() : Ok(caseNumber);
        }
    }
}
