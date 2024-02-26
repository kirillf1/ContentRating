using ContentRating.Domain.Shared;

namespace ContentRating.Domain.AggregatesModel.RatingContentAggregate
{
    public class Rater : Entity
    {
        public Rater(Guid id, RaterType raterType)
        {
            Id = id;
            Score = Score.DefaultScore;
            RaterType = raterType;
        }
        public Score Score { get; private set; }
        public RaterType RaterType { get; private set; }
        public void ChangeCurrentScore(Score newScore)
        {
            Score = newScore;
        }
    }
}
