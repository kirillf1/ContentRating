
using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;

namespace ContentRatingAPI.Application.ContentRoomEditor.InviteEditor
{
    public class InviteEditorCommandHandler : IRequestHandler<InviteEditorCommand>
    {
        private readonly IContentEditorRoomRepository contentEditorRoomRepository;

        public InviteEditorCommandHandler(IContentEditorRoomRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task Handle(InviteEditorCommand request, CancellationToken cancellationToken)
        {
            var room = await contentEditorRoomRepository.GetRoom(request.RoomId);
            var inviteInitiator = room.TryGetEditorFromRoom(request.InitiatorId) ?? throw new ArgumentException("Unknown editor");
            var newEditor = new Editor(request.NewEditorId, request.NewEditorName);
            room.InviteEditor(inviteInitiator, newEditor);

            contentEditorRoomRepository.Update(room);
            await contentEditorRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
