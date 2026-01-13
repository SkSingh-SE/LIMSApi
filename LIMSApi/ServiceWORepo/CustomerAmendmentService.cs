using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.ServiceWORepo
{
    public class CustomerAmendmentService : ICustomerAmendmentService
    {
        private readonly LIMSContext _db;
        private readonly IPaymentService _paymentService;
        private readonly Services.Interface.IPriceCalculationService _priceCalculationService;
        private readonly ILogger<CustomerAmendmentService> _logger;

        public CustomerAmendmentService(LIMSContext db, IPaymentService paymentService, Services.Interface.IPriceCalculationService priceCalculationService, ILogger<CustomerAmendmentService> logger)
        {
            _db = db;
            _paymentService = paymentService;
            _priceCalculationService = priceCalculationService;
            _logger = logger;
        }

        // ---------------------------------------------
        // Validate Public Amendment Token
        // ---------------------------------------------
        public async Task<AmendmentTokenValidationResponse> ValidateTokenAsync(Guid token)
        {
            var tokenEntity = await _db.ReportAmendmentTokens
                .Include(t => t.Report)
                    .ThenInclude(r => r.ReportHeader)
                        .ThenInclude(h => h.Sample)
                .FirstOrDefaultAsync(t => t.Token == token);

            if (tokenEntity == null)
                return Invalid("Invalid amendment link.");

            if (tokenEntity.IsUsed)
                return Invalid("This amendment link has already been used.");

            if (tokenEntity.LinkExpiryOn < DateTime.UtcNow)
                return Invalid("This amendment link has expired.");

            return new AmendmentTokenValidationResponse
            {
                IsValid = true,
                CustomerName = tokenEntity.Report.ReportHeader.CustomerName,
                ReportNo = tokenEntity.Report.ReportHeader.ReportNo,
                SampleNo = tokenEntity.Report.ReportHeader.Sample?.SampleNo
            };
        }


        // ---------------------------------------------
        // Create Customer Amendment Request
        // ---------------------------------------------
        public async Task<long> CreateAmendmentRequestAsync(CustomerAmendmentRequestDto dto)
        {
            using var tx = await _db.Database.BeginTransactionAsync();

            var tokenEntity = await _db.ReportAmendmentTokens
                .Include(t => t.Report)
                    .ThenInclude(r => r.ReportHeader)
                        .ThenInclude(h => h.Sample)
                            .ThenInclude(s => s.SampleInward)
                                .ThenInclude(i => i.Customer)
                .FirstOrDefaultAsync(t => t.Token == dto.Token);

            if (tokenEntity == null)
                throw new InvalidOperationException("Invalid amendment token.");

            if (tokenEntity.IsUsed)
                throw new InvalidOperationException("Amendment link already used.");

            if (tokenEntity.LinkExpiryOn < DateTime.UtcNow)
                throw new InvalidOperationException("Amendment link expired.");

            var amendment = new CustomerAmendment
            {
                ReportID = tokenEntity.ReportID,
                TokenID = tokenEntity.ID,
                CustomerName = dto.CustomerName,
                Reason = dto.Reason,
                Remarks = dto.Remarks,
                Status = "Pending",
                CreatedOn = DateTime.UtcNow
            };

            _db.CustomerAmendments.Add(amendment);
            tokenEntity.IsUsed = true;

            await _db.SaveChangesAsync();

            // -----------------------------------------
            // 💰 FREE vs PAID DECISION (CORRECT LOGIC)
            // -----------------------------------------
            var isChargeable = DateTime.UtcNow > tokenEntity.FreeUntil;

            if (!isChargeable)
            {
                amendment.Status = "Approved";
                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return amendment.ID;
            }

            // -----------------------------------------
            // 💳 PAID AMENDMENT FLOW
            // -----------------------------------------
            var config = await _db.Configurations
                .FirstOrDefaultAsync(c => c.KeyName == "AmendmentChargeableAmount");

            var amount = config != null ? Convert.ToDecimal(config.Value) : 0;

            if (amount <= 0)
                throw new InvalidOperationException("Amendment amount configuration missing.");

            var customer = tokenEntity.Report.ReportHeader.Sample
                ?.SampleInward?.Customer
                ?? throw new InvalidOperationException("Customer not found.");

            var paymentRequest = new PaymentRequestDto
            {
                PaymentType = PaymentType.AmendmentInvoice,
                ReportID = amendment.ReportID,
                Amount = amount,
                CustomerID = customer.ID
            };

            var paymentResult = await _paymentService.ProcessPaymentAsync(paymentRequest);

            var paymentToken = paymentResult.GetType()
                .GetProperty("paymentToken")?.GetValue(paymentResult)?.ToString()
                ?? throw new InvalidOperationException("Payment creation failed.");

            var paymentOrder = await _db.PaymentOrders
                .FirstAsync(x => x.PaymentToken == paymentToken);

            amendment.IsChargeable = true;
            amendment.Amount = amount;
            amendment.PaymentOrderID = paymentOrder.ID;
            amendment.Status = "PaymentPending";

            // Create ChargeEvent for amendment
            var inward = tokenEntity.Report.ReportHeader.Sample?.SampleInward;
            if (inward != null)
            {
                var chargeEvent = new ChargeEvent
                {
                    InwardID = inward.ID,
                    ReportID = amendment.ReportID,
                    ChargeType = "Amendment",
                    Description = $"Report Amendment - {tokenEntity.Report.ReportHeader.ReportNo}",
                    Quantity = 1,
                    Rate = amount,
                    Amount = amount,
                    Status = Helpers.Enums.ChargeEventStatus.DRAFT.ToString(),
                    CreatedOn = DateTime.UtcNow
                };
                _db.ChargeEvents.Add(chargeEvent);
            }

            await _db.SaveChangesAsync();
            await _paymentService.SendPaymentLinkAsync(paymentOrder.ID);

            await tx.CommitAsync();
            return amendment.ID;
        }



        private static AmendmentTokenValidationResponse Invalid(string message)
        {
            return new AmendmentTokenValidationResponse
            {
                IsValid = false,
                Message = message
            };
        }
    }

}
