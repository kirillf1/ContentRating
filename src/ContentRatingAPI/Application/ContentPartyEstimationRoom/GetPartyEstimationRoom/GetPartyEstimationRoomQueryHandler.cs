// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Web.Contracts.ContentPartyEstimationRoom;
using ContentRatingAPI.Infrastructure.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ContentPartyEstimationRoomAggregate = ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.ContentPartyEstimationRoom;
using ContentPartyRatingAggregate = ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.ContentPartyRating;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.GetPartyEstimationRoom
{
    public class GetPartyEstimationRoomQueryHandler : IRequestHandler<GetPartyEstimationRoomQuery, Result<PartyEstimationRoomResponse>>
    {
        IMongoCollection<ContentPartyEstimationRoomAggregate> partyRatingRoomCollection;
        IMongoCollection<ContentPartyRatingAggregate> contentRatingCollection;

        public GetPartyEstimationRoomQueryHandler(MongoContext mongoContext, IOptions<MongoDBOptions> options)
        {
            partyRatingRoomCollection = mongoContext.GetCollection<ContentPartyEstimationRoomAggregate>(
                options.Value.ContentPartyEstimationRoomCollectionName
            );
            contentRatingCollection = mongoContext.GetCollection<ContentPartyRatingAggregate>(options.Value.ContentPartyRatingCollectionName);
        }

        public async Task<Result<PartyEstimationRoomResponse>> Handle(GetPartyEstimationRoomQuery request, CancellationToken cancellationToken)
        {
            var estimationRoom = await partyRatingRoomCollection.AsQueryable()
                .Where(c => c.Id == request.RoomId)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (estimationRoom is null)
            {
                return Result.NotFound();
            }

            var ratings = await contentRatingCollection.AsQueryable()
                .Where(r => r.RoomId == request.RoomId)
                .ToListAsync(cancellationToken: cancellationToken);

            var contentRatings = estimationRoom.ContentForEstimation.Select(content =>
            {
                var rating = ratings.FirstOrDefault(r => r.ContentId == content.Id);
                return new ContentRatingResponse(
                    rating?.Id ?? Guid.Empty,
                    content.Id,
                    content.Name,
                    content.Url,
                    content.ContentType,
                    rating?.RaterScores.Select(s => new RatingByRaterResponse(s.Key, s.Value.Value)) ?? Enumerable.Empty<RatingByRaterResponse>(),
                    rating?.AverageContentScore.Value ?? 0.0
                );
            });

            var response = new PartyEstimationRoomResponse(
                estimationRoom.Id,
                estimationRoom.Name,
                estimationRoom.RatingRange.MinRating.Value,
                estimationRoom.RatingRange.MaxRating.Value,
                estimationRoom.RoomCreator.Name,
                contentRatings,
                estimationRoom.Raters.Select(r => new RaterResponse(r.Id, r.Name))
            );

            return response;
        }
    }
}
