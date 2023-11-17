using System.Numerics;

namespace AiAssaultArena.Contract;

public record TankResponse
{
    public Vector2 Position { get; set; }
    public float Acceleration { get; set; }
    public Vector2 Velocity { get; set; }
    public float BodyRotation { get; set; }
    public float TurretRotation { get; set; }
    public float SensorRotation { get; set; }
    public int Health { get; set; }
    public Guid Id { get; set; }
    public float AngularVelocity { get; set; }
}
