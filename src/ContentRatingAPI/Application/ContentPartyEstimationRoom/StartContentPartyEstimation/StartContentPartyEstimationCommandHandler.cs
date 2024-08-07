using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRatingAPI.Application.ContentPartyEstimationRoom.ContentService;
using ContentPartyEstimationRoomAggregate = ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.ContentPartyEstimationRoom;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.StartContentPartyEstimation
{
    public class StartContentPartyEstimationCommandHandler : IRequestHandler<StartContentPartyEstimationCommand, Result<bool>>
    {
        private readonly IContentPartyEstimationRoomRepository roomRepository;
        private readonly IContentForEstimationService contentForEstimationService;

        public StartContentPartyEstimationCommandHandler(IContentPartyEstimationRoomRepository roomRepository, IContentForEstimationService contentForEstimationService)
        {
            this.roomRepository = roomRepository;
            this.contentForEstimationService = contentForEstimationService;
        }
        public async Task<Result<bool>> Handle(StartContentPartyEstimationCommand request, CancellationToken cancellationToken)
        {
            var creator = new Rater(request.CreatorId, RoleType.Admin, request.CreatorName);
            var ratingRange = new RatingRange(new Rating(request.MaxRating), new Rating(request.MinRating));
            var contentForEstimation = await contentForEstimationService.RequestContentForEstimationFromEditor(request.ContentListId, request.CreatorId);

            var existingRoom = await roomRepository.GetRoom(request.RoomId);
            if (existingRoom is not null)
                return Result.Invalid(new ValidationError($"Room with id {request.RoomId} exists"));

            var newRoom = ContentPartyEstimationRoomAggregate.Create(request.RoomId, creator, contentForEstimation,"test", ratingRange);
            roomRepository.Add(newRoom);
            await roomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
