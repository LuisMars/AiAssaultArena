using AiAssaultArena.Contract;
using AiAssaultArena.Simulation;
using AiAssaultArena.Simulation.Entities;
using AiAssaultArena.Simulation.Math;

namespace AiAssaultArena.Api.Mappers;

public static class ResponseMappers
{
    public static List<ArenaWallResponse> ToResponse(this IEnumerable<ArenaWallEntity> walls)
    {
        return walls.Select(w => w.ToResponse()).ToList();
    }

    public static ArenaWallResponse ToResponse(this ArenaWallEntity wall)
    {
        return new ArenaWallResponse
        {
            Width = wall.Width,
            Height = wall.Height,
            Start = wall.Start.ToResponse()
        };
    }

    public static TankResponse ToResponse(this TankEntity tank)
    {
        return new TankResponse
        {
            Id = tank.Id,
            Position = tank.Position.ToResponse(),
            Acceleration = tank.Acceleration,
            Velocity = tank.Velocity.ToResponse(),
            BodyRotation = tank.BodyRotation,
            TurretRotation = tank.TurretRotation,
            SensorRotation = tank.SensorRotation,
            Health = tank.Health,
            AngularVelocity = tank.AngularVelocity,
            CurrentTurretHeat = tank.CurrentTurretHeat
        };
    }

    public static SensorResponse? ToResponse(this SensorOutput? sensedTank)
    {
        if (sensedTank is null)
        {
            return null;
        }
        return new SensorResponse
        {
            Health = sensedTank.Tank.Health,
            Position = sensedTank.Position.ToResponse(),
            TankId = sensedTank.Tank.Id
        };
    }

    public static Vector2Response ToResponse(this Vector2 vector)
    {
        return new Vector2Response { X = vector.X, Y = vector.Y };
    }

    public static BulletResponse ToResponse(this BulletEntity bullet)
    {
        return new BulletResponse
        {
            Id = bullet.Id,
            Position = bullet.Position.ToResponse(),
            Velocity = bullet.Velocity.ToResponse(),
            ShooterId = bullet.ShooterId
        };
    }

    public static GameStateResponse GetGameStateResponse(this Runner runner)
    {
        return new GameStateResponse
        {
            Tanks = runner.Tanks.Select(t => t.ToResponse()).ToList(),
            Bullets = runner.Bullets.Values.Select(t => t.ToResponse()).ToList()
        };
    }
}
