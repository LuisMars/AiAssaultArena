using AiAssaultArena.Contract;
using AiAssaultArena.Contract.ClientDefinitions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using TypedSignalR.Client;

var tank = new Tank();
await tank.StartAsync();
Console.ReadLine();

tank.Dispose();

public class Tank : ITankClient, IDisposable
{
    private readonly HubConnection _hubConnection;
    private readonly ITankServer _server;
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

    private Guid Id { get; set; }
    private ParametersResponse Parameters { get; set; }

    public void Dispose()
    {
        _hubConnection.StopAsync();
        _hubConnection.DisposeAsync();
    }

    public Task OnMatchStart(ParametersResponse parameters)
    {
        Parameters = parameters;
        return Task.CompletedTask;
    }

    public async Task OnRoundEnd()
    {
        //await _server.();
    }

    public Task OnTankReceived(TankReceivedResponse tankReceivedResponse)
    {
        Id = tankReceivedResponse.TankId;
        return Task.CompletedTask;
    }

    public async Task OnTankStateUpdated(TankResponse gameStateResponse, SensorResponse? sensorResponse)
    {
        await _server.SendUpdate(new TankMoveParameters { TurretTurnDirection = -0.1f, SensorTurnDirection = -0.1f, TurnDirection = 1, Acceleration = 1, Shoot = true });
    }
    public async Task StartAsync()
    {
        await _hubConnection.StartAsync(); 
        await _server.RegisterAsync(Guid.NewGuid(), "Tank", "SampleBot" + Guid.NewGuid().ToString()[..4]);
    }
}

