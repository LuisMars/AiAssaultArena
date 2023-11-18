﻿using AiAssaultArena.Contract;
using AiAssaultArena.Simulation;
using AiAssaultArena.Simulation.Entities;
using AiAssaultArena.Simulation.Math;

namespace AiAssaultArena.Api.Mappers;

public static class ResponseMappers
{
    public static IEnumerable<ArenaWallResponse> ToResponse(this IEnumerable<ArenaWallEntity> walls)
    {
        return walls.Select(w => w.ToResponse());
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
            AngularVelocity = tank.AngularVelocity
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
            Position = bullet.Position.ToTuple(),
            Velocity = bullet.Velocity.ToTuple(),
            ShooterId = bullet.ShooterId
        };
    }

    public static GameStateResponse GetGameStateResponse(this Runner runner)
    {
        return new GameStateResponse
        {
            Tanks = runner.Tanks.Select(t => t.ToResponse()),
            Bullets = runner.Bullets.Select(t => t.ToResponse())
        };
    }
}