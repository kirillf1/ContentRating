using ContentRatingAPI.Application.ContentFileManager;
using Microsoft.Extensions.Options;

namespace ContentRatingAPI.Infrastructure.ContentFileManagers.FileSavers
{
    public abstract class FileSaverBase
    {
        protected readonly IOptions<ContentFileOptions> options;

        protected FileSaverBase(IOptions<ContentFileOptions> options)
        {
            this.options = options;
        }
        public abstract Task<SavedContentFileInfo> SaveFile(Guid fileId, string fileExtension, byte[] data, CancellationToken cancellationToken = default);
        protected async Task<string> SaveFile(string fileName, byte[] data, CancellationToken cancellationToken = default)
        {
            if (!Path.HasExtension(fileName))
                throw new ArgumentException("Invalid file name");

            if(!Directory.Exists(options.Value.Directory))
                Directory.CreateDirectory(options.Value.Directory);

            var fullPath = Path.Combine(options.Value.Directory, fileName);
            await File.WriteAllBytesAsync(fullPath, data, cancellationToken);
            return fullPath;
        }
        
    }
}
