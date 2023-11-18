namespace AiAssaultArena.Contract;
public record GameStateResponse
{
    public IEnumerable<TankResponse> Tanks { get; set; } = new List<TankResponse>();
    public IEnumerable<BulletResponse> Bullets { get; set; } = new List<BulletResponse>();
}
