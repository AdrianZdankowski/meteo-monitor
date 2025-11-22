using Microsoft.AspNetCore.SignalR;

namespace backend.Hubs;

public class DashboardHub : Hub
{
    public async Task SendSensorUpdate(string sensorId, double value, double timestamp)
    {
        await Clients.All.SendAsync("ReceiveSensorUpdate", sensorId, value, timestamp);
    }
}
