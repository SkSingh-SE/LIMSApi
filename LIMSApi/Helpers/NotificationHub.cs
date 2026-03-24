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

            await base.OnDisconnectedAsync(exception);
        }
    }
}
