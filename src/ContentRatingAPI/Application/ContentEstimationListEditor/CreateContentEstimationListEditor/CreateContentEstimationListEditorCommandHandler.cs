using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;
using ContentEstimationListEditorAggregate = ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.ContentEstimationListEditor;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.CreateContentEstimationListEditor
{
    public class CreateContentEstimationListEditorCommandHandler : IRequestHandler<CreateContentEstimationListEditorCommand, Result<bool>>
    {
        private readonly IContentEstimationListEditorRepository contentEditorRoomRepository;

        public CreateContentEstimationListEditorCommandHandler(IContentEstimationListEditorRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task<Result<bool>> Handle(CreateContentEstimationListEditorCommand request, CancellationToken cancellationToken)
        {
            var roomCreator = new ContentEditor(request.CreatorId, request.CreatorName);
            var newRoom = ContentEstimationListEditorAggregate.Create(request.Id, roomCreator, request.RoomName);

            contentEditorRoomRepository.Add(newRoom);
            await contentEditorRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(true);

        }
    }
}
