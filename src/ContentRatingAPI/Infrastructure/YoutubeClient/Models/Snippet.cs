﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Infrastructure.YoutubeClient.Models
{
    public class Snippet
    {
        public DateTimeOffset? PublishedAt { get; set; }
        public string? ChannelId { get; set; } = default!;
        public string? Title { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public Thumbnails? Thumbnails { get; set; } = default!;
        public string? ChannelTitle { get; set; } = default!;
        public string? PlaylistId { get; set; } = default!;
        public long? Position { get; set; } = default!;
        public ResourceId? ResourceId { get; set; } = default!;
        public string? VideoOwnerChannelTitle { get; set; } = default!;
        public string? VideoOwnerChannelId { get; set; } = default!;
    }
}
