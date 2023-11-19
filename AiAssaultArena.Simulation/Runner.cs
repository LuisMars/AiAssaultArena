using AiAssaultArena.Contract;
using AiAssaultArena.Simulation.Collisions;
using AiAssaultArena.Simulation.Entities;
using AiAssaultArena.Simulation.Math;
using AiAssaultArena.Simulation.Updaters;
using System.Collections.Concurrent;

namespace AiAssaultArena.Simulation;
public class Runner
{
    public float Width { get; set; }
    public float Height { get; set; }
    public List<TankEntity> Tanks { get; set; } = [];
    public List<ArenaWallEntity> Walls { get; set; }
    public ConcurrentDictionary<Guid, BulletEntity> Bullets { get; set; } = [];
    public bool IsWaiting => Tanks.Count < 2;

    public Runner(float width, float height)
    {
        Width = width;
        Height = height;
        Walls =
        [
            new (new Vector2(-Width / 2, -50 - Height / 2), Width, 50),
            new (new Vector2(-Width / 2, Height / 2), Width, 50),

            new (new Vector2(-50 - Width / 2, -Height / 2), 50, Height),
            new (new Vector2(Width / 2, -Height / 2), 50, Height),
        ];
    }

    public void AddTank(TankEntity tank)
    {
        Tanks.Add(tank);
    }

    private IEnumerable<(TankEntity, TankEntity)> MatchTanks()
    {
        for (int i = 0; i < Tanks.Count - 1; i++)
        {
            for (int j = i + 1; j < Tanks.Count; j++)
            {
                yield return (Tanks[i], Tanks[j]);
            }
        }
    }

    public void Update(float deltaSeconds)
    {
        //const int iterations = 5;
        //var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds / iterations;

        Walls.ForEach(wall => Tanks.ForEach((tank) => wall.Collided(tank, deltaSeconds)));

        foreach (var (tankA, tankB) in MatchTanks())
        {
            tankA.Collided(tankB, deltaSeconds);
        }

        foreach (var (bulletId, bullet) in Bullets)
        {
            if (Tanks.Any(tank => bullet.ShooterId != tank.Id && bullet.Collided(tank)) || Walls.Any(wall => bullet.Collided(wall)))
            {
                Bullets.Remove(bulletId, out _);
            }
        }

        Tanks.ForEach(tank => TankUpdater.Update(deltaSeconds, tank));
        Bullets.Values.ToList().ForEach(bullet => BulletUpdater.Update(deltaSeconds, bullet));
    }

    public void UpdateTank(TankEntity tankEntity, TankMoveParameters parameters)
    {
        var tank = Tanks.First(t => t.Id == tankEntity.Id);
        tankEntity.Move(parameters);
        if (parameters.Shoot)
        {
            var bullet = tankEntity.Shoot();
            if (bullet != null)
            {
                Bullets[bullet.Id] = bullet;
            }
        }
    }
}
