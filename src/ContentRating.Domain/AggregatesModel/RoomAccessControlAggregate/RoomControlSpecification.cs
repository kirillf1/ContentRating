using ContentRating.Domain.Shared.RoomStates;

namespace ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate
{
    public class RoomControlSpecification
    {
        public RoomControlSpecification()
        {
            _canControlAnotherUserRoles = [RoleType.Admin];
            _canEditContentRoles = [RoleType.Admin, RoleType.Editor];
        }
        private List<RoleType> _canControlAnotherUserRoles;
        private List<RoleType> _canEditContentRoles;
        public bool CanKickAnotherUser(User user)
        {
            return _canControlAnotherUserRoles.Contains(user.Role);
        }
        public bool CanInviteAnotherUser(User user)
        {
            return _canControlAnotherUserRoles.Contains(user.Role);
        }
        public bool CanEditContent(RoomAccessControl roomAccessControl, User user)
        {
            if (!roomAccessControl.Users.Contains(user))
                return false;
            if (roomAccessControl.RoomState == RoomState.EvaluationComplete)
                return false;
            if (user.Role == RoleType.Admin)
                return true;
            var roomState = roomAccessControl.RoomState;
            if (roomState != RoomState.Editing)
                return false;
            return _canEditContentRoles.Contains(user.Role);
        }
    }
}
