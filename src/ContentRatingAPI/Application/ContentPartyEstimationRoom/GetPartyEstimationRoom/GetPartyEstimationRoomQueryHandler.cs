// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
            var query =
                from estimationRoom in partyRatingRoomCollection.AsQueryable().Where(c => c.Id == request.RoomId)
                join rating in contentRatingCollection on estimationRoom.Id equals rating.RoomId
                select new PartyEstimationRoomResponse(
                    estimationRoom.Id,
                    estimationRoom.Name,
                    estimationRoom.RatingRange.MinRating.Value,
                    estimationRoom.RatingRange.MaxRating.Value,
                    estimationRoom.RoomCreator.Name,
                    estimationRoom.ContentForEstimation.Select(c => new ContentRatingResponse(
                        rating.Id,
                        c.Id,
                        c.Name,
                        c.Url,
                        c.ContentType,
                        rating.RaterScores.Select(s => new RatingByRaterResponse(s.Key, s.Value.Value)),
                        rating.AverageContentScore.Value
                    )),
                    estimationRoom.Raters.Select(r => new RaterResponse(r.Id, r.Name))
                );
            var response = await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (response is null)
            {
                return Result.NotFound();
            }

            return response;
        }
    }
}
