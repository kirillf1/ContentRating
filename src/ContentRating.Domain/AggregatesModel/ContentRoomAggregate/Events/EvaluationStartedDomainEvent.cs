using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomAggregate.Events
{
    public record EvaluationStartedDomainEvent(Guid RoomId) : INotification;
    
}
