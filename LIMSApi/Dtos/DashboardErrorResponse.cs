using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class DashboardErrorResponse
    {
        [Required]
        public string Error { get; set; } = string.Empty;
        
        [Required]
        public string Message { get; set; } = string.Empty;
        
        public int StatusCode { get; set; }
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        public string? TraceId { get; set; }
    }
}