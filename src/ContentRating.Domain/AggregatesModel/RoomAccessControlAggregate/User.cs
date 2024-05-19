using ContentRating.Domain.Shared;

namespace ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate;

public class User : Entity
{
    public User(Guid id, RoleType role)
    {
        Id = id;
        Role = role;
    }
    public RoleType Role { get; private set; }

}
