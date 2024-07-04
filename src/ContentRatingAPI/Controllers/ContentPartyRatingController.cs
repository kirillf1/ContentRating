﻿using ContentRatingAPI.Application.ContentPartyRating.EstimateContent;
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
        private readonly IUserInfoService userInfoService;

        public ContentPartyRatingController(IMediator mediator, IUserInfoService userInfoService)
        {
            this.mediator = mediator;
            this.userInfoService = userInfoService;
        }
        [HttpPut("{roomId:guid}/{contentRatingId:guid}")]
        public async Task<IActionResult> EstimateContent(Guid roomId, Guid contentRatingId,
            [FromBody] EstimateContentRequest request)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Forbid("Unknown user info");

            await mediator.Send(new EstimateContentCommand(contentRatingId, userInfo.Id, request.RaterForChangeScoreId, request.NewScore));
            return Ok();
        }

    }
}
