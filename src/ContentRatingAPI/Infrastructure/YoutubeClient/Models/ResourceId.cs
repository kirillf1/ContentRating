// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Infrastructure.YoutubeClient.Models
{
    public class ResourceId
    {
        public string? Kind { get; set; } = default!;
        public string? VideoId { get; set; } = default!;
    }
}
