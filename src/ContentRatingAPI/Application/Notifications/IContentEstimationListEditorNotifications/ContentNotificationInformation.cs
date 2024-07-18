using ContentRating.Domain.Shared.Content;

namespace ContentRatingAPI.Application.Notifications.IContentEstimationListEditorNotifications
{
    public class ContentNotificationInformation
    {
        public ContentNotificationInformation(Guid id, string name, string path, ContentType contentType, DateTime lastModificationDate)
        {
            Id = id;
            Name = name;
            Path = path;
            ContentType = contentType;
            LastModificationDate = lastModificationDate;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string Path { get; }
        public ContentType ContentType { get; }
        public DateTime LastModificationDate { get; }
    }
}
