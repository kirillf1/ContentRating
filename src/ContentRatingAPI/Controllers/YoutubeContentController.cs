// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using Ardalis.Result.AspNetCore;
using ContentRating.Web.Contracts.YoutubeContent;
using ContentRatingAPI.Application.YoutubeContent;
using ContentRatingAPI.Application.YoutubeContent.GetYoutubePlayLists;
using ContentRatingAPI.Application.YoutubeContent.GetYoutubeVideos;
using ContentRatingAPI.Application.YoutubeContent.GetYoutubeVideoTitle;
using ContentRatingAPI.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentRatingAPI.Controllers
{
    [Authorize]
    [Route("api/youtube-content")]
    [ApiController]
    public class YoutubeContentController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IUserInfoService userInfoService;

        public YoutubeContentController(
            IMediator mediator,
            IUserInfoService userInfoService,
            HttpClient httpClient
        )
        {
            this.mediator = mediator;
            this.userInfoService = userInfoService;
        }

        [HttpGet]
        [TranslateResultToActionResult]
        public async Task<Result<IEnumerable<YoutubePlaylist>>> GetYoutubePlaylists()
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
            {
                return Result.Forbidden();
            }

            return await mediator.Send(new GetYoutubePlayListsQuery(userInfo.Id));
        }

        [HttpGet("{playlistId}")]
        [TranslateResultToActionResult]
        public async Task<Result<IEnumerable<YoutubeVideo>>> GetVideosByPlaylist(string playlistId)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
            {
                return Result.Forbidden();
            }

            return await mediator.Send(new GetYoutubeVideosQuery(userInfo.Id, playlistId));
        }

        [HttpPost("video-title")]
        [TranslateResultToActionResult]
        public async Task<Result<YoutubeVideoTitle>> GetVideoTitle(
            [FromBody] GetYoutubeVideoTitleRequest request
        )
        {
            return await mediator.Send(new GetYoutubeVideoTitleQuery(request.Url));
        }
    }
}
