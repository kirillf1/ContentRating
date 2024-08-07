using ContentRatingAPI.Application.ContentFileManager;
using HeyRed.Mime;
using Microsoft.Extensions.Options;

namespace ContentRatingAPI.Infrastructure.ContentFileManagers.FileSavers
{
    public class ImageFileSaver : FileSaverBase
    {
        public ImageFileSaver(IOptions<ContentFileOptions> options) : base(options)
        {
        }

        public async override Task<SavedContentFileInfo> SaveFile(Guid fileId, string fileExtension, byte[] data, CancellationToken cancellationToken = default)
        {
            var mimeType = MimeTypesMap.GetMimeType(fileExtension);
            if (!mimeType.StartsWith("image"))
                throw new ArgumentException("File extension must be image");
            var path = await base.SaveFile(Path.Combine("images", $"{fileId}{fileExtension}"), data, cancellationToken);
            return new SavedContentFileInfo(fileId, DateTime.UtcNow, path, ContentRating.Domain.Shared.Content.ContentType.Image);
        }
    }
}
