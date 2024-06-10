﻿using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRatingAPI.Application.ContentPartyEstimationRoom.ContentService;
using ContentPartyEstimationRoomAggregate = ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.ContentPartyEstimationRoom;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.CreateContentPartyEstimation
{
    public class CreateContentPartyEstimationCommandHandler : IRequestHandler<CreateContentPartyEstimationCommand>
    {
        private readonly IContentPartyEstimationRoomRepository roomRepository;
        private readonly IContentForEstimationService contentForEstimationService;

        public CreateContentPartyEstimationCommandHandler(IContentPartyEstimationRoomRepository roomRepository, IContentForEstimationService contentForEstimationService)
        {
            this.roomRepository = roomRepository;
            this.contentForEstimationService = contentForEstimationService;
        }
        public async Task Handle(CreateContentPartyEstimationCommand request, CancellationToken cancellationToken)
        {
            var creator = new Rater(request.CreatorId, RoleType.Admin, request.CreatorName);
            var ratingRange = new RatingRange(new Rating(request.MaxRating), new Rating(request.MinRating));
            var contentForEstimation = await contentForEstimationService.RequestContentForEstimationFromEditor(request.ContentListId, request.CreatorId);

            var newRoom = ContentPartyEstimationRoomAggregate.Create(request.RoomId, creator, contentForEstimation, ratingRange);
            roomRepository.Add(newRoom);
            await roomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
