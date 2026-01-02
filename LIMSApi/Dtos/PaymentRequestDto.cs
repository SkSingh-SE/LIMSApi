using LIMSApi.Helpers.Enums;

namespace LIMSApi.Dtos
{
    public class PaymentRequestDto
    {
        public PaymentType PaymentType { get; set; }

        public long? InwardID { get; set; }
        public string? CaseNo { get; set; }

        public long? SampleID { get; set; }
        public long? ReportID { get; set; }

        public long CustomerID { get; set; }
        public decimal Amount { get; set; }

        // Razorpay response
        public string? RazorpayOrderId { get; set; }
        public string? RazorpayPaymentId { get; set; }
        public string? RazorpaySignature { get; set; }

        public string? PaymentToken { get; set; }
        public long? TaxInvoiceID { get; set; } 
    }


    public class PaymentTokenValidationResponse
    {
        public bool IsValid { get; set; }

        public string? Message { get; set; }

        public string? OrderNo { get; set; }
        public PaymentType? PaymentType { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";

        public string? RazorpayOrderId { get; set; }
        public string? RazorpayKey { get; set; }

        public string? CustomerName { get; set; }
    }

    public class RazorpayWebhookPayload
    {
        public string Event { get; set; } = null!;
        public RazorpayPayload Payload { get; set; } = null!;
    }


    public class RazorpayPayload
    {
        public RazorpayPaymentWrapper Payment { get; set; } = null!;
    }

    public class RazorpayPaymentWrapper
    {
        public RazorpayPaymentEntity Entity { get; set; } = null!;
    }

    public class RazorpayPaymentEntity
    {
        public string Id { get; set; } = null!;

        // Razorpay uses snake_case
        public string Order_Id { get; set; } = null!;

        public string Status { get; set; } = null!;
        public long Amount { get; set; }
        public bool Captured { get; set; }

        public Dictionary<string, string>? Notes { get; set; }
    }



}
