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
    private readonly Dictionary<string, Guid> _connections = [];

    private readonly IHubContext<MatchHub, IMatchServer> _context;


    private readonly SemaphoreSlim _semaphoreSlim = new(1);
    private readonly MatchRepository _matchRepository;

    public MatchService(IHubContext<MatchHub, IMatchServer> context, MatchRepository matchRepository)
    {
        //Runner = new Runner(_width, _height);
        _context = context;
        _matchRepository = matchRepository;
        _ = Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    await Task.Delay(1000);
                    foreach (var tank in _matchRepository.GetAvailableTanks())
                    {
                        await _context.Clients.Group("Spectators").OnTankConnected(tank.Name, tank.Id);
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
        await _context.Clients.Group("Spectators").OnTankConnected(tankName, tankId);
        await _context.Clients.Client(connectionId).OnTankReceived(new TankReceivedResponse { TankId = tankId });
    }

    public void MoveTank(string connectionId, TankMoveParameters parameters)
    {
        if (!_matchRepository.TryGetTank(connectionId, out var tank))
        {
            Console.WriteLine($"ConnectionId {connectionId} has no tank associated with it");
            return;
        }
        if (!_matchRepository.TryGetMatch(tank.MatchId!.Value, out var match))
        {
            Console.WriteLine($"Tank {tank.Id} is not currently in a match");
            return;
        }
        match.UpdateTank(tank.Id, parameters);
    }

    public async Task RemoveTankAsync(string connectionId)
    {
        if (!_matchRepository.TryGetTank(connectionId, out var tank))
        {
            Console.WriteLine($"ConnectionId {connectionId} has no tank associated with it");
            return;
        }

        if (!_matchRepository.TryGetMatch(tank.MatchId!.Value, out var match))
        {
            Console.WriteLine($"Tank {tank.Id} is not currently in a match");
            return;
        }

        _matchRepository.Remove(match);
        await match.EndRoundAsync();
    }

    public async Task StartMatchAsync(string connectionId, Guid tankAId, Guid tankBId)
    {
        Console.WriteLine($"Starting match for {connectionId} with tanks {_connectedTanks[tankAId]} and {_connectedTanks[tankBId]}");

        if (_matches.Any(m => m.Value.WebClientConnectionId == connectionId))
        {
            var existingMatch = _matches.FirstOrDefault(m => m.Value.WebClientConnectionId == connectionId);
            await existingMatch.Value.EndRoundAsync();
            _matches.Remove(existingMatch.Key);
            Console.WriteLine($"Existing match {existingMatch.Key} was removed");
        }

        var runner = new Runner(_width, _height);
        var tankA = new TankEntity(new Vector2(-_width / 4, 0), tankAId) { BodyRotation = MathHelper.PiOver2 * 3f };
        var tankB = new TankEntity(new Vector2(_width / 4, 0), tankBId) { BodyRotation = -MathHelper.PiOver2 * -1f };
        runner.AddTank(tankA);
        runner.AddTank(tankB);

        var match = new Match(_context)
        {
            Width = _width,
            Height = _height,
            Runner = runner,
            WebClientConnectionId = connectionId,
            TankConnectionIds = new Dictionary<Guid, string>
            {
                { tankAId, _connectedTanks[tankAId].ConnectionId },
                { tankBId, _connectedTanks[tankBId].ConnectionId }
            }
        };

        _matches[match.Id] = match;

        await match.StartAsync();
    }
}
