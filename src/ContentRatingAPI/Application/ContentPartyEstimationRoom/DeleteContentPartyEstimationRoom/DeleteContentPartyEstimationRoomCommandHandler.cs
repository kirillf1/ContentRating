// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.DeleteContentPartyEstimationRoom
{
    public class DeleteContentPartyEstimationRoomCommandHandler : IRequestHandler<DeleteContentPartyEstimationRoomCommand, Result<bool>>
    {
        private readonly IContentPartyEstimationRoomRepository _roomRepository;
        private readonly IContentPartyRatingRepository _ratingRepository;

        public DeleteContentPartyEstimationRoomCommandHandler(
            IContentPartyEstimationRoomRepository roomRepository,
            IContentPartyRatingRepository ratingRepository)
        {
            _roomRepository = roomRepository;
            _ratingRepository = ratingRepository;
        }

        public async Task<Result<bool>> Handle(DeleteContentPartyEstimationRoomCommand request, CancellationToken cancellationToken)
        {
            var room = await _roomRepository.GetRoom(request.RoomId);
            if (room == null)
            {
                return Result.NotFound();
            }

            // Проверяем, что пользователь является создателем комнаты
            if (room.RoomCreator.Id != request.InitiatorId)
            {
                return Result.Forbidden();
            }

            // Удаляем все связанные рейтинги
            var contentRatings = await _ratingRepository.GetContentRatingsByRoom(request.RoomId);
            foreach (var rating in contentRatings)
            {
                _ratingRepository.Delete(rating);
            }

            // Удаляем комнату
            _roomRepository.Delete(room);

            // Сохраняем изменения
            await _roomRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(true);
        }
    }
} 