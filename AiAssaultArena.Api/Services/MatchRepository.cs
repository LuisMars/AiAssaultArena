
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

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

    public bool TryGetTank(Guid id, [MaybeNullWhen(false)] out TankDbo tank)
    {
        return _connectedTanks.TryGetValue(id, out tank);
    }

    public bool TryGetMatch(Guid matchId, [MaybeNullWhen(false)] out Match match)
    {
        return _matches.TryGetValue(matchId, out match);
    }

    public bool TryGetMatch(string connectionId, [MaybeNullWhen(false)] out Match match)
    {
        match = _matches.Values.FirstOrDefault(m => m.WebClientConnectionId == connectionId);
        return match is not null;
    }

    public void RemoveMatch(Match match)
    {
        var tanks = _connectedTanks.Values.Where(t => t.MatchId == match.Id);
        foreach (var tank in tanks)
        {
            tank.MatchId = null;
        }
        _matches.Remove(match.Id);
    }

    public void RemoveTank(TankDbo tank)
    {
        _connectedTanks.Remove(tank.Id);
    }

    public void AddMatch(Match match)
    {
        _matches[match.Id] = match;
    }

    public IEnumerable<Match> GetAllMatches()
    {
        return _matches.Values;
    }
}
