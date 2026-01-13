using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class ChartDataPointDto
    {
        [Required]
        public string Label { get; set; } = string.Empty;
        
        public decimal Value { get; set; }
        
        public DateTime? Date { get; set; }
        
        public Dictionary<string, object>? Metadata { get; set; }
    }
}