﻿using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using ContentEditorRoomAggregate = ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.ContentRoomEditor;

namespace ContentRatingAPI.Application.ContentRoomEditor.CreateContentEditorRoom
{
    public class CreateContentEditorRoomCommandHandler : IRequestHandler<CreateContentEditorRoomCommand>
    {
        private readonly IContentEditorRoomRepository contentEditorRoomRepository;

        public CreateContentEditorRoomCommandHandler(IContentEditorRoomRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task Handle(CreateContentEditorRoomCommand request, CancellationToken cancellationToken)
        {
            var roomCreator = new Editor(request.CreatorId, request.CreatorName);
            var newRoom = new ContentEditorRoomAggregate(request.Id, roomCreator, request.RoomName);

            contentEditorRoomRepository.Add(newRoom);
            await contentEditorRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}