// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Application.YoutubeContent.GetYoutubePlayLists
{
    public class GetYoutubePlayListsQuery : IRequest<Result<IEnumerable<YoutubePlaylist>>>
    {
        public GetYoutubePlayListsQuery(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; }
    }
}
