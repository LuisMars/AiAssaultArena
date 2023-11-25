using AiAssaultArena.Api.Hubs;
using AiAssaultArena.Api.Mappers;
using AiAssaultArena.Contract;
using AiAssaultArena.Contract.ClientDefinitions;
using AiAssaultArena.Simulation;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace AiAssaultArena.Api.Services;

public class Match(IHubContext<MatchHub, IMatchServer> context, Runner runner, string webClientConnectionId, Dictionary<Guid, string> tankConnectionIds, Action<Match> onRoundEnd)
{
    public Guid Id { get; } = Guid.NewGuid();
    public float Width { get; set; }
    public float Height { get; set; }
    internal string WebClientConnectionId { get; set; } = webClientConnectionId;
    internal Dictionary<Guid, string> TankConnectionIds { get; set; } = tankConnectionIds;
    private IHubContext<MatchHub, IMatchServer> Context { get; set; } = context;

    public bool HasEnded { get; set; }

    private readonly Action<Match> _onRoundEnd = onRoundEnd;
    private readonly Runner _runner = runner;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public async Task StartAsync()
    {
        var parameters = new ParametersResponse()
        {
            Walls = _runner.Walls.ToResponse(),
            ArenaWidth = Width,
            ArenaHeight = Height
        };

        await Context.Clients.Client(WebClientConnectionId).OnMatchStart(parameters);
        await Context.Clients.Clients(TankConnectionIds.Values).OnMatchStart(parameters);
        _ = ExecuteAsync(_runner, _cancellationTokenSource.Token);
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

                if (deltaSeconds <= frameRate)
                {
                    continue;
                }

                stopwatch.Restart();
                runner.Update(deltaSeconds);
                updates++;
                var updatesPerSecond = updates / (ulong)(1 + totalTime.Elapsed.TotalSeconds);
                var gameStateResponse = runner.GetGameStateResponse(totalTime.Elapsed, updatesPerSecond);

                // Send the game state response without awaiting
                updateElapsed -= frameRate;

                await Context.Clients.Client(WebClientConnectionId).OnGameUpdated(gameStateResponse);

                foreach (var tank in _runner.Tanks)
                {
                    var connectionId = TankConnectionIds[tank.Id];

                    if (tank is not null)
                    {
                        await Context.Clients.Client(connectionId).OnTankStateUpdated(tank.ToResponse(), runner.Sensors[tank.Id].ToResponse());
                    }
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

        _onRoundEnd(this);
        Console.WriteLine($"Match {Id} ended");
    }

    public void UpdateTank(Guid id, TankMoveParameters parameters)
    {
        _runner.UpdateTank(id, parameters);
    }
}
