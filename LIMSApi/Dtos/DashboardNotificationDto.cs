using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class DashboardNotificationDto
    {
        public long Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Message { get; set; } = string.Empty;
        
        [Required]
        public string Type { get; set; } = string.Empty; // Info, Warning, Error, Success
        
        public string Priority { get; set; } = "Normal"; // Low, Normal, High, Critical
        
        public DateTime CreatedAt { get; set; }
        
        public bool IsRead { get; set; }
        
        public string? ActionUrl { get; set; }
        
        public Dictionary<string, object>? Metadata { get; set; }
    }
}