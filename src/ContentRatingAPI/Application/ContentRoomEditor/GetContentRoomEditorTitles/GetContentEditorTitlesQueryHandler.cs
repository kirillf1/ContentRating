using ContentEditorRoomAggregate = ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.ContentRoomEditor;
using ContentRatingAPI.Infrastructure.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ContentRatingAPI.Application.ContentRoomEditor.GetContentRoomEditorTitles
{
    public class GetContentEditorTitlesQueryHandler : IRequestHandler<GetContentEditorTitlesQuery, Result<IEnumerable<ContentRoomEditorTitle>>>
    {
        IMongoCollection<ContentEditorRoomAggregate> collection;
        public GetContentEditorTitlesQueryHandler(MongoContext mongoContext, IOptions<MongoDBOptions> options)
        {
            collection = mongoContext.GetCollection<ContentEditorRoomAggregate>(options.Value.ContentEditorRoomCollectionName);
        }
        public async Task<Result<IEnumerable<ContentRoomEditorTitle>>> Handle(GetContentEditorTitlesQuery request, CancellationToken cancellationToken)
        {
            return await collection.AsQueryable()
                 .Where(c => c.InvitedEditors.Any(c => c.Id == request.EditorId) || c.RoomCreator.Id == request.EditorId)
                 .Select(c => new ContentRoomEditorTitle(c.Id, c.Name, c.AddedContent.Count, c.Name))
                 .ToListAsync(cancellationToken);
        }
    }
}
