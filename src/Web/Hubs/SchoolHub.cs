using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs;

/// <summary>
/// Hub de SignalR per broadcast d'esdeveniments relacionats amb escoles (creació, actualització, eliminació).
/// </summary>
public class SchoolHub : Hub
{
    /// <summary>
    /// Notifica a tots els clients que s'ha creat una nova escola.
    /// </summary>
    /// <param name="code">Codi de l'escola.</param>
    /// <param name="name">Nom de l'escola.</param>
    /// <param name="city">Ciutat de l'escola.</param>
    public async Task BroadcastSchoolCreated(string code, string name, string city)
    {
        await Clients.All.SendAsync("SchoolCreated", code, name, city);
    }

    /// <summary>
    /// Notifica a tots els clients que s'ha actualitzat una escola.
    /// </summary>
    /// <param name="code">Codi de l'escola.</param>
    /// <param name="name">Nom de l'escola.</param>
    /// <param name="city">Ciutat de l'escola.</param>
    public async Task BroadcastSchoolUpdated(string code, string name, string city)
    {
        await Clients.All.SendAsync("SchoolUpdated", code, name, city);
    }

    /// <summary>
    /// Notifica a tots els clients que s'ha eliminat una escola.
    /// </summary>
    /// <param name="code">Codi de l'escola eliminada.</param>
    public async Task BroadcastSchoolDeleted(string code)
    {
        await Clients.All.SendAsync("SchoolDeleted", code);
    }
}
