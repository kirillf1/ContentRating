using ContentRating.Domain.Shared;

namespace ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate;

public class User : Entity
{
    public User(Guid id, string name, RoleType role)
    {
        Id = id;
        Name = name;
        Role = role;
        IsConnected = false;
    }

    public string Name { get; private set; }
    public RoleType Role { get; private set; }
    public bool IsConnected { get; private set; }

}
