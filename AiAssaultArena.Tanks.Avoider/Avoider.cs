using AiAssaultArena.Contract;
using AiAssaultArena.Tanks.Common;
using System.Diagnostics;
using System.Numerics;

namespace AiAssaultArena.Tanks.Avoider;

public class Avoider() : BaseTank("Avoider Tank")
{
    private readonly TankMoveParameters _moveParameters = new() { Acceleration = 1f, TurnDirection = 1f, TurretTurnDirection = 1f, SensorTurnDirection = 1f };
    private Vector2 Position { get; set; }
    private Vector2 PreviousLastKnownPosition { get; set; }
    private Vector2 LastKnownPosition { get; set; }
    private Vector2 PerceivedVelocity { get; set; }
    private Vector2 PerceivedAcceleration { get; set; }
    private bool WasTracking { get; set; }
    private float LastTrackedDirection { get; set; } = 1;
    private float Width { get; set; } = 1200;
    private float Height { get; set; } = 720;
    private float TankRadius { get; set; } = 25;

    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private TimeSpan? LastSeen { get; set; }

    private static float RandomNormal()
    {
        return Random.Shared.NextSingle() - 0.5f;
    }

    protected override Task OnUpdate(TankResponse tankResponse, SensorResponse? sensorResponse)
    {

        var isTracking = sensorResponse is not null;
        UpdateTankInfo(tankResponse, sensorResponse);
        var futurePosition = CalculateFuturePosition();

        if (isTracking)
        {
            var timeToImpact = TimeToImpact(tankResponse);
            var predictedPosition = CalculateFuturePosition(timeToImpact);

            // Calculate angles taking into account the body rotation
            var possibleFuturePositionAngle = CalculateAngle(Position, predictedPosition);
            var turretAngleToFuturePosition = GetHeading(possibleFuturePositionAngle - tankResponse.TurretRotation);

            var angleToLastKnownPosition = CalculateAngle(Position, futurePosition);
            var sensorAngleToLastKnownPosition = GetHeading(angleToLastKnownPosition - tankResponse.SensorRotation);

            _moveParameters.SensorTurnDirection = MathF.Sign(sensorAngleToLastKnownPosition) + RandomNormal() * 0.1f;
            _moveParameters.TurretTurnDirection = MathF.Sign(turretAngleToFuturePosition);
            var isReadyToShoot = SensorResponses >= 3 && MathF.Abs(turretAngleToFuturePosition) < MathF.PI / 16;
            if (isReadyToShoot)
            {
                //_moveParameters.TurretTurnDirection += RandomNormal() * 0.1f;
            }
            _moveParameters.Shoot = isReadyToShoot && timeToImpact < 10f && tankResponse.CurrentTurretHeat < 70;

            var bodyAngleToLastKnownPosition = GetHeading(possibleFuturePositionAngle - tankResponse.BodyRotation + (MathF.PI * 0.5f));
            _moveParameters.TurnDirection = MathF.Sign(bodyAngleToLastKnownPosition);

            var distance = (Position - predictedPosition).Length();
            _moveParameters.TurnDirection += distance > 300 ? 0.5f : -0.5f;
            _moveParameters.Acceleration += distance < 300 ? 0.5f : -0.5f;
            if (EnemyShooting)
            {
                Console.WriteLine("Avoiding");

                _moveParameters.Acceleration *= -1;
            }
        }
        else if (WasTracking)
        {
            // Determine the direction to turn the sensor based on overshooting
            var angleToLastKnownPosition = CalculateAngle(Position, futurePosition);

            var currentSensorAngle = GetHeading(tankResponse.SensorRotation);
            var sensorAngleDifference = GetHeading(angleToLastKnownPosition - currentSensorAngle);
            var currentTurretAngle = GetHeading(tankResponse.SensorRotation);
            var turretAngleDifference = GetHeading(angleToLastKnownPosition - currentTurretAngle);

            // If the angleDifference is positive, the sensor should turn left, and vice versa
            _moveParameters.SensorTurnDirection = (sensorAngleDifference > 0 ? -1f : 1f) + RandomNormal() * 0.1f;

            // Reset turret direction and shooting as we are not currently tracking            
            _moveParameters.Shoot = false;
            _moveParameters.TurretTurnDirection = (turretAngleDifference > 0 ? -1f : 1f) + RandomNormal() * 0.1f;

            LastTrackedDirection = _moveParameters.SensorTurnDirection > 0 ? 1 : -1;

            _moveParameters.Acceleration *= -1;
        }
        else
        {
            _moveParameters.SensorTurnDirection = -1 * LastTrackedDirection;
            _moveParameters.TurretTurnDirection = -1 * LastTrackedDirection;
        }

        WasTracking = isTracking;
        return SendAsync(tankResponse, _moveParameters);
    }
    private int SensorResponses { get; set; }
    private float LastTurretHeat { get; set; } = 100;
    private bool EnemyShooting { get; set; }
    private void UpdateTankInfo(TankResponse gameStateResponse, SensorResponse? sensorResponse)
    {
        Position = new Vector2(gameStateResponse.Position.X, gameStateResponse.Position.Y);

        if (sensorResponse is not null)
        {
            SensorResponses++;

            var timeDelta = 1f / 60;
            if (LastSeen is not null)
            {
                timeDelta = (float)(_stopwatch.Elapsed - LastSeen.Value).TotalSeconds;
            }
            LastSeen = _stopwatch.Elapsed;

            var pastPosition = PreviousLastKnownPosition;
            PreviousLastKnownPosition = LastKnownPosition;
            LastKnownPosition = new Vector2(sensorResponse.Position.X, sensorResponse.Position.Y);
            PerceivedVelocity = (LastKnownPosition - PreviousLastKnownPosition) / timeDelta;
            var previousVelocity = (PreviousLastKnownPosition - pastPosition) / timeDelta;
            var changeInVelocity = PerceivedVelocity - previousVelocity;
            PerceivedAcceleration = changeInVelocity / timeDelta;
            EnemyShooting = LastTurretHeat < sensorResponse.TurretHeat;
            LastTurretHeat = sensorResponse.TurretHeat;
        }
        else
        {
            SensorResponses = 0;
        }
    }

