using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.KickRater
{
    public class KickRaterCommandHandler : IRequestHandler<KickRaterCommand, Result<bool>>
    {
        private readonly IContentPartyEstimationRoomRepository partyEstimationRoomRepository;

        public KickRaterCommandHandler(IContentPartyEstimationRoomRepository partyEstimationRoomRepository)
        {
            this.partyEstimationRoomRepository = partyEstimationRoomRepository;
        }
        public async Task<Result<bool>> Handle(KickRaterCommand request, CancellationToken cancellationToken)
        {
            var room = await partyEstimationRoomRepository.GetRoom(request.RoomId);
            if(room is null)
                return Result.NotFound();
            room.KickRater(request.RaterForKickId, request.KickInitiatorId);

            partyEstimationRoomRepository.Update(room);
            await partyEstimationRoomRepository.UnitOfWork.SaveChangesAsync();
            return Result.Success(true);
        }
    }
}
