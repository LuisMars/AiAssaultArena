using AiAssaultArena.Api.Hubs;
using AiAssaultArena.Contract;
using AiAssaultArena.Contract.ClientDefinitions;
using AiAssaultArena.Simulation;
using AiAssaultArena.Simulation.Entities;
using AiAssaultArena.Simulation.Math;
using Microsoft.AspNetCore.SignalR;

namespace AiAssaultArena.Api.Services;

public class MatchService
{
    private readonly float _width = 1200;
    private readonly float _height = 720;

    private readonly IHubContext<MatchHub, IMatchServer> _context;
    private readonly MatchRepository _matchRepository;

    public MatchService(IHubContext<MatchHub, IMatchServer> context, MatchRepository matchRepository)
    {
        _context = context;
        _matchRepository = matchRepository;
        _ = Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    await Task.Delay(1000);
                    foreach (var tank in _matchRepository.GetAllTanks())
                    {
                        if (tank.MatchId is null)
                        {
                            await _context.Clients.Group("Spectators").OnTankAvailable(tank.Name, tank.Id);
                            continue;
                        }

                        if (tank.MatchId.HasValue && _matchRepository.TryGetMatch(tank.MatchId.Value, out var match))
                        {
                            await _context.Clients.GroupExcept("Spectators", match.WebClientConnectionId).OnTankUnavailable(tank.Id);
                        }
                        else
                        {
                            await _context.Clients.Group("Spectators").OnTankUnavailable(tank.Id);
                        }
                    }
                }
                catch
                {

                }
            }
        });
    }

    public async Task AddTankAsync(string connectionId, string tankName)
    {
        var tankId = Guid.NewGuid();
        _matchRepository.AddTank(tankId, connectionId, tankName);
        await _context.Clients.Group("Spectators").OnTankAvailable(tankName, tankId);
        await _context.Clients.Client(connectionId).OnRegisterSuccesfull(new TankReceivedResponse { TankId = tankId });
        Console.WriteLine($"Added tank {tankId}: {tankName}");
    }

    public void MoveTank(string connectionId, TankMoveParameters parameters)
    {
        if (!_matchRepository.TryGetTank(connectionId, out var tank))
        {
            Console.WriteLine($"ConnectionId {connectionId} has no tank associated with it");
            return;
        }
        if (tank.MatchId is null || !_matchRepository.TryGetMatch(tank.MatchId.Value, out var match))
        {
            Console.WriteLine($"Tank {tank.Id} is not currently in a match");
            return;
        }
        match.UpdateTank(tank.Id, parameters);
    }

    public async Task RemoveConnectionAsync(string connectionId)
    {
        if (await RemoveTankAsync(connectionId))
        {
            Console.WriteLine($"Removed tank with connectionId {connectionId}");
            return;
        }

        if (await RemoveMatchAsync(connectionId))
        {
            Console.WriteLine($"Removed match with connectionId {connectionId}");
        }
    }

    public async Task<bool> RemoveTankAsync(string connectionId)
    {
        if (!_matchRepository.TryGetTank(connectionId, out var tank))
        {
            Console.WriteLine($"ConnectionId {connectionId} has no tank associated with it");
            return false;
        }
        _matchRepository.RemoveTank(tank);
        await _context.Clients.Group("Spectators").OnTankUnavailable(tank.Id);
        Console.WriteLine($"Removed tank {tank.Id}");
        if (!_matchRepository.TryGetMatch(tank.MatchId!.Value, out var match))
        {
            Console.WriteLine($"Tank {tank.Id} is not currently in a match");
            return true;
        }

        await EndMatch(match);
        Console.WriteLine($"Match {match.Id} ended because {tank.Id} was removed");
        return true;
    }

    public async Task<bool> RemoveMatchAsync(string connectionId)
    {
        if (!_matchRepository.TryGetMatch(connectionId, out var match))
        {
            Console.WriteLine($"ConnectionId {connectionId} has no match associated with it");
            return false;
        }

        await EndMatch(match);
        Console.WriteLine($"Match {match.Id} was removed");
        return true;
    }

    private async Task EndMatch(Match match)
    {
        await match.EndRoundAsync();
        _matchRepository.RemoveMatch(match);
    }

    public async Task StartMatchAsync(string connectionId, Guid tankAId, Guid tankBId)
    {
        if (!_matchRepository.TryGetTank(tankAId, out var tankAInfo))
        {
            Console.WriteLine($"Tank {tankAId} is not connected");
            return;
        }

        if (tankAInfo.MatchId is not null)
        {
            Console.WriteLine($"Tank {tankAId} is already in a match");
            return;
        }

        if (!_matchRepository.TryGetTank(tankBId, out var tankBInfo))
        {
            Console.WriteLine($"Tank {tankAId} is not connected");
            return;
        }

        if (tankBInfo.MatchId is not null)
        {
            Console.WriteLine($"Tank {tankBId} is already in a match");
            return;
        }

        Console.WriteLine($"Starting match for {connectionId} with tanks {tankAInfo!.Name} and {tankBInfo!.Name}");

        if (_matchRepository.TryGetMatch(connectionId, out var existingMatch))
        {
            await EndMatch(existingMatch);
            Console.WriteLine($"Existing match {existingMatch.Id} was removed");
        }

        var runner = new Runner(_width, _height);
        var tankA = new TankEntity(new Vector2(-_width / 4, 0), tankAId) { BodyRotation = MathHelper.PiOver2 * 3f };
        var tankB = new TankEntity(new Vector2(_width / 4, 0), tankBId) { BodyRotation = -MathHelper.PiOver2 * -1f };
        runner.AddTank(tankA);
        runner.AddTank(tankB);

        var tankConnectionIds = new Dictionary<Guid, string>
            {
                { tankAId, tankAInfo.ConnectionId },
                { tankBId, tankBInfo.ConnectionId }
            };

        var match = new Match(_context, runner, connectionId, tankConnectionIds, _matchRepository.RemoveMatch)
        {
            Width = _width,
            Height = _height
        };

        tankAInfo.MatchId = match.Id;
        tankBInfo.MatchId = match.Id;
        _matchRepository.AddMatch(match);

        await match.StartAsync();
    }
}
