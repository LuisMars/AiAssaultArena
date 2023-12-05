using AiAssaultArena.Contract;
using AiAssaultArena.Simulation.Math;

namespace AiAssaultArena.Simulation.Entities;

public class TankEntity(Vector2 position, Guid? id)
{
    public Vector2 Position { get; set; } = position;
    public float Acceleration { get; set; }
    public Vector2 Velocity { get; set; } = new();
    public float BodyRotation { get; set; }
    public float TurretRotation { get; set; }
    public float SensorRotation { get; set; }
    public float Health { get; set; } = MaxHealth;
    public Guid Id { get; private set; } = id ?? Guid.NewGuid();
    public float AngularAcceleration { get; set; }
    public float AngularVelocity { get; set; }

    public float TurretAngularAcceleration { get; set; }
    public float TurretAngularVelocity { get; set; }
    public float SensorAngularAcceleration { get; set; }
    public float SensorAngularVelocity { get; set; }

    public float CurrentTurretHeat { get; set; } = 0;
    public bool IsOverheated => CurrentTurretHeat >= OverheatThreshold;

    public const float MaxHealth = 100;

    public const float MomentOfInertia = 1f / 12f * Mass * (Width * Width + Height * Height);

    public const float Width = 40;
    public const float Height = 60;
    public const float HalfWidth = Width / 2;
    public const float HalfHeight = Height / 2;
    public const float Density = 78; // kg/dm³
    public const float Mass = Density * Width * Height * 40;
    public const float TurretLength = 50;

    public const float MaxAcceleration = 250f;
    public const float MaxSpeed = 125f;
    public const float MaxAngularAcceleration = 50f;
    public const float MaxAngularVelocity = 1f;

    public const float MaxTurretAngularVelocity = 1f;
    public const float MaxTurretAngularAcceleration = 50f;

    public const float MaxSensorAngularVelocity = 1f;
    public const float MaxSensorAngularAcceleration = 100f;

    public const float Friction = 0.1f;
    public const float AngularFriction = 0.00001f;
    public const float LateralDamping = 0.25f;

    public const float MaxHeat = 100.0f; // Maximum heat capacity
    public const float HeatPerShot = 10.0f; // Heat generated per shot
    public const float HeatDissipationRate = 0.15f;
    public const float OverheatThreshold = 80.0f; // Overheat threshold

    public void Move(TankMoveParameters parameters)
    {
        //Update body rotation
        AngularAcceleration = parameters.TurnDirection.Clamp(1) * MaxAngularAcceleration;

        // Calculate forward vector and update velocity
        Acceleration = parameters.Acceleration.Clamp(1) * MaxAcceleration;

        // Update turret and sensor rotation
        TurretAngularAcceleration = parameters.TurretTurnDirection.Clamp(1) * MaxTurretAngularAcceleration;
        SensorAngularAcceleration = parameters.SensorTurnDirection.Clamp(1) * MaxSensorAngularAcceleration;
    }

    public BulletEntity Shoot()
    {
        if (CurrentTurretHeat >= MaxHeat)
        {
            Health -= 1;
            return null;
        }

        var failShot = CurrentTurretHeat >= OverheatThreshold && Random.Shared.NextSingle() > 0.5f;
        
        var heatIncrease = HeatPerShot * (1 + MathF.Pow(CurrentTurretHeat / MaxHeat, 2));
        CurrentTurretHeat = MathF.Min(MaxHeat, CurrentTurretHeat + heatIncrease);

        if (failShot)
        {
            return null;
        }

        var bulletStartPosition = Position + new Vector2(0, TurretLength).Rotate(TurretRotation); // Offset to start from turret
        var bullet = new BulletEntity(bulletStartPosition, Velocity, TurretRotation, Id);
        return bullet;
    }
}
