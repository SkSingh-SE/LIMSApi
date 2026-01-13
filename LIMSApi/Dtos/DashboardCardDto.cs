using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class DashboardCardDto
    {
        [Required]
        public string Key { get; set; } = string.Empty;
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public int Count { get; set; }
        
        public string Status { get; set; } = "Normal"; // Normal, Warning, Critical
        
        public List<string> AllowedRoles { get; set; } = new();
        
        public string? Description { get; set; }
        
        public Dictionary<string, object>? Metadata { get; set; }
    }
}