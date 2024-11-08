﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Infrastructure.YoutubeClient.Models
{
    public class PlaylistModel
    {
        public string? Kind { get; set; }
        public string? Etag { get; set; }
        public string? NextPageToken { get; set; }
        public List<Item>? Items { get; set; }
        public PageInfo? PageInfo { get; set; }
    }
}
