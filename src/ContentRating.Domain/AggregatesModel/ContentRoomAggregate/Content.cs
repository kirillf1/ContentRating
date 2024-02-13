using ContentRating.Domain.Shared;

namespace ContentRating.Domain.AggregatesModel.ContentRoomAggregate
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

        public string Path { get; }
        public string Name { get; }
        public ContentType Type { get; }
        public override bool Equals(object obj)
        {
            if (obj == null || obj is not Content)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            Content item = (Content)obj;

            return item.Id == this.Id
                && (item.Path.Equals(Path, StringComparison.OrdinalIgnoreCase) || item.Name.Equals(Name, StringComparison.OrdinalIgnoreCase));
                
        }
    }
}
