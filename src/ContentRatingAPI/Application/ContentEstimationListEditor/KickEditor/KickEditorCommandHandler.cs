using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;


namespace ContentRatingAPI.Application.ContentEstimationListEditor.KickEditor
{
    public class KickEditorCommandHandler : IRequestHandler<KickEditorCommand, Result<bool>>
    {
        private readonly IContentEstimationListEditorRepository contentEditorRoomRepository;

        public KickEditorCommandHandler(IContentEstimationListEditorRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task<Result<bool>> Handle(KickEditorCommand request, CancellationToken cancellationToken)
        {
            var room = await contentEditorRoomRepository.GetContentEstimationListEditor(request.RoomId);
            if (room is null)
                return Result.NotFound();

            var kickInitiator = room.TryGetEditorFromRoom(request.InitiatorId);
            if (kickInitiator is null)
                return Result.NotFound("Editor");

            var editorForKick = room.TryGetEditorFromRoom(request.TargetEditorId);
            if (editorForKick is null)
                return Result.NotFound("Editor");
            room.KickEditor(kickInitiator, editorForKick);

            contentEditorRoomRepository.Update(room);
            await contentEditorRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }
    }
}
