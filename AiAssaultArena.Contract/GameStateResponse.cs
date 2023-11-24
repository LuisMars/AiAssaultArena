
namespace AiAssaultArena.Contract;
public record GameStateResponse
{
    public List<TankResponse> Tanks { get; set; } = new List<TankResponse>();
    public List<BulletResponse> Bullets { get; set; } = new List<BulletResponse>();
    public TimeSpan Elapsed { get; set; }
    public ulong UpdatesPerSecond { get; set; }
}
