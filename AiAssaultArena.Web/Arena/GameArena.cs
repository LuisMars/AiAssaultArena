using AiAssaultArena.Contract;
using AiAssaultArena.Contract.ClientDefinitions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TypedSignalR.Client;

namespace AiAssaultArena.Web.Arena;

public class GameArena : IMatchHubClient
{
    public GraphicsDeviceManager Graphics { get; set; }
    public SpriteBatch SpriteBatch { get; private set; }
    public EntityDrawer EntityDrawer { get; set; }
    public List<TankResponse> Tanks { get; set; } = new();
    public List<BulletResponse> Bullets { get; set; } = new();
    public List<ArenaWallResponse> Walls { get; set; } = new();

    private readonly HubConnection _hubConnection;
    public GameArena(GraphicsDeviceManager graphics)
    {
        Graphics = graphics;
        _hubConnection = new HubConnectionBuilder().WithUrl("http://localhost:5167/match").Build();
        _hubConnection.Register<IMatchHubClient>(this);
        _hubConnection.StartAsync();
    }

    public Task GetParameters(ParametersResponse parameters)
    {
        EntityDrawer = new EntityDrawer(SpriteBatch, new ParametersResponse());
        return Task.CompletedTask;
    }

    public void LoadContent(SpriteBatch spriteBatch)
    {
        SpriteBatch = spriteBatch; 
    }

    public void Draw(GameTime gameTime)
    {
        Graphics.GraphicsDevice.Clear(Color.Black);

        if (SpriteBatch is null || EntityDrawer is null)
        {
            return;
        }

        SpriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(Graphics.GraphicsDevice.Viewport.Width / 2, Graphics.GraphicsDevice.Viewport.Height / 2, 0));

        Tanks.ForEach(tank => EntityDrawer.DrawTank(tank));
        Bullets.ForEach(bullet => EntityDrawer.DrawBullet(bullet));
        Walls.ForEach(wall => EntityDrawer.DrawWall(wall));

        SpriteBatch.End();
    }
}
