using AiAssaultArena.Contract;
using AiAssaultArena.Simulation.Math;

namespace AiAssaultArena.Simulation.Entities;

public class TankEntity
{
    public Vector2 Position { get; set; }
    public float Acceleration { get; set; }
    public Vector2 Velocity { get; set; }
    public float BodyRotation { get; set; }
    public float TurretRotation { get; set; }
    public float SensorRotation { get; set; }
    public int Health { get; set; }
    public Guid Id { get; private set; }
    public float AngularAcceleration { get; set; }
    public float AngularVelocity { get; set; }

    public float TurretAngularAcceleration { get; set; }
    public float TurretAngularVelocity { get; set; }
    public float SensorAngularAcceleration { get; set; }
    public float SensorAngularVelocity { get; set; }

    public const float MomentOfInertia = 1f / 12f * Mass * (Width * Width + Height * Height);
    public const float Width = 40;
    public const float Height = 60;
    public const float HalfWidth = Width / 2;
    public const float HalfHeight = Height / 2;
    public const float Density = 78; // kg/dm³
    public const float Mass = Density * Width * Height * 40;
    public const float TurretLength = 50;

    public const float MaxAcceleration = 500f;
    public const float MaxSpeed = 200f;
    public const float MaxAngularAcceleration = 50f;
    public const float MaxAngularVelocity = 2.5f;

    public const float MaxTurretAngularVelocity = 1.5f;
    public const float MaxTurretAngularAcceleration = 50f;

    public const float MaxSensorAngularVelocity = 15f;
    public const float MaxSensorAngularAcceleration = 100f;
    
    public const float Friction = 0.1f;
    public const float AngularFriction = 0.00001f;
    public const float LateralDamping = 0.25f;

    public TankEntity(Vector2 position)
    {
        Position = position;
        Velocity = Vector2.Zero;
        Acceleration = 0;
        BodyRotation = 0f;
        TurretRotation = 0f;
        SensorRotation = 0f;
        Health = 100;
        Id = Guid.NewGuid();
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
        var bulletStartPosition = Position + new Vector2(0, TurretLength).Rotate(BodyRotation + TurretRotation); // Offset to start from turret
        var bullet = new BulletEntity(bulletStartPosition, Velocity, BodyRotation + TurretRotation, Id);
        return bullet;
    }
}
