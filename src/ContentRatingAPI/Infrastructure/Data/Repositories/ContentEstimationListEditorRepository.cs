// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;
using ContentRating.Domain.Shared;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ContentRatingAPI.Infrastructure.Data.Repositories
{
    public class ContentEstimationListEditorRepository : IContentEstimationListEditorRepository
    {
        public ContentEstimationListEditorRepository(MongoContext mongoContext, IOptions<MongoDBOptions> options)
        {
            this.mongoContext = mongoContext;
            UnitOfWork = mongoContext;
            dbSet = mongoContext.GetCollection<ContentEstimationListEditor>(options.Value.ContentEstimationListEditorCollectionName);
        }

        public IUnitOfWork UnitOfWork { get; }
        private readonly MongoContext mongoContext;
        private readonly IMongoCollection<ContentEstimationListEditor> dbSet;

        public ContentEstimationListEditor Add(ContentEstimationListEditor editor)
        {
            mongoContext.AddCommand((s) => dbSet.InsertOneAsync(s, editor), editor);
            return editor;
        }

        public void Delete(ContentEstimationListEditor editor)
        {
            mongoContext.AddCommand((s) => dbSet.DeleteOneAsync(s, Builders<ContentEstimationListEditor>.Filter.Eq(_ => _.Id, editor.Id)), editor);
        }

        public async Task<ContentEstimationListEditor?> GetContentEstimationListEditor(Guid id)
        {
            return await dbSet.Find(Builders<ContentEstimationListEditor>.Filter.Eq(_ => _.Id, id)).SingleOrDefaultAsync();
        }

        public ContentEstimationListEditor Update(ContentEstimationListEditor editor)
        {
            mongoContext.AddCommand(
                (s) => dbSet.ReplaceOneAsync(s, Builders<ContentEstimationListEditor>.Filter.Eq(_ => _.Id, editor.Id), editor),
                editor
            );
            return editor;
        }

        public async Task<bool> HasEditorInContentEstimationList(Guid listId, Guid editorId)
        {
            return await dbSet
                .AsQueryable()
                .AnyAsync(c => c.Id == listId && (c.ContentListCreator.Id == editorId || c.InvitedEditors.Any(c => c.Id == editorId)));
        }
    }
}
