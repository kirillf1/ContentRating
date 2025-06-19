using Ardalis.Result;
using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.DeleteContentEstimationListEditor
{
    public class DeleteContentEstimationListEditorCommandHandler : IRequestHandler<DeleteContentEstimationListEditorCommand, Result<bool>>
    {
        private readonly IContentEstimationListEditorRepository contentEditorRoomRepository;

        public DeleteContentEstimationListEditorCommandHandler(IContentEstimationListEditorRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }

        public async Task<Result<bool>> Handle(DeleteContentEstimationListEditorCommand request, CancellationToken cancellationToken)
        {
            var room = await contentEditorRoomRepository.GetContentEstimationListEditor(request.RoomId);
            if (room is null)
            {
                return Result.NotFound("Топ не найден");
            }

            if (room.ContentListCreator.Id != request.UserId)
            {
                return Result.Forbidden("У вас нет прав для удаления этого топа");
            }

            contentEditorRoomRepository.Delete(room);
            await contentEditorRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(true);
        }
    }
} 