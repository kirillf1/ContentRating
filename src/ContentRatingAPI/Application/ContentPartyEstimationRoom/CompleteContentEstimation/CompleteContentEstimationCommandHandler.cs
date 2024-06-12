using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.CompleteContentEstimation
{
    public class CompleteContentEstimationCommandHandler : IRequestHandler<CompleteContentEstimationCommand>
    {
        private readonly IContentPartyEstimationRoomRepository contentPartyEstimationRoomRepository;

        public CompleteContentEstimationCommandHandler(IContentPartyEstimationRoomRepository contentPartyEstimationRoomRepository)
        {
            this.contentPartyEstimationRoomRepository = contentPartyEstimationRoomRepository;
        }
        public async Task Handle(CompleteContentEstimationCommand request, CancellationToken cancellationToken)
        {
            var room = await contentPartyEstimationRoomRepository.GetRoom(request.RoomId);
            var rater = room.Raters.Single(c => c.Id == request.InitiatorId);
            room.CompleteContentEstimation(rater);

            contentPartyEstimationRoomRepository.Update(room);
            await contentPartyEstimationRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
