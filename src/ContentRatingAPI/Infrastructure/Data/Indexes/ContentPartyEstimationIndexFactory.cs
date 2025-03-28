// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ContentRatingAPI.Infrastructure.Data.Indexes
{
    public class ContentPartyEstimationIndexFactory : IMongoCollectionIndexFactory
    {
        private readonly IMongoClient mongoClient;
        private readonly IOptions<MongoDBOptions> options;
        private readonly ILogger<ContentPartyEstimationIndexFactory> logger;

        public ContentPartyEstimationIndexFactory(
            IMongoClient mongoClient,
            IOptions<MongoDBOptions> options,
            ILogger<ContentPartyEstimationIndexFactory> logger
        )
        {
            this.mongoClient = mongoClient;
            this.options = options;
            this.logger = logger;
        }

        public async Task CreateIndexesInCollection()
        {
            var database = mongoClient.GetDatabase(options.Value.DatabaseName);
            var collection = database.GetCollection<ContentPartyEstimationRoom>(options.Value.ContentPartyEstimationRoomCollectionName);
            var contentPartyRatingIndexDefinition = Builders<ContentPartyEstimationRoom>.IndexKeys;
            var contentPartyRatingOptions = new CreateIndexOptions { Unique = false };

            var indexModel = new CreateIndexModel<ContentPartyEstimationRoom>(
                contentPartyRatingIndexDefinition.Ascending(
                    $"{nameof(ContentPartyEstimationRoom.ContentForEstimation)}.{nameof(ContentForEstimation.Url)}"
                ),
                contentPartyRatingOptions
            );
            await collection.Indexes.CreateOneAsync(indexModel);
            logger.LogInformation("Index for {property} created", nameof(ContentForEstimation.Url));
        }
    }
}
