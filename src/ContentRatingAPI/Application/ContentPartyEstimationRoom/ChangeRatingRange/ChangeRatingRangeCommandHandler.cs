using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.ChangeRatingRange
{
    public class ChangeRatingRangeCommandHandler : IRequestHandler<ChangeRatingRangeCommand, Result>
    {
        private readonly IContentPartyEstimationRoomRepository partyEstimationRoomRepository;

        public ChangeRatingRangeCommandHandler(IContentPartyEstimationRoomRepository partyEstimationRoomRepository)
        {
            this.partyEstimationRoomRepository = partyEstimationRoomRepository;
        }
        public async Task<Result> Handle(ChangeRatingRangeCommand request, CancellationToken cancellationToken)
        {
            var room = await partyEstimationRoomRepository.GetRoom(request.RoomId);
            if (room is null)
                return Result.NotFound();
            var raterInitiator = room.Raters.Single(c=> c.Id == request.EditorId);
            room.ChangeRatingRange(raterInitiator, new Rating(request.MinRating), new Rating(request.MaxRating));

            partyEstimationRoomRepository.Update(room);
            await partyEstimationRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
