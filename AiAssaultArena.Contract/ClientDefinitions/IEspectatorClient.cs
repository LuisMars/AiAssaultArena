namespace AiAssaultArena.Contract.ClientDefinitions;

public interface ISharedClient
{
    Task OnParametersReceived(ParametersResponse parameters);
    Task OnRoundEnd();
}

public interface IMatchHubClient : ISharedClient
{
    Task OnGameUpdated(GameStateResponse gameStateResponse);
    
    Task OnTankConnected(string tankName, Guid tankId);

    Task OnTankDisconnected(Guid tankId);
}

public interface ITankClient : ISharedClient
{
    Task OnTankReceived(TankReceivedResponse tankReceivedResponse);
    Task OnTankStateUpdated(TankResponse gameStateResponse, SensorResponse? sensorResponse);
}

public interface ITankServer
{
    Task SendUpdate(TankMoveParameters parameters);
}

public interface IWebServer
{
    Task StartMatch(Guid tankAId, Guid tankBId);
}

public interface IServer : IWebServer, ITankServer
{

}

public interface IMatchServer : IMatchHubClient, ITankClient
{

}
