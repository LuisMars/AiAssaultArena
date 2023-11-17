using AiAssaultArena.Simulation.Entities;

namespace AiAssaultArena.Simulation.Updaters;

public class BulletUpdater
{
    public static void Update(float deltaSeconds, BulletEntity bullet)
    {
        bullet.Position += bullet.Velocity * deltaSeconds;
    }
}