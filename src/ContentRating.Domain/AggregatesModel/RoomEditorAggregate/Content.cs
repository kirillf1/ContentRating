using ContentRating.Domain.Shared;

namespace ContentRating.Domain.AggregatesModel.RoomEditorAggregate
{
    public class Content : Entity
    {
        public Content(Guid id, string path, string name, ContentType type)
        {
            Id = id;
            Path = path;
            Name = name;
            Type = type;
        }

        public string Path { get; private set; }
        public string Name { get; private set; }
        public ContentType Type { get; private set; }
        public void ModifyContent(ContentModification contentModification)
        {
            Path = contentModification.NewPath;
            Name = contentModification.NewName;
            Type = contentModification.NewContentType;
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

            return item.Id == Id
               || item.Path.Equals(Path, StringComparison.OrdinalIgnoreCase)
               || item.Name.Equals(Name, StringComparison.OrdinalIgnoreCase);

        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
