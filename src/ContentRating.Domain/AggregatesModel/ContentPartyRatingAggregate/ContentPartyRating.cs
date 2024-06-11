using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.Exceptions;
using System.Collections.ObjectModel;

namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate
{
    public class ContentPartyRating : Entity, IAggregateRoot
    {
        public Guid RoomId { get; private set; }
        public int RatersCount => _raterScores.Count;
        public ReadOnlyDictionary<Guid, Score> RaterScores
        {
            get { return _raterScores.AsReadOnly(); }
            private set { _raterScores = value.ToDictionary(); }
        }
        public bool IsContentEstimated { get; private set; }
        public Score AverageContentScore => new(_raterScores.Values.Average(c => c.Value));

        public ContentRatingSpecification Specification { get; private set; }
        private Dictionary<Guid, Score> _raterScores;
        public void AddNewRaterInContentEstimation(ContentRater contentRater)
        {
            CheckEstimationIsCompleted();
            var raterId = contentRater.RaterId;
            if (_raterScores.ContainsKey(raterId))
                throw new ArgumentException("Rater is already added");
            _raterScores[raterId] = Specification.MinScore;
        }
        public void RemoveRaterFromContentEstimation(ContentRater contentRater)
        {
            CheckEstimationIsCompleted();
            
            _raterScores.Remove(contentRater.RaterId);
        }
        public void ChangeEstimationSpecification(ContentRatingSpecification newSpecification)
        {
            Specification = newSpecification;
            var newMaxScore = newSpecification.MaxScore;
            var newMinScore = newSpecification.MinScore;
            foreach (var raterId in _raterScores.Keys)
            {
                var score = _raterScores[raterId];
                if (score > newMaxScore)
                    _raterScores[raterId] = newMaxScore;
                else if (score < newMinScore)
                    _raterScores[raterId] = newMinScore;
            }
        }
        public void EstimateContent(Estimation estimation)
        {
            CheckEstimationIsCompleted();

            if (!_raterScores.ContainsKey(estimation.RaterForChangeScore.RaterId) || !_raterScores.ContainsKey(estimation.ContentEstimationInitiator.RaterId))
                throw new ForbiddenRatingOperationException("Unknown raters");

            if (!Specification.HasAccessToEstimateContent(estimation.ContentEstimationInitiator, estimation.RaterForChangeScore))
                throw new ForbiddenRatingOperationException("Invalid raters access");
            
            if (!Specification.IsSatisfiedScore(estimation.NewScore))
                throw new ForbiddenRatingOperationException("Invalid score value");

            AddDomainEvent(new ContentRatingChangedDomainEvent(RoomId, Id, estimation.RaterForChangeScore, estimation.NewScore, AverageContentScore));
        }
        public void CompleteEstimation()
        {
            IsContentEstimated = true;
        }
        private void CheckEstimationIsCompleted()
        {
            if (IsContentEstimated)
                throw new ForbiddenRatingOperationException("Content estimated");
        }
        private ContentPartyRating(Guid contentId, Guid roomId, ContentRatingSpecification specification)
        {
            Id = contentId;
            RoomId = roomId;
            Specification = specification;
            _raterScores = [];
            IsContentEstimated = false;
        }
        public static ContentPartyRating Create(Guid contentId, Guid roomId, ContentRatingSpecification specification)
        {
            return new ContentPartyRating(contentId, roomId, specification);
        }
    }
}
