﻿using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.InviteRater
{
    public class InviteRaterCommandHandler : IRequestHandler<InviteRaterCommand, Result<bool>>
    {
        private readonly IContentPartyEstimationRoomRepository partyEstimationRoomRepository;

        public InviteRaterCommandHandler(IContentPartyEstimationRoomRepository partyEstimationRoomRepository)
        {
            this.partyEstimationRoomRepository = partyEstimationRoomRepository;
        }
        public async Task<Result<bool>> Handle(InviteRaterCommand request, CancellationToken cancellationToken)
        {
            var room = await partyEstimationRoomRepository.GetRoom(request.RoomId);
            if (room is null)
                return Result.NotFound();
            var newRater = new Rater(request.RaterForInviteId, request.RoleType, request.RaterName);
            room.InviteRater(newRater, request.InviteInitiatorId);

            partyEstimationRoomRepository.Update(room);
            await partyEstimationRoomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(true);
        }
    }
}
