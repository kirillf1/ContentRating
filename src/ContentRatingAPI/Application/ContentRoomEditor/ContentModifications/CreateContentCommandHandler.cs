
using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;

namespace ContentRatingAPI.Application.ContentRoomEditor.ContentModifications
{
    public class CreateContentCommandHandler : IRequestHandler<CreateContentCommand>
    {
        private readonly IContentEditorRoomRepository contentEditorRoomRepository;

        public CreateContentCommandHandler(IContentEditorRoomRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task Handle(CreateContentCommand request, CancellationToken cancellationToken)
        {
            var room = await contentEditorRoomRepository.GetRoom(request.RoomId);
            var newContentData = new ContentData(request.ContentId, request.Name, request.Path, request.ContentType);
            var editor = room.TryGetEditorFromRoom(request.EditorId) ?? throw new ArgumentException("Unknown editor");
            room.CreateContent(editor, newContentData);

            contentEditorRoomRepository.Update(room);
            await contentEditorRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
