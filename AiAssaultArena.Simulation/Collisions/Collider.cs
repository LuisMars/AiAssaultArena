using AiAssaultArena.Simulation.Entities;
using AiAssaultArena.Simulation.Math;

namespace AiAssaultArena.Simulation.Collisions;

public static class Collider
{
    public static bool Collided(this ArenaWallEntity wall, TankEntity tank, float deltaSeconds)
    {
        // Get the corners of both rectangles
        var cornersTank = GetCorners(tank);

        // Check all possible separating axes
        var axes = GetAxes(cornersTank).Concat(GetAxes(wall.Corners)).ToList();

        var mtv = Vector2.Zero;
        var minOverlap = float.PositiveInfinity;

        foreach (var axis in axes)
        {
            // Project the corners onto the axis
            ProjectCorners(cornersTank, axis, out float minFirst, out float maxFirst);
            ProjectCorners(wall.Corners, axis, out float minOther, out float maxOther);

            // Calculate overlap
            var overlap = MathF.Min(maxFirst, maxOther) - MathF.Max(minFirst, minOther);

            // Check for separation
            if (overlap <= 0)
            {
                // No collision on this axis
                return false;
            }

            // Determine the direction of overlap
            var direction = axis;
            if (minFirst < minOther)
            {
                // Tank is on the left/up side of the wall
                direction = -axis; // Reverse the direction to repel the tank
            }

            if (overlap < minOverlap)
            {
                minOverlap = overlap;
                mtv = direction; // Use the corrected direction for MTV
            }
        }

        // Normalize the MTV
        mtv.Normalize();

        // Move the tanks apart along the MTV
        var correction = mtv * minOverlap;
        tank.Position += correction;

        var collisionPoint = new Vector2(
            MathHelper.Clamp(tank.Position.X, wall.Start.X, wall.Start.X + wall.Width),
            MathHelper.Clamp(tank.Position.Y, wall.Start.Y, wall.Start.Y + wall.Height)
        );

        // Adjust velocities based on collision normal
        AdjustVelocityAndRotation(tank, mtv, deltaSeconds, collisionPoint);
        return true;
    }

    public static bool Collided(this TankEntity first, TankEntity other, float deltaSeconds)
    {
        // Get the corners of both rectangles
        var cornersFirst = GetCorners(first);
        var cornersOther = GetCorners(other);

        // Check all possible separating axes
        var axes = GetAxes(cornersFirst).Concat(GetAxes(cornersOther)).ToList();

        var mtv = Vector2.Zero;
        var minOverlap = float.PositiveInfinity;

        foreach (var axis in axes)
        {
            // Project the corners onto the axis
            ProjectCorners(cornersFirst, axis, out float minFirst, out float maxFirst);
            ProjectCorners(cornersOther, axis, out float minOther, out float maxOther);

            // Calculate overlap
            var overlap = MathF.Min(maxFirst, maxOther) - MathF.Max(minFirst, minOther);

            // Check for separation
            if (overlap <= 0)
            {
                // No collision on this axis
                return false;
            }

            // Determine the direction of overlap
            var direction = axis;
            if (minFirst < minOther)
            {
                // Tank is on the left/up side of the wall
                direction = -axis; // Reverse the direction to repel the tank
            }

            if (overlap < minOverlap)
            {
                minOverlap = overlap;
                mtv = direction; // Use the corrected direction for MTV
            }
        }

        // Normalize the MTV
        mtv.Normalize();

        // Move the tanks apart along the MTV
        var correction = mtv * minOverlap;
        first.Position += correction / 2;
        other.Position -= correction / 2;

        // Adjust velocities based on collision normal
        var collisionPoint = (first.Position + other.Position) / 2;
        AdjustVelocityAndRotation(first, mtv, deltaSeconds, collisionPoint);
        AdjustVelocityAndRotation(other, -mtv, deltaSeconds, collisionPoint);

        return true;
    }

    public static bool Collided(this BulletEntity bullet, TankEntity tank)
    {
        // Convert circle center to tank's local space
        var localCircleCenter = (bullet.Position - tank.Position).Rotate(-tank.BodyRotation);


        // Find the closest point on the AABB to the circle
        var closestPoint = new Vector2(
            MathHelper.Clamp(localCircleCenter.X, -TankEntity.HalfWidth, TankEntity.HalfWidth),
            MathHelper.Clamp(localCircleCenter.Y, -TankEntity.HalfHeight, TankEntity.HalfHeight)
        );

        // Determine the distance from the circle's center to this point
        Vector2 distance = localCircleCenter - closestPoint;

        // Check if the distance is less than the circle's radius
        return distance.LengthSquared() <= BulletEntity.Radius * BulletEntity.Radius;
    }

    public static bool Collided(this BulletEntity bullet, ArenaWallEntity wall)
    {

        // Find the closest point on the AABB to the circle
        var closestPoint = new Vector2(
            MathHelper.Clamp(bullet.Position.X, wall.Start.X, wall.Start.X + wall.Width),
            MathHelper.Clamp(bullet.Position.Y, wall.Start.Y, wall.Start.Y + wall.Height)
        );

        // Determine the distance from the circle's center to this point
        Vector2 distance = bullet.Position - closestPoint;

        // Check if the distance is less than the circle's radius
        return distance.LengthSquared() <= BulletEntity.Radius * BulletEntity.Radius;
    }

