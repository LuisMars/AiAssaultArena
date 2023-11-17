using System.Numerics;

namespace AiAssaultArena.Contract;

public record ArenaWallResponse
{
    public float Width { get; set; }
    public float Height { get; set; }
    public Vector2 Start { get; set; }
}
