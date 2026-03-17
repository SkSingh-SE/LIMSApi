namespace LIMSApi.Dtos
{
    public class ReplanRequestDto
    {
        public string Reason { get; set; } = string.Empty;
    }

    public class ReplanApprovalDto
    {
        public string? Remarks { get; set; }
    }
}
