using ContentRatingAPI.Application.Notifications.IContentEstimationListEditorNotifications;
using Microsoft.AspNetCore.SignalR;

namespace ContentRatingAPI.Hubs.NotificationServices
{
    public class ContentEstimationListEditorNotificationHubService : IContentEstimationListEditorNotificationService
    {
        private readonly IHubContext<ContentEstimationListEditorHub> hubContext;

        public ContentEstimationListEditorNotificationHubService(IHubContext<ContentEstimationListEditorHub> hubContext)
        {
            this.hubContext = hubContext;
        }
        public async Task NotifyContentCreated(Guid contentListId, Guid editorId, ContentNotificationInformation contentNotification)
        {
           await hubContext.Clients.Group(contentListId.ToString()).SendAsync("ContentCreated", editorId, contentNotification);
        }

        public async Task NotifyContentDeleted(Guid contentListId, Guid deletedContentId)
        {
            await hubContext.Clients.Group(contentListId.ToString()).SendAsync("ContentDeleted", deletedContentId);
        }

        public async Task NotifyContentUpdated(Guid contentListId, Guid editorId, ContentNotificationInformation contentNotification)
        {
            await hubContext.Clients.Group(contentListId.ToString()).SendAsync("ContentUpdated", editorId, contentNotification);
        }

        public async Task NotifyEditorInvited(Guid contentListId, Guid newEditorId, string editorName, Guid inviterId)
        {
            await hubContext.Clients.Group(contentListId.ToString()).SendAsync("EditorInvited", newEditorId, editorName, inviterId);
        }

        public async Task NotifyEditorKicked(Guid contentListId, Guid kickedEditorId, Guid kickInitiatorId)
        {
            await hubContext.Clients.Group(contentListId.ToString()).SendAsync("EditorKicked", kickedEditorId, kickInitiatorId);
        }
    }
}
