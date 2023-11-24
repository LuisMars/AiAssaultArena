using AiAssaultArena.Api.Mappers;
using AiAssaultArena.Api.Services;
using AiAssaultArena.Contract;
using AiAssaultArena.Contract.ClientDefinitions;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace AiAssaultArena.Api.Hubs;

[SignalRHub]
public class MatchHub(MatchService matchService) : Hub<IMatchServer>, IServer
{
    private readonly MatchService _matchService = matchService;

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return _matchService.RemoveTankAsync(Context.ConnectionId);
    }

    public override async Task OnConnectedAsync()
    {
        Clients.Client(Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public Task SendUpdate(TankMoveParameters parameters)
    {
        _matchService.MoveTank(Context.ConnectionId, parameters);
        return Task.CompletedTask;
    }

    public Task StartMatchAsync(Guid tankAId, Guid tankBId)
    {
        return _matchService.StartMatchAsync(Context.ConnectionId, tankAId, tankBId);
    }

    public async Task RegisterAsync(string clientType, Guid? guid, string? name = null)
    {
        switch (clientType)
        {
            case "Tank":
                await Groups.AddToGroupAsync(Context.ConnectionId, "Tanks");
                await _matchService.AddTankAsync(Context.ConnectionId, name ?? "Unnamed tank");
                break;
            case "WebClient":
                await Groups.AddToGroupAsync(Context.ConnectionId, "Spectators");
                break;
        }
    }
}
