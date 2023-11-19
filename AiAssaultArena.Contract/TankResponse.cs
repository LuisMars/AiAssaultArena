namespace AiAssaultArena.Contract;

public record TankResponse
{
    public Vector2Response Position { get; set; }
    public float Acceleration { get; set; }
    public Vector2Response Velocity { get; set; }
    public float BodyRotation { get; set; }
    public float TurretRotation { get; set; }
    public float SensorRotation { get; set; }
    public float Health { get; set; }
    public Guid Id { get; set; }
    public float AngularVelocity { get; set; }
    public float CurrentTurretHeat { get; set; }
}
