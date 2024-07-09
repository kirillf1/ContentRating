using Ardalis.Result.AspNetCore;
using ContentRatingAPI.Application.ContentRoomEditor.ContentModifications;
using ContentRatingAPI.Application.ContentRoomEditor.CreateContentEditorRoom;
using ContentRatingAPI.Application.ContentRoomEditor.GetContentRoomEditor;
using ContentRatingAPI.Application.ContentRoomEditor.GetContentRoomEditorTitles;
using ContentRatingAPI.Application.ContentRoomEditor.InviteEditor;
using ContentRatingAPI.Application.ContentRoomEditor.KickEditor;
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
        private readonly IUserInfoService userInfoService;

        public ContentRoomEditorController(IMediator mediator, IUserInfoService userInfoService)
        {
            this.mediator = mediator;
            this.userInfoService = userInfoService;
        }

        [TranslateResultToActionResult]
        [Authorize]
        [HttpGet]
        public async Task<Result<IEnumerable<ContentRoomEditorTitle>>> GetRooms()
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

            return await mediator.Send(new GetContentEditorTitlesQuery(userInfo.Id));
        }

        [TranslateResultToActionResult]
        [Authorize(policy: Policies.ContentRoomEditorUserAccessPolicyName)]
        [HttpGet("{roomId:guid}")]
        public async Task<Result<ContentRoomEditorResponse>> GetRoomEditor(Guid roomId)
        {
            return await mediator.Send(new GetContentRoomEditorQuery(roomId));
        }

        [Authorize]
        [HttpPost("{roomId:guid}")]
        [TranslateResultToActionResult]
        public async Task<Result> CreateContentRoomEditor(Guid roomId, 
            [FromBody] CreateContentRoomEditorRequest createContentRoomEditorRequest)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

            return await mediator.Send(new CreateContentEditorRoomCommand(roomId, userInfo.Id, userInfo.Name, 
                createContentRoomEditorRequest.RoomName));
            
        }

        [Authorize(policy: Policies.ContentRoomEditorUserAccessPolicyName)]
        [HttpPost("{roomId:guid}/content/{contentId:guid}")]
        [TranslateResultToActionResult]
        public async Task<Result> AddContentInRoomEditor(Guid roomId, Guid contentId, 
            [FromBody] CreateContentRequest createContentRequest)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

           return await mediator.Send(new CreateContentCommand(roomId, userInfo.Id, contentId,
                createContentRequest.Name, createContentRequest.Path, createContentRequest.ContentType));
           
        }

        [Authorize(policy: Policies.ContentRoomEditorUserAccessPolicyName)]
        [HttpPut("{roomId:guid}/content/{contentId:guid}")]
        [TranslateResultToActionResult]
        public async Task<Result> UpdateContentInRoomEditor(Guid roomId, Guid contentId,
            [FromBody] UpdateContentRequest updateContentRequest)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

           return await mediator.Send(new UpdateContentCommand(contentId, roomId, userInfo.Id, updateContentRequest.Name,
                updateContentRequest.Path, updateContentRequest.ContentType));
        }

        [TranslateResultToActionResult]
        [Authorize(policy: Policies.ContentRoomEditorUserAccessPolicyName)]
        [HttpDelete("{roomId:guid}/content/{contentId:guid}")]
        public async Task<Result> DeleteContentInRoomEditor(Guid roomId, Guid contentId)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

            return await mediator.Send(new RemoveContentCommand(contentId, roomId, userInfo.Id));
          
        }


        [TranslateResultToActionResult]
        [Authorize(policy: Policies.ContentRoomEditorUserAccessPolicyName)]
        [HttpPost("{roomId:guid}/editor/{editorId:guid}")]
        public async Task<Result> InviteEditorInRoomEditor(Guid roomId, Guid editorId,
            [FromBody] InviteEditorRequest inviteEditorRequest)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

            return await mediator.Send(new InviteEditorCommand(roomId, editorId, inviteEditorRequest.EditorId, inviteEditorRequest.EditorName));
        }

        [TranslateResultToActionResult]
        [Authorize(policy: Policies.ContentRoomEditorUserAccessPolicyName)]
        [HttpDelete("{roomId:guid}/editor/{editorId:guid}")]
        public async Task<Result> KickEditorInRoomEditor(Guid roomId, Guid editorId)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

            return await mediator.Send(new KickEditorCommand(roomId, userInfo.Id, editorId));
            
        }
    }
}
