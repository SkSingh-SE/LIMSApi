using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class DashboardChartDto
    {
        [Required]
        public string Key { get; set; } = string.Empty;
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string ChartType { get; set; } = string.Empty; // Line, Bar, Pie, etc.
        
        public List<ChartDataPointDto> DataPoints { get; set; } = new();
        
        public List<string> AllowedRoles { get; set; } = new();
        
        public Dictionary<string, object>? Options { get; set; }
    }
}