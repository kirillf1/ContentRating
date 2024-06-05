
namespace ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate
{
    public class UserAccessInformation : ValueObject
    {
        public UserAccessInformation(bool canEditContent, bool canRate, bool canInviteUsers, bool canKickUsers, User? userInformation = null)
        {
            CanViewRoom = canEditContent;
            CanRate = canRate;
            CanInviteUsers = canInviteUsers;
            CanKickUsers = canKickUsers;
            UserInformation = userInformation;
        }

        public bool CanViewRoom { get; }
        public bool CanRate { get; }
        public bool CanInviteUsers { get; }
        public bool CanKickUsers { get; }
        public User? UserInformation { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CanViewRoom;
            yield return UserInformation?.Id ?? default;
            yield return CanKickUsers;
            yield return CanInviteUsers;
            yield return CanViewRoom;
        }
    }
}
