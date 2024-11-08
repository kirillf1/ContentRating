// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.GetPartyEstimationRoomTitles
{
    public class PartyEstimationTitle
    {
        public PartyEstimationTitle(Guid id, string roomName, string roomCreatorName, int ratersCount, int contentCount, bool isEstimated)
        {
            Id = id;
            RoomName = roomName;
            RoomCreatorName = roomCreatorName;
            RatersCount = ratersCount;
            ContentCount = contentCount;
            IsEstimated = isEstimated;
        }

        public Guid Id { get; }
        public string RoomName { get; }
        public string RoomCreatorName { get; }
        public int RatersCount { get; }
        public int ContentCount { get; }
        public bool IsEstimated { get; }
    }
}
