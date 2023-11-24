using AiAssaultArena.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiAssaultArena.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MatchController(MatchService matchService) : ControllerBase
{
    private readonly MatchService _matchService = matchService;

    [HttpGet]
    public IEnumerable<Match> Matches()
    {
        return _matchService.Matches;
    }
}
