using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;

namespace ContentRatingAPI.Application.ContentRoomEditor.KickEditor
{
    public class KickEditorCommandHandler : IRequestHandler<KickEditorCommand, Result>
    {
        private readonly IContentEditorRoomRepository contentEditorRoomRepository;

        public KickEditorCommandHandler(IContentEditorRoomRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task<Result> Handle(KickEditorCommand request, CancellationToken cancellationToken)
        {
            var room = await contentEditorRoomRepository.GetRoom(request.RoomId);
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
