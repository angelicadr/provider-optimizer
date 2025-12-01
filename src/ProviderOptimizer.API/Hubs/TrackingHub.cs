using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ProviderOptimizer.API.Hubs
{
    public class TrackingHub : Hub
    {
        public async Task SendStatusUpdate(string requestId, string message)
        {
            await Clients.Group(requestId).SendAsync("statusUpdate", message);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}
