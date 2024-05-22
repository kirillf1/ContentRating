namespace ContentRating.Domain.AggregatesModel.ContentRatingAggregate.Exceptions
{
    public class ForbiddenRatingOperationException(string message) : Exception(message)
    {
    }
}
