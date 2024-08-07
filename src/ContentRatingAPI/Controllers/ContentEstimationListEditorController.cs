using Ardalis.Result.AspNetCore;
using ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications;
using ContentRatingAPI.Application.ContentEstimationListEditor.CreateContentEstimationListEditor;
using ContentRatingAPI.Application.ContentEstimationListEditor.GetContentEstimationListEditor;
using ContentRatingAPI.Application.ContentEstimationListEditor.GetContentEstimationListEditorTitles;
using ContentRatingAPI.Application.ContentEstimationListEditor.InviteEditor;
using ContentRatingAPI.Application.ContentEstimationListEditor.KickEditor;
using ContentRatingAPI.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentRatingAPI.Controllers
{
    [Route("api/content-estimation-list-editor")]
    [ApiController]
    public class ContentEstimationListEditorController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IUserInfoService userInfoService;

        public ContentEstimationListEditorController(IMediator mediator, IUserInfoService userInfoService)
        {
            this.mediator = mediator;
            this.userInfoService = userInfoService;
        }

        [TranslateResultToActionResult]
        [Authorize]
        [HttpGet]
        public async Task<Result<IEnumerable<ContentEstimationListEditorTitle>>> GetRooms()
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

            return await mediator.Send(new GetContentEstimationListEditorTitlesQuery(userInfo.Id));
        }

        [TranslateResultToActionResult]
        [Authorize(policy: Policies.ContentEstimationListEditorUserAccessPolicyName)]
        [HttpGet("{roomId:guid}")]
        public async Task<Result<ContentEstimationListEditorResponse>> GetRoomEditor(Guid roomId)
        {
            return await mediator.Send(new GetContentEstimationListEditorQuery(roomId));
        }

        [Authorize]
        [HttpPost]
        [TranslateResultToActionResult]
        public async Task<Result<bool>> CreateContentRoomEditor(
            [FromBody] CreateContentEstimationListEditorRequest createContentRoomEditorRequest)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

            return await mediator.Send(new CreateContentEstimationListEditorCommand(createContentRoomEditorRequest.Id, userInfo.Id, userInfo.Name, 
                createContentRoomEditorRequest.RoomName));
            
        }

        [Authorize(policy: Policies.ContentEstimationListEditorUserAccessPolicyName)]
        [HttpPost("{roomId:guid}/content")]
        [TranslateResultToActionResult]
        public async Task<Result<bool>> AddContentInRoomEditor(Guid roomId, [FromBody] CreateContentRequest createContentRequest)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

           return await mediator.Send(new CreateContentCommand(roomId, userInfo.Id, createContentRequest.Id,
                createContentRequest.Name, createContentRequest.Url, createContentRequest.ContentType));
           
        }

        [Authorize(policy: Policies.ContentEstimationListEditorUserAccessPolicyName)]
        [HttpPut("{roomId:guid}/content/{contentId:guid}")]
        [TranslateResultToActionResult]
        public async Task<Result<bool>> UpdateContentInRoomEditor(Guid roomId, Guid contentId,
            [FromBody] UpdateContentRequest updateContentRequest)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

           return await mediator.Send(new UpdateContentCommand(contentId, roomId, userInfo.Id, updateContentRequest.Name,
                updateContentRequest.Url, updateContentRequest.ContentType));
        }

        [TranslateResultToActionResult]
        [Authorize(policy: Policies.ContentEstimationListEditorUserAccessPolicyName)]
        [HttpDelete("{roomId:guid}/content/{contentId:guid}")]
        public async Task<Result> DeleteContentInRoomEditor(Guid roomId, Guid contentId)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

            return await mediator.Send(new RemoveContentCommand(contentId, roomId, userInfo.Id));
          
        }

        [TranslateResultToActionResult]
        [Authorize(policy: Policies.ContentEstimationListEditorUserAccessPolicyName)]
        [HttpPost("{roomId:guid}/editor")]
        public async Task<Result<bool>> InviteEditorInRoomEditor(Guid roomId,
            [FromBody] InviteEditorRequest inviteEditorRequest)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

            return await mediator.Send(new InviteEditorCommand(roomId, userInfo.Id, inviteEditorRequest.EditorId, inviteEditorRequest.EditorName));
        }

        [TranslateResultToActionResult]
        [Authorize(policy: Policies.ContentEstimationListEditorUserAccessPolicyName)]
        [HttpDelete("{roomId:guid}/editor/{editorId:guid}")]
        public async Task<Result<bool>> KickEditorInRoomEditor(Guid roomId, Guid editorId)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

            return await mediator.Send(new KickEditorCommand(roomId, userInfo.Id, editorId));
            
        }
    }
}
