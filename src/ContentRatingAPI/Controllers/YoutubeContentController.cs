using Ardalis.Result.AspNetCore;
using ContentRatingAPI.Application.YoutubeContent;
using ContentRatingAPI.Application.YoutubeContent.GetYoutubePlayLists;
using ContentRatingAPI.Application.YoutubeContent.GetYoutubeVideos;
using ContentRatingAPI.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentRatingAPI.Controllers
{
    [Route("api/youtube-content")]
    [ApiController]
    public class YoutubeContentController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IUserInfoService userInfoService;

        public YoutubeContentController(IMediator mediator, IUserInfoService userInfoService)
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
                return Result.Forbidden();
           return await mediator.Send(new GetYoutubePlayListsQuery(userInfo.Id));
        }
        [HttpGet("{playlistId}")]
        public async Task<Result<IEnumerable<YoutubeVideo>>> GetVideosByPlaylist(string playlistId)
        {
            var userInfo = userInfoService.TryGetUserInfo();
            if (userInfo is null)
                return Result.Forbidden();
            return await mediator.Send(new GetYoutubeVideosQuery(userInfo.Id, playlistId));
        }
    }
}
