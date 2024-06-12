using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.KickRater
{
    public class KickRaterCommandHandler : IRequestHandler<KickRaterCommand>
    {
        private readonly IContentPartyEstimationRoomRepository partyEstimationRoomRepository;

        public KickRaterCommandHandler(IContentPartyEstimationRoomRepository partyEstimationRoomRepository)
        {
            this.partyEstimationRoomRepository = partyEstimationRoomRepository;
        }
        public async Task Handle(KickRaterCommand request, CancellationToken cancellationToken)
        {
            var room = await partyEstimationRoomRepository.GetRoom(request.RoomId);
            room.KickRater(request.RaterForKickId, request.KickInitiatorId);

            partyEstimationRoomRepository.Update(room);
            await partyEstimationRoomRepository.UnitOfWork.SaveChangesAsync();
        }
    }
}
