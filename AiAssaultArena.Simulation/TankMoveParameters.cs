namespace AiAssaultArena.Simulation;

public class TankMoveParameters
{
    public float Acceleration { get; set; } // Positive or negative for forward/backward
    public float TurnSpeed { get; set; } // Positive or negative for right/left
    public float TurretTurnSpeed { get; set; } // Turret rotation speed
    public float SensorTurnSpeed { get; set; } // Sensor rotation speed
    public bool Shoot { get; set; }
}
