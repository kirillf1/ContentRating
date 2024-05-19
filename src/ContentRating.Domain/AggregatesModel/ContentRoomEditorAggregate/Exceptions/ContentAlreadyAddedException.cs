namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Exceptions
{
    public class ContentAlreadyAddedException(string? message, Content foundContent) : Exception(message)
    {
        public Content FoundContent { get; } = foundContent;
    }
}
