using AiAssaultArena.Simulation.Math;

namespace AiAssaultArena.Simulation.Entities;

public class ArenaWallEntity
{
    public float Width { get; private set; }
    public float Height { get; private set; }
    public Vector2 Start { get; }
    public Vector2 Center { get; private set; }
    public IList<Vector2> Corners { get; set; }

    public ArenaWallEntity(Vector2 start, float width, float heigth)
    {
        Start = start;
        Width = width;
        Height = heigth;
        Corners = new[]
        {
            Start,
            Start + new Vector2(Width, 0),
            Start + new Vector2(Width, Height),
            Start + new Vector2(0, Height)
        };
        Center = Start + new Vector2(Width / 2, Height / 2);
    }
}
