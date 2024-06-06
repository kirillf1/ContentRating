using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.Exceptions;

namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate
{
    public class ContentPartyRating : Entity, IAggregateRoot
    {
        public Guid RoomId { get; private set; }
        public IReadOnlyCollection<ContentRater> Raters => _raters;
        private ContentRatingSpecification _specification;
        private List<ContentRater> _raters;
        public bool IsContentEstimated { get; private set; }

        public Score AverageContentScore => new(_raters.Average(c => c.CurrentScore.Value));
        public ContentRater InviteRater(RaterInvitation invitation)
        {
            CheckCanChangeRating();
            if (_raters.Find(c => invitation.Id == c.Id) is not null)
                throw new ArgumentException("Rater is already added");
            var rater = new ContentRater(invitation.Id, invitation.RaterType, _specification.MinScore);
            _raters.Add(rater);
            return rater;
        }
        public void RemoveRater(ContentRater oldRater)
        {
            CheckCanChangeRating();
            if (!_raters.Contains(oldRater))
                return;

            _raters.Remove(oldRater);
        }
        public void EstimateContent(Estimation estimation)
        {
            CheckCanChangeRating();

            if (!_specification.IsSatisfiedRatersForContentEstimation(estimation.Initiator, estimation.CurrentRater))
                throw new ForbiddenRatingOperationException("Invalid raters access");

            estimation.CurrentRater.Rate(estimation.NewScore, _specification);
            AddDomainEvent(new ContentRatingChangedDomainEvent(RoomId, Id, estimation.CurrentRater, AverageContentScore));
        }
        public void CompleteEstimation()
        {
            IsContentEstimated = true;
        }
        private void CheckCanChangeRating()
        {
            if (IsContentEstimated)
                throw new ForbiddenRatingOperationException("Content estimated");
        }
        private ContentPartyRating(Guid contentId, Guid roomId, ContentRatingSpecification specification)
        {
            Id = contentId;
            RoomId = roomId;
            _specification = specification;
            _raters = new();
            IsContentEstimated = false;
        }
        public static ContentPartyRating Create(Guid contentId, Guid roomId, ContentRatingSpecification specification)
        {
            return new ContentPartyRating(contentId, roomId, specification);
        }
    }
}
