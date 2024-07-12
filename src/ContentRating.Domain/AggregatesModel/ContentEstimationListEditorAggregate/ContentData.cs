using ContentRating.Domain.Shared.Content;

namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate
{
    public class ContentData : ValueObject
    {
        public ContentData(Guid id, string newName, string newPath, ContentType newContentType)
        {
            Id = id;
            Name = newName;
            Path = newPath;
            Type = newContentType;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string Path { get; }
        public ContentType Type { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Path;
            yield return Type;
        }
    }
}
