using AiAssaultArena.Contract;
using AiAssaultArena.Contract.ClientDefinitions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using TypedSignalR.Client;

namespace AiAssaultArena.Tanks.Common;

public abstract class BaseTank(string name) : ITankClient, IDisposable
{
    private HubConnection _hubConnection;
    private ITankServer _server;
    protected Guid? Id { get; set; } = null;
    protected ParametersResponse? Parameters { get; set; }
    public string Name { get; } = name;

    internal void Setup(string url)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{url}Match")
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
            await Task.Delay(TimeSpan.FromSeconds(5));
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

    public Task OnRegisterSuccesfull(TankReceivedResponse tankReceivedResponse)
    {
        Id = tankReceivedResponse.TankId;
        Console.WriteLine($"I'm tank {Id}");
        return Task.CompletedTask;
    }

    public Task OnTankStateUpdated(TankResponse gameStateResponse, SensorResponse? sensorResponse)
    {
        return OnUpdate(gameStateResponse, sensorResponse);
    }

    protected Task SendAsync(TankMoveParameters parameters)
    {
        return _server.SendUpdate(parameters);
    }

    protected abstract Task OnUpdate(TankResponse gameStateResponse, SensorResponse? sensorResponse);

    public async Task StartAsync()
    {
        while (_hubConnection.State == HubConnectionState.Disconnected)
        {
            try
            {
                await _hubConnection.StartAsync();
                var id = Id ?? Guid.NewGuid();
                await _server.RegisterAsync("Tank", Id, Name);
                Console.WriteLine("Connected");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }

}