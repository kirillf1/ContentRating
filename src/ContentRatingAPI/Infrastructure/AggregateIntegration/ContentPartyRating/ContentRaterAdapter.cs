using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ContentRatingAPI.Infrastructure.AggregateIntegration.ContentPartyRating
{
    // TODO Create abstraction if need different logic. For example if divide project on two services and need http requests
    public class ContentRaterAdapter
    {
        private readonly ContentRaterTranslator contentRaterTranslator;
        private readonly IContentPartyEstimationRoomRepository roomRepository;


        public ContentRaterAdapter(ContentRaterTranslator contentRaterTranslator, IContentPartyEstimationRoomRepository roomRepository)
        {
            this.contentRaterTranslator = contentRaterTranslator;
            this.roomRepository = roomRepository;
        }
        public async Task<ContentRater> GetContentRater(Guid roomId, Guid raterId)
        {
            var room = await roomRepository.GetRoom(roomId);

            if(room is null)
                throw new ArgumentNullException(nameof(room));

            var rater = room.Raters.First(c => c.Id == raterId);
            return contentRaterTranslator.Translate(rater);
        }
        public async Task<List<ContentRater>> GetContentRates(Guid roomId, params Guid[] raterIds)
        {
            var room = await roomRepository.GetRoom(roomId);

            if (room is null)
                throw new ArgumentNullException(nameof(room));

            var raters = room.Raters.Where(c => raterIds.Contains(c.Id));

            return raters.Select(contentRaterTranslator.Translate).ToList();                 
        }
    }
}
