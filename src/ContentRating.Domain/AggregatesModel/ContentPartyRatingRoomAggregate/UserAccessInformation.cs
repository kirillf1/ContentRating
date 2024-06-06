namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingRoomAggregate
{
    public class UserAccessInformation : ValueObject
    {
        public UserAccessInformation(bool canEditContent, bool canRate, bool canInviteUsers, bool canKickUsers, Rater? userInformation = null)
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
        public Rater? UserInformation { get; }

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
