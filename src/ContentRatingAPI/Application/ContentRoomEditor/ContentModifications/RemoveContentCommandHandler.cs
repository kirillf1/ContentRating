
using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;

namespace ContentRatingAPI.Application.ContentRoomEditor.ContentModifications
{
    public class RemoveContentCommandHandler : IRequestHandler<RemoveContentCommand, Result>
    {
        private readonly IContentEditorRoomRepository contentEditorRoomRepository;

        public RemoveContentCommandHandler(IContentEditorRoomRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task<Result> Handle(RemoveContentCommand request, CancellationToken cancellationToken)
        {
            var room = await contentEditorRoomRepository.GetRoom(request.RoomId);
            if (room is null)
                return Result.NotFound();

            var editor = room.TryGetEditorFromRoom(request.EditorId);
            if (editor is null)
                return Result.NotFound("Editor");

            var contentForDelete = room.AddedContent.FirstOrDefault(c=> c.Id == request.ContentId) ?? throw new ArgumentException("Unknown content");
            room.RemoveContent(editor, contentForDelete);

            contentEditorRoomRepository.Update(room);
            await contentEditorRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
