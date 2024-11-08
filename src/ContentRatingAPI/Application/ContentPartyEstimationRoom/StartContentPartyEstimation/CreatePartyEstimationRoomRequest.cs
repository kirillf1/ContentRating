// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.StartContentPartyEstimation
{
    public class CreatePartyEstimationRoomRequest
    {
        public required Guid RoomId { get; set; }
        public required Guid ContentListId { get; set; }
        public required string RoomName { get; set; }
        public required double MinRating { get; set; }
        public required double MaxRating { get; set; }
    }
}
