
using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.RemoveUnavailableContent
{
    public class RemoveUnavailableContentCommandHandler : IRequestHandler<RemoveUnavailableContentCommand>
    {
        private readonly IContentPartyEstimationRoomRepository contentPartyEstimationRoomRepository;

        public RemoveUnavailableContentCommandHandler(IContentPartyEstimationRoomRepository contentPartyEstimationRoomRepository)
        {
            this.contentPartyEstimationRoomRepository = contentPartyEstimationRoomRepository;
        }
        public async Task Handle(RemoveUnavailableContentCommand request, CancellationToken cancellationToken)
        {
            var room = await contentPartyEstimationRoomRepository.GetRoom(request.RoomId);
            room.RemoveUnavailableContent(request.RemoveContentInitiatorId, request.ContentId);

            await contentPartyEstimationRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
