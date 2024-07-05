using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;

namespace ContentRatingAPI.Application.ContentRoomEditor.ContentModifications
{
    public class UpdateContentCommandHandler : IRequestHandler<UpdateContentCommand, Result>
    {
        private readonly IContentEditorRoomRepository contentEditorRoomRepository;

        public UpdateContentCommandHandler(IContentEditorRoomRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task<Result> Handle(UpdateContentCommand request, CancellationToken cancellationToken)
        {
            var room = await contentEditorRoomRepository.GetRoom(request.RoomId);
            if (room is null)
                return Result.NotFound();

            var newContentData = new ContentData(request.Id, request.Name, request.Path, request.ContentType);
            var editor = room.TryGetEditorFromRoom(request.EditorId);
            if (editor is null)
                return Result.NotFound("Editor");

            room.UpdateContent(editor, newContentData);

            contentEditorRoomRepository.Update(room);
            await contentEditorRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
