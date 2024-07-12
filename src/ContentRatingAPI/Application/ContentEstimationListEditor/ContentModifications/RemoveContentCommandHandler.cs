using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;


namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public class RemoveContentCommandHandler : IRequestHandler<RemoveContentCommand, Result>
    {
        private readonly IContentEstimationListEditorRepository contentEditorRoomRepository;

        public RemoveContentCommandHandler(IContentEstimationListEditorRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task<Result> Handle(RemoveContentCommand request, CancellationToken cancellationToken)
        {
            var room = await contentEditorRoomRepository.GetContentEstimationListEditor(request.RoomId);
            if (room is null)
                return Result.NotFound();

            var editor = room.TryGetEditorFromRoom(request.EditorId);
            if (editor is null)
                return Result.NotFound("Editor");

            var contentForDelete = room.AddedContent.FirstOrDefault(c => c.Id == request.ContentId) ?? throw new ArgumentException("Unknown content");
            room.RemoveContent(editor, contentForDelete);

            contentEditorRoomRepository.Update(room);
            await contentEditorRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
