using AiAssaultArena.Web.Arena;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AiAssaultArena.Web;
/// <summary>
/// This is the main type for your game.
/// </summary>
public class MainGame : Game
{
    public GraphicsDeviceManager Graphics { get; private set; }
    public SpriteBatch SpriteBatch { get; private set; }

    public GameArena Arena { get; set; }

    public MainGame()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        Arena = new GameArena(Graphics);
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();

    }

    protected override void LoadContent()
    {
        // Create a new SpriteBatch, which can be used to draw textures.
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Arena.LoadContent(SpriteBatch);
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

    protected override void Draw(GameTime gameTime)
    {
        Arena.Draw(gameTime);

        base.Draw(gameTime);
    }
}
