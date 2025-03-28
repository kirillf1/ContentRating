﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRatingAPI.Application.ContentPartyRating.ContentRaterService;
using ContentRatingAPI.Application.Notifications.IContentPartyEstimationNotifications;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.KickRater
{
    public class RaterKickedDomainEventHandler : INotificationHandler<RaterKickedDomainEvent>
    {
        private readonly ContentPartyRatingService contentPartyRatingService;
        private readonly IContentPartyEstimationNotificationService notificationService;

        public RaterKickedDomainEventHandler(
            ContentPartyRatingService contentPartyRatingService,
            IContentPartyEstimationNotificationService notificationService
        )
        {
            this.contentPartyRatingService = contentPartyRatingService;
            this.notificationService = notificationService;
        }

        public async Task Handle(RaterKickedDomainEvent notification, CancellationToken cancellationToken)
        {
            var kickedRater = notification.KickedRater.MapToContentRater();
            await contentPartyRatingService.RemoveRaterScoreInContentRatingList(notification.RoomId, kickedRater);

            await notificationService.NotifyRaterKicked(notification.RoomId, notification.KickedRater.Id);
        }
    }
}
