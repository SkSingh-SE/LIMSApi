using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.ServiceWORepo
{
    public interface IPaymentService
    {
        Task<object> ProcessPaymentAsync(PaymentRequestDto dto);

        Task<PaymentTokenValidationResponse> ValidateTokenAsync(string token);

        Task SendPaymentLinkAsync(long paymentOrderId);

        Task SettlePaymentAsync(PaymentOrder order);

        string GeneratePaymentLink(string token);
    }

}
