// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRatingAPI.Application.ContentFileManager;
using ContentRatingAPI.Infrastructure.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ContentRatingAPI.Infrastructure.ContentFileManagers
{
    public class SavedContentMongoStorage : ISavedContentStorage
    {
        private readonly MongoContext mongoContext;
        private readonly IOptions<MongoDBOptions> options;
        private readonly IMongoCollection<SavedContentFileInfo> dbSet;

        public SavedContentMongoStorage(MongoContext mongoContext, IOptions<MongoDBOptions> options)
        {
            this.mongoContext = mongoContext;
            this.options = options;
            dbSet = mongoContext.GetCollection<SavedContentFileInfo>(options.Value.SavedContentFileCollectionName);
        }

        public async Task Add(SavedContentFileInfo file)
        {
            await dbSet.InsertOneAsync(file);
        }

        public async Task DeleteSavedContent(Guid Id)
        {
            await dbSet.DeleteOneAsync(c => c.Id == Id);
        }

        public async Task<IEnumerable<SavedContentFileInfo>> GetOldCheckedOrUncheckedContent(TimeSpan checkInterval)
        {
            var startCheckDate = DateTime.UtcNow - checkInterval;
            return await dbSet.AsQueryable().Where(c => c.LastCheckDate == null || c.LastCheckDate < startCheckDate).ToListAsync();
        }

        public async Task<SavedContentFileInfo?> GetSavedContent(Guid Id)
        {
            return await dbSet.AsQueryable().FirstOrDefaultAsync(c => c.Id == Id);
        }

        public async Task Update(SavedContentFileInfo file)
        {
            await dbSet.ReplaceOneAsync(Builders<SavedContentFileInfo>.Filter.Eq(_ => _.Id, file.Id), file);
        }
    }
}
