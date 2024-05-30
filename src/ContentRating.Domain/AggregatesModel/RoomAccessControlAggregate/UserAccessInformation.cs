
namespace ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate
{
    public class UserAccessInformation : ValueObject
    {
        public UserAccessInformation(Guid userId, bool canEditContent, bool canRate, bool canInviteUsers, bool canKickUsers)
        {
            UserId = userId;
            CanViewRoom = canEditContent;
            CanRate = canRate;
            CanInviteUsers = canInviteUsers;
            CanKickUsers = canKickUsers;
        }

        public Guid UserId { get; }
        public bool CanViewRoom { get; }
        public bool CanRate { get; }
        public bool CanInviteUsers { get; }
        public bool CanKickUsers { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            throw new NotImplementedException();
        }
    }
}
