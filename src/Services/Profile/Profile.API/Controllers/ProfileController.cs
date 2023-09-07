using LibraLibrium.Services.Profile.API.Application.Queries;
using LibraLibrium.Services.Profile.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraLibrium.Services.Profile.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly ILogger<ProfileController> _logger;
    private readonly IMediator _mediator;

    public ProfileController(ILogger<ProfileController> logger, IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("{identity}")]
    public async Task<IActionResult> GetProfileByIdentity([FromRoute] string identity)
    {
        var query = new GetProfileQuery(identity);

        _logger.LogInformation(
               "Sending query: {QueryName} - {IdProperty}: {queryId} ({@Query})",
               query.GetGenericTypeName(),
               nameof(query.Identity),
               query.Identity,
               query);

        var queryResult = await _mediator.Send(query);

        if (queryResult is null)
        {
            return BadRequest();
        }

        return Ok(queryResult);
    }
}
