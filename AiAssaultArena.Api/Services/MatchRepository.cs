
using System.Diagnostics.CodeAnalysis;

namespace AiAssaultArena.Api.Services;


public class TankDbo
{
    public Guid Id { get; init; }
    public string ConnectionId { get; set; }
    public string Name { get; init; }

    public Guid? MatchId { get; set; }
}

public class MatchRepository
{
    private readonly Dictionary<Guid, Match> _matches = [];
    private readonly Dictionary<Guid, TankDbo> _connectedTanks = [];

    public IEnumerable<TankDbo> GetAvailableTanks()
    {
        return _connectedTanks.Values.Where(t => t.MatchId is null);
    }

    public void AddTank(Guid tankId, string connectionId, string tankName)
    {
        _connectedTanks[tankId] = new TankDbo
        {
            Id = tankId,
            ConnectionId = connectionId,
            Name = tankName
        };
    }

    public bool TryGetTank(string connectionId, [MaybeNullWhen(false)] out TankDbo tank)
    {
        tank = _connectedTanks.Values.FirstOrDefault(t => t.ConnectionId == connectionId);
        return tank is not null;
    }

    public bool TryGetMatch(Guid matchId, [MaybeNullWhen(false)] out Match match)
    {
        return _matches.TryGetValue(matchId, out match);
    }

    public void Remove(Match match)
    {
        //TODO: free tanks
        _matches.Remove(match.Id);
    }
}
