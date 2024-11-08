// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRatingAPI.Infrastructure.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ContentEstimationListEditorAggregate = ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.ContentEstimationListEditor;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.GetContentEstimationListEditorTitles
{
    public class GetContentEstimationListEditorTitlesQueryHandler
        : IRequestHandler<GetContentEstimationListEditorTitlesQuery, Result<IEnumerable<ContentEstimationListEditorTitle>>>
    {
        IMongoCollection<ContentEstimationListEditorAggregate> collection;

        public GetContentEstimationListEditorTitlesQueryHandler(MongoContext mongoContext, IOptions<MongoDBOptions> options)
        {
            collection = mongoContext.GetCollection<ContentEstimationListEditorAggregate>(options.Value.ContentEstimationListEditorCollectionName);
        }

        public async Task<Result<IEnumerable<ContentEstimationListEditorTitle>>> Handle(
            GetContentEstimationListEditorTitlesQuery request,
            CancellationToken cancellationToken
        )
        {
            return await collection
                .AsQueryable()
                .Where(c => c.InvitedEditors.Any(c => c.Id == request.EditorId) || c.ContentListCreator.Id == request.EditorId)
                .Select(c => new ContentEstimationListEditorTitle(c.Id, c.Name, c.AddedContent.Count, c.Name))
                .ToListAsync(cancellationToken);
        }
    }
}
