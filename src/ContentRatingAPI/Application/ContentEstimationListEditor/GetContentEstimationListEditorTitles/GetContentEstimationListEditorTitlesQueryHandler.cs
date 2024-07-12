using ContentEstimationListEditorAggregate = ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.ContentEstimationListEditor;
using ContentRatingAPI.Infrastructure.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.GetContentEstimationListEditorTitles
{
    public class GetContentEstimationListEditorTitlesQueryHandler : IRequestHandler<GetContentEstimationListEditorTitlesQuery, Result<IEnumerable<ContentEstimationListEditorTitle>>>
    {
        IMongoCollection<ContentEstimationListEditorAggregate> collection;
        public GetContentEstimationListEditorTitlesQueryHandler(MongoContext mongoContext, IOptions<MongoDBOptions> options)
        {
            collection = mongoContext.GetCollection<ContentEstimationListEditorAggregate>(options.Value.ContentEstimationListEditorCollectionName);
        }
        public async Task<Result<IEnumerable<ContentEstimationListEditorTitle>>> Handle(GetContentEstimationListEditorTitlesQuery request, CancellationToken cancellationToken)
        {
            return await collection.AsQueryable()
                 .Where(c => c.InvitedEditors.Any(c => c.Id == request.EditorId) || c.RoomCreator.Id == request.EditorId)
                 .Select(c => new ContentEstimationListEditorTitle(c.Id, c.Name, c.AddedContent.Count, c.Name))
                 .ToListAsync(cancellationToken);
        }
    }
}
