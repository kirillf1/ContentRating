// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events;
using ContentRatingAPI.Application.Notifications.IContentEstimationListEditorNotifications;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public class ContentAddedToListDomainEventHandler : INotificationHandler<ContentAddedToListDomainEvent>
    {
        private readonly IContentEstimationListEditorNotificationService notificationService;

        public ContentAddedToListDomainEventHandler(IContentEstimationListEditorNotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public async Task Handle(ContentAddedToListDomainEvent notification, CancellationToken cancellationToken)
        {
            var contentInfo = new ContentNotificationInformation(
                notification.NewContent.Id,
                notification.NewContent.Name,
                notification.NewContent.Path,
                notification.NewContent.Type,
                notification.NewContent.ContentModificationHistory.LastContentModifiedDate
            );

            await notificationService.NotifyContentCreated(
                notification.ContentListId,
                notification.NewContent.ContentModificationHistory.EditorId,
                contentInfo
            );
        }
    }
}
