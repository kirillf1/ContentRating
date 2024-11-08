// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events;
using ContentRatingAPI.Application.Notifications.IContentEstimationListEditorNotifications;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.InviteEditor
{
    public class EditorInvitedDomainEventHandler : INotificationHandler<EditorInvitedDomainEvent>
    {
        private readonly IContentEstimationListEditorNotificationService notificationService;

        public EditorInvitedDomainEventHandler(IContentEstimationListEditorNotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public async Task Handle(EditorInvitedDomainEvent notification, CancellationToken cancellationToken)
        {
            await notificationService.NotifyEditorInvited(
                notification.ContentListId,
                notification.NewEditor.Id,
                notification.NewEditor.Name,
                notification.Inviter.Id
            );
        }
    }
}
