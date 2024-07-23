using MongoDB.Driver.Linq;
using MongoDB.Driver;
using ContentEstimationListEditorAggregate = ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.ContentEstimationListEditor;
using ContentRatingAPI.Infrastructure.Data;
using Microsoft.Extensions.Options;
using AspNetCore.Identity.Mongo.Mongo;
using ContentRatingAPI.Infrastructure.Data.Caching;
namespace ContentRatingAPI.Application.ContentEstimationListEditor.GetContentEstimationListEditor
{
    public class GetContentEstimationListEditorQueryHandler : IRequestHandler<GetContentEstimationListEditorQuery, Result<ContentEstimationListEditorResponse>>
    {
        private readonly GenericCacheBase<ContentEstimationListEditorAggregate> cache;
        IMongoCollection<ContentEstimationListEditorAggregate> collection;
        public GetContentEstimationListEditorQueryHandler(MongoContext mongoContext, IOptions<MongoDBOptions> options, GenericCacheBase<ContentEstimationListEditorAggregate> cache)
        {
            collection = mongoContext.GetCollection<ContentEstimationListEditorAggregate>(options.Value.ContentEstimationListEditorCollectionName);
            this.cache = cache;
        }
        public async Task<Result<ContentEstimationListEditorResponse>> Handle(GetContentEstimationListEditorQuery request, CancellationToken cancellationToken)
        {
            if(cache.TryGetValue(request.RoomId, out var listEditor) && listEditor is not null)
            {
                return new ContentEstimationListEditorResponse(
                    listEditor.Id,
                    listEditor.Name,
                    listEditor.ContentListCreator.Name,
                    listEditor.ContentListCreator.Id,
                    listEditor.AddedContent.Select(a => new ContentResponse(
                        a.Id,
                        a.Path,
                        a.Name,
                        a.Type,
                        a.ContentModificationHistory.EditorId,
                        a.ContentModificationHistory.LastContentModifiedDate
                    )),
                    listEditor.InvitedEditors.Select(e => new InvitedEditorResponse(e.Id, e.Name)));
            }
            var response = await collection.AsQueryable()
                .Where(c => c.Id == request.RoomId)
                .Select(c => new ContentEstimationListEditorResponse(
                    c.Id,
                    c.Name,
                    c.ContentListCreator.Name,
                    c.ContentListCreator.Id,
                    c.AddedContent.Select(a => new ContentResponse(
                        a.Id,
                        a.Path,
                        a.Name,
                        a.Type,
                        a.ContentModificationHistory.EditorId,
                        a.ContentModificationHistory.LastContentModifiedDate
                    )),
                    c.InvitedEditors.Select(e => new InvitedEditorResponse(e.Id, e.Name))
                ))
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (response is null)
                return Result.NotFound();

            return response;
        }
    }
}
