// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Web.Contracts.YoutubeContent;
using ContentRatingAPI.Application.Identity;
using ContentRatingAPI.Infrastructure.Authorization.Google;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;

namespace ContentRatingAPI.Application.YoutubeContent.GetYoutubePlayLists
{
    public class GetYoutubePlayListsQueryHandler
        : IRequestHandler<GetYoutubePlayListsQuery, Result<IEnumerable<YoutubePlaylist>>>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IYoutubeClient youtubeClient;
        private readonly GoogleTokenRefreshService tokenRefreshService;

        public GetYoutubePlayListsQueryHandler(
            UserManager<ApplicationUser> userManager,
            IYoutubeClient youtubeClient,
            GoogleTokenRefreshService tokenRefreshService
        )
        {
            this.userManager = userManager;
            this.youtubeClient = youtubeClient;
            this.tokenRefreshService = tokenRefreshService;
        }

        public async Task<Result<IEnumerable<YoutubePlaylist>>> Handle(
            GetYoutubePlayListsQuery request,
            CancellationToken cancellationToken
        )
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

            return await youtubeClient.GetAvailablePlayLists(user);
        }
    }
}
