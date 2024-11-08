// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Infrastructure.ContentFileManagers.FileSavers
{
    public class FFMPEGOptions
    {
        public string FFMPEGPath { get; set; } = "ffmpeg";
        public double SegmentTime { get; set; } = 3;
    }
}
