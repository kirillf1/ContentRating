// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate
{
    public interface IContentPartyEstimationRoomRepository : IRepository<ContentPartyEstimationRoom>
    {
        ContentPartyEstimationRoom Add(ContentPartyEstimationRoom room);
        ContentPartyEstimationRoom Update(ContentPartyEstimationRoom room);
        void Delete(ContentPartyEstimationRoom room);
        Task<ContentPartyEstimationRoom?> GetRoom(Guid id);
        Task<bool> HasRaterInRoom(Guid roomId, Guid raterId);
    }
}
