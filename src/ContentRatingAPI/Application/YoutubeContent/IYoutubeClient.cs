// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Web.Contracts.YoutubeContent;
using ContentRatingAPI.Application.Identity;

namespace ContentRatingAPI.Application.YoutubeContent
{
    public interface IYoutubeClient
    {
        public Task<Result<IEnumerable<YoutubeVideo>>> GetVideosFromPlayList(string playListId, ApplicationUser user);
        public Task<Result<IEnumerable<YoutubePlaylist>>> GetAvailablePlayLists(ApplicationUser user);
    }
}
