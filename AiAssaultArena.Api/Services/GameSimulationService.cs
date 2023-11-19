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
            Walls = _runner.Walls.ToResponse(),
            ArenaWidth = _width,
            ArenaHeight = _height
        };
    }

    private readonly float _width = 1200;
    private readonly float _height = 720;
    private readonly Dictionary<string, TankEntity> _tanks = [];
    private readonly Runner _runner;
    private readonly IHubContext<MatchHub, IMatchClient> _context;

    public GameSimulationService(IHubContext<MatchHub, IMatchClient> context)
    {
        _runner = new Runner(_width, _height);
        AddTank("FAKE");

        _context = context;

        _ = ExecuteAsync(new CancellationTokenSource().Token);
    }

    private async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(1000, stoppingToken);

        while (_runner.IsWaiting)
        {
            await Task.Delay(100, stoppingToken);
        }

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
                _runner.Update(deltaSeconds);

                updates++;

                if (updateElapsed > frameRate)
                {
                    // Prepare the game state response without awaiting
                    var gameStateResponse = _runner.GetGameStateResponse();
                    //var updatesPerSecond = updates / (ulong)(1 + totalTime.Elapsed.TotalSeconds);
                    //Console.WriteLine($"deltaNanoseconds: {1000000000 * deltaSeconds}\tUpdatesPerSecond: {updatesPerSecond}");

                    // Send the game state response without awaiting
                    updateElapsed = 0;
                    _ = Task.Run(() =>
                    {
                        _context.Clients.Group("Spectators").OnGameUpdated(gameStateResponse);
                    }, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }

    public void AddTank(string connectionId)
    {
        var isFirst = _tanks.Count == 0;
        TankEntity tank;

        if (isFirst)
        {
            tank = new(new Vector2(-_width / 4, 0)) { BodyRotation = MathHelper.PiOver2 * 3f };

        }
        else
        {
            tank = new(new Vector2(_width / 4, 0)) { BodyRotation = -MathHelper.PiOver2 * -1f };
        }

        _tanks[connectionId] = tank;
        _runner.AddTank(tank);
        if (connectionId == "FAKE")
        {
            return;
        }
        
        _context.Clients.Client(connectionId).OnTankReceived(new TankReceivedResponse { TankId = tank.Id });
    }

    public void MoveTank(string connectionId, TankMoveParameters parameters)
    {
        _runner.UpdateTank(_tanks[connectionId], parameters);
    }
}
