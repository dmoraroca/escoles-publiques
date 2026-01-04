using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs;

public class SchoolHub : Hub
{
    public async Task BroadcastSchoolCreated(string code, string name, string city)
    {
        await Clients.All.SendAsync("SchoolCreated", code, name, city);
    }

    public async Task BroadcastSchoolUpdated(string code, string name, string city)
    {
        await Clients.All.SendAsync("SchoolUpdated", code, name, city);
    }

    public async Task BroadcastSchoolDeleted(string code)
    {
        await Clients.All.SendAsync("SchoolDeleted", code);
    }
}
