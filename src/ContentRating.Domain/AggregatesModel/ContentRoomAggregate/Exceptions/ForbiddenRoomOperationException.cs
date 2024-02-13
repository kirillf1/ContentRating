namespace ContentRating.Domain.AggregatesModel.ContentRoomAggregate.Exceptions
{
    public class ForbiddenRoomOperationException(string message) : Exception(message)
    {
    }
}
