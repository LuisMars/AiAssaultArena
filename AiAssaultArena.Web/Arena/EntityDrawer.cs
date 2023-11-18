using AiAssaultArena.Contract;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;

namespace AiAssaultArena.Web.Arena;

public class EntityDrawer
{
    private readonly SpriteBatch _spriteBatch;
    private readonly ParametersResponse _parameters;

    public EntityDrawer(SpriteBatch spriteBatch, ParametersResponse parameters)
    {
        _spriteBatch = spriteBatch;
        this._parameters = parameters;
    }
    public void DrawBullet(BulletResponse bullet)
    {
        _spriteBatch.DrawCircle(bullet.Position, _parameters.Radius, 20, Color.Red, 1);
    }

    public void DrawWall(ArenaWallResponse wall)
    {
        _spriteBatch.FillRectangle(wall.Start.X, wall.Start.Y, wall.Width, wall.Height, Color.Red, 1);
    }

    public void DrawTank(TankResponse tank)
    {
        var origin = new Vector2(_parameters.Width / 2, _parameters.Height / 2);
        var rectangle = new RectangleF((tank.Position.X, tank.Position.Y), new Size2(_parameters.Width, _parameters.Height));
        _spriteBatch.DrawRectangle(rectangle, Color.Red, tank.BodyRotation, origin, 1f);

        var position = new Vector2(tank.Position.X, tank.Position.Y);
        var end = position + new Vector2(0, 50).Rotate(tank.BodyRotation + tank.TurretRotation);
        _spriteBatch.DrawLine(position, end, Color.Red);

        _spriteBatch.DrawCircle(position, 10, 10, Color.Wheat);
    }
}
