using ContentRating.Domain.Shared;

namespace ContentRating.Domain.AggregatesModel.ContentRoomAggregate
{
    public class ContentModification : ValueObject
    {
        public ContentModification(string newName, string newPath, ContentType newContentType)
        {
            NewName = newName;
            NewPath = newPath;
            NewContentType = newContentType;
        }

        public string NewName { get; }
        public string NewPath { get; }
        public ContentType NewContentType { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return NewName;
            yield return NewPath;
            yield return NewContentType;
        }
    }
}
