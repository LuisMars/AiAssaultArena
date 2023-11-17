using AiAssaultArena.Simulation.Collisions;
using AiAssaultArena.Simulation.Entities;
using AiAssaultArena.Simulation.Math;
using AiAssaultArena.Simulation.Updaters;
using System.Threading.Tasks;

namespace AiAssaultArena.Simulation;
public class Runner
{
    private readonly float _width;
    private readonly float _height;
    private readonly List<TankEntity> _tanks;
    private readonly List<BulletEntity> _bullets = [];
    private readonly List<ArenaWallEntity> _walls;

    public Runner(float width, float height, List<TankEntity> tanks)
    {
        //Tanks = new List<TankEntity>
        //{
        //    new (new Vector2(-_width / 4, 0)) { Color = Color.White, BodyRotation = MathHelper.PiOver2 * 3.1f, Acceleration = -100 },
        //    new (new Vector2(_width / 4, 30)) { Color = Color.White, BodyRotation = -MathHelper.PiOver2 * -1.05f, Acceleration = 100 }
        //};
        _walls =
        [
            new (new Vector2(-_width / 2, -50 - _height / 2), _width, 50),
            new (new Vector2(-_width / 2, _height / 2), _width, 50),

            new (new Vector2(-50 - _width / 2, -_height / 2), 50, _height),
            new (new Vector2(_width / 2, -_height / 2), 50, _height),
        ];
        _width = width;
        _height = height;
        _tanks = tanks;
    }

    private IEnumerable<(TankEntity, TankEntity)> MatchTanks()
    {
        for (int i = 0; i < _tanks.Count - 1; i++)
        {
            for (int j = i + 1; j < _tanks.Count; j++)
            {
                yield return (_tanks[i], _tanks[j]);
            }
        }
    }

    public void Update(float deltaSeconds)
    {
        //const int iterations = 5;
        //var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds / iterations;

        _walls.ForEach(wall => _tanks.ForEach((tank) => wall.Collided(tank, deltaSeconds)));

        foreach (var (tankA, tankB) in MatchTanks())
        {
            tankA.Collided(tankB, deltaSeconds);
        }

        foreach (var bullet in _bullets.ToList())
        {
            if (_tanks.Any(tank => bullet.ShooterId != tank.Id && bullet.Collided(tank)) || _walls.Any(wall => bullet.Collided(wall)))
            {
                _bullets.Remove(bullet);
            }
        }

        _tanks.ForEach(tank => TankUpdater.Update(deltaSeconds, tank));
        _bullets.ForEach(bullet => BulletUpdater.Update(deltaSeconds, bullet));
    }
}
