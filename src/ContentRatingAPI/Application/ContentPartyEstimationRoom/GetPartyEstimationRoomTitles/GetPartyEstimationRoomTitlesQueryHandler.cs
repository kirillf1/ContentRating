using ContentRatingAPI.Infrastructure.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ContentPartyEstimationRoomAggregate = ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.ContentPartyEstimationRoom;
namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.GetPartyEstimationRoomTitles
{
    public class GetPartyEstimationRoomTitlesQueryHandler : IRequestHandler<GetPartyEstimationRoomTitlesQuery, Result<IEnumerable<PartyEstimationTitle>>>
    {
        IMongoCollection<ContentPartyEstimationRoomAggregate> collection;
        public GetPartyEstimationRoomTitlesQueryHandler(MongoContext mongoContext, IOptions<MongoDBOptions> options)
        {
            collection = mongoContext.GetCollection<ContentPartyEstimationRoomAggregate>(options.Value.ContentPartyEstimationRoomCollectionName);
            
        }
        public async Task<Result<IEnumerable<PartyEstimationTitle>>> Handle(GetPartyEstimationRoomTitlesQuery request, CancellationToken cancellationToken)
        {
            var query = collection.AsQueryable()
                .Where(c => c.Raters.Any(r => r.Id == request.RelatedWithRaterId));
            
            if (request.IncludeEstimated && !request.IncludeNotEstimated)
                query = query.Where(c => c.IsAllContentEstimated);

            else if (request.IncludeNotEstimated && !request.IncludeEstimated)
                query = query.Where(c => c.IsAllContentEstimated == false);

            var titles = await query
                .Select(c => new PartyEstimationTitle(c.Id, c.Name, c.RoomCreator.Name, c.Raters.Count, c.ContentForEstimation.Count, c.IsAllContentEstimated))
                .ToListAsync();
                
           return Result.Success(titles.AsEnumerable());
        }
    }
}
