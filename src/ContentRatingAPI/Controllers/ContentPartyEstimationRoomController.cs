using ContentRatingAPI.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentRatingAPI.Controllers
{
    [Route("api/content-party-estimation-room")]
    [ApiController]
    public class ContentPartyEstimationRoomController : ControllerBase
    {
        private readonly IMediator mediator;

        public ContentPartyEstimationRoomController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetRooms()
        {
            return Ok();
        }
        [Authorize(policy: Policies.ContentEstimationRoomUserAccessPolicyName)]
        [HttpGet("{roomId:guid}")]
        public async Task<IActionResult> GetContentPartyEstimationRoom(Guid roomId)
        {
            return Ok();
        }

        [Authorize]
        [HttpPost("{roomId:guid}")]
        public async Task<IActionResult> CreateEstimationRoom(Guid roomId)
        {
            return Ok();
        }

        [Authorize(policy: Policies.ContentEstimationRoomUserAccessPolicyName)]
        [HttpPost("{roomId:guid}/rater/{raterId:guid}")]
        public async Task<IActionResult> AddRaterToRoom(Guid roomId, Guid raterId)
        {
            return Ok();
        }

        [Authorize(policy: Policies.ContentEstimationRoomUserAccessPolicyName)]
        [HttpPut("{roomId:guid}/rater/{raterId:guid}")]
        public async Task<IActionResult> UpdateRaterInRoom(Guid roomId, Guid raterId)
        {
            return Ok();
        }

        [Authorize(policy: Policies.ContentEstimationRoomUserAccessPolicyName)]
        [HttpDelete("{roomId:guid}/rater/{raterId:guid}")]
        public async Task<IActionResult> DeleteRaterInRoom(Guid roomId, Guid raterId)
        {
            return Ok();
        }

        [Authorize(policy: Policies.ContentEstimationRoomUserAccessPolicyName)]
        [HttpPut("{roomId:guid}/complete-estimation")]
        public async Task<IActionResult> CompleteRoomEstimation(Guid roomId)
        {
            return Ok();
        }

        [Authorize(policy: Policies.ContentEstimationRoomUserAccessPolicyName)]
        [HttpDelete("{roomId:guid}/content/{contentId:guid}")]
        public async Task<IActionResult> DeleteUnusedContent(Guid roomId, Guid contentId)
        {
            return Ok();
        }

        [Authorize(policy: Policies.ContentEstimationRoomUserAccessPolicyName)]
        [HttpPut("{roomId:guid}/rating-range")]
        public async Task<IActionResult> ChangeRatingRange(Guid roomId)
        {
            return Ok();
        }
    }
}
