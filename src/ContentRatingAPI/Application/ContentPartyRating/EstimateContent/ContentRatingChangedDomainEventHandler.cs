// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.Events;
using ContentRatingAPI.Application.Notifications.IContentPartyEstimationNotifications;

namespace ContentRatingAPI.Application.ContentPartyRating.EstimateContent
{
    public class ContentRatingChangedDomainEventHandler : INotificationHandler<ContentRatingChangedDomainEvent>
    {
        private readonly IContentPartyEstimationNotificationService notificationService;

        public ContentRatingChangedDomainEventHandler(IContentPartyEstimationNotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public async Task Handle(ContentRatingChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            var roomId = notification.RoomId;
            var raterId = notification.Rater.RaterId;
            var ratingId = notification.ContentRatingId;
            var newScore = notification.NewRaterScore.Value;

            await notificationService.NotifyRatingChanged(roomId, raterId, ratingId, newScore);
        }
    }
}
