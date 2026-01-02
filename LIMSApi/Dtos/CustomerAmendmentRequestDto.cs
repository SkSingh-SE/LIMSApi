namespace LIMSApi.Dtos
{
    public class CustomerAmendmentRequestDto
    {
        public Guid Token { get; set; }

        public string CustomerName { get; set; }
        public string Reason { get; set; }
        public string? Remarks { get; set; }
    }


    public class AmendmentTokenValidationResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }

        public string CustomerName { get; set; }
        public string ReportNo { get; set; }
        public string SampleNo { get; set; }
    }

}
