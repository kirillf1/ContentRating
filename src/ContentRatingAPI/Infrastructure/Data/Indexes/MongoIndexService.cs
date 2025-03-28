// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Infrastructure.Data.Indexes
{
    public class MongoIndexService : IHostedService
    {
        private readonly IEnumerable<IMongoCollectionIndexFactory> indexFactories;

        public MongoIndexService(IEnumerable<IMongoCollectionIndexFactory> indexFactories)
        {
            this.indexFactories = indexFactories;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var indexFactory in indexFactories)
            {
                await indexFactory.CreateIndexesInCollection();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
