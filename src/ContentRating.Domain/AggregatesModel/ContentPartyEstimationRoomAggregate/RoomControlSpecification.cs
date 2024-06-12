namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate
{
    public class RoomControlSpecification
    {
        public RoomControlSpecification()
        {
            _canControlAnotherRaterRoles = [RoleType.Admin];
           _canEditContentListRoles = [RoleType.Admin];
        }
        public IReadOnlyCollection<RoleType> EditContentRoles
        {
            get => _canEditContentListRoles;
            set { _canEditContentListRoles = value.ToList(); }
        }
        public IReadOnlyCollection<RoleType> ControlAnotherRaterRoles
        {
            get => _canControlAnotherRaterRoles;
            set { _canControlAnotherRaterRoles = value.ToList(); }
        }
        private List<RoleType> _canEditContentListRoles;
        private List<RoleType> _canControlAnotherRaterRoles;
        public bool CanKickAnotherRater(Rater rater)
        {
            return _canControlAnotherRaterRoles.Contains(rater.Role);
        }
        public bool CanInviteAnotherRater(Rater rater)
        {
            return _canControlAnotherRaterRoles.Contains(rater.Role);
        }
        public bool CanEditContentList(Rater rater)
        {
            return _canEditContentListRoles.Contains(rater.Role);
        }
    }
}
