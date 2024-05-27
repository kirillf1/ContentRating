using ContentRating.Domain.Shared;

namespace ContentRating.Domain.AggregatesModel.ContentRatingAggregate
{
    public class RaterInvitation : ValueObject
    {
        public RaterInvitation(Guid id, RaterType raterType)
        {
            Id = id;
            RaterType = raterType;
        }

        public Guid Id { get; }
        public RaterType RaterType { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
            yield return RaterType;
        }
    }
}
