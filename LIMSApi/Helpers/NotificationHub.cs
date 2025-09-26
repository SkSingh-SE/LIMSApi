using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LIMSApi.Helpers
{
  
    public class NotificationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var userName = Context.User?.Identity?.Name;
            var employeeId = Context.User?.FindFirst("EmployeeID")?.Value;
            Console.WriteLine($"Connected user: {userName}, EmployeeID: {employeeId}");
            return base.OnConnectedAsync();
        }
    }

}
