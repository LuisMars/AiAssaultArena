namespace AiAssaultArena.Contract;

public class TankMoveParameters
{
    public float Acceleration { get; set; } // Positive or negative for forward/backward
    public float TurnDirection { get; set; } // Positive or negative for right/left
    public float TurretTurnDirection { get; set; } // Turret rotation speed
    public float SensorTurnDirection { get; set; } // Sensor rotation speed
    public bool Shoot { get; set; }
}
