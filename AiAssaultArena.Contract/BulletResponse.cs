using System.Numerics;

namespace AiAssaultArena.Contract;
public record BulletResponse
{
    public Guid Id { get; set; }
    public Vector2Response Position { get; set; }
    public Vector2Response Velocity { get; set; }
    public Guid ShooterId { get; set; }
}
