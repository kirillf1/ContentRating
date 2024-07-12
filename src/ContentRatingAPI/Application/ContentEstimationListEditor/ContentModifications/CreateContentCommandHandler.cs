using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;


namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public class CreateContentCommandHandler : IRequestHandler<CreateContentCommand, Result>
    {
        private readonly IContentEstimationListEditorRepository contentEditorRoomRepository;

        public CreateContentCommandHandler(IContentEstimationListEditorRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task<Result> Handle(CreateContentCommand request, CancellationToken cancellationToken)
        {
            var room = await contentEditorRoomRepository.GetContentEstimationListEditor(request.RoomId);
            if (room is null)
                return Result.NotFound();

            var newContentData = new ContentData(request.ContentId, request.Name, request.Path, request.ContentType);
            var editor = room.TryGetEditorFromRoom(request.EditorId) ?? throw new ArgumentException("Unknown editor");
            room.CreateContent(editor, newContentData);

            contentEditorRoomRepository.Update(room);
            await contentEditorRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
