
using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;

namespace ContentRatingAPI.Application.ContentRoomEditor.ContentModifications
{
    public class RemoveContentCommandHandler : IRequestHandler<RemoveContentCommand>
    {
        private readonly IContentEditorRoomRepository contentEditorRoomRepository;

        public RemoveContentCommandHandler(IContentEditorRoomRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task Handle(RemoveContentCommand request, CancellationToken cancellationToken)
        {
            var room = await contentEditorRoomRepository.GetRoom(request.RoomId);
            var editor = room.TryGetEditorFromRoom(request.EditorId) ?? throw new ArgumentException("Unknown editor");
            var contentForDelete = room.AddedContent.FirstOrDefault(c=> c.Id == request.ContentId) ?? throw new ArgumentException("Unknown content");
            room.RemoveContent(editor, contentForDelete);

            contentEditorRoomRepository.Update(room);
            await contentEditorRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
