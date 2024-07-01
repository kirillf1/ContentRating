using ContentRatingAPI.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentRatingAPI.Controllers
{
    [Route("api/content-room-editor")]
    [ApiController]
    public class ContentRoomEditorController : ControllerBase
    {
        private readonly IMediator mediator;

        public ContentRoomEditorController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            return Ok();
        }

        [Authorize(policy: Policies.ContentRoomEditorUserAccessPolicyName)]
        [HttpGet("{roomId:guid}")]
        public async Task<IActionResult> GetRoomEditor(Guid roomId)
        {
            return Ok();
        }

        [Authorize]
        [HttpPost("{roomId:guid}")]
        public async Task<IActionResult> CreateContentEstimationRoom(Guid roomId)
        {
            return Ok();
        }

        [Authorize(policy: Policies.ContentRoomEditorUserAccessPolicyName)]
        [HttpPost("{roomId:guid}/content/{contentId:guid}")]
        public async Task<IActionResult> AddContentInRoomEditor(Guid roomId, Guid contentId)
        {
            return Ok();
        }

        [Authorize(policy: Policies.ContentRoomEditorUserAccessPolicyName)]
        [HttpPut("{roomId:guid}/content/{contentId:guid}")]
        public async Task<IActionResult> UpdateContentInRoomEditor(Guid roomId, Guid contentId)
        {
            return Ok();
        }

        [Authorize(policy: Policies.ContentRoomEditorUserAccessPolicyName)]
        [HttpDelete("{roomId:guid}/content/{contentId:guid}")]
        public async Task<IActionResult> DeleteContentInRoomEditor(Guid roomId, Guid contentId)
        {
            return Ok();
        }

        [Authorize(policy: Policies.ContentRoomEditorUserAccessPolicyName)]
        [HttpPost("{roomId:guid}/editor/{editorId:guid}")]
        public async Task<IActionResult> AddEditorInRoomEditor(Guid roomId, Guid editorId)
        {
            return Ok();
        }

        [Authorize(policy: Policies.ContentRoomEditorUserAccessPolicyName)]
        [HttpPut("{roomId:guid}/editor/{editorId:guid}")]
        public async Task<IActionResult> UpdateEditorInRoomEditor(Guid roomId, Guid editorId)
        {
            return Ok();
        }

        [Authorize(policy: Policies.ContentRoomEditorUserAccessPolicyName)]
        [HttpDelete("{roomId:guid}/editor/{editorId:guid}")]
        public async Task<IActionResult> DeleteEditorInRoomEditor(Guid roomId, Guid editorId)
        {
            return Ok();
        }

    }
}
