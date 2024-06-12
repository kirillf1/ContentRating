using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;

namespace ContentRatingAPI.Application.ContentRoomEditor.KickEditor
{
    public class KickEditorCommandHandler : IRequestHandler<KickEditorCommand>
    {
        private readonly IContentEditorRoomRepository contentEditorRoomRepository;

        public KickEditorCommandHandler(IContentEditorRoomRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task Handle(KickEditorCommand request, CancellationToken cancellationToken)
        {
            var room = await contentEditorRoomRepository.GetRoom(request.RoomId);
            var kickInitiator = room.TryGetEditorFromRoom(request.InitiatorId) ?? throw new ArgumentException("Unknown editor");
            var editorForKick = room.TryGetEditorFromRoom(request.TargetEditorId) ?? throw new ArgumentException("Unknown editor for kick");
            room.KickEditor(kickInitiator, editorForKick);

            contentEditorRoomRepository.Update(room);
            await contentEditorRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
