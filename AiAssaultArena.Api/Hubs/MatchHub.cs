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

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _simulation.RemoveTank(Context.ConnectionId);
        return Task.CompletedTask;
    }

    public override async Task OnConnectedAsync()
    {
        Clients.Client(Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public Task SendUpdate(TankMoveParameters parameters)
    {
        _simulation.MoveTank(Context.ConnectionId, parameters);
        return Task.CompletedTask;
    }

    public Task StartMatchAsync(Guid tankAId, Guid tankBId)
    {
        return _simulation.StartMatchAsync(Context.ConnectionId, tankAId, tankBId);
    }

    public async Task RegisterAsync(Guid guid, string clientType, string? name = null)
    {
        switch (clientType)
        {
            case "Tank":
                await Groups.AddToGroupAsync(Context.ConnectionId, "Tanks");
                await _simulation.AddTankAsync(Context.ConnectionId, name ?? "Unnamed tank");
                break;
            case "WebClient":
                await Groups.AddToGroupAsync(Context.ConnectionId, "Spectators");
                break;
        }
    }
}
