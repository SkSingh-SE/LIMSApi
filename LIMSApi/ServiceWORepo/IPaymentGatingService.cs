using LIMSApi.Dtos;

namespace LIMSApi.ServiceWORepo
{
    public interface IPaymentGatingService
    {
        /// <summary>
        /// Checks whether testing is gated (blocked) for a given sample inward.
        /// Walk-in: hard-blocked until payment received.
        /// Credit: warns if outstanding exceeds credit limit.
        /// PO: warns if PO balance is insufficient.
        /// </summary>
        Task<PaymentGateResult> CheckPaymentGateAsync(long sampleInwardId);

        /// <summary>
        /// Checks a customer's outstanding balance against their credit limit.
        /// Used at inward creation time to prevent accepting samples from over-limit customers.
        /// </summary>
        Task<CreditCheckResult> CheckCreditLimitAsync(long customerId);
    }
}
