using ContentRating.Domain.AggregatesModel.ContentRatingAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentRatingAggregate.Exceptions;
using ContentRating.Domain.Shared;

namespace ContentRating.Domain.AggregatesModel.ContentRatingAggregate
{
    public class ContentRating : Entity, IAggregateRoot
    {
        public Guid RoomId { get; private set; }
        public IReadOnlyCollection<Rater> Raters => _raters;
        private ContentRatingSpecification _specification;
        private List<Rater> _raters;
        public bool IsContentEstimated { get; private set; }

        public Score AverageContentScore => new(_raters.Average(c => c.CurrentScore.Value));
        public Rater InviteRater(RaterInvitation invitation)
        {
            CheckCanChangeRating();
            if (_raters.Find(c => invitation.Id == c.Id) is not null)
                throw new ArgumentException("Rater is already added");
            var rater = new Rater(invitation.Id, invitation.RaterType, _specification.MinScore);
            _raters.Add(rater);
            return rater;
        }
        public void RemoveRater(Rater oldRater)
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
        private ContentRating(Guid contentId, Guid roomId, ContentRatingSpecification specification)
        {
            Id = contentId;
            RoomId = roomId;
            _specification = specification;
            _raters = new();
            IsContentEstimated = false;
        }
        public static ContentRating Create(Guid contentId, Guid roomId, ContentRatingSpecification specification)
        {
            return new ContentRating(contentId, roomId, specification);
        }
    }
}
