using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRatingAPI.Infrastructure.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ContentRatingAPI.Infrastructure.ContentFileManagers.ContentFilePathFinder
{
    public class MongoContentPathFinder : IContentPathFinder
    {
        private readonly MongoContext mongoContext;
        private readonly IOptions<MongoDBOptions> options;

        public MongoContentPathFinder(MongoContext mongoContext, IOptions<MongoDBOptions> options)
        {
            this.mongoContext = mongoContext;
            this.options = options;
        }
        public async Task<bool> HasFileIdInContent(Guid savedFileInfoId)
        {
            var endFileString = savedFileInfoId.ToString();

            var partyEstimationRoomCollection = mongoContext.GetCollection<ContentPartyEstimationRoom>(options.Value.ContentPartyEstimationRoomCollectionName);
            var hasFile = await partyEstimationRoomCollection.AsQueryable()
                .SelectMany(c => c.ContentForEstimation)
                .AnyAsync(c => c.Url.EndsWith(endFileString));
            
            if (hasFile)
                return true;

            var contentListEditorCollection = mongoContext.GetCollection<ContentEstimationListEditor>(options.Value.ContentEstimationListEditorCollectionName);
            return await contentListEditorCollection.AsQueryable()
                .SelectMany(c => c.AddedContent)
                .AnyAsync(c => c.Path.EndsWith(endFileString));
        }
    }
}
