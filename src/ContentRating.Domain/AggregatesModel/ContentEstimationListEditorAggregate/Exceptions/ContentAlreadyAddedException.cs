using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;

namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Exceptions
{
    public class ContentAlreadyAddedException(string? message, Content foundContent) : Exception(message)
    {
        public Content FoundContent { get; } = foundContent;
    }
}
