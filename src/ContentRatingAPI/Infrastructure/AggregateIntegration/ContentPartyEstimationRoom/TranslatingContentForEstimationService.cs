using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using ContentRatingAPI.Application.ContentPartyEstimationRoom.ContentService;

namespace ContentRatingAPI.Infrastructure.AggregateIntegration.ContentPartyEstimationRoom
{
    public class TranslatingContentForEstimationService : IContentForEstimationService
    {
        private readonly IContentEditorRoomRepository contentEditorRoomRepository;

        public TranslatingContentForEstimationService(IContentEditorRoomRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task<IEnumerable<ContentForEstimation>> RequestContentForEstimationFromEditor(Guid contentListId, Guid raterId)
        {
            var room = await contentEditorRoomRepository.GetRoom(contentListId);
            return room.AddedContent.Select(c => new ContentForEstimation(c.Id, c.Name, c.Path, c.Type));
        }
    }
}
