using AiAssaultArena.Contract;
using AiAssaultArena.Tanks.Common;
using System.Numerics;

namespace AiAssaultArena.Tanks.Corners;

public class Corners : BaseTank
{
    private readonly TankMoveParameters _moveParameters = new() { Acceleration = 0f, TurnDirection = 0, TurretTurnDirection = 0, SensorTurnDirection = 0 };

    private float Width { get; set; } = 1200;
    private float Height { get; set; } = 720;
    private float TankRadius { get; set; } = 25;
    private Vector2[] TankCorners { get; set; }
    private int CurrentCornerIndex { get; set; } = 0;

    public Corners() : base("Corners Tank")
    {
        // Initialize the corners
        TankCorners =
        [
            new Vector2(Width / 2 - 2 * TankRadius, Height / 2 - 2 * TankRadius), // Top right corner
            new Vector2(-(Width / 2 - 2 * TankRadius), Height / 2 - 2 * TankRadius), // Top left corner
            new Vector2(-(Width / 2 - 2 * TankRadius), -(Height / 2 - 2 * TankRadius)), // Bottom left corner
            new Vector2(Width / 2 - 2 * TankRadius, -(Height / 2 - 2 * TankRadius)) // Bottom right corner
        ];
        CurrentCornerIndex = Random.Shared.Next(0, 4);
    }

    protected override Task OnUpdate(TankResponse gameStateResponse, SensorResponse? sensorResponse)
    {
        var position = new Vector2(gameStateResponse.Position.X, gameStateResponse.Position.Y);
        var targetCorner = TankCorners[CurrentCornerIndex];

        // Check if the tank has reached the current corner
        if (Vector2.Distance(position, targetCorner) < TankRadius * 5)
        {
            // Move to the next corner
            CurrentCornerIndex = (CurrentCornerIndex + 1) % TankCorners.Length;
            targetCorner = TankCorners[CurrentCornerIndex];
        }

        // Calculate the direction to the target corner
        var directionToTarget = targetCorner - position;
        var distance = directionToTarget.Length();
        directionToTarget = Vector2.Normalize(directionToTarget);
        var angleToTarget = MathF.Atan2(directionToTarget.Y, directionToTarget.X);

        // Set movement parameters
        _moveParameters.Acceleration = MathF.Min(1, distance / (TankRadius * 2 * 5));
        _moveParameters.TurnDirection = GetHeading(angleToTarget - gameStateResponse.BodyRotation);
        _moveParameters.TurretTurnDirection = GetHeading(angleToTarget - gameStateResponse.TurretRotation + MathF.PI * 0.5f);
        _moveParameters.SensorTurnDirection = GetHeading(angleToTarget - gameStateResponse.SensorRotation + MathF.PI * 0.5f);
        _moveParameters.Shoot = sensorResponse is not null && gameStateResponse.CurrentTurretHeat < 70;
        return SendAsync(gameStateResponse, _moveParameters);
    }

    private float CalculateTurnDirection(float targetAngle, float currentAngle)
    {
        var angleDifference = GetHeading(targetAngle - currentAngle);
        return angleDifference > 0 ? 1.0f : -1.0f;
    }

    private static float GetHeading(float rotation)
    {
        rotation %= MathF.Tau;
        rotation = rotation < 0 ? rotation + MathF.Tau : rotation;
        rotation -= 0.5f * MathF.PI;
        return rotation;
    }
}
