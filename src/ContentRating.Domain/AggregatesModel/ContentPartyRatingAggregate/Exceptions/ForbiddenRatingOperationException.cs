namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.Exceptions
{
    public class ForbiddenRatingOperationException(string message) : Exception(message)
    {
    }
}
