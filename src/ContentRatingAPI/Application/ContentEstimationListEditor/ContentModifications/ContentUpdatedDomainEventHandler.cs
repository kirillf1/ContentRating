// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events;
using ContentRatingAPI.Application.Notifications.IContentEstimationListEditorNotifications;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public class ContentUpdatedDomainEventHandler : INotificationHandler<ContentUpdatedDomainEvent>
    {
        private readonly IContentEstimationListEditorNotificationService notificationService;

        public ContentUpdatedDomainEventHandler(IContentEstimationListEditorNotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public async Task Handle(ContentUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var contentInfo = new ContentNotificationInformation(
                notification.UpdatedContent.Id,
                notification.UpdatedContent.Name,
                notification.UpdatedContent.Path,
                notification.UpdatedContent.Type,
                notification.UpdatedContent.ContentModificationHistory.LastContentModifiedDate
            );

            await notificationService.NotifyContentUpdated(
                notification.ContentListId,
                notification.UpdatedContent.ContentModificationHistory.EditorId,
                contentInfo
            );
        }
    }
}
