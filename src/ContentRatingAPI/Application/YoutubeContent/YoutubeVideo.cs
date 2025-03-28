﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Application.YoutubeContent
{
    public class YoutubeVideo
    {
        public YoutubeVideo(string url, string urlEmbed, string name)
        {
            Url = url;
            UrlEmbed = urlEmbed;
            Name = name;
        }

        public string Url { get; }
        public string UrlEmbed { get; }
        public string Name { get; }
    }
}
