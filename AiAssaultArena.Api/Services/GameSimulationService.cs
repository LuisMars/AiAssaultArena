using AiAssaultArena.Api.Hubs;
using AiAssaultArena.Api.Mappers;
using AiAssaultArena.Contract;
using AiAssaultArena.Contract.ClientDefinitions;
using AiAssaultArena.Simulation;
using AiAssaultArena.Simulation.Entities;
using AiAssaultArena.Simulation.Math;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Text;

namespace AiAssaultArena.Api.Services;

public class GameSimulationService
{

    private class Match
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Task Task { get; set; }
        public Runner Runner { get; set; }
        public string WebClientConnectionId { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public Dictionary<Guid, string> TankConnectionIds { get; internal set; }
        public Func<Runner, CancellationToken, Task> Execution { get; internal set; }

        public void Start()
        {
            Task = Execution(Runner, CancellationTokenSource.Token);
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
                        _context.Clients.Group("Spectators").OnGameUpdated(gameStateResponse);
                        foreach (var (connectionId, tankId) in _connections)
                        {
                            if (connectionId == "FAKE")
                            {
                                continue;
                            }
                            var tank = runner.GetTank(tankId);
                            if (tank is not null)
                            {
                                _context.Clients.Client(connectionId).OnTankStateUpdated(tank.ToResponse(), runner.Sensors[tank.Id].ToResponse());
                            }
                        }
                    }, stoppingToken);
                }

                if (runner.Tanks.Any(t => t.Health <= 0) || totalTime.Elapsed.TotalMinutes > 1)
                {
                    await _context.Clients.All.OnRoundEnd();
                    _connections.Clear();
                    runner = new Runner(_width, _height);
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
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
        //if (_connections.TryGetValue(connectionId, out Guid value))
        //{
        //    Runner.UpdateTank(value, parameters);
        //}
    }

    public void RemoveTank(string connectionId)
    {
        var (tankId, (_, tankName)) = _connectedTanks.First(t => t.Value.ConnectionId == connectionId);
        _context.Clients.Group("Spectators").OnTankDisconnected(tankId);
        _connectedTanks.Remove(tankId);
    }

    public async Task StartMatch(string connectionId, Guid tankAId, Guid tankBId)
    {
        var runner = new Runner(_width, _height);
        var tankA = new TankEntity(new Vector2(-_width / 4, 0), tankAId) { BodyRotation = MathHelper.PiOver2 * 3f };
        var tankB = new TankEntity(new Vector2(_width / 4, 0), tankBId) { BodyRotation = -MathHelper.PiOver2 * -1f };
        runner.AddTank(tankA);
        runner.AddTank(tankB);
        var parameters = new ParametersResponse()
        {
            Walls = runner.Walls.ToResponse(),
            ArenaWidth = _width,
            ArenaHeight = _height
        };

        await _context.Clients.Clients(new List<string> { connectionId, _connectedTanks[tankAId].ConnectionId, _connectedTanks[tankBId].ConnectionId }).OnParametersReceived(parameters);
        var match = new Match
        {
            Runner = runner,
            CancellationTokenSource = new CancellationTokenSource(),
            WebClientConnectionId = connectionId,
            TankConnectionIds = new Dictionary<Guid, string> 
            {
                { tankAId, _connectedTanks[tankAId].ConnectionId },
                { tankBId, _connectedTanks[tankBId].ConnectionId }
            },
            Execution = ExecuteAsync
        };

        _matches[match.Id] = match;

        match.Start();
    }
}
