// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events
{
    public record ContentEstimationStartedDomainEvent(
        Guid RoomId,
        IEnumerable<Rater> Raters,
        IEnumerable<ContentForEstimation> ContentForEstimation,
        RatingRange RatingRange
    ) : INotification;
}
