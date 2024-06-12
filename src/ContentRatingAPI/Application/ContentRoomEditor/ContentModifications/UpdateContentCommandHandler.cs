using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;

namespace ContentRatingAPI.Application.ContentRoomEditor.ContentModifications
{
    public class UpdateContentCommandHandler : IRequestHandler<UpdateContentCommand>
    {
        private readonly IContentEditorRoomRepository contentEditorRoomRepository;

        public UpdateContentCommandHandler(IContentEditorRoomRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task Handle(UpdateContentCommand request, CancellationToken cancellationToken)
        {
            var room = await contentEditorRoomRepository.GetRoom(request.RoomId);
            var newContentData = new ContentData(request.Id, request.Name, request.Path, request.ContentType);
            var editor = room.TryGetEditorFromRoom(request.EditorId) ?? throw new ArgumentException("Unknown editor");
            room.UpdateContent(editor, newContentData);

            contentEditorRoomRepository.Update(room);
            await contentEditorRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
