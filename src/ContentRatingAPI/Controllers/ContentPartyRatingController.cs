using Ardalis.Result.AspNetCore;
using ContentRatingAPI.Application.ContentPartyRating.EstimateContent;
using ContentRatingAPI.Application.ContentPartyRating.GetContentRating;
using ContentRatingAPI.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentRatingAPI.Controllers
{
    [Route("api/content-party-rating")]
    [ApiController]
    [Authorize]
    public class ContentPartyRatingController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IUserInfoService userInfoService;

        public ContentPartyRatingController(IMediator mediator, IUserInfoService userInfoService)
        {
            this.mediator = mediator;
            this.userInfoService = userInfoService;
        }

        [TranslateResultToActionResult]
        [HttpGet("{contentRatingId:guid}")]
        public async Task<Result<ContentPartyRatingResponse>> GetContentRating(Guid contentRatingId)
        {
            return await mediator.Send(new GetContentRatingQuery(contentRatingId));

        }

        [TranslateResultToActionResult]
        [HttpPut("{contentRatingId:guid}")]
        public async Task<Result> EstimateContent(Guid contentRatingId,
            [FromBody] EstimateContentRequest request)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();

            return await mediator.Send(new EstimateContentCommand(contentRatingId, userInfo.Id, request.RaterForChangeScoreId, request.NewScore));

        }

    }
}
