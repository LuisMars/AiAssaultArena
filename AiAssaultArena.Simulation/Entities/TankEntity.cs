using AiAssaultArena.Contract;
using AiAssaultArena.Simulation.Math;

namespace AiAssaultArena.Simulation.Entities;

public class TankEntity
{
    public Vector2 Position { get; set; } = new();
    public float Acceleration { get; set; }
    public Vector2 Velocity { get; set; } = new();
    public float BodyRotation { get; set; }
    public float TurretRotation { get; set; }
    public float SensorRotation { get; set; }
    public float Health { get; set; } = MaxHealth;
    public Guid Id { get; private set; }
    public float AngularAcceleration { get; set; }
    public float AngularVelocity { get; set; }

    public float TurretAngularAcceleration { get; set; }
    public float TurretAngularVelocity { get; set; }
    public float SensorAngularAcceleration { get; set; }
    public float SensorAngularVelocity { get; set; }

    public float CurrentTurretHeat { get; set; } = 0;

    public const float MaxHealth = 100;

    public const float MomentOfInertia = 1f / 12f * Mass * (Width * Width + Height * Height);

    public const float Width = 40;
    public const float Height = 60;
    public const float HalfWidth = Width / 2;
    public const float HalfHeight = Height / 2;
    public const float Density = 78; // kg/dm³
    public const float Mass = Density * Width * Height * 40;
    public const float TurretLength = 50;

    public const float MaxAcceleration = 500f;
    public const float MaxSpeed = 125f;
    public const float MaxAngularAcceleration = 50f;
    public const float MaxAngularVelocity = 1.5f;

    public const float MaxTurretAngularVelocity = 0.5f;
    public const float MaxTurretAngularAcceleration = 50f;

    public const float MaxSensorAngularVelocity = 3f;
    public const float MaxSensorAngularAcceleration = 50f;
    
    public const float Friction = 0.1f;
    public const float AngularFriction = 0.00001f;
    public const float LateralDamping = 0.25f;

    public const float TurretCoolDown = 1f;

    public TankEntity(Vector2 position, Guid? id)
    {
        Position = position;
        Id = id ?? Guid.NewGuid();
    }

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
        if (CurrentTurretHeat != 0)
        {
            return null;
        }
        CurrentTurretHeat = TurretCoolDown;
        var bulletStartPosition = Position + new Vector2(0, TurretLength).Rotate(BodyRotation + TurretRotation); // Offset to start from turret
        var bullet = new BulletEntity(bulletStartPosition, Velocity, BodyRotation + TurretRotation, Id);
        return bullet;
    }
}
