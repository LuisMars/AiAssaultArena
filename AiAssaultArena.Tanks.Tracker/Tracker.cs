using AiAssaultArena.Contract;
using AiAssaultArena.Tanks.Common;
using System.Diagnostics;
using System.Numerics;

namespace AiAssaultArena.Tanks.Tracker;

public class Tracker() : BaseTank("Tracker Tank")
{
    private readonly TankMoveParameters _moveParameters = new() { Acceleration = 0.1f, TurnDirection = 0.1f, TurretTurnDirection = 0, SensorTurnDirection = 1f };
    private Vector2 LastKnownPosition { get; set; }
    private Vector2 PerceivedSpeed { get; set; }
    private Vector2 PerceivedAcceleration { get; set; }
    private float Distance { get; set; }
    private bool WasTracking { get; set; }
    private float LastTrackedDirection { get; set; }
    private float _width { get; set; } = 1200;
    private float _heigth { get; set; } = 720;
    private float _tankRadius { get; set; } = 25;
    protected override Task OnUpdate(TankResponse gameStateResponse, SensorResponse? sensorResponse)
    {
        var isTracking = sensorResponse is not null;

        var position = UpdateTankInfo(gameStateResponse, sensorResponse);

        if (isTracking)
        {
            var futurePosition = CalculateFuturePosition();

            // Calculate angles taking into account the body rotation
            var possibleFuturePositionAngle = CalculateAngle(position, futurePosition);
            var turretAngleToFuturePosition = GetHeading(possibleFuturePositionAngle - (gameStateResponse.TurretRotation + gameStateResponse.BodyRotation));

            var angleToLastKnownPosition = CalculateAngle(position, LastKnownPosition);
            var sensorAngleToLastKnownPosition = GetHeading(angleToLastKnownPosition - (gameStateResponse.SensorRotation + gameStateResponse.BodyRotation));

            _moveParameters.SensorTurnDirection = sensorAngleToLastKnownPosition - _moveParameters.TurnDirection;
            _moveParameters.TurretTurnDirection = turretAngleToFuturePosition - _moveParameters.TurnDirection;

            if (!WasTracking)
            {
                _moveParameters.SensorTurnDirection *= 2;
            }

            _moveParameters.Shoot = MathF.Abs(turretAngleToFuturePosition) < MathF.PI / 4 && gameStateResponse.CurrentTurretHeat == 0;
        }
        else if (WasTracking)
        {

            // Determine the direction to turn the sensor based on overshooting
            var angleToLastKnownPosition = CalculateAngle(position, LastKnownPosition);
            var currentSensorAngle = GetHeading(gameStateResponse.SensorRotation + gameStateResponse.BodyRotation);
            var angleDifference = GetHeading(angleToLastKnownPosition - currentSensorAngle);

            // If the angleDifference is positive, the sensor should turn left, and vice versa
            _moveParameters.SensorTurnDirection = (angleDifference > 0 ? -1 : 1) - _moveParameters.TurnDirection;


            // Reset turret direction and shooting as we are not currently tracking
            _moveParameters.Shoot = false;

            LastTrackedDirection = _moveParameters.SensorTurnDirection > 0 ? 1 : -1;
        }
        else
        {
            _moveParameters.SensorTurnDirection = (-0.5f * LastTrackedDirection) - _moveParameters.TurnDirection;

        }

        WasTracking = isTracking;
        return SendAsync(_moveParameters);
    }

    private Vector2 UpdateTankInfo(TankResponse gameStateResponse, SensorResponse? sensorResponse)
    {
        var position = new Vector2(gameStateResponse.Position.X, gameStateResponse.Position.Y);

        if (sensorResponse is not null)
        {
            var lastPosition = LastKnownPosition;
            LastKnownPosition = new Vector2(sensorResponse.Position.X, sensorResponse.Position.Y);
            PerceivedSpeed = LastKnownPosition - lastPosition;
            Distance = (LastKnownPosition - position).Length();

            PerceivedAcceleration = (PerceivedSpeed - (lastPosition - LastKnownPosition)) / Distance;
        }
        return position;
    }
    private Vector2 CalculateFuturePosition()
    {
        var timeToImpact = Distance / 450; // bullet speed is 450 units/sec
        var futurePosition = LastKnownPosition + PerceivedSpeed * timeToImpact + 0.5f * PerceivedAcceleration * timeToImpact * timeToImpact;

        // Ensure the future position doesn't go outside the boundaries considering the tank's radius
        var maxX = _width / 2 - _tankRadius;
        var maxY = _heigth / 2 - _tankRadius;

        futurePosition.X = Math.Clamp(futurePosition.X, -maxX, maxX);
        futurePosition.Y = Math.Clamp(futurePosition.Y, -maxY, maxY);

        return futurePosition;
    }

    private float CalculateAngle(Vector2 from, Vector2 to)
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
