using ContentRatingAPI.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentRatingAPI.Controllers
{
    [Route("api/content-party-rating")]
    [ApiController]
    [Authorize(policy: Policies.ContentEstimationRoomUserAccessPolicyName)]
    public class ContentPartyRatingController : ControllerBase
    {
        private readonly IMediator mediator;

        public ContentPartyRatingController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpPut("{roomId:guid}/{contentRatingId:guid}")]
        public async Task<IActionResult> EstimateContent(Guid roomId, Guid contentRatingId)
        {
            return Ok();
        }

    }
}
