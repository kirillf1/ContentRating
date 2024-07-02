using ContentRating.Domain.Shared.Content;
using ContentRatingAPI.Application.ContentFileManager;
using ContentRatingAPI.Infrastructure.ContentFileManagers.FileSavers;
using HeyRed.Mime;
using Microsoft.Extensions.Options;
using System.Text;

namespace ContentRatingAPI.Infrastructure.ContentFileManagers
{
    public class ContentFileMongoManager : IContentFileManager
    {
        private readonly IOptions<ContentFileOptions> options;
        private readonly ISavedContentStorage savedContentStorage;
        private readonly IDictionary<ContentType, FileSaverBase> fileSavers;


        public ContentFileMongoManager(IOptions<ContentFileOptions> options, ISavedContentStorage savedContentStorage, IDictionary<ContentType, FileSaverBase> fileSavers)
        {
            this.options = options;
            this.savedContentStorage = savedContentStorage;
            this.fileSavers = fileSavers;
        }

        public async Task RemoveFile(Guid id, CancellationToken cancellationToken = default)
        {
            var savedContent = await savedContentStorage.GetSavedContent(id);
            if(cancellationToken.IsCancellationRequested) return;
            await savedContentStorage.DeleteSavedContent(id);
            // maybe should transfer delete logic in this class
            savedContent.RemoveFile();
        }

        public Task<int> RemoveUnusedSavedContentFiles(TimeSpan notUseMinTime, CancellationToken cancellationToken = default)
        {
            // should implement new class who by mongo collections find api path saved content in the end of 
            throw new NotImplementedException();
        }

        public async Task<SavedContentFileInfo> SaveNewContentFile(string fileName, byte[] contentBytes, CancellationToken cancellationToken = default)
        {
            var contentType = GetContentTypeByFileName(fileName);
            if (!fileSavers.TryGetValue(contentType, out FileSaverBase? value))
                throw new NotImplementedException("Content type");
            var fileSaver = value;
            var newContent = await fileSaver.SaveFile(Guid.NewGuid(), Path.GetExtension(fileName), contentBytes, cancellationToken);
            await savedContentStorage.Add(newContent);
            return newContent;
        }
        public async Task<ContentFile> GetFile(Guid id, string baseUrlForSegmentManifest, CancellationToken cancellationToken = default)
        {
            var savedContentFile = await savedContentStorage.GetSavedContent(id);
            var mimeType = MimeTypesMap.GetMimeType(savedContentFile.Path);
            if (savedContentFile.IsSegmented)
                return await CreateSegmentedFile(savedContentFile, mimeType, baseUrlForSegmentManifest, cancellationToken);
            var fileBytes = await File.ReadAllBytesAsync(savedContentFile.Path, cancellationToken);
            return new ContentFile(fileBytes, savedContentFile.Path, mimeType);

        }

        public async Task<ContentFile> GetFileSegment(Guid id, string segmentName, CancellationToken cancellationToken = default)
        {
            var savedContentFile = await savedContentStorage.GetSavedContent(id);
            if (!savedContentFile.IsSegmented)
                throw new ArgumentException("File is not segmented");
            var parentDirectoryPath = Directory.GetParent(savedContentFile.Path)!.ToString();
            var fileNames = Directory.GetFiles(parentDirectoryPath);
            var segmentFileName = fileNames.First(c => c.Contains(segmentName));
            var mimeType = MimeTypesMap.GetMimeType(segmentFileName);
            var fileBytes = await File.ReadAllBytesAsync(segmentFileName, cancellationToken);
            return new ContentFile(fileBytes, segmentFileName, mimeType);

        }
        private async Task<ContentFile> CreateSegmentedFile(SavedContentFileInfo savedContentFile, string mimeType, string baseUrlForSegmentManifest, CancellationToken cancellationToken)
        {
            if (mimeType != "application/vnd.apple.mpegurl")
                throw new NotSupportedException("Unknown format");
            // TODO IF add new content should divide logic 
            var fileStringHLS = await File.ReadAllTextAsync(savedContentFile.Path, cancellationToken);
            var fileName = Path.GetFileNameWithoutExtension(savedContentFile.Path);
            fileStringHLS = fileStringHLS.Replace(fileName, $"{baseUrlForSegmentManifest}/{fileName}");
            var fileBytes = Encoding.UTF8.GetBytes(fileStringHLS);
            return new ContentFile(fileBytes, savedContentFile.Path, mimeType);
        }
        private static ContentType GetContentTypeByFileName(string fileName)
        {
            var mimeType = MimeTypesMap.GetMimeType(fileName);
            if (mimeType.StartsWith("video"))
                return ContentType.Video;
            if (mimeType.StartsWith("audio"))
                return ContentType.Audio;
            if (mimeType.StartsWith("image"))
                return ContentType.Image;
            throw new NotImplementedException(mimeType);
        }

    }
}
