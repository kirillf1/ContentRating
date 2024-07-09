using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using ContentRating.Domain.Shared.RoomStates;
using ContentEditorRoomAggregate = ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.ContentRoomEditor;

namespace ContentRatingAPI.Application.ContentRoomEditor.CreateContentEditorRoom
{
    public class CreateContentEditorRoomCommandHandler : IRequestHandler<CreateContentEditorRoomCommand, Result>
    {
        private readonly IContentEditorRoomRepository contentEditorRoomRepository;

        public CreateContentEditorRoomCommandHandler(IContentEditorRoomRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task<Result> Handle(CreateContentEditorRoomCommand request, CancellationToken cancellationToken)
        {
            var roomCreator = new Editor(request.CreatorId, request.CreatorName);
            var newRoom = ContentEditorRoomAggregate.Create( request.Id, roomCreator, request.RoomName);

            contentEditorRoomRepository.Add(newRoom);
            await contentEditorRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
            
        }
    }
}
