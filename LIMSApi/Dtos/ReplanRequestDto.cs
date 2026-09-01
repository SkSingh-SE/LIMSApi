namespace LIMSApi.Dtos
{
    public class ReplanRequestDto
    {
        public long InwardId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class ReplanApprovalDto
    {
        public long ReplanRequestId { get; set; }
        public string? Remarks { get; set; }
    }
}

