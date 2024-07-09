using MongoDB.Driver.Linq;
using MongoDB.Driver;
using ContentEditorRoomAggregate = ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.ContentRoomEditor;
using ContentRatingAPI.Infrastructure.Data;
using Microsoft.Extensions.Options;
using AspNetCore.Identity.Mongo.Mongo;
namespace ContentRatingAPI.Application.ContentRoomEditor.GetContentRoomEditor
{
    public class GetContentRoomEditorQueryHandler : IRequestHandler<GetContentRoomEditorQuery, Result<ContentRoomEditorResponse>>
    {
        IMongoCollection<ContentEditorRoomAggregate> collection;
        public GetContentRoomEditorQueryHandler(MongoContext mongoContext, IOptions<MongoDBOptions> options)
        {
            collection = mongoContext.GetCollection<ContentEditorRoomAggregate>(options.Value.ContentEditorRoomCollectionName);
        }
        public async Task<Result<ContentRoomEditorResponse>> Handle(GetContentRoomEditorQuery request, CancellationToken cancellationToken)
        {
            var response = await collection.AsQueryable()
                .Where(c => c.Id == request.RoomId)
                .Select(c => new ContentRoomEditorResponse(
                    c.Id,
                    c.Name,
                    c.RoomCreator.Name,
                    c.RoomCreator.Id,
                    c.AddedContent.Select(a => new ContentResponse(
                        a.Id,
                        a.Path,
                        a.Name,
                        a.Type,
                        a.ContentModificationHistory.EditorId,
                        a.ContentModificationHistory.LastContentModifiedDate
                    )),
                    c.InvitedEditors.Select(e=> new InvitedEditorResponse(e.Id, e.Name))
                ))
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (response is null)
                return Result.NotFound();

            return response;
        }
    }
}
