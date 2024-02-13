using ContentRating.Domain.Shared;

namespace ContentRating.Domain.AggregatesModel.ContentRoomAggregate
{
    public class User : Entity
    {
        public User(Guid id, string name, bool isMock)
        {
            Id = id;
            Name = name;
            IsMock = isMock;
        }

        public string Name { get; }
        public bool IsMock { get; }
    }
}
