﻿using AiAssaultArena.Contract;
using AiAssaultArena.Contract.ClientDefinitions;
using AiAssaultArena.Web.Arena;
using AiAssaultArena.Web.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TypedSignalR.Client;

namespace AiAssaultArena.Web;


public class MainGame : Game, IMatchHubClient
{
    public SpriteBatch SpriteBatch { get; private set; }
    public EntityDrawer EntityDrawer { get; set; }

    public Dictionary<Guid, string> ConnectedTanks {  get; private set; } = new();

    public IEnumerable<TankResponse> Tanks { get; set; } = new List<TankResponse>();
    public IEnumerable<BulletResponse> Bullets { get; set; } = new List<BulletResponse>();
    public IEnumerable<ArenaWallResponse> Walls { get; set; } = new List<ArenaWallResponse>();
    public ParametersResponse Parameters { get; set; }

    private Texture2D Tracks { get; set; }
    private Texture2D Body { get; set; }
    private Texture2D Turret { get; set; }
    private Texture2D Sensor { get; set; }
    private Texture2D Bullet { get; set; }


    private readonly HubConnection _hubConnection;
    public GraphicsDeviceManager Graphics { get; private set; }


    public MainGame()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        _hubConnection = new HubConnectionBuilder().WithUrl("http://localhost:5167/match?clientType=WebClient").Build();
        _hubConnection.Register<IMatchHubClient>(this);
        _hubConnection.StartAsync();
    }

    public Task OnParametersReceived(ParametersResponse parameters)
    {
        Parameters = parameters;
        Walls = parameters.Walls;
        EntityDrawer = new EntityDrawer(SpriteBatch, Parameters, Tracks, Body, Turret, Sensor, Bullet);

        return Task.CompletedTask;
    }

    public Task OnGameUpdated(GameStateResponse gameStateResponse)
    {
        Tanks = gameStateResponse.Tanks;
        Bullets = gameStateResponse.Bullets;
        return Task.CompletedTask;
    }

    public Task OnRoundEnd()
    {
        return Task.CompletedTask;
    }

    public Task OnTankConnected(string tankName, Guid tankId)
    {
        ConnectedTanks[tankId] = tankName;
        return Task.CompletedTask;
    }

    public Task OnTankDisconnected(Guid tankId)
    {
        ConnectedTanks.Remove(tankId);
        return Task.CompletedTask;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Create a new SpriteBatch, which can be used to draw textures.
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Tracks = Content.Load<Texture2D>("tracks");
        Body = Content.Load<Texture2D>("body");
        Turret = Content.Load<Texture2D>("turret");
        Sensor = Content.Load<Texture2D>("sensor");
        Bullet = Content.Load<Texture2D>("bullet");
    }

    protected override void UnloadContent()
    {
        // TODO: Unload any non ContentManager content here
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Escape) || keyboardState.IsKeyDown(Keys.Back))
        {
#if ANDROID
            // Exit() is obsolete on ANDROID
            Microsoft.Xna.Framework.Game.Activity.MoveTaskToBack(true);
#elif __IOS__ || __TVOS__
            // Exit() is obsolete on iOS
#else
            Exit();
#endif
        }

        base.Update(gameTime);
    }

    private Matrix GetTransformMatrix()
    {
        // Desired maximum width and height
        float maxWidth = Parameters.ArenaWidth;
        float maxHeight = Parameters.ArenaHeight;

        // Calculate scaling factors for width and height
        float scaleX = Graphics.GraphicsDevice.Viewport.Width / maxWidth;
        float scaleY = Graphics.GraphicsDevice.Viewport.Height / maxHeight;

        // Choose the smaller scaling factor to fit within the display
        float scale = Math.Min(1, Math.Min(scaleX, scaleY));
        // Create the transformation matrix
        Matrix transformMatrix = Matrix.CreateScale(scale) * Matrix.CreateTranslation(Graphics.GraphicsDevice.Viewport.Width / 2, Graphics.GraphicsDevice.Viewport.Height / 2, 0);

        return transformMatrix;
    }

    protected override void Draw(GameTime gameTime)
    {
        if (SpriteBatch is null)
        {
            return;
        }

        //Graphics.GraphicsDevice.Clear(Color.Purple);
        if (EntityDrawer is null)
        {
            SpriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(Graphics.GraphicsDevice.Viewport.Width / 2, Graphics.GraphicsDevice.Viewport.Height / 2, 0));
            SpriteBatch.DrawCircle(0, 0, 100, 32, Color.Red);
            SpriteBatch.End();
            return;
        }

        SpriteBatch.Begin(transformMatrix: GetTransformMatrix());
        //SpriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(Graphics.GraphicsDevice.Viewport.Width / 2, Graphics.GraphicsDevice.Viewport.Height / 2, 0));

        SpriteBatch.FillRectangle(-Parameters.ArenaWidth / 2 + 4, -Parameters.ArenaHeight / 2 + 4, Parameters.ArenaWidth, Parameters.ArenaHeight, new Color(0.75f, 0.75f, 0.75f));
        SpriteBatch.FillRectangle(-Parameters.ArenaWidth / 2, -Parameters.ArenaHeight / 2, Parameters.ArenaWidth, Parameters.ArenaHeight, new Color(0.5f, 0.5f, 0.5f));
        Tanks.ForEach(EntityDrawer.DrawTank);
        Bullets.ForEach(EntityDrawer.DrawBullet);
        //Walls.ForEach(EntityDrawer.DrawWall);
        SpriteBatch.End();

        base.Draw(gameTime);
    }

}
