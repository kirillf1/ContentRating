using ContentRating.Domain.AggregatesModel.RatingContentAggregate.Events;
using ContentRating.Domain.AggregatesModel.RatingContentAggregate.Exceptions;
using ContentRating.Domain.Shared;

namespace ContentRating.Domain.AggregatesModel.RatingContentAggregate
{
    public class RatingContent : Entity, IAggregateRoot
    {
        public RatingContent(Guid contentId, Guid roomId, IEnumerable<Rater> raters)
        {
            Id = contentId;
            RoomId = roomId;
            _raters = new(raters);
        }

        public Guid RoomId { get; private set; }
        public IReadOnlyCollection<Rater> Raters => _raters;
        private List<Rater> _raters;
        public Score AverageContentScore => new(_raters.Average(c => c.Score.Value));
        public void AddRater(Rater newRater) 
        {
            if (_raters.Contains(newRater))
                return;

            _raters.Add(newRater);
        }
        public void RemoveRater(Rater oldRater)
        {
            if (!_raters.Contains(oldRater))
                return;

            _raters.Remove(oldRater);
        }
        public void RateContent(Rater raterInitiatior, Rater targetRater, Score score)
        {
            if (!_raters.Contains(raterInitiatior) || !_raters.Contains(targetRater))
            {
                throw new ForbiddenRatingOperationException("This rater can't rate this content");
            }
            if (raterInitiatior.RaterType == RaterType.Mock)
            {
                throw new ForbiddenRatingOperationException("Mock rater can't itself change content");
            }
            if (raterInitiatior.RaterType != RaterType.Owner && targetRater.RaterType != RaterType.Mock)
            {
                throw new ForbiddenRatingOperationException("Default rater can change score only for mock rater");
            }

            targetRater.ChangeCurrentScore(score);

            AddDomainEvent(new ContentRatingChangedDomainEvent(RoomId, Id, targetRater, AverageContentScore));
        }
    }
}