    private Vector2 CalculateFuturePosition(float inSeconds = 1 / 60f)
    {
        if (float.IsNaN(inSeconds) || float.IsInfinity(inSeconds))
        {
            Console.WriteLine("invalid time");
            return LastKnownPosition + PerceivedVelocity;
        }

        if (inSeconds < 0)
        {
            return LastKnownPosition + PerceivedVelocity;
        }

        // To ensure the future position doesn't go outside the boundaries
        var maxX = Width / 2 - TankRadius;
        var maxY = Height / 2 - TankRadius;

        // Calculate future position using the kinematic equation
        var futurePosition = LastKnownPosition
                                 + PerceivedVelocity * inSeconds
                                 + 0.5f * PerceivedAcceleration * inSeconds * inSeconds;

        // Clamp values if necessary (based on your game's logic)
        futurePosition.X = Math.Clamp(futurePosition.X, -maxX, maxX);
        futurePosition.Y = Math.Clamp(futurePosition.Y, -maxY, maxY);

        return futurePosition;
    }

    public float TimeToImpact(TankResponse gameStateResponse)
    {
        if (SensorResponses < 3)
        {
            return -1;
        }
        var turretLength = 50;
        var bulletSpeed = 450;

        var enemyAngle = CalculateAngle(Position, LastKnownPosition);
        var bulletStartPosition = Position + new Vector2(turretLength * MathF.Cos(enemyAngle), turretLength * MathF.Sin(enemyAngle));
        var totalBulletVelocity = new Vector2(gameStateResponse.Velocity.X, gameStateResponse.Velocity.Y) + (bulletSpeed * new Vector2(MathF.Cos(enemyAngle), MathF.Sin(enemyAngle)));

        return CalculateIntersectionTime(bulletStartPosition, totalBulletVelocity, LastKnownPosition, PerceivedVelocity, PerceivedAcceleration);
    }

    private float CalculateIntersectionTime(Vector2 bulletPosition, Vector2 bulletVelocity,
                                        Vector2 targetPosition, Vector2 targetVelocity, Vector2 targetAcceleration)
    {
        var relativeVelocity = bulletVelocity - targetVelocity;
        var relativePosition = bulletPosition - targetPosition;

        var A = 0.5f * targetAcceleration.LengthSquared();
        var B = Vector2.Dot(relativeVelocity, targetAcceleration) + relativeVelocity.LengthSquared();
        var C = Vector2.Dot(relativePosition, relativeVelocity) - (25 * 25); // 25 is the target radius

        // Handle the case when target's acceleration is zero
        if (A == 0)
        {
            if (B == 0)
            {
                Console.WriteLine("Bullet and target will never meet");
                return -4.0f;
            }
            var t = -C / B;
            return t >= 0 ? t : -4.0f; // Return time if it's positive
        }

        // Solve the quadratic equation
        var discriminant = B * B - 4 * A * C;
        if (discriminant < 0)
        {
            Console.WriteLine("No real solutions, no intersection");
            return -2.0f;
        }

        var sqrtDiscriminant = MathF.Sqrt(discriminant);
        var t1 = (-B + sqrtDiscriminant) / (2 * A);
        var t2 = (-B - sqrtDiscriminant) / (2 * A);

        // We are interested in the smallest positive solution
        if (t1 < 0 && t2 < 0)
        {
            Console.WriteLine("2 negative solutions");
            return -3.0f;
        }
        if (t1 < 0)
        {
            t1 = float.MaxValue;
        }
        if (t2 < 0)
        {
            t2 = float.MaxValue;
        }

        return Math.Min(t1, t2);
    }


    private static float CalculateAngle(Vector2 from, Vector2 to)
    {
        var direction = to - from;
        return MathF.Atan2(direction.Y, direction.X);
    }

    private static float GetHeading(float rotation)
    {
        rotation %= MathF.Tau;
        rotation = rotation < 0 ? rotation + MathF.Tau : rotation;
        rotation -= 0.5f * MathF.PI;
        return rotation;
    }
}
