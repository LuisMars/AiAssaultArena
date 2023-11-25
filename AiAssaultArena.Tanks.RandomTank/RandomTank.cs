using AiAssaultArena.Contract;
using AiAssaultArena.Tanks.Common;

namespace AiAssaultArena.Tanks.RandomTank;

public class RandomTank() : BaseTank("Random Tank")
{
    protected override Task OnUpdate(TankResponse gameStateResponse, SensorResponse? sensorResponse)
    {
        return SendAsync(new TankMoveParameters
        {
            TurretTurnDirection = 2 * (Random.Shared.NextSingle() - 0.5f),
            SensorTurnDirection = 2 * (Random.Shared.NextSingle() - 0.5f),
            TurnDirection = 2 * (Random.Shared.NextSingle() - 0.5f),
            Acceleration = 1,
            Shoot = Random.Shared.NextSingle() > 0.5f
        });
    }
}
