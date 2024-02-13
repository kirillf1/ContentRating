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
    }
}
