using Ardalis.Result.AspNetCore;
using ContentRatingAPI.Application.ContentPartyEstimationRoom.ChangeRatingRange;
using ContentRatingAPI.Application.ContentPartyEstimationRoom.CompleteContentEstimation;
using ContentRatingAPI.Application.ContentPartyEstimationRoom.GetPartyEstimationRoom;
using ContentRatingAPI.Application.ContentPartyEstimationRoom.GetPartyEstimationRoomTitles;
using ContentRatingAPI.Application.ContentPartyEstimationRoom.InviteRater;
using ContentRatingAPI.Application.ContentPartyEstimationRoom.KickRater;
using ContentRatingAPI.Application.ContentPartyEstimationRoom.RemoveUnavailableContent;
using ContentRatingAPI.Application.ContentPartyEstimationRoom.StartContentPartyEstimation;
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
        private readonly IUserInfoService userInfoService;

        public ContentPartyEstimationRoomController(IMediator mediator, IUserInfoService userInfoService)
        {
            this.mediator = mediator;
            this.userInfoService = userInfoService;
        }
        [TranslateResultToActionResult]
        [HttpGet]
        [Authorize]
        public async Task<Result<IEnumerable<PartyEstimationTitle>>> GetRooms([FromQuery] GetPartyEstimationRoomTitlesRequest request)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

            var query = new GetPartyEstimationRoomTitlesQuery(request.IncludeEstimated, request.IncludeNotEstimated, userInfo.Id);
            return await mediator.Send(query);
        }

        [TranslateResultToActionResult]
        [Authorize(policy: Policies.ContentEstimationRoomUserAccessPolicyName)]
        [HttpGet("{roomId:guid}")]
        public async Task<Result<PartyEstimationRoomResponse>> GetContentPartyEstimationRoom(Guid roomId)
        {
            return await mediator.Send(new GetPartyEstimationRoomQuery(roomId));
        }

        [TranslateResultToActionResult]
        [Authorize]
        [HttpPost("{roomId:guid}")]
        public async Task<Result> CreateEstimationRoom(Guid roomId, 
            [FromBody] CreatePartyEstimationRoomRequest request)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

            return await mediator.Send(new StartContentPartyEstimationCommand(roomId, request.ContentListId, request.RoomName,
                request.MinRating, request.MaxRating, userInfo.Id, userInfo.Name));
            
        }

        [TranslateResultToActionResult]
        [Authorize(policy: Policies.ContentEstimationRoomUserAccessPolicyName)]
        [HttpPost("{roomId:guid}/rater/{raterId:guid}")]
        [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Invalid, ResultStatus.Error)]
        public async Task<Result> InviteRaterToRoom(Guid roomId, Guid raterId,
            [FromBody] InviteRaterRequest request)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

            return await mediator.Send(new InviteRaterCommand(roomId, userInfo.Id, raterId, request.RoleType, request.RaterName));
        }

        [TranslateResultToActionResult]
        [Authorize(policy: Policies.ContentEstimationRoomUserAccessPolicyName)]
        [HttpDelete("{roomId:guid}/rater/{raterId:guid}")]
        public async Task<Result> KickRater(Guid roomId, Guid raterId)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

            return await mediator.Send(new KickRaterCommand(roomId, userInfo.Id, raterId));
        }

        [TranslateResultToActionResult]
        [Authorize(policy: Policies.ContentEstimationRoomUserAccessPolicyName)]
        [HttpPut("{roomId:guid}/complete-estimation")]
        public async Task<Result> CompleteRoomEstimation(Guid roomId)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

            return await mediator.Send(new CompleteContentEstimationCommand(userInfo.Id, roomId));
        }

        [TranslateResultToActionResult]
        [Authorize(policy: Policies.ContentEstimationRoomUserAccessPolicyName)]
        [HttpDelete("{roomId:guid}/content/{contentId:guid}")]
        public async Task<Result> DeleteUnavailableContent(Guid roomId, Guid contentId)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

            return await mediator.Send(new RemoveUnavailableContentCommand(roomId, contentId, userInfo.Id));
        }

        [TranslateResultToActionResult]
        [Authorize(policy: Policies.ContentEstimationRoomUserAccessPolicyName)]
        [HttpPut("{roomId:guid}/rating-range")]
        public async Task<IActionResult> ChangeRatingRange(Guid roomId, 
            [FromBody] ChangeRatingRangeRequest request)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Forbid("Unknown user info");

            await mediator.Send(new ChangeRatingRangeCommand(roomId, userInfo.Id, request.MinRating, request.MaxRating));
            return Ok();
        }
    }
}
