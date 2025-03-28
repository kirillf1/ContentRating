// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Infrastructure.ContentFileManagers
{
    public class ContentFileOptions
    {
        public string Directory { get; set; } = "";
        public double OldCheckedFileTakeIntervalInHours { get; set; } = 24;
        public double CheckUnusedFilesIntervalInHours { get; set; } = 24;
    }
}
