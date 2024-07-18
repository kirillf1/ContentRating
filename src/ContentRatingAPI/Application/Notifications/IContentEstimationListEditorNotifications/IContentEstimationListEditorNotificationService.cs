namespace ContentRatingAPI.Application.Notifications.IContentEstimationListEditorNotifications
{
    public interface IContentEstimationListEditorNotificationService
    {
        public Task NotifyContentCreated(Guid contentListId, Guid editorId, ContentNotificationInformation contentNotification);
        public Task NotifyContentUpdated(Guid contentListId, Guid editorId, ContentNotificationInformation contentNotification);
        public Task NotifyContentDeleted(Guid contentListId, Guid editorId, Guid deletedContentId);
        public Task NotifyEditorKicked(Guid contentListId, Guid kickedEditorId);
        public Task NotifyEditorInvited(Guid contentListId, Guid newEditorId, string editorName);

    }
}
