using Indice.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MorningNewsBrief.Common.Models;
using MorningNewsBrief.Common.Models.Proxies.NewsApi.Filters;
using MorningNewsBrief.Common.Services.Abstractions;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace MorningNewsBrief.Api.Controllers {
    /// <summary>
    /// Morning News Briefing
    /// </summary>
    [ApiController]
    [Route("/api/morning-news-brief")]
    [SwaggerTag("Get the morning news briefing.")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public class MorningNewsBriefController : ControllerBase {

        private readonly ILogger<MorningNewsBriefController> _logger;
        private readonly INewsBriefService _newsBriefFacade;

        public MorningNewsBriefController(ILogger<MorningNewsBriefController> logger, INewsBriefService newsBriefFacade) {
            _logger = logger;
            _newsBriefFacade = newsBriefFacade ?? throw new ArgumentNullException(nameof(newsBriefFacade));
        }

        /// <summary>
        /// Get the morning news briefing
        /// </summary>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">The mornign news briefing object</response>
        [HttpGet()]
        [SwaggerOperation(
            Summary = "Get the morning news briefing",
            Description = "This endpoint will return the available news briefing or the cached version if its not found.",
            OperationId = "GetNewsBrief",
            Tags = new[] { "Briefing" })]
        [SwaggerResponse(200, "The posted order payload", type: typeof(NewsBriefing))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NewsBriefing))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        //TODO: Create a new policy for VaryByQueryKeys ListOptions<NewsBriefingFilter> options
        [OutputCache(Duration = 300, VaryByQueryKeys = new[] { "*" })]
        public async Task<IActionResult> GetNewsBriefing([FromQuery, SwaggerRequestBody("The briefing optional filter and sort parameters", Required = false)] ListOptions<NewsBriefingFilter> options, CancellationToken cancellationToken) {
            var briefing = await _newsBriefFacade.GetNewsBriefing(options, cancellationToken);
            if (briefing == null) {
                return NotFound();
            }
            return Ok(briefing);
        }
    }
}