    private static void AdjustVelocityAndRotation(TankEntity tank, Vector2 collisionNormal, float deltaSeconds, Vector2 collisionPoint)
    {
        var radius = CalculateCollisionRadius(tank, collisionPoint);
        var relativeLinearVelocity = tank.Velocity;
        var relativeAngularVelocity = tank.AngularVelocity;

        // Manually calculating the perpendicular vector
        var perpendicular = new Vector2(-relativeAngularVelocity * radius, relativeAngularVelocity * radius);
        var velocityAtCollisionPoint = relativeLinearVelocity + perpendicular;

        var impulse = CalculateCollisionImpulse(velocityAtCollisionPoint, collisionNormal);
        ApplyLinearImpulse(tank, impulse, deltaSeconds);
        ApplyAngularImpulse(tank, impulse, collisionPoint, deltaSeconds);
    }

    private static float CalculateCollisionRadius(TankEntity tank, Vector2 collisionPoint)
    {
        // Calculate the offset from the tank's center to the collision point
        var offset = collisionPoint - tank.Position;

        // Calculate the tank's half-width and half-height
        var halfWidth = TankEntity.Width / 2;
        var halfHeight = TankEntity.Height / 2;

        // Find the nearest point on the tank's bounding box to the collision point
        var nearestX = MathF.Max(-halfWidth, MathF.Min(halfWidth, offset.X));
        var nearestY = MathF.Max(-halfHeight, MathF.Min(halfHeight, offset.Y));

        // Calculate the distance from the nearest point on the bounding box to the collision point
        var nearestPointOnBoundingBox = new Vector2(nearestX, nearestY) + tank.Position;
        return (collisionPoint - nearestPointOnBoundingBox).Length();
    }

    private static Vector2 CalculateCollisionImpulse(Vector2 velocityAtCollisionPoint, Vector2 collisionNormal)
    {
        // Example implementation - adjust as needed
        var restitution = 0.5f; // Restitution coefficient
        var mass = TankEntity.Mass; // Assume tank has a Mass property
        var j = -(1 + restitution) * Vector2.Dot(velocityAtCollisionPoint, collisionNormal);
        j /= 1 / mass;
        return j * collisionNormal;
    }

    private static void ApplyLinearImpulse(TankEntity tank, Vector2 impulse, float deltaSeconds)
    {
        tank.Velocity += impulse * deltaSeconds / TankEntity.Mass; // Adjusting linear velocity based on impulse
    }

    private static void ApplyAngularImpulse(TankEntity tank, Vector2 impulse, Vector2 collisionPoint, float deltaSeconds)
    {
        Vector2 r = collisionPoint - tank.Position;
        float angularImpulseMagnitude = r.Cross(impulse);
        tank.AngularVelocity += angularImpulseMagnitude * deltaSeconds / TankEntity.MomentOfInertia; // Adjusting angular velocity
    }
        
    private static IList<Vector2> GetCorners(TankEntity tank)
    {
        var center = tank.Position;
        var width = TankEntity.Width;
        var height = TankEntity.Height;
        var rotation = tank.BodyRotation;

        var halfWidth = width / 2;
        var halfHeight = height / 2;

        var corners = new Vector2[4];

        // Define corners relative to (0,0)
        corners[0] = new Vector2(-halfWidth, -halfHeight);
        corners[1] = new Vector2(halfWidth, -halfHeight);
        corners[2] = new Vector2(halfWidth, halfHeight);
        corners[3] = new Vector2(-halfWidth, halfHeight);

        // Rotate and translate each corner
        var cos = MathF.Cos(rotation);
        var sin = MathF.Sin(rotation);

        for (int i = 0; i < 4; i++)
        {
            var rotatedX = corners[i].X * cos - corners[i].Y * sin;
            var rotatedY = corners[i].X * sin + corners[i].Y * cos;

            corners[i] = new Vector2(rotatedX, rotatedY) + center;
        }

        return corners;
    }

    private static IList<Vector2> GetAxes(IList<Vector2> corners)
    {
        var axes = new List<Vector2>();

        for (int i = 0; i < corners.Count; i++)
        {
            // Get the edge vector
            var edge = corners[(i + 1) % corners.Count] - corners[i];
            // Get the normal of the edge (perpendicular vector)
            var normal = new Vector2(-edge.Y, edge.X);
            normal.Normalize();

            axes.Add(normal);
        }

        return axes;
    }

    private static void ProjectCorners(IList<Vector2> corners, Vector2 axis, out float min, out float max)
    {
        min = float.PositiveInfinity;
        max = float.NegativeInfinity;

        foreach (var corner in corners)
        {
            var projection = Vector2.Dot(corner, axis);

            if (projection < min)
            {
                min = projection;
            }

            if (projection > max)
            {
                max = projection;
            }
        }
    }
}
