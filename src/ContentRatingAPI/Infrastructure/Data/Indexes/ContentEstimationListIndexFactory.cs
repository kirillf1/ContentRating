// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ContentRatingAPI.Infrastructure.Data.Indexes
{
    public class ContentEstimationListIndexFactory : IMongoCollectionIndexFactory
    {
        private readonly IMongoClient mongoClient;
        private readonly IOptions<MongoDBOptions> options;
        private readonly ILogger<ContentEstimationListIndexFactory> logger;

        public ContentEstimationListIndexFactory(
            IMongoClient mongoClient,
            IOptions<MongoDBOptions> options,
            ILogger<ContentEstimationListIndexFactory> logger
        )
        {
            this.mongoClient = mongoClient;
            this.options = options;
            this.logger = logger;
        }

        public async Task CreateIndexesInCollection()
        {
            var database = mongoClient.GetDatabase(options.Value.DatabaseName);
            var collection = database.GetCollection<ContentEstimationListEditor>(options.Value.ContentEstimationListEditorCollectionName);
            var indexDefinition = Builders<ContentEstimationListEditor>.IndexKeys;
            var contentPartyRatingOptions = new CreateIndexOptions { Unique = false };

            var indexModel = new CreateIndexModel<ContentEstimationListEditor>(
                indexDefinition.Ascending($"{nameof(ContentEstimationListEditor.AddedContent)}.{nameof(Content.Path)}"),
                contentPartyRatingOptions
            );
            await collection.Indexes.CreateOneAsync(indexModel);
            logger.LogInformation("Index for {Property} created", nameof(Content.Path));
        }
    }
}
