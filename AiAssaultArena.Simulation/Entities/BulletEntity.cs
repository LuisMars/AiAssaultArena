using AiAssaultArena.Simulation.Math;

namespace AiAssaultArena.Simulation.Entities;

public class BulletEntity
{
    public const float Speed = 450f;
    public const float Radius = 5f;

    public Guid Id { get; set; } = Guid.NewGuid();
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public Guid ShooterId { get; set; }

    public BulletEntity(Vector2 position, Vector2 velocity, float rotation, Guid shooterId)
    {
        Position = position;
        Velocity = new Vector2(0, Speed).Rotate(rotation) + velocity;
        //Velocity = Velocity.Truncate(Speed);
        ShooterId = shooterId;
    }
}
