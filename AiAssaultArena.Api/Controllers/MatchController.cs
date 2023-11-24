using AiAssaultArena.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiAssaultArena.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MatchController(MatchRepository matchRepository) : ControllerBase
{
    private readonly MatchRepository _matchRepository = matchRepository;

    [HttpGet]
    public IEnumerable<Match> Matches()
    {
        return _matchRepository.GetAllMatches();
    }
}
