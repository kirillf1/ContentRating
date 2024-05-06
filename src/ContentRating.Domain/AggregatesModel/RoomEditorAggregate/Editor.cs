using ContentRating.Domain.Shared;

namespace ContentRating.Domain.AggregatesModel.RoomEditorAggregate
{
    public class Editor : Entity
    {
        public Editor(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Name { get; }
    }
}
