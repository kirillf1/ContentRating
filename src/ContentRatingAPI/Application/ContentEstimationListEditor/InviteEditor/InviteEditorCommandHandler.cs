using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;


namespace ContentRatingAPI.Application.ContentEstimationListEditor.InviteEditor
{
    public class InviteEditorCommandHandler : IRequestHandler<InviteEditorCommand, Result>
    {
        private readonly IContentEstimationListEditorRepository contentEditorRoomRepository;

        public InviteEditorCommandHandler(IContentEstimationListEditorRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task<Result> Handle(InviteEditorCommand request, CancellationToken cancellationToken)
        {
            var room = await contentEditorRoomRepository.GetContentEstimationListEditor(request.RoomId);
            if (room is null)
                return Result.NotFound();

            var inviteInitiator = room.TryGetEditorFromRoom(request.InitiatorId);
            if (inviteInitiator is null)
                return Result.NotFound();

            var newEditor = new ContentEditor(request.NewEditorId, request.NewEditorName);
            room.InviteEditor(inviteInitiator, newEditor);

            contentEditorRoomRepository.Update(room);
            await contentEditorRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
