using System.Numerics;

namespace AiAssaultArena.Contract;
public record BulletResponse
{
    public Guid Id { get; set; }
    public (float X, float Y) Position { get; set; }
    public (float X, float Y) Velocity { get; set; }
    public Guid ShooterId { get; set; }
}
