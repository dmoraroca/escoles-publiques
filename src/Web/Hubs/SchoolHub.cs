using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs;
/// <summary>
/// Encapsulates the functional responsibility of school hub within the application architecture.
/// </summary>
public class SchoolHub : Hub
{
    /// <summary>
    /// Executes the broadcast school created operation as part of this component.
    /// </summary>
    public async Task BroadcastSchoolCreated(string code, string name, string city)
    {
        await Clients.All.SendAsync("SchoolCreated", code, name, city);
    }
    /// <summary>
    /// Executes the broadcast school updated operation as part of this component.
    /// </summary>
    public async Task BroadcastSchoolUpdated(string code, string name, string city)
    {
        await Clients.All.SendAsync("SchoolUpdated", code, name, city);
    }
    /// <summary>
    /// Executes the broadcast school deleted operation as part of this component.
    /// </summary>
    public async Task BroadcastSchoolDeleted(string code)
    {
        await Clients.All.SendAsync("SchoolDeleted", code);
    }
}
