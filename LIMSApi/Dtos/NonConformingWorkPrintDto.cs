namespace LIMSApi.Dtos
{
    public class NonConformingWorkPrintDto
    {
        public long Id { get; set; }
        public string? NcNo { get; set; }
        public DateTime? NcDate { get; set; }
        public string? Description { get; set; }
        public string? RootCauseAnalysis { get; set; }
        public string? CorrectiveAction { get; set; }
        public DateTime? ClosureDate { get; set; }
        public string? SignatureTDQM { get; set; }
    }
}
