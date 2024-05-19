namespace ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate
{
    public class RoomControlSpecification
    {
        public RoomControlSpecification()
        {
            _canControlAnotherUserRoles = new List<RoleType>() { RoleType.Admin };
        }
        private List<RoleType> _canControlAnotherUserRoles;
        public bool CanKickAnotherUser(User user)
        {
            return _canControlAnotherUserRoles.Contains(user.Role);
        }
        public bool CanInviteAnotherUser(User user)
        {
            return _canControlAnotherUserRoles.Contains(user.Role);
        }
        public bool RoomIsWorking(RoomAccessControl room)
        {
            return room.RoomState == Shared.RoomStates.RoomState.ContentEvaluation;
        }
    }
}
