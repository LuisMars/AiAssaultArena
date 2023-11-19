using AiAssaultArena.Api.Services;
using AiAssaultArena.Contract;
using AiAssaultArena.Contract.ClientDefinitions;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace AiAssaultArena.Api.Hubs;

[SignalRHub]
public class MatchHub(GameSimulationService simulation) : Hub<IMatchClient>, IMatchClient
{
    private readonly GameSimulationService _simulation = simulation;

    public async Task OnParametersReceived(ParametersResponse parameters)
    {
        await Clients.All.OnParametersReceived(parameters);
    }

    public override async Task OnConnectedAsync()
    {
        // Check if the client sent a "clientType" parameter
        var request = Context.GetHttpContext()?.Request;
        var clientType = "Spectators";
        if (request is not null)
        {
            clientType = request.Query["clientType"];
        }

        if (clientType == "Tank")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Tanks");
            _simulation.AddTank(Context.ConnectionId);
        }
        else
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Spectators");
        }

        var parameters = _simulation.Parameters;
        await Clients.Client(Context.ConnectionId).OnParametersReceived(parameters);
        await base.OnConnectedAsync();
    }

    public Task SendUpdate(TankMoveParameters parameters)
    {
        _simulation.MoveTank(Context.ConnectionId, parameters);
        return Task.CompletedTask;
    }

    public Task OnGameUpdated(GameStateResponse gameStateResponse)
    {
        return Task.CompletedTask;
    }

    public Task OnTankReceived(TankReceivedResponse response)
    {
        return Task.CompletedTask;
    }

    public Task OnTankStateUpdated(TankResponse gameStateResponse)
    {
        return Task.CompletedTask;
    }
}
