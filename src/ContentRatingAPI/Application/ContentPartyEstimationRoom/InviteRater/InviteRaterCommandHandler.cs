using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.InviteRater
{
    public class InviteRaterCommandHandler : IRequestHandler<InviteRaterCommand>
    {
        private readonly IContentPartyEstimationRoomRepository partyEstimationRoomRepository;

        public InviteRaterCommandHandler(IContentPartyEstimationRoomRepository partyEstimationRoomRepository)
        {
            this.partyEstimationRoomRepository = partyEstimationRoomRepository;
        }
        public async Task Handle(InviteRaterCommand request, CancellationToken cancellationToken)
        {
            var room = await partyEstimationRoomRepository.GetRoom(request.RoomId);
            var newRater = new Rater(request.RaterForInviteId, request.RoleType, request.RaterName);
            room.InviteRater(newRater, request.InviteInitiatorId);

            partyEstimationRoomRepository.Update(room);
            await partyEstimationRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
