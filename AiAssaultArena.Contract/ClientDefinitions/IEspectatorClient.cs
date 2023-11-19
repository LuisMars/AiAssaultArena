namespace AiAssaultArena.Contract.ClientDefinitions;

public interface ISharedClient
{
    Task OnParametersReceived(ParametersResponse parameters);
    Task OnRoundEnd();
}

public interface IMatchHubClient : ISharedClient
{
    Task OnGameUpdated(GameStateResponse gameStateResponse);
}

public interface ITankClient : ISharedClient
{
    Task OnTankReceived(TankReceivedResponse tankReceivedResponse);
    Task OnTankStateUpdated(TankResponse gameStateResponse, SensorResponse? sensorResponse);
}

public interface ITankServer
{
    Task SendUpdate(TankMoveParameters parameters);
    Task StartNew();
}

public interface IMatchServer : IMatchHubClient, ITankClient
{

}
