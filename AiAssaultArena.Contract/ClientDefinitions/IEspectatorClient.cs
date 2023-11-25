namespace AiAssaultArena.Contract.ClientDefinitions;

public interface ISharedClient
{
    Task OnMatchStart(ParametersResponse parameters);
    Task OnRoundEnd();
}

public interface IMatchHubClient : ISharedClient
{
    Task OnGameUpdated(GameStateResponse gameStateResponse);

    Task OnTankAvailable(string tankName, Guid tankId);

    Task OnTankUnavailable(Guid tankId);
}

public interface ITankClient : ISharedClient
{
    Task OnRegisterSuccesfull(TankReceivedResponse tankReceivedResponse);
    Task OnTankStateUpdated(TankResponse gameStateResponse, SensorResponse? sensorResponse);
}

public interface ISharedServer
{
    Task RegisterAsync(string clientType, Guid? guid, string? name = null);
}

public interface ITankServer : ISharedServer
{
    Task SendUpdate(TankMoveParameters parameters);
}

public interface IWebServer : ISharedServer
{
    Task StartMatchAsync(Guid tankAId, Guid tankBId);
}

public interface IServer : IWebServer, ITankServer
{

}

public interface IMatchServer : IMatchHubClient, ITankClient
{

}
