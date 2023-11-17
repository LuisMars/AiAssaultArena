using System.Numerics;

namespace AiAssaultArena.Contract;
public record BulletResponse
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public Guid ShooterId { get; set; }
}
