using AiAssaultArena.Simulation.Entities;
using AiAssaultArena.Simulation.Math;

namespace AiAssaultArena.Simulation.Updaters;

public class TankUpdater
{
    public static void Update(float deltaSeconds, TankEntity tank)
    {
        // Update the position based on the velocity
        tank.Velocity += new Vector2(0, tank.Acceleration).Rotate(tank.BodyRotation) * deltaSeconds;

        tank.Velocity = tank.Velocity.Truncate(TankEntity.MaxSpeed);

        // Apply angular friction to slow down angular velocity (angular damping)
        var angularFrictionFactor = MathF.Pow(TankEntity.AngularFriction, deltaSeconds);
        tank.AngularVelocity *= angularFrictionFactor;
        tank.AngularVelocity = tank.AngularVelocity.ClampValue(-1, 1);
        tank.BodyRotation += tank.AngularVelocity * deltaSeconds;

        // Apply friction to slow down the tank
        var frictionFactor = MathF.Pow(TankEntity.Friction, deltaSeconds);

        // Calculate the direction of the tank's forward movement
        var forwardDirection = new Vector2(1, 0).Rotate(tank.BodyRotation);

        // Calculate the dot product of the tank's velocity and forward direction
        var dotProduct = Vector2.Dot(tank.Velocity, forwardDirection);

        // Apply lateral damping (making it harder to move perpendicular to body rotation)
        if (dotProduct < 0)
        {
            tank.Velocity -= forwardDirection * dotProduct * (1 - TankEntity.LateralDamping);
        }

        // Apply friction to slow down the tank
        tank.Velocity *= frictionFactor;
        tank.Position += tank.Velocity * deltaSeconds;

        // Normalize rotations to stay within 0 to 2π radians
        tank.BodyRotation = NormalizeRotation(tank.BodyRotation);
        tank.TurretRotation = NormalizeRotation(tank.TurretRotation);
        tank.SensorRotation = NormalizeRotation(tank.SensorRotation);
    }

    private static float NormalizeRotation(float rotation)
    {
        rotation %= MathHelper.TwoPi;
        return rotation < 0 ? rotation + MathHelper.TwoPi : rotation;
    }
}
