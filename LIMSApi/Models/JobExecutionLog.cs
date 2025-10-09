namespace LIMSApi.Models
{
    public class JobExecutionLog
    {
        public int Id { get; set; }
        public string JobName { get; set; }
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
