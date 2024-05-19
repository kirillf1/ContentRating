﻿using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events
{
    public record ContentAddedToRoomDomainEvent(Content NewContent, Guid RoomId) : INotification;

}