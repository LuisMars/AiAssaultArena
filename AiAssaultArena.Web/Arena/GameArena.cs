using AiAssaultArena.Contract;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
namespace AiAssaultArena.Web.Arena;

public class GameArena
{
    public GraphicsDeviceManager Graphics { get; set; }
    public SpriteBatch SpriteBatch { get; private set; }
    public EntityDrawer EntityDrawer { get; set; }
    public List<TankResponse> Tanks { get; set; } = new(); 
    public List<BulletResponse> Bullets { get; set; } = new();
    public List<ArenaWallResponse> Walls { get; set; } = new();

    public GameArena(GraphicsDeviceManager graphics)
    {
        Graphics = graphics;
    }

    public void LoadContent(SpriteBatch spriteBatch)
    {
        SpriteBatch = spriteBatch;
        EntityDrawer = new EntityDrawer(spriteBatch, new ParametersResponse());
    }

    public void Draw(GameTime gameTime)
    {
        Graphics.GraphicsDevice.Clear(Color.Black);

        if (SpriteBatch is null)
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
