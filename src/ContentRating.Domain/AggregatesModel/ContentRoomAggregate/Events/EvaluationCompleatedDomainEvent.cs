﻿using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomAggregate.Events
{
    public record EvaluationCompleatedDomainEvent(Guid RoomId, IReadOnlyCollection<User> InvitedUsers): INotification;
}
