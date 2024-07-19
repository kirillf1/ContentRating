
using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.RemoveUnavailableContent
{
    public class RemoveUnavailableContentCommandHandler : IRequestHandler<RemoveUnavailableContentCommand, Result<bool>>
    {
        private readonly IContentPartyEstimationRoomRepository contentPartyEstimationRoomRepository;

        public RemoveUnavailableContentCommandHandler(IContentPartyEstimationRoomRepository contentPartyEstimationRoomRepository)
        {
            this.contentPartyEstimationRoomRepository = contentPartyEstimationRoomRepository;
        }
        public async Task<Result<bool>> Handle(RemoveUnavailableContentCommand request, CancellationToken cancellationToken)
        {
            var room = await contentPartyEstimationRoomRepository.GetRoom(request.RoomId);
            if(room is null)
                return Result.NotFound();

            room.RemoveUnavailableContent(request.RemoveContentInitiatorId, request.ContentId);

            await contentPartyEstimationRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(true);
        }
    }
}
