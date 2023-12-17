using Indice.Serialization;
using Indice.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using MorningNewsBrief.Common.Models;
using MorningNewsBrief.Common.Models.Proxies.NewsApi.Filters;
using MorningNewsBrief.Common.Services;
using MorningNewsBrief.Common.Services.Abstractions;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace MorningNewsBrief.Api.Controllers {
    /// <summary>
    /// Morning News Briefing
    /// </summary>
    [ApiController]
    [Route("/api/morning-news-brief")]
    [SwaggerTag("Get Weather forecast and place orders. Very weird and unstructed API :)")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public class MorningNewsBriefController : ControllerBase {

        private readonly ILogger<MorningNewsBriefController> _logger;
        private readonly INewsBriefFacade _newsBriefFacade;
        private readonly IDistributedCache _cache;

        public MorningNewsBriefController(ILogger<MorningNewsBriefController> logger, INewsBriefFacade newsBriefFacade, IDistributedCache cache) {
            _logger = logger;
            _newsBriefFacade = newsBriefFacade ?? throw new ArgumentNullException(nameof(newsBriefFacade));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <summary>
        /// Get the morning news briefing
        /// </summary>
        /// <param name="options"></param>
        /// <returns>The mornign news briefing object</returns>
        [HttpGet()]
        [SwaggerOperation(
            Summary = "Get the morning news briefing",
            Description = "This endpoint will return the available news briefing or the cached version if its not found.",
            OperationId = "GetNewsBrief",
            Tags = new[] { "NewsBrief" })]
        [SwaggerResponse(200, "The posted order payload", type: typeof(NewsBriefing))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NewsBriefing))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status502BadGateway, Type = typeof(ProblemDetails))]
        [OutputCache(Duration = 300, VaryByQueryKeys = new[] { "*" })]
        public async Task<IActionResult> GetNewsBriefing([FromQuery] ListOptions<NewsBriefingFilter> options) {
            var briefing = await _newsBriefFacade.GetNewsBriefing(options);
            if (briefing == null) {
                return NotFound();
            }
            return Ok(briefing);
        }
    }
}
