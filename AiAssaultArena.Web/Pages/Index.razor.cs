using AiAssaultArena.Web.Hub;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace AiAssaultArena.Web.Pages;
public partial class Index
{
    //[Inject]
    public MainGame Game { get; set; }

    [Inject]
    public Client Client { get; set; }

    public Guid TankAId { get; set; }
    public Guid TankBId { get; set; }

    public bool IsInitialized { get; set; }

    public string Url { get; set; }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (Client.IsConnected && !IsInitialized)
        {
            JsRuntime.InvokeAsync<object>("initRenderJS", DotNetObjectReference.Create(this));
            IsInitialized = true;
        }
    }

    [JSInvokable]
    public void TickDotNet()
    {
        if (!Client.IsConnected)
        {
            return;
        }

        // init game
        if (Game == null)
        {
            Game = new MainGame(Client, StateHasChanged);
            Game.Run();
        }

        // run gameloop
        Game.Tick();
        StateHasChanged();
    }

    private async Task ConnectAsync(MouseEventArgs e)
    {
        await Client.ConnectAsync(Url);
        Client.OnMessage = StateHasChanged;
    }
}
