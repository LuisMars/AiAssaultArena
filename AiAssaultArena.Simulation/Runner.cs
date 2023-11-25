using AiAssaultArena.Contract;
using AiAssaultArena.Simulation.Collisions;
using AiAssaultArena.Simulation.Entities;
using AiAssaultArena.Simulation.Math;
using AiAssaultArena.Simulation.Updaters;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AiAssaultArena.Simulation;
public class Runner
{
    public float Width { get; set; }
    public float Height { get; set; }
    public List<TankEntity> Tanks { get; set; } = [];
    public List<ArenaWallEntity> Walls { get; set; }
    public ConcurrentDictionary<Guid, BulletEntity> Bullets { get; set; } = [];
    public Dictionary<Guid, SensorOutput?> Sensors { get; set; } = [];

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
        const int iterations = 5;
        deltaSeconds /= iterations;

        Tanks.ForEach(t => Sensors[t.Id] = null);

        for (var i = 0; i < iterations; i++)
        {
            Walls.ForEach(wall => Tanks.ForEach((tank) => wall.Collided(tank, deltaSeconds)));

            foreach (var (tankA, tankB) in MatchTanks())
            {
                tankA.Collided(tankB, deltaSeconds);
            }

            foreach (var (bulletId, bullet) in Bullets)
            {
                var tank = Tanks.FirstOrDefault(tank => bullet.ShooterId != tank.Id && bullet.Collided(tank));
                if (tank is not null)
                {
                    tank.Health -= 10;
                    Bullets.Remove(bulletId, out _);
                    continue;
                }
                if (Walls.Any(bullet.Collided) || bullet.Position.LengthSquared() > 10000000)
                {
                    Bullets.Remove(bulletId, out _);
                }
            }

            Tanks.ForEach(tank => TankUpdater.Update(deltaSeconds, tank));
            Bullets.Values.ToList().ForEach(bullet => BulletUpdater.Update(deltaSeconds, bullet));

            Tanks.ForEach(tank => TankUpdater.Update(deltaSeconds, tank));

            foreach (var (tankA, tankB) in MatchTanks())
            {
                if (tankA.Senses(tankB))
                {
                    Sensors[tankA.Id] = new(tankB, tankB.Position, 0);
                }
                if (tankB.Senses(tankA))
                {
                    Sensors[tankB.Id] = new(tankA, tankA.Position, 0);
                }
            }
        } 
    }


    public void UpdateTank(Guid tankId, TankMoveParameters parameters)
    {
        var tank = GetTank(tankId);
        if (tank is null)
        {
            return;
        }
        tank.Move(parameters);
        if (parameters.Shoot)
        {
            var bullet = tank.Shoot();
            if (bullet is not null)
            {
                Bullets[bullet.Id] = bullet;
            }
        }
    }

    public TankEntity? GetTank(Guid tankId)
    {
        return Tanks.FirstOrDefault(t => t.Id == tankId);
    }
}
