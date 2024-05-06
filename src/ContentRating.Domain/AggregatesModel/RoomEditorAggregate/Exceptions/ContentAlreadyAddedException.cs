namespace ContentRating.Domain.AggregatesModel.RoomEditorAggregate.Exceptions
{
    public class ContentAlreadyAddedException(string? message, Content foundContent) : Exception(message)
    {
        public Content FoundContent { get; } = foundContent;
    }
}
