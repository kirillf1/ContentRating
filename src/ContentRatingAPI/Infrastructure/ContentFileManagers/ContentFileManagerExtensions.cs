using ContentRating.Domain.Shared.Content;
using ContentRatingAPI.Application.ContentFileManager;
using ContentRatingAPI.Infrastructure.ContentFileManagers.FileSavers;
using Microsoft.Extensions.Options;

namespace ContentRatingAPI.Infrastructure.ContentFileManagers
{
    public static class ContentFileManagerExtensions
    {
        public static IServiceCollection AddContentFileManager(this IHostApplicationBuilder builder)
        {
           
            builder.Services.Configure<ContentFileOptions>(builder.Configuration.GetSection("ContentFile"));
            builder.Services.Configure<FFMPEGOptions>(builder.Configuration.GetSection("FFMEPGOptions"));

            builder.Services.AddScoped<ISavedContentStorage, SavedContentMongoStorage>();
            builder.Services.AddScoped<ImageFileSaver>();
            builder.Services.AddScoped<HLSVideoSaver>();
            builder.Services.AddScoped<AudioFileSaver>();
            builder.Services.AddScoped<IContentFileManager, ContentFileMongoManager>(s =>
            {
                var imageSaver = s.GetRequiredService<ImageFileSaver>();
                var audioSaver = s.GetRequiredService<AudioFileSaver>();
                var videoSaver = s.GetRequiredService<HLSVideoSaver>();
                var storage = s.GetRequiredService<ISavedContentStorage>();
                var contentFileOptions = s.GetRequiredService<IOptions<ContentFileOptions>>();
                var fileSavers = new Dictionary<ContentType, FileSaverBase>() {
                    { ContentType.Video, videoSaver },
                    { ContentType.Audio, audioSaver },
                    { ContentType.Image, imageSaver },
                };
                return new ContentFileMongoManager(contentFileOptions, storage, fileSavers, s.GetRequiredService<ILogger<ContentFileMongoManager>>());
            });
            return builder.Services;
        }
    }
}
