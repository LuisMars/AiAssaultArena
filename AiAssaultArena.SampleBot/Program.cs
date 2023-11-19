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
			.WithUrl("http://localhost:5167/Match?clientType=Tank")
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
			await _hubConnection.StartAsync();
		};
		_server = _hubConnection.CreateHubProxy<ITankServer>();

		_ = Task.Run(async () =>
		{
			while (true)
			{
				await Task.Delay(10000);
				await _server.SendUpdate(new TankMoveParameters { Shoot = true, TurretTurnDirection = -1, SensorTurnDirection = 1, TurnDirection = -1, Acceleration = 1 });
				await Task.Delay(10000);
				await _server.SendUpdate(new TankMoveParameters { Shoot = true, TurretTurnDirection = 0, SensorTurnDirection = 0, TurnDirection = 0, Acceleration = 0 });
			}
		});
	}

	private Guid Id { get; set; }
	private ParametersResponse Parameters { get; set; }

	public void Dispose()
	{
		_hubConnection.StopAsync();
		_hubConnection.DisposeAsync();
	}

	public Task OnParametersReceived(ParametersResponse parameters)
	{
		Parameters = parameters;
		return Task.CompletedTask;
	}

	public Task OnTankReceived(TankReceivedResponse tankReceivedResponse)
	{
		Id = tankReceivedResponse.TankId;
		return Task.CompletedTask;
	}

	public Task OnTankStateUpdated(TankResponse gameStateResponse)
	{
		return _server.SendUpdate(new TankMoveParameters { Shoot = true });
	}

	public Task StartAsync()
	{
		return _hubConnection.StartAsync();
	}
}

