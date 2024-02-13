namespace ContentRating.Domain.AggregatesModel.ContentRoomAggregate.Exceptions
{
    public class ContentAlreadyAddedException(string? message, Content foundContent) : Exception(message)
    {
        public Content FoundContent { get; } = foundContent;
    }
}
