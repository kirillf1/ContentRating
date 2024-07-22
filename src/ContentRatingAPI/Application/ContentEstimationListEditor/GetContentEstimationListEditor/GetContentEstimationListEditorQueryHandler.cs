using MongoDB.Driver.Linq;
using MongoDB.Driver;
using ContentEstimationListEditorAggregate = ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.ContentEstimationListEditor;
using ContentRatingAPI.Infrastructure.Data;
using Microsoft.Extensions.Options;
using AspNetCore.Identity.Mongo.Mongo;
namespace ContentRatingAPI.Application.ContentEstimationListEditor.GetContentEstimationListEditor
{
    public class GetContentEstimationListEditorQueryHandler : IRequestHandler<GetContentEstimationListEditorQuery, Result<ContentEstimationListEditorResponse>>
    {
        IMongoCollection<ContentEstimationListEditorAggregate> collection;
        public GetContentEstimationListEditorQueryHandler(MongoContext mongoContext, IOptions<MongoDBOptions> options)
        {
            collection = mongoContext.GetCollection<ContentEstimationListEditorAggregate>(options.Value.ContentEstimationListEditorCollectionName);
        }
        public async Task<Result<ContentEstimationListEditorResponse>> Handle(GetContentEstimationListEditorQuery request, CancellationToken cancellationToken)
        {
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
