using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LIMSApi.Helpers
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.Identity?.Name;
            var employeeId = Context.User?.FindFirst("EmployeeID")?.Value;

            // Reject connections without a valid employee identity
            if (string.IsNullOrEmpty(employeeId))
            {
                Context.Abort();
                return;
            }

            // Add user to their personal group for targeted notifications
            await Groups.AddToGroupAsync(Context.ConnectionId, $"employee-{employeeId}");
            // Add every authenticated user to the shared dashboard live group
            await Groups.AddToGroupAsync(Context.ConnectionId, "dashboard-live");

            Console.WriteLine($"SignalR connected: {userName} (EmployeeID: {employeeId})");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var employeeId = Context.User?.FindFirst("EmployeeID")?.Value;
            if (!string.IsNullOrEmpty(employeeId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"employee-{employeeId}");
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "dashboard-live");
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Explicit join/leave so the dashboard can re-subscribe after reconnects.
        /// </summary>
        public async Task JoinDashboard()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "dashboard-live");
        }

        public async Task LeaveDashboard()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "dashboard-live");
        }
    }
}
