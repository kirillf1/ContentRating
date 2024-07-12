using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRatingAPI.Application.ContentPartyEstimationRoom.ContentService;

namespace ContentRatingAPI.Infrastructure.AggregateIntegration.ContentPartyEstimationRoom
{
    public class TranslatingContentForEstimationService : IContentForEstimationService
    {
        private readonly IContentEstimationListEditorRepository contentEditorRoomRepository;

        public TranslatingContentForEstimationService(IContentEstimationListEditorRepository contentEditorRoomRepository)
        {
            this.contentEditorRoomRepository = contentEditorRoomRepository;
        }
        public async Task<IEnumerable<ContentForEstimation>> RequestContentForEstimationFromEditor(Guid contentListId, Guid raterId)
        {
            var room = await contentEditorRoomRepository.GetContentEstimationListEditor(contentListId);
            return room.AddedContent.Select(c => new ContentForEstimation(c.Id, c.Name, c.Path, c.Type));
        }
    }
}
