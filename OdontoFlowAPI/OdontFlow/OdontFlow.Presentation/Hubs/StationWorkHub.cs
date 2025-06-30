using Microsoft.AspNetCore.SignalR;

namespace OdontFlow.API.Hubs;
public class StationWorkHub : Hub
{
    public async Task NotifyUpdate()
    {
        await Clients.All.SendAsync("ReceiveStationWorkUpdate");
    }
}
