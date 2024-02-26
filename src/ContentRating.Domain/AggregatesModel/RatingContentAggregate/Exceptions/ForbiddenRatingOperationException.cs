namespace ContentRating.Domain.AggregatesModel.RatingContentAggregate.Exceptions
{
    public class ForbiddenRatingOperationException(string message) : Exception(message)
    {
    }
}
