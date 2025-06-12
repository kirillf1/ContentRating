// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRating.Web.Contracts.ContentPartyEstimationRoom
{
    public class GetPartyEstimationRoomTitlesRequest
    {
        public bool IncludeEstimated { get; set; }
        public bool IncludeNotEstimated { get; set; }
    }
} 