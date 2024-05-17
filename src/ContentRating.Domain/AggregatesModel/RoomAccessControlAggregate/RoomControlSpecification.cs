namespace ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate
{
    public class RoomControlSpecification
    {
        public RoomControlSpecification()
        {
            _canKickRoles = new List<RoleType>() { RoleType.Admin };
        }
        private List<RoleType> _canKickRoles;
        public bool CanKickAnotherUser(User user)
        {
            return _canKickRoles.Contains(user.Role);
        }
        public bool RoomIsWorking(RoomAccessControl room)
        {
            return room.RoomState == Shared.RoomStates.RoomState.ContentEvaluation;
        }
    }
}
