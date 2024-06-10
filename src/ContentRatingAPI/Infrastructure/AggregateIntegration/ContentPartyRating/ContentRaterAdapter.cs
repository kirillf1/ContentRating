using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRatingAPI.Infrastructure.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ContentRatingAPI.Infrastructure.AggregateIntegration.ContentPartyRating
{
    // TODO Create abstraction if need different logic. For example if divide project on two services and need http requests
    public class ContentRaterAdapter
    {
        private readonly ContentRaterTranslator contentRaterTranslator;
        private readonly MongoContext mongoContext;
        private readonly IOptions<MongoDBOptions> options;

        public ContentRaterAdapter(ContentRaterTranslator contentRaterTranslator, MongoContext mongoContext, IOptions<MongoDBOptions> options)
        {
            this.contentRaterTranslator = contentRaterTranslator;
            this.mongoContext = mongoContext;
            this.options = options;
        }
        public async Task<ContentRater> GetContentRater(Guid roomId, Guid raterId)
        {
            var rooms = mongoContext.GetCollection<ContentPartyEstimationRoom>(options.Value.ContentPartyEstimationRoomCollectionName);
            var rater = await rooms.AsQueryable().
                Where(c=> c.Id == roomId).
                SelectMany(c=> c.Raters).
                Where(c=> c.Id == raterId).
                FirstAsync();
            return contentRaterTranslator.Translate(rater);
        }
        public async Task<List<ContentRater>> GetContentRates(Guid roomId, params Guid[] raterIds)
        {
            var rooms = mongoContext.GetCollection<ContentPartyEstimationRoom>(options.Value.ContentPartyEstimationRoomCollectionName);
            var raters = await rooms.AsQueryable().
                Where(c => c.Id == roomId).
                SelectMany(c => c.Raters).
                Where(c => raterIds.Contains(c.Id)).
                ToListAsync();

            return raters.Select( contentRaterTranslator.Translate).ToList();
                   
        }
    }
}
