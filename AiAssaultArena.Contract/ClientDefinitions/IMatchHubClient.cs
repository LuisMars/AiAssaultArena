namespace AiAssaultArena.Contract.ClientDefinitions;
public interface IMatchHubClient
{
    Task GetParameters(ParametersResponse parameters);
}
