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
    public ParametersResponse Parameters
    {
        get => new()
        {
            Walls = Runner.Walls.ToResponse(),
            ArenaWidth = _width,
            ArenaHeight = _height
        };
    }

    private readonly float _width = 1200;
    private readonly float _height = 720;
    private readonly Dictionary<string, Guid> _connections = [];
    private Runner Runner { get; set; }
    private readonly IHubContext<MatchHub, IMatchServer> _context;

    public GameSimulationService(IHubContext<MatchHub, IMatchServer> context)
    {
        Runner = new Runner(_width, _height);
        _context = context;
    }

    private async Task ExecuteAsync(CancellationToken stoppingToken)
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
                Runner.Update(deltaSeconds);

                updates++;

                if (updateElapsed > frameRate)
                {
                    // Prepare the game state response without awaiting
                    var gameStateResponse = Runner.GetGameStateResponse();
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
                            var tank = Runner.GetTank(tankId);
                            if (tank is not null)
                            {
                                _context.Clients.Client(connectionId).OnTankStateUpdated(tank.ToResponse(), Runner.Sensors[tank.Id].ToResponse());
                            }
                        }
                    }, stoppingToken);
                }

                if (Runner.Tanks.Any(t => t.Health <= 0) || totalTime.Elapsed.TotalMinutes > 1)
                {
                    await _context.Clients.All.OnRoundEnd();
                    _connections.Clear();
                    Runner = new Runner(_width, _height);
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

    public async Task AddTankAsync(string connectionId)
    {
        try
        {
            await _semaphoreSlim.WaitAsync();
            var isFirst = Runner.Tanks.Count == 0;
            TankEntity tank;

            if (!isFirst)
            {
                tank = new(new Vector2(-_width / 4, 0)) { BodyRotation = MathHelper.PiOver2 * 3f };

            }
            else
            {
                tank = new(new Vector2(_width / 4, 0)) { BodyRotation = -MathHelper.PiOver2 * -1f };
            }

            _connections[connectionId] = tank.Id;
            Runner.AddTank(tank);
            if (connectionId == "FAKE")
            {
                return;
            }

            await _context.Clients.Client(connectionId).OnTankReceived(new TankReceivedResponse { TankId = tank.Id });

            if (Runner.Tanks.Count == 2)
            {
                _ = ExecuteAsync(new CancellationTokenSource().Token);
            }
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public void MoveTank(string connectionId, TankMoveParameters parameters)
    {
        if (_connections.TryGetValue(connectionId, out Guid value))
        {
            Runner.UpdateTank(value, parameters);
        }
    }
}
