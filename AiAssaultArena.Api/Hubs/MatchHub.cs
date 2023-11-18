using AiAssaultArena.Api.Services;
using AiAssaultArena.Contract;
using AiAssaultArena.Contract.ClientDefinitions;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace AiAssaultArena.Api.Hubs;

[SignalRHub]
public class MatchHub(GameSimulationService simulation) : Hub<IMatchHubClient>
{
    private readonly GameSimulationService _simulation = simulation;

    public async Task OnParametersReceived(ParametersResponse parameters)
    {
        await Clients.All.OnParametersReceived(parameters);
    }

    public override async Task OnConnectedAsync()
    {
        var parameters = _simulation.Parameters;
        await Clients.Client(Context.ConnectionId).OnParametersReceived(parameters); 
        await base.OnConnectedAsync();
    }
}
