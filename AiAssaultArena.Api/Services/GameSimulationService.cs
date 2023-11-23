using AiAssaultArena.Api.Hubs;
using AiAssaultArena.Api.Mappers;
using AiAssaultArena.Contract;
using AiAssaultArena.Contract.ClientDefinitions;
using AiAssaultArena.Simulation;
using AiAssaultArena.Simulation.Entities;
using AiAssaultArena.Simulation.Math;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace AiAssaultArena.Api.Services;

public class GameSimulationService
{

    private class Match
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public Guid Id { get; } = Guid.NewGuid();
        public Task Task { get; set; }
        public Runner Runner { get; set; }
        public string WebClientConnectionId { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public Dictionary<Guid, string> TankConnectionIds { get; set; }
        public IHubContext<MatchHub, IMatchServer> Context { get; set; }
        public bool HasEnded { get; set; }

        public async Task StartAsync()
        {
            var parameters = new ParametersResponse()
            {
                Walls = Runner.Walls.ToResponse(),
                ArenaWidth = Width,
                ArenaHeight = Height
            };

            await Context.Clients.Client(WebClientConnectionId).OnMatchStart(parameters);
            await Context.Clients.Clients(TankConnectionIds.Values).OnMatchStart(parameters);
            Task = ExecuteAsync(Runner, CancellationTokenSource.Token);
        }

        public async Task EndRoundAsync()
        {
            HasEnded = true;
            CancellationTokenSource.Cancel();
            await Context.Clients.Client(WebClientConnectionId).OnRoundEnd();
            await Context.Clients.Clients(TankConnectionIds.Values).OnRoundEnd();
        }

        private async Task ExecuteAsync(Runner runner, CancellationToken stoppingToken)
        {
            await Task.Delay(1000, stoppingToken);
            var totalTime = Stopwatch.StartNew();
            var stopwatch = Stopwatch.StartNew();
            var updateElapsed = 0f;
            var frameRate = 1f / 60;
            var updates = 0ul;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Calculate elapsed time since the last iteration
                    var deltaSeconds = (float)stopwatch.Elapsed.TotalSeconds;
                    updateElapsed += deltaSeconds;
                    stopwatch.Restart();
                    // Perform game update with the dynamic timestep
                    runner.Update(deltaSeconds);

                    updates++;

                    if (updateElapsed > frameRate)
                    {
                        // Prepare the game state response without awaiting
                        var gameStateResponse = runner.GetGameStateResponse();
                        var updatesPerSecond = updates / (ulong)(1 + totalTime.Elapsed.TotalSeconds);
                        Console.WriteLine($"deltaNanoseconds: {1000000000 * deltaSeconds}\tUpdatesPerSecond: {updatesPerSecond}");

                        // Send the game state response without awaiting
                        updateElapsed = 0;
                        _ = Task.Run(() =>
                        {
                            Context.Clients.Group("Spectators").OnGameUpdated(gameStateResponse);

                            foreach (var tank in Runner.Tanks)
                            {
                                var connectionId = TankConnectionIds[tank.Id];

                                if (tank is not null)
                                {
                                    Context.Clients.Client(connectionId).OnTankStateUpdated(tank.ToResponse(), runner.Sensors[tank.Id].ToResponse());
                                }
                            }
                        }, stoppingToken);
                    }

                    if (runner.Tanks.Any(t => t.Health <= 0) || totalTime.Elapsed.TotalMinutes > 1)
                    {
                        await EndRoundAsync();
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }
        }

    }

    private readonly float _width = 1200;
    private readonly float _height = 720;
    private readonly Dictionary<string, Guid> _connections = [];

    private readonly Dictionary<Guid, (string ConnectionId, string TankName)> _connectedTanks = [];
    private readonly Dictionary<Guid, Match> _matches = [];


    //private Runner Runner { get; set; }
    private readonly IHubContext<MatchHub, IMatchServer> _context;


    public GameSimulationService(IHubContext<MatchHub, IMatchServer> context)
    {
        //Runner = new Runner(_width, _height);
        _context = context;
        _ = Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    await Task.Delay(1000);
                    foreach (var kvp in _connectedTanks)
                    {
                        await _context.Clients.Group("Spectators").OnTankConnected(kvp.Value.TankName, kvp.Key);
                    }
                }
                catch
                {

                }
            }
        });
    }

    private readonly SemaphoreSlim _semaphoreSlim = new(1);

    public async Task AddTankAsync(string connectionId, string tankName)
    {
        var tankId = Guid.NewGuid();
        _connectedTanks[tankId] = (connectionId, tankName);
        await _context.Clients.Group("Spectators").OnTankConnected(tankName, tankId);
    }

    public void MoveTank(string connectionId, TankMoveParameters parameters)
    {
        var matchKvp = _matches.FirstOrDefault(m => m.Value.TankConnectionIds.ContainsValue(connectionId));
        var match = matchKvp.Value;
        var tankKvp = match.TankConnectionIds.FirstOrDefault(t => t.Value == connectionId);
        var tankId = tankKvp.Key;
        match.Runner.UpdateTank(tankId, parameters);
    }

    public void RemoveTank(string connectionId)
    {
        if (!_connectedTanks.Any(t => t.Value.ConnectionId == connectionId))
        {
            return;
        }

        var (tankId, (_, tankName)) = _connectedTanks.First(t => t.Value.ConnectionId == connectionId);
        _context.Clients.Group("Spectators").OnTankDisconnected(tankId);
        _connectedTanks.Remove(tankId);
    }

    public async Task StartMatchAsync(string connectionId, Guid tankAId, Guid tankBId)
    {
        if (_matches.Any(m => m.Value.WebClientConnectionId == connectionId))
        {
            var existingMatch = _matches.FirstOrDefault(m => m.Value.WebClientConnectionId == connectionId);
            await existingMatch.Value.EndRoundAsync();
            _matches.Remove(existingMatch.Key);
        }

        var runner = new Runner(_width, _height);
        var tankA = new TankEntity(new Vector2(-_width / 4, 0), tankAId) { BodyRotation = MathHelper.PiOver2 * 3f };
        var tankB = new TankEntity(new Vector2(_width / 4, 0), tankBId) { BodyRotation = -MathHelper.PiOver2 * -1f };
        runner.AddTank(tankA);
        runner.AddTank(tankB);

        var match = new Match
        {
            Width = _width,
            Height = _height,
            Runner = runner,
            CancellationTokenSource = new CancellationTokenSource(),
            WebClientConnectionId = connectionId,
            TankConnectionIds = new Dictionary<Guid, string>
            {
                { tankAId, _connectedTanks[tankAId].ConnectionId },
                { tankBId, _connectedTanks[tankBId].ConnectionId }
            },
            Context = _context
        };

        _matches[match.Id] = match;

        await match.StartAsync();
    }
}
