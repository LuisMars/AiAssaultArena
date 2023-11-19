using AiAssaultArena.Simulation.Math;

namespace AiAssaultArena.Simulation.Entities;

public record SensorOutput(TankEntity Tank, Vector2 Position, float DistanceSquared);
