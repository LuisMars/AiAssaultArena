using AiAssaultArena.Api.Mappers;
using AiAssaultArena.Api.Services;
using AiAssaultArena.Contract;
using AiAssaultArena.Contract.ClientDefinitions;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace AiAssaultArena.Api.Hubs;

[SignalRHub]
public class MatchHub(GameSimulationService simulation) : Hub<IMatchServer>, IServer
{
    private readonly GameSimulationService _simulation = simulation;

    public async Task OnParametersReceived(ParametersResponse parameters)
    {
        await Clients.All.OnParametersReceived(parameters);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _simulation.RemoveTank(Context.ConnectionId);
        return Task.CompletedTask;
    }

    public override async Task OnConnectedAsync()
    {
        // Check if the client sent a "clientType" parameter
        var request = Context.GetHttpContext()?.Request;
        var clientType = "";

        if (request?.Query?.TryGetValue("name", out var clientTypeValue) ?? false)
        {
            clientType = clientTypeValue.ToString();
        }

        switch (clientType)
        {
            case "Tank":
                await Groups.AddToGroupAsync(Context.ConnectionId, "Tanks");
                var name = "Unnamed tank";
                if (request?.Query?.TryGetValue("name", out var nameValue) ?? false)
                {
                    name = nameValue.ToString();
                }

                await _simulation.AddTankAsync(Context.ConnectionId, name);
                break;
            case "WebClient":
                await Groups.AddToGroupAsync(Context.ConnectionId, "Spectators");
                break;
        }

        await base.OnConnectedAsync();
    }

    public Task SendUpdate(TankMoveParameters parameters)
    {
        _simulation.MoveTank(Context.ConnectionId, parameters);
        return Task.CompletedTask;
    }

    public Task StartMatch(string tankNameA, string tankNameB)
    {
        _simulation.StartMatch(Context.ConnectionId, tankNameA, tankNameB);
        return Task.CompletedTask;
    }
}
