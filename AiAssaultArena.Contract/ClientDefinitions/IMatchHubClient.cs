namespace AiAssaultArena.Contract.ClientDefinitions;
public interface IMatchHubClient
{
    Task OnParametersReceived(ParametersResponse parameters);
    Task OnGameUpdated(GameStateResponse gameStateResponse);
}
