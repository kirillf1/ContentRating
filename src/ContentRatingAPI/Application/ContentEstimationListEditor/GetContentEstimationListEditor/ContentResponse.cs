using ContentRating.Domain.Shared.Content;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.GetContentEstimationListEditor
{
    public class ContentResponse
    {
        public ContentResponse(Guid id, string path, string name, ContentType contentType, Guid creatorId, DateTime lastModificationDate)
        {
            Id = id;
            Path = path;
            Name = name;
            ContentType = contentType;
            CreatorId = creatorId;
            LastModificationDate = lastModificationDate;
        }

        public Guid Id { get; }
        public string Path { get; }
        public string Name { get; }
        public ContentType ContentType { get; }
        public Guid CreatorId { get; }
        public DateTime LastModificationDate { get; }
    }
}
