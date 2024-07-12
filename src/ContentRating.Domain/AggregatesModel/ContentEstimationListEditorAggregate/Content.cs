using ContentRating.Domain.Shared.Content;

namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate
{
    public class Content : Entity
    {
        public Content(Guid id, string path, string name, ContentType type, ContentModificationHistory contentCreator)
        {
            Id = id;
            Path = path;
            Name = name;
            Type = type;
            ContentModificationHistory = contentCreator;
        }

        public string Path { get; private set; }
        public string Name { get; private set; }
        public ContentType Type { get; private set; }
        public ContentModificationHistory ContentModificationHistory { get; private set; }

        public void ModifyContent(ContentData contentModification)
        {
            Path = contentModification.Path;
            Name = contentModification.Name;
            Type = contentModification.Type;
            ContentModificationHistory = ContentModificationHistory.MarkContentModification();
        }
        public override bool Equals(object obj)
        {
            if (obj == null || obj is not Content)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                return false;

            Content item = (Content)obj;

            return item.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
