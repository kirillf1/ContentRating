using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events;
using ContentRating.Domain.Shared.RoomStates;

namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate
{
    public class ContentPartyEstimationRoom : Entity, IAggregateRoot
    {
        public RoomControlSpecification RoomSpecification { get; private set; }
        public RatingRange RatingRange { get; private set; }
        public Rater RoomCreator { get; }
        public IReadOnlyCollection<Rater> Raters
        {
            get { return _raters; }
            private set { _raters = value.ToList(); }
        }
        public IReadOnlyCollection<ContentForEstimation> ContentForEstimation
        {
            get { return _contentList; }
            private set {_contentList = value.ToList(); }
        }
        private List<Rater> _raters;
        private List<ContentForEstimation> _contentList;
        public bool IsAllContentEstimated { get; private set; }
        public void KickRater(Guid raterForKickId, Guid kickInitiatorId)
        {
            if (IsAllContentEstimated)
                throw new InvalidRoomStageOperationException("Сan't kick rater when all content estimated");

            var initiator = _raters.Find(c => c.Id == kickInitiatorId);
            var raterForKick = _raters.Find(c => c.Id == raterForKickId);
            if (initiator is null || raterForKick is null)
            {
                throw new ArgumentException("Room don't contain this rater");
            }
            if (!RoomSpecification.CanKickAnotherUser(initiator))
                throw new ForbiddenRoomOperationException("This rater does not have the right to kick other users ");

            if (initiator == raterForKick)
                throw new ForbiddenRoomOperationException("The rater cannot kick himself");

            if (raterForKick == RoomCreator)
                throw new ForbiddenRoomOperationException("Cannot kick room creator");

            _raters.Remove(raterForKick);
            AddDomainEvent(new RaterKickedDomainEvent(raterForKick, Id));

        }
        public void InviteRater(Rater newRater, Guid inviterId)
        {
            if (IsAllContentEstimated)
                throw new InvalidRoomStageOperationException("Сan't invite rater when the room is not working");

            if (_raters.Contains(newRater))
            {
                throw new ArgumentException("This rater is already invited");
            }

            var inviter = _raters.Find(c => c.Id == inviterId) ?? throw new ArgumentException("Unknown inviter");

            if (!RoomSpecification.CanInviteAnotherUser(inviter))
                throw new ForbiddenRoomOperationException("This rater does not have the access to kick other raters");

            _raters.Add(newRater);

            AddDomainEvent(new RaterInvitedDomainEvent(newRater, Id));
        }
        public void RemoveUnavailableContent(Guid removeContentInitiatorId, Guid contentId)
        {
            if (IsAllContentEstimated)
                throw new InvalidRoomStageOperationException("Сan't remove content when the room is not working");

            var rater = _raters.Find(c => c.Id == removeContentInitiatorId) ?? throw new ArgumentException("Unknown rater");

            if (!RoomSpecification.CanEditContentList(rater))
                throw new ForbiddenRoomOperationException("This rater does not have the access to remove content");
            var content = _contentList.Find(c=> c.Id == contentId);
            if (content is null)
                return;

            AddDomainEvent(new UnavailableContentRemovedDomainEvent(Id, content, rater));

        }
        public void ChangeRatingRange(Rater ratingRangeChanger, Rating minRating, Rating maxRating)
        {
            if (ratingRangeChanger != RoomCreator)
                throw new ForbiddenRoomOperationException("Only creator can change rating range");

            var oldRatingRange = RatingRange;
            RatingRange = new RatingRange(maxRating, minRating);

            if (oldRatingRange != RatingRange)
                AddDomainEvent(new RatingRangeChangedDomainEvent(Id, RatingRange.MaxRating, RatingRange.MinRating));
        }
        public void CompleteContentEstimation()
        {
            IsAllContentEstimated = true;
            AddDomainEvent(new AllContentEstimatedDomainEvent(Id, Raters));
        }

        private ContentPartyEstimationRoom(Guid id, Rater creator, RoomControlSpecification specification, 
            List<Rater> invitedUsersWithCreator, IEnumerable<ContentForEstimation> contentList, RatingRange ratingRange)
        {
            Id = id;
            if (!invitedUsersWithCreator.Contains(creator))
                invitedUsersWithCreator.Add(creator);
            RoomCreator = creator;
            IsAllContentEstimated = false;
            _raters = invitedUsersWithCreator;
            RatingRange = ratingRange;
            RoomSpecification = specification;
            _contentList = new(contentList);

        }
        public static ContentPartyEstimationRoom Create(Guid id, Rater creator, IEnumerable<ContentForEstimation> contentList, 
            RatingRange? ratingRange = null, List<Rater>? otherInvitedUsers = null)
        {
            otherInvitedUsers ??= new List<Rater>();
            ratingRange ??= new RatingRange();
            otherInvitedUsers.Add(creator);
            return new ContentPartyEstimationRoom(id, creator, new RoomControlSpecification(), otherInvitedUsers, contentList, ratingRange);
        }
    }
}
