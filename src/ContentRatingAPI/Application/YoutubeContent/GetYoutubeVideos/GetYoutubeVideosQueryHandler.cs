// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRatingAPI.Application.Identity;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;

namespace ContentRatingAPI.Application.YoutubeContent.GetYoutubeVideos
{
    public class GetYoutubeVideosQueryHandler : IRequestHandler<GetYoutubeVideosQuery, Result<IEnumerable<YoutubeVideo>>>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IYoutubeClient youtubeClient;

        public GetYoutubeVideosQueryHandler(UserManager<ApplicationUser> userManager, IYoutubeClient youtubeClient)
        {
            this.userManager = userManager;
            this.youtubeClient = youtubeClient;
        }

        public async Task<Result<IEnumerable<YoutubeVideo>>> Handle(GetYoutubeVideosQuery request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString());

            if (user is null)
            {
                return Result.Error("Unknown user id");
            }

            if (user.AuthenticationScheme != GoogleDefaults.AuthenticationScheme)
            {
                return Result.Invalid(new ValidationError("User must be login by google"));
            }

            return await youtubeClient.GetVideosFromPlayList(request.PlaylistId, user.ExternalResourceAccessToken!);
        }
    }
}
