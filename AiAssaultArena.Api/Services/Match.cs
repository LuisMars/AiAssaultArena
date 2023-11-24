using AiAssaultArena.Api.Hubs;
using AiAssaultArena.Api.Mappers;
using AiAssaultArena.Contract;
using AiAssaultArena.Contract.ClientDefinitions;
using AiAssaultArena.Simulation;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace AiAssaultArena.Api.Services;

public class Match(IHubContext<MatchHub, IMatchServer> context)
{
    public float Width { get; set; }
    public float Height { get; set; }
    public Guid Id { get; } = Guid.NewGuid();
    private Task Task { get; set; }
    private Runner Runner { get; set; }
    public string WebClientConnectionId { get; set; }
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    public Dictionary<Guid, string> TankConnectionIds { get; set; }
    private IHubContext<MatchHub, IMatchServer> Context { get; set; } = context;
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
        Task = ExecuteAsync(Runner, _cancellationTokenSource.Token);
    }

    public async Task EndRoundAsync()
    {
        HasEnded = true;
        _cancellationTokenSource.Cancel();
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
                    var updatesPerSecond = updates / (ulong)(1 + totalTime.Elapsed.TotalSeconds);
                    var gameStateResponse = runner.GetGameStateResponse(totalTime.Elapsed, updatesPerSecond);
                    //Console.WriteLine($"deltaNanoseconds: {1000000000 * deltaSeconds}\tUpdatesPerSecond: {updatesPerSecond}");

                    // Send the game state response without awaiting
                    updateElapsed = 0;
                    _ = Task.Run(() =>
                    {
                        Context.Clients.Client(WebClientConnectionId).OnGameUpdated(gameStateResponse);

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

    public void UpdateTank(Guid id, TankMoveParameters parameters)
    {
        Runner.UpdateTank(id, parameters);
    }
}
