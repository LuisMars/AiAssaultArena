namespace AiAssaultArena.Contract;

public record SensorResponse
{
    public Guid TankId { get; set; }
    public Vector2Response Position { get; set; }
    public float Health { get; set; }
}