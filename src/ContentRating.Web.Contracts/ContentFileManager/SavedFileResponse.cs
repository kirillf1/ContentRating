// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRating.Web.Contracts.ContentFileManager
{
    public class SavedFileResponse
    {
        public required Guid Id { get; set; }
        public required string FileRoute { get; set; }
    }
} 