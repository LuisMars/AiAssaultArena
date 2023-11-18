using AiAssaultArena.Contract;
using AiAssaultArena.Contract.ClientDefinitions;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace AiAssaultArena.Api.Hubs;

[SignalRHub]
public class MatchHub : Hub<IMatchHubClient>
{
    public async Task SendParameters(ParametersResponse parameters)
    {
        await Clients.All.GetParameters(parameters);
    }

    public override async Task OnConnectedAsync()
    {
        var parameters = new ParametersResponse();
        await Clients.Client(Context.ConnectionId).GetParameters(parameters); 
        await base.OnConnectedAsync();
    }
}
