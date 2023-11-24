using AiAssaultArena.Contract;
using AiAssaultArena.Contract.ClientDefinitions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using TypedSignalR.Client;

namespace AiAssaultArena.SampleBot;

public class Tank : ITankClient, IDisposable
{
    private readonly HubConnection _hubConnection;
    private readonly ITankServer _server;
    private Guid? Id { get; set; } = null;
    private ParametersResponse? Parameters { get; set; }

    public Tank()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5167/Match")
            .ConfigureLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Information);
                logging.AddConsole();  // Log to the console.
            })
            .Build();

        _hubConnection.Register<ITankClient>(this);

        _hubConnection.Closed += async (exception) =>
        {
            Console.WriteLine($"Connection closed: {exception?.Message}");
            await Task.Delay(TimeSpan.FromSeconds(5)); // Reconnect after a delay
            await StartAsync();
        };
        _server = _hubConnection.CreateHubProxy<ITankServer>();
    }

    public void Dispose()
    {
        _hubConnection.StopAsync();
        _hubConnection.DisposeAsync();
    }

    public Task OnMatchStart(ParametersResponse parameters)
    {
        Console.WriteLine("Starting Match");
        Parameters = parameters;
        return Task.CompletedTask;
    }

    public async Task OnRoundEnd()
    {
        Parameters = null;
        Console.WriteLine("Round Ended");
    }

    public Task OnTankReceived(TankReceivedResponse tankReceivedResponse)
    {
        Id = tankReceivedResponse.TankId;
        Console.WriteLine($"I'm tank {Id}");
        return Task.CompletedTask;
    }

    public async Task OnTankStateUpdated(TankResponse gameStateResponse, SensorResponse? sensorResponse)
    {
        await _server.SendUpdate(new TankMoveParameters
        {
            TurretTurnDirection = 2 * (Random.Shared.NextSingle() - 0.5f),
            SensorTurnDirection = 2 * (Random.Shared.NextSingle() - 0.5f),
            TurnDirection = 2 * (Random.Shared.NextSingle() - 0.5f),
            Acceleration = 2 * (Random.Shared.NextSingle() - 0.5f),
            Shoot = Random.Shared.NextSingle() > 0.5f
        });
    }

    public async Task StartAsync()
    {
        await _hubConnection.StartAsync();
        var id = Id ?? Guid.NewGuid();
        await _server.RegisterAsync("Tank", Id, $"SampleBot {id.ToString()[..4]}");
        Console.WriteLine("Connected");
    }
}