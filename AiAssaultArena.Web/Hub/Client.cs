using AiAssaultArena.Contract.ClientDefinitions;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using TypedSignalR.Client;

namespace AiAssaultArena.Web.Hub;

public class Client
{
    private HubConnection HubConnection { get; set; }
    public bool IsConnected { get; set; }
    public IWebServer Server { get; private set; }
    public Action OnMessage { get; set; }

    public async Task ConnectAsync(string url)
    {
        HubConnection = new HubConnectionBuilder().WithUrl($"{url}/match").Build();
        Server = HubConnection.CreateHubProxy<IWebServer>();

        HubConnection.Closed += async (exception) =>
        {
            IsConnected = false;
            OnMessage();
            Console.WriteLine($"Connection closed: {exception?.Message}");
            await Task.Delay(TimeSpan.FromSeconds(5)); // Reconnect after a delay
            await HubConnection.StartAsync();
            IsConnected = true;
            OnMessage();
        };
        await HubConnection.StartAsync();
        IsConnected = true;
    }
    public void Register(IMatchHubClient client)
    {
        HubConnection.Register(client);
    }

    public Task StartMatchAsync(Guid tankAId, Guid tankBId)
    {
        return Server.StartMatchAsync(tankAId, tankBId);
    }
}
