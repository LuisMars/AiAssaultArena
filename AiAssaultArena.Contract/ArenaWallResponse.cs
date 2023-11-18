namespace AiAssaultArena.Contract;

public record ArenaWallResponse
{
    public float Width { get; set; }
    public float Height { get; set; }
    public Vector2Response Start { get; set; }
}
