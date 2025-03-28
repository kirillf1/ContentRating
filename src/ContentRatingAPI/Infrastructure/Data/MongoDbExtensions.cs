﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRating.Domain.Shared;
using ContentRatingAPI.Application.ContentFileManager;
using ContentRatingAPI.Infrastructure.Data.Caching;
using ContentRatingAPI.Infrastructure.Data.Indexes;
using ContentRatingAPI.Infrastructure.Data.MapConvensions;
using ContentRatingAPI.Infrastructure.Data.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

namespace ContentRatingAPI.Infrastructure.Data
{
    public static class MongoDbExtensions
    {
        public static IServiceCollection AddMongoDbStorage(this IHostApplicationBuilder builder)
        {
            RegisterEntityClassMap();
            builder.Services.AddSingleton<IMongoClient>(c =>
            {
                var options = c.GetRequiredService<IOptions<MongoDBOptions>>();
                var clientSettings = MongoClientSettings.FromConnectionString(options.Value.Connection);
                var instrumentationOptions = new InstrumentationOptions { CaptureCommandText = true };
                clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber(instrumentationOptions));
                clientSettings.MaxConnectionPoolSize = 200;
                return new MongoClient(clientSettings);
            });

            builder.Services.Configure<MongoDBOptions>(builder.Configuration.GetSection(MongoDBOptions.Position));
            builder.Services.AddScoped<IUnitOfWork, MongoContext>();
            builder.Services.AddScoped<MongoContext>();
            builder.Services.AddScoped<IChangeTracker, InMemoryChangeTracker>();

            builder.Services.AddHostedService<MongoIndexService>();
            builder.Services.AddTransient<IMongoCollectionIndexFactory, ContentPartyRatingIndexFactory>();
            builder.Services.AddTransient<IMongoCollectionIndexFactory, ContentPartyEstimationIndexFactory>();
            builder.Services.AddTransient<IMongoCollectionIndexFactory, ContentEstimationListIndexFactory>();

            builder.Services.AddMemoryCache();
            builder.Services.AddTransient<GenericCacheBase<ContentPartyEstimationRoom>, GenericInMemoryCache<ContentPartyEstimationRoom>>();
            builder.Services.AddTransient<GenericCacheBase<ContentEstimationListEditor>, GenericInMemoryCache<ContentEstimationListEditor>>();
            builder.Services.AddTransient<GenericCacheBase<ContentPartyRating>, GenericInMemoryCache<ContentPartyRating>>();

            builder.Services.AddTransient<IContentEstimationListEditorRepository, ContentEstimationListEditorRepository>();
            builder.Services.Decorate<IContentEstimationListEditorRepository, CachingContentEstimationListEditorRepository>();
            builder.Services.AddTransient<IContentPartyEstimationRoomRepository, ContentPartyEstimationRoomRepository>();
            builder.Services.Decorate<IContentPartyEstimationRoomRepository, CachingPartyEstimationRoomRepository>();
            builder.Services.AddTransient<IContentPartyRatingRepository, ContentPartyRatingRepository>();
            return builder.Services;
        }

        private static void RegisterEntityClassMap()
        {
            var conventionPack = new ConventionPack { new MapReadOnlyPropertiesConvention(), new IgnoreIfNullConvention(true) };
            ConventionRegistry.Register("Conventions", conventionPack, _ => true);
            BsonClassMap.RegisterClassMap<Entity>(cm =>
            {
                cm.AutoMap();
                cm.UnmapMember(m => m.DomainEvents);
            });
            BsonClassMap.RegisterClassMap<ContentPartyEstimationRoom>(classMap =>
            {
                classMap.AutoMap();
            });

            BsonClassMap.RegisterClassMap<ContentPartyRating>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetDictionaryRepresentation(c => c.RaterScores, DictionaryRepresentation.ArrayOfDocuments);
            });

            BsonClassMap.RegisterClassMap<ContentEstimationListEditor>(classMap =>
            {
                classMap.AutoMap();
            });

            BsonClassMap.RegisterClassMap<SavedContentFileInfo>(classMap =>
            {
                classMap.AutoMap();
            });
        }
    }
}
