using AiAssaultArena.Contract;
using AiAssaultArena.Contract.ClientDefinitions;
using AiAssaultArena.Web.Arena;
using AiAssaultArena.Web.Hub;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiAssaultArena.Web;


public class MainGame : Game, IMatchHubClient
{
    private readonly Client _client;

    public SpriteBatch SpriteBatch { get; private set; }
    public EntityDrawer EntityDrawer { get; set; }

    public Dictionary<Guid, string> ConnectedTanks { get; private set; } = new();

    public List<TankResponse> Tanks { get; set; } = new List<TankResponse>();
    public List<BulletResponse> Bullets { get; set; } = new List<BulletResponse>();
    public TimeSpan Elapsed { get; private set; }
    public ulong UpdatesPerSecond { get; private set; }
    public List<ArenaWallResponse> Walls { get; set; } = new List<ArenaWallResponse>();
    public ParametersResponse Parameters { get; set; }

    private Texture2D Tracks { get; set; }
    private Texture2D Body { get; set; }
    private Texture2D Turret { get; set; }
    private Texture2D Sensor { get; set; }
    private Texture2D Bullet { get; set; }
    public Action OnMessage { get; set; }

    public bool IsGameOver { get; set; }

    public float Zoom { get; set; } = 1.0f;
    public Vector2 Pan { get; set; } = new();

    public GraphicsDeviceManager Graphics { get; private set; }



    public MainGame(Client client, Action onMessage, Guid? id)
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        _client = client;
        _client.Register(this);
        _client.Server.RegisterAsync("WebClient", id);
        OnMessage = onMessage;
    }

    public void ZoomBy(float amount)
    {
        Zoom += amount;
    }

    public Task OnMatchStart(ParametersResponse parameters)
    {
        IsGameOver = false;
        Parameters = parameters;
        Walls = parameters.Walls;
        EntityDrawer = new EntityDrawer(SpriteBatch, Parameters, Tracks, Body, Turret, Sensor, Bullet);
        OnMessage();
        return Task.CompletedTask;
    }

    public Task OnGameUpdated(GameStateResponse gameStateResponse)
    {
        Tanks = gameStateResponse.Tanks;
        Bullets = gameStateResponse.Bullets;
        Elapsed = gameStateResponse.Elapsed;
        UpdatesPerSecond = gameStateResponse.UpdatesPerSecond;
        OnMessage();
        return Task.CompletedTask;
    }

    public Task OnRoundEnd()
    {
        IsGameOver = true;
        OnMessage();
        return Task.CompletedTask;
    }

    public void Clear()
    {
        Tanks.Clear();
        Bullets.Clear();
        Walls.Clear();
    }

    public Task OnTankAvailable(string tankName, Guid tankId)
    {
        ConnectedTanks[tankId] = tankName;
        OnMessage();
        return Task.CompletedTask;
    }

    public Task OnTankUnavailable(Guid tankId)
    {
        ConnectedTanks.Remove(tankId);
        if (Tanks.Any(tank => tank.Id == tankId))
        {
            IsGameOver = true;
            Clear();
        }
        OnMessage();
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

    private MouseState PreviousMouseState { get; set; }

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
        // Handle mouse input here
        var mouseState = Mouse.GetState();

        // Zoom with the mouse wheel
        Zoom += (mouseState.ScrollWheelValue - PreviousMouseState.ScrollWheelValue) * 0.0005f;
        Zoom = Math.Max(0.1f, Math.Min(10, Zoom)); // Keep zoom within limits

        // Pan with mouse drag
        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            Pan += new Vector2(PreviousMouseState.X - mouseState.X, PreviousMouseState.Y - mouseState.Y) / Zoom;
        }
        if (mouseState.MiddleButton == ButtonState.Pressed)
        {
            Pan = new Vector2();
            Zoom = 1;
        }
        PreviousMouseState = mouseState;

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
        var scaleMatrix = Matrix.CreateScale(scale, scale, 1);
        var zoomMatrix = Matrix.CreateScale(Zoom * Zoom, Zoom * Zoom, 1);
        var centerMatrix = Matrix.CreateTranslation(Graphics.GraphicsDevice.Viewport.Width / 2, Graphics.GraphicsDevice.Viewport.Height / 2, 0);
        var panMatrix = Matrix.CreateTranslation(-Pan.X, -Pan.Y, 0);
        Matrix transformMatrix = scaleMatrix * panMatrix * zoomMatrix * centerMatrix;

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

        var arenaColor = new Color(193, 203, 211);
        var lightArenaColor = new Color(210, 218, 223);
        var darkArenaColor = new Color(176, 189, 199);
        SpriteBatch.FillRectangle(-Parameters.ArenaWidth / 2 + 8, -Parameters.ArenaHeight / 2 + 8, Parameters.ArenaWidth, Parameters.ArenaHeight, new Color(0, 0, 0, 0.125f));
        SpriteBatch.FillRectangle(-Parameters.ArenaWidth / 2, -Parameters.ArenaHeight / 2, Parameters.ArenaWidth, Parameters.ArenaHeight, lightArenaColor);
        SpriteBatch.FillRectangle(4 + -Parameters.ArenaWidth / 2, 4 + -Parameters.ArenaHeight / 2, Parameters.ArenaWidth - 4, Parameters.ArenaHeight - 4, darkArenaColor);
        SpriteBatch.FillRectangle(4 + -Parameters.ArenaWidth / 2, 4 + -Parameters.ArenaHeight / 2, Parameters.ArenaWidth - 8, Parameters.ArenaHeight - 8, arenaColor);
        for (int i = 0; i < Tanks.Count; i++)
        {
            var tank = Tanks[i];
            EntityDrawer.DrawTank(tank, i);
            foreach (var bullet in Bullets.Where(b => b.ShooterId == tank.Id))
            {
                EntityDrawer.DrawBullet(bullet, i);
            }
        }
        //Walls.ForEach(EntityDrawer.DrawWall);
        SpriteBatch.End();

        base.Draw(gameTime);
    }

}
