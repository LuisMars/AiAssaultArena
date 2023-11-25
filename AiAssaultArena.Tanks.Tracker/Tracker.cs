using AiAssaultArena.Contract;
using AiAssaultArena.Tanks.Common;
using System.Diagnostics;
using System.Numerics;

namespace AiAssaultArena.Tanks.Tracker;

public class Tracker() : BaseTank("Tracker Tank")
{
    private readonly TankMoveParameters _moveParameters = new() { Acceleration = 1f, TurnDirection = 1f, TurretTurnDirection = 1f, SensorTurnDirection = 1f };
    private Vector2 LastKnownPosition { get; set; }
    private Vector2 PerceivedSpeed { get; set; }
    private float Distance { get; set; }
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
        var position = UpdateTankInfo(tankResponse, sensorResponse);
        var futurePosition = CalculateFuturePosition(tankResponse);

        if (isTracking)
        {
            // Calculate angles taking into account the body rotation
            var possibleFuturePositionAngle = CalculateAngle(position, futurePosition);
            var turretAngleToFuturePosition = GetHeading(possibleFuturePositionAngle - tankResponse.TurretRotation);

            var angleToLastKnownPosition = CalculateAngle(position, LastKnownPosition);
            var sensorAngleToLastKnownPosition = GetHeading(angleToLastKnownPosition - tankResponse.SensorRotation);

            _moveParameters.SensorTurnDirection = MathF.Sign(sensorAngleToLastKnownPosition) + RandomNormal() * 0.1f;
            _moveParameters.TurretTurnDirection = MathF.Sign(turretAngleToFuturePosition);
            var isReadyToShoot = MathF.Abs(turretAngleToFuturePosition) < MathF.PI / 8;
            if (isReadyToShoot)
            {
                _moveParameters.TurretTurnDirection += RandomNormal() * 0.1f;
            }
            _moveParameters.Shoot = isReadyToShoot && tankResponse.CurrentTurretHeat == 0;
        }
        else if (WasTracking)
        {
            // Determine the direction to turn the sensor based on overshooting
            var angleToLastKnownPosition = CalculateAngle(position, futurePosition);
            var currentSensorAngle = GetHeading(tankResponse.SensorRotation);
            var angleDifference = GetHeading(angleToLastKnownPosition - currentSensorAngle);

            // If the angleDifference is positive, the sensor should turn left, and vice versa
            _moveParameters.SensorTurnDirection = (angleDifference > 0 ? -1f : 1f) + RandomNormal() * 0.1f;

            // Reset turret direction and shooting as we are not currently tracking            
            _moveParameters.Shoot = false;
            _moveParameters.TurretTurnDirection = _moveParameters.SensorTurnDirection;

            LastTrackedDirection = _moveParameters.SensorTurnDirection > 0 ? 1 : -1;
        }
        else
        {
            _moveParameters.SensorTurnDirection = -1 * LastTrackedDirection;
        }

        WasTracking = isTracking;
        return SendAsync(_moveParameters);
    }

    private Vector2 UpdateTankInfo(TankResponse gameStateResponse, SensorResponse? sensorResponse)
    {
        var position = new Vector2(gameStateResponse.Position.X, gameStateResponse.Position.Y);

        if (sensorResponse is not null)
        {
            var timeDelta = 1f / 60;
            if (LastSeen is not null)
            {
                timeDelta = (float)(_stopwatch.Elapsed - LastSeen.Value).TotalSeconds;
            }
            LastSeen = _stopwatch.Elapsed;

            var lastPosition = LastKnownPosition;
            LastKnownPosition = new Vector2(sensorResponse.Position.X, sensorResponse.Position.Y);
            PerceivedSpeed = (LastKnownPosition - lastPosition) / timeDelta;
            Distance = Vector2.Distance(LastKnownPosition, position);
        }
        
        return position;
    }

    private Vector2 CalculateFuturePosition(TankResponse gameStateResponse)
    {
        // To ensure the future position doesn't go outside the boundaries
        var maxX = Width / 2 - TankRadius;
        var maxY = Height / 2 - TankRadius;

        // Assuming bullet speed is 450 units per second
        // Initial estimation of time to impact based on current distance
        var initialTimeToImpact = (Distance - 50) / 450;

        // Predict where the target will be after the initial time to impact
        var predictedFuturePosition = LastKnownPosition + (PerceivedSpeed * initialTimeToImpact);

        predictedFuturePosition.X = Math.Clamp(predictedFuturePosition.X, -maxX, maxX);
        predictedFuturePosition.Y = Math.Clamp(predictedFuturePosition.Y, -maxY, maxY);

        // Recalculate time to impact based on the predicted future position
        var newDistance = Vector2.Distance(predictedFuturePosition, new Vector2(gameStateResponse.Position.X, gameStateResponse.Position.Y));
        var timeToImpact = (newDistance - 50) / 450;

        // Predict the final future position based on recalculated time to impact
        var futurePosition = LastKnownPosition + (PerceivedSpeed * timeToImpact);

        futurePosition.X = Math.Clamp(futurePosition.X, -maxX, maxX);
        futurePosition.Y = Math.Clamp(futurePosition.Y, -maxY, maxY);

        return futurePosition;
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
