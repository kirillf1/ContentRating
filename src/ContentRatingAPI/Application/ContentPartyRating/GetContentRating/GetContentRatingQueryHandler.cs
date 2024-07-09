using ContentRatingAPI.Infrastructure.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ContentPartyRatingAggregate = ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.ContentPartyRating;

namespace ContentRatingAPI.Application.ContentPartyRating.GetContentRating
{
    public class GetContentRatingQueryHandler : IRequestHandler<GetContentRatingQuery, Result<ContentPartyRatingResponse>>
    {
        IMongoCollection<ContentPartyRatingAggregate> collection;

        public GetContentRatingQueryHandler(MongoContext mongoContext, IOptions<MongoDBOptions> options)
        {
            collection = mongoContext.GetCollection<ContentPartyRatingAggregate>(options.Value.ContentPartyRatingCollectionName);
        }
        public async Task<Result<ContentPartyRatingResponse>> Handle(GetContentRatingQuery request, CancellationToken cancellationToken)
        {
            var rating = await collection.AsQueryable()
                .Where(c => c.Id == request.RatingId)
                .Select(c => new ContentPartyRatingResponse(c.Id, c.AverageContentScore.Value,
                    c.RaterScores.Select(r => new RatingResponse(r.Key, r.Value.Value))))
                .FirstOrDefaultAsync();

            if (rating is null)
                return Result.NotFound();
            return rating;
        }
    }
}
