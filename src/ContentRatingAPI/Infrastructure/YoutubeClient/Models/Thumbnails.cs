// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Infrastructure.YoutubeClient.Models
{
    public class Thumbnails
    {
        public Default? Default { get; set; } = default!;
        public Default? Medium { get; set; } = default!;
        public Default? High { get; set; } = default!;
        public Default? Standard { get; set; } = default!;
        public Default? Maxres { get; set; } = default!;
    }
}
