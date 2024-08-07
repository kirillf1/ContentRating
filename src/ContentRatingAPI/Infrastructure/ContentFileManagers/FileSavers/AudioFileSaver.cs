using ContentRatingAPI.Application.ContentFileManager;
using HeyRed.Mime;
using Microsoft.Extensions.Options;

namespace ContentRatingAPI.Infrastructure.ContentFileManagers.FileSavers
{
    public class AudioFileSaver(IOptions<ContentFileOptions> options) : FileSaverBase(options)
    {
        public override async Task<SavedContentFileInfo> SaveFile(Guid fileId, string fileExtension, byte[] data, CancellationToken cancellationToken = default)
        {
            var mimeType = MimeTypesMap.GetMimeType(fileExtension);
            if (!mimeType.StartsWith("audio"))
                throw new ArgumentException("File extension must be audio");
            var path = await base.SaveFile(Path.Combine("audio_files", $"{fileId}{fileExtension}"), data, cancellationToken);
            return new SavedContentFileInfo(fileId, DateTime.UtcNow, path, ContentRating.Domain.Shared.Content.ContentType.Audio);
        }
    }
}
