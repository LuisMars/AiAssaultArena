using AiAssaultArena.Contract;
using AiAssaultArena.Tanks.Common;
using System.Numerics;

namespace AiAssaultArena.Tanks.RandomTank;

public class RandomTank : BaseTank
{
    private float Width { get; } = 1200;
    private float Height { get; } = 720;
    private float TankRadius { get; } = 25;
    private Vector2 CurrentTargetPosition { get; set; }
    private bool IsMovingToTarget { get; set; } = false;

    public RandomTank() : base("Random Tank")
    {
        // Generate the first random position
        CurrentTargetPosition = GenerateRandomPosition();
    }

    protected override Task OnUpdate(TankResponse gameStateResponse, SensorResponse? sensorResponse)
    {
        var position = new Vector2(gameStateResponse.Position.X, gameStateResponse.Position.Y);

        // Check if the tank has reached the current target position
        if (Vector2.Distance(position, CurrentTargetPosition) < TankRadius || !IsMovingToTarget)
        {
            CurrentTargetPosition = GenerateRandomPosition();
            IsMovingToTarget = true;
        }

        // Movement logic towards the target position
        var directionToTarget = Vector2.Normalize(CurrentTargetPosition - position);
        var angleToTarget = MathF.Atan2(directionToTarget.Y, directionToTarget.X);

        return SendAsync(new TankMoveParameters
        {
            TurnDirection = CalculateTurnDirection(angleToTarget, gameStateResponse.BodyRotation),
            Acceleration = 1, // Adjust as needed
            TurretTurnDirection = CalculateTurnDirection(angleToTarget, gameStateResponse.TurretRotation),
            SensorTurnDirection = CalculateTurnDirection(angleToTarget, gameStateResponse.SensorRotation),
            Shoot = Random.Shared.NextSingle() > 0.9f
        });
    }

    private Vector2 GenerateRandomPosition()
    {
        var randomX = Random.Shared.NextSingle() * (Width - 2 * TankRadius) - (Width / 2 - TankRadius);
        var randomY = Random.Shared.NextSingle() * (Height - 2 * TankRadius) - (Height / 2 - TankRadius);
        return new Vector2(randomX, randomY);
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
