using LIMSApi.Dtos;
using LIMSApi.Helpers;

namespace LIMSApi.ServiceWORepo
{
    public interface IDashboardService
    {
        Task<DashboardResponseDto> GetDashboardAsync(LoggedInUserDTO userContext);
        Task<List<DashboardCardDto>> GetDashboardCardsAsync(LoggedInUserDTO userContext);
        Task<List<DashboardChartDto>> GetDashboardChartsAsync(LoggedInUserDTO userContext);
        Task<List<DashboardNotificationDto>> GetDashboardNotificationsAsync(LoggedInUserDTO userContext);
    }
}