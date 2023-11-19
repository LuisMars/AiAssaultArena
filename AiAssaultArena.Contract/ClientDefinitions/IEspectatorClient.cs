namespace AiAssaultArena.Contract.ClientDefinitions;

public interface IParameterClient
{
    Task OnParametersReceived(ParametersResponse parameters);
}

public interface IMatchHubClient : IParameterClient
{
    Task OnGameUpdated(GameStateResponse gameStateResponse);
}

public interface ITankClient : IParameterClient
{
    Task OnTankReceived(TankReceivedResponse tankReceivedResponse);
    Task OnTankStateUpdated(TankResponse gameStateResponse);
}

public interface ITankServer
{
    Task SendUpdate(TankMoveParameters parameters);
}

public interface IMatchClient : IMatchHubClient, ITankClient, ITankServer
{

}
