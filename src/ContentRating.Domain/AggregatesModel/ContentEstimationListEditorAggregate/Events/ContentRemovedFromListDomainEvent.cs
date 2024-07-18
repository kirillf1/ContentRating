using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;

namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events
{
    public record ContentRemovedFromListDomainEvent(Content Content, Guid RoomId) : INotification;
}
