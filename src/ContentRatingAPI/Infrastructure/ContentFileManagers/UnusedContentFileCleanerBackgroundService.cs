// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRatingAPI.Application.ContentFileManager;
using Microsoft.Extensions.Options;

namespace ContentRatingAPI.Infrastructure.ContentFileManagers
{
    public class UnusedContentFileCleanerBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<UnusedContentFileCleanerBackgroundService> logger;

        public UnusedContentFileCleanerBackgroundService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<UnusedContentFileCleanerBackgroundService> logger
        )
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                double delayTimeInHours = 24;
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    logger.LogInformation("Starting delete unused files");
                    var contentFileManager = scope.ServiceProvider.GetRequiredService<IContentFileManager>();
                    var fileOptions = scope.ServiceProvider.GetRequiredService<IOptions<ContentFileOptions>>();
                    var removeResult = await contentFileManager.RemoveUnusedSavedContentFiles(
                        TimeSpan.FromHours(fileOptions.Value.OldCheckedFileTakeIntervalInHours),
                        stoppingToken
                    );
                    if (removeResult.IsSuccess)
                    {
                        logger.LogInformation("Unused content removed, count: {count}", removeResult.Value);
                    }
                    else
                    {
                        logger.LogError("Can't remove unused content, errors: {errors}", removeResult.Errors);
                    }
                    delayTimeInHours = fileOptions.Value.CheckUnusedFilesIntervalInHours;
                }
                await Task.Delay(TimeSpan.FromHours(delayTimeInHours), stoppingToken);
            }
        }
    }
}
