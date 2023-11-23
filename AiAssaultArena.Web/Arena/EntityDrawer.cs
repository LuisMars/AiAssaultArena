using AiAssaultArena.Contract;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Threading.Tasks;

namespace AiAssaultArena.Web.Arena;

public class EntityDrawer
{
    private readonly SpriteBatch _spriteBatch;
    private readonly ParametersResponse _parameters;
    private readonly Texture2D _tracks;
    private readonly Texture2D _body;
    private readonly Texture2D _turret;
    private readonly Texture2D _sensor;
    private readonly Texture2D _bullet;

    public EntityDrawer(SpriteBatch spriteBatch, ParametersResponse parameters, Texture2D tracks, Texture2D body, Texture2D turret, Texture2D sensor, Texture2D bullet)
    {
        _spriteBatch = spriteBatch;
        _parameters = parameters;
        _tracks = tracks;
        _body = body;
        _turret = turret;
        _sensor = sensor;
        _bullet = bullet;
    }

    public Color FromId(Guid id)
    {
        var colorBytes = id.ToByteArray();
        return new Color((colorBytes[0] + colorBytes[3]) / 2, (colorBytes[1] + colorBytes[4]) / 2, (colorBytes[2] + colorBytes[5]) / 2);
    }

    public void DrawBullet(BulletResponse bullet)
    {
        var color = FromId(bullet.ShooterId);
        var scale = _parameters.BulletRadius / _bullet.Width;
        var origin = new Vector2(_parameters.BulletRadius * 0.5f);
        var position = new Vector2(bullet.Position.X, bullet.Position.Y);
        var angle = new Vector2(bullet.Velocity.X, bullet.Velocity.Y).ToAngle();
        var shadowOffset = new Vector2(4);

        var texturePosition = position - origin.Rotate(angle);
        var textureShadowPosition = (position + shadowOffset) - origin.Rotate(angle);
        var shadowColor = new Color(Color.Black, 0.25f);

        _spriteBatch.Draw(_bullet, textureShadowPosition, null, shadowColor, angle, origin, scale, SpriteEffects.None, 1);
        _spriteBatch.Draw(_bullet, texturePosition, null, color, angle, origin, scale, SpriteEffects.None, 1);

        //_spriteBatch.DrawCircle(bullet.Position.X, bullet.Position.Y, _parameters.BulletRadius, 20, Color.Red, 1);
    }

    public void DrawWall(ArenaWallResponse wall)
    {
        _spriteBatch.FillRectangle(wall.Start.X, wall.Start.Y, wall.Width, wall.Height, Color.Red, 1);
    }

    public void DrawTank(TankResponse tank)
    {
        var color = FromId(tank.Id);
        var size = new Vector2(_parameters.Width, _parameters.Height);
        var origin = size * 0.5f;
        var position = new Vector2(tank.Position.X, tank.Position.Y);
        var shadowOffset = new Vector2(4);
        var scale = _parameters.Width / _tracks.Width;
        var texturePosition = position - origin.Rotate(tank.BodyRotation);
        var shadowPosition = (position + shadowOffset) - origin.Rotate(tank.BodyRotation);
        var shadowColor = new Color(Color.Black, 0.25f);


        _spriteBatch.Draw(_tracks, shadowPosition, null, shadowColor, tank.BodyRotation, origin, scale, SpriteEffects.None, 1);
        _spriteBatch.Draw(_tracks, texturePosition, null, Color.White, tank.BodyRotation, origin, scale, SpriteEffects.None, 1);

        _spriteBatch.Draw(_body, shadowPosition, null, shadowColor, tank.BodyRotation, origin, scale, SpriteEffects.None, 1);
        _spriteBatch.Draw(_body, texturePosition, null, color, tank.BodyRotation, origin, scale, SpriteEffects.None, 1);

        var turretPosition = position - origin.Rotate(tank.BodyRotation + tank.TurretRotation);
        var turretShadowPosition = (position + shadowOffset) - origin.Rotate(tank.BodyRotation + tank.TurretRotation);

        _spriteBatch.Draw(_turret, turretShadowPosition, null, shadowColor, tank.BodyRotation + tank.TurretRotation, origin, scale, SpriteEffects.None, 1);
        _spriteBatch.Draw(_turret, turretPosition, null, color, tank.BodyRotation + tank.TurretRotation, origin, scale, SpriteEffects.None, 1);

        var sensorPosition = position - origin.Rotate(tank.BodyRotation + tank.SensorRotation);
        var sensorShadowPosition = (position + shadowOffset) - origin.Rotate(tank.BodyRotation + tank.SensorRotation);

        _spriteBatch.Draw(_sensor, sensorShadowPosition, null, shadowColor, tank.BodyRotation + tank.SensorRotation, origin, scale, SpriteEffects.None, 1);
        _spriteBatch.Draw(_sensor, sensorPosition, null, Color.White, tank.BodyRotation + tank.SensorRotation, origin, scale, SpriteEffects.None, 1);

        //var rectangle = new RectangleF((tank.Position.X, tank.Position.Y), new Size2(_parameters.Width, _parameters.Height));
        //_spriteBatch.DrawRectangle(rectangle, Color.White, tank.BodyRotation, origin, 1f);
        //var end = position + new Vector2(0, 50).Rotate(tank.BodyRotation + tank.TurretRotation);
        //_spriteBatch.DrawLine(position, end, Color.White);
        //_spriteBatch.DrawCircle(position, 10, 10, Color.White);
    }
}
