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
        public void AddNewRater(Guid raterId, RaterType raterType)
        {
            CheckCanCangeRating();
            if (_raters.Find(c=> raterId == c.Id) is not null)
                return;
            var rater = new Rater(raterId, raterType, _specification.MinScore);
            _raters.Add(rater);
        }
        public void RemoveRater(Rater oldRater)
        {
            CheckCanCangeRating();
            if (_raters.Contains(oldRater))
                return;

            _raters.Remove(oldRater);
        }
        public void EstimateContent(Estimation estimation)
        {
            CheckCanCangeRating();

            if (!_specification.IsSatisfiedRaters(estimation.Initiator, estimation.CurrentRater))
                throw new ForbiddenRatingOperationException("Invalid raters access");

            estimation.CurrentRater.Rate(estimation.NewScore, _specification);
            AddDomainEvent(new ContentRatingChangedDomainEvent(RoomId, Id, estimation.CurrentRater, estimation.NewScore));
        }
        public void CompleteEstimation()
        {
            IsContentEstimated = true;
        }
        private void CheckCanCangeRating()
        {
            if (IsContentEstimated)
                throw new ForbiddenRatingOperationException("Content estimated");
        }
        private ContentRating(Guid contentId, Guid roomId, IEnumerable<Rater> raters, ContentRatingSpecification specification)
        {
            Id = contentId;
            RoomId = roomId;
            _specification = specification;
            _raters = new(raters);
            IsContentEstimated = false;
        }
        public static ContentRating Create(Guid contentId, Guid roomId, IEnumerable<Rater> raters, ContentRatingSpecification specification)
        {
            return new ContentRating(contentId, roomId, raters, specification);
        }
    }
}
