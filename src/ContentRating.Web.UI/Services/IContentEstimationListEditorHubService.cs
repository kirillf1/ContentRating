using ContentRating.Domain.Shared.Content;

namespace ContentRating.Web.UI.Services
{
    public interface IContentEstimationListEditorHubService
    {
        Task ConnectAsync(Guid roomId);
        Task DisconnectAsync();
        bool IsConnected { get; }
        
        // События для подписки
        event Action<Guid, ContentNotificationData>? ContentCreated;
        event Action<Guid, ContentNotificationData>? ContentUpdated;
        event Action<Guid>? ContentDeleted;
        event Action<Guid, string, Guid>? EditorInvited;
        event Action<Guid, Guid>? EditorKicked;
    }

    public class ContentNotificationData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public ContentType ContentType { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
} 