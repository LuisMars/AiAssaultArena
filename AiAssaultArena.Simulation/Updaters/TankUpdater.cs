using AiAssaultArena.Simulation.Entities;
using AiAssaultArena.Simulation.Math;

namespace AiAssaultArena.Simulation.Updaters;

public class TankUpdater
{
    public static void Update(float deltaSeconds, TankEntity tank)
    {
        // Apply angular acceleration
        tank.AngularVelocity += tank.AngularAcceleration * deltaSeconds;
        tank.AngularVelocity = tank.AngularVelocity.Clamp(TankEntity.MaxAngularVelocity);
        // Apply angular friction to slow down angular velocity (angular damping)
        var angularFrictionFactor = MathF.Pow(TankEntity.AngularFriction, deltaSeconds);
        tank.AngularVelocity *= angularFrictionFactor;

        tank.BodyRotation += tank.AngularVelocity * deltaSeconds;
        tank.BodyRotation = NormalizeRotation(tank.BodyRotation);


        // Calculate the direction of the tank's forward movement
        var forwardDirection = new Vector2(1, 0).Rotate(tank.BodyRotation);

        // Calculate the dot product of the tank's velocity and forward direction
        var dotProduct = Vector2.Dot(tank.Velocity, forwardDirection);

        // Apply lateral damping (making it harder to move perpendicular to body rotation)
        if (dotProduct < 0)
        {
            tank.Velocity -= forwardDirection * dotProduct * (1 - TankEntity.LateralDamping);
        }

        // Update the position based on the velocity
        tank.Velocity += new Vector2(0, tank.Acceleration).Rotate(tank.BodyRotation) * deltaSeconds;
        tank.Velocity = tank.Velocity.Truncate(TankEntity.MaxSpeed);

        // Apply friction to slow down the tank
        var frictionFactor = MathF.Pow(TankEntity.Friction, deltaSeconds);
        tank.Velocity *= frictionFactor;

        // Update position
        tank.Position += tank.Velocity * deltaSeconds;

        // Update turret
        tank.TurretAngularVelocity += tank.TurretAngularAcceleration * deltaSeconds;
        tank.TurretAngularVelocity = tank.TurretAngularVelocity.Clamp(TankEntity.MaxTurretAngularVelocity);
        tank.TurretAngularVelocity *= angularFrictionFactor;
        tank.TurretRotation += tank.TurretAngularVelocity * deltaSeconds;
        tank.TurretRotation = NormalizeRotation(tank.TurretRotation);

        // Update turret
        tank.SensorAngularVelocity += tank.SensorAngularAcceleration * deltaSeconds;
        tank.SensorAngularVelocity = tank.SensorAngularVelocity.Clamp(TankEntity.MaxSensorAngularVelocity);
        tank.SensorAngularVelocity *= angularFrictionFactor;
        tank.SensorRotation += tank.SensorAngularVelocity * deltaSeconds;
        tank.SensorRotation = NormalizeRotation(tank.SensorRotation);
    }

    private static float NormalizeRotation(float rotation)
    {
        rotation %= MathHelper.TwoPi;
        return rotation < 0 ? rotation + MathHelper.TwoPi : rotation;
    }
}
