
using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;

namespace ContentRatingAPI.Application.ContentRoomEditor.InviteEditor
{
    public class InviteEditorCommandHandler : IRequestHandler<InviteEditorCommand, Result>
    {
        private readonly IContentEditorRoomRepository contentEditorRoomRepository;

        public InviteEditorCommandHandler(IContentEditorRoomRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task<Result> Handle(InviteEditorCommand request, CancellationToken cancellationToken)
        {
            var room = await contentEditorRoomRepository.GetRoom(request.RoomId);
            if (room is null)
                return Result.NotFound();

            var inviteInitiator = room.TryGetEditorFromRoom(request.InitiatorId);
            if (inviteInitiator is null)
                return Result.NotFound();

            var newEditor = new Editor(request.NewEditorId, request.NewEditorName);
            room.InviteEditor(inviteInitiator, newEditor);

            contentEditorRoomRepository.Update(room);
            await contentEditorRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
