namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;

public class Rater : Entity
{
    public Rater(Guid id, RoleType role, string name)
    {
        Id = id;
        Role = role;
        Name = name;
    }
    public RoleType Role { get; private set; }
    public string Name { get; }
}
