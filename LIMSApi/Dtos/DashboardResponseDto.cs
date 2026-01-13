using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class DashboardResponseDto
    {
        public List<DashboardCardDto> Cards { get; set; } = new();
        public List<DashboardChartDto> Charts { get; set; } = new();
        public List<DashboardNotificationDto> Notifications { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}