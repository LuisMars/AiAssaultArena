using AiAssaultArena.Contract;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using Microsoft.Xna.Framework;
using System;
using System.Security.Cryptography;

namespace AiAssaultArena.Web.Pages;
public partial class Index
{
    Game _game;

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            JsRuntime.InvokeAsync<object>("initRenderJS", DotNetObjectReference.Create(this));
        }
    }

    [JSInvokable]
    public void TickDotNet()
    {
        // init game
        if (_game == null)
        {
            _game = new MainGame();
            _game.Run();
        }

        // run gameloop
        _game.Tick();
    }

}