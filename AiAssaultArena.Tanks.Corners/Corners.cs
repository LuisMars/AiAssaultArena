using AiAssaultArena.Contract;
using AiAssaultArena.Tanks.Common;
using System.Numerics;

namespace AiAssaultArena.Tanks.Corners;

public class Corners : BaseTank
{
    private readonly TankMoveParameters _moveParameters = new() { Acceleration = 0f, TurnDirection = 0, TurretTurnDirection = 0, SensorTurnDirection = 0 };

    private float _width { get; set; } = 1200;
    private float _height { get; set; } = 720;
    private float _tankRadius { get; set; } = 25;
    private Vector2[] _corners { get; set; }
    private int _currentCornerIndex { get; set; } = 0;

    public Corners() : base("Corners Tank")
    {
        // Initialize the corners
        _corners =
        [
            new Vector2(_width / 2 - 2 * _tankRadius, _height / 2 - 2 * _tankRadius), // Top right corner
            new Vector2(-(_width / 2 - 2 * _tankRadius), _height / 2 - 2 * _tankRadius), // Top left corner
            new Vector2(-(_width / 2 - 2 * _tankRadius), -(_height / 2 - 2 * _tankRadius)), // Bottom left corner
            new Vector2(_width / 2 - 2 * _tankRadius, -(_height / 2 - 2 * _tankRadius)) // Bottom right corner
        ];
        _currentCornerIndex = Random.Shared.Next(0, 4);
    }

    protected override Task OnUpdate(TankResponse gameStateResponse, SensorResponse? sensorResponse)
    {
        var position = new Vector2(gameStateResponse.Position.X, gameStateResponse.Position.Y);
        var targetCorner = _corners[_currentCornerIndex];

        // Check if the tank has reached the current corner
        if (Vector2.Distance(position, targetCorner) < _tankRadius * 5)
        {
            // Move to the next corner
            _currentCornerIndex = (_currentCornerIndex + 1) % _corners.Length;
            targetCorner = _corners[_currentCornerIndex];
        }

        // Calculate the direction to the target corner
        var directionToTarget = targetCorner - position;
        var distance = directionToTarget.Length();
        directionToTarget = Vector2.Normalize(directionToTarget);
        var angleToTarget = MathF.Atan2(directionToTarget.Y, directionToTarget.X);

        // Set movement parameters
        _moveParameters.Acceleration = distance / (_tankRadius * 2 * 5); // Adjust as needed
        _moveParameters.TurnDirection = GetHeading(angleToTarget - gameStateResponse.BodyRotation);
        _moveParameters.Shoot = true;
        return SendAsync(_moveParameters);
    }

    private float CalculateTurnDirection(float targetAngle, float currentAngle)
    {
        var angleDifference = GetHeading(targetAngle - currentAngle);
        return angleDifference > 0 ? 1.0f : -1.0f; // Adjust the turn direction based on the angle difference
    }

    private static float GetHeading(float rotation)
    {
        rotation %= MathF.Tau;
        rotation = rotation < 0 ? rotation + MathF.Tau : rotation;
        rotation -= 0.5f * MathF.PI;
        return rotation;
    }
}
