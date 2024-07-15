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
        private readonly ILogger<ContentFileMongoManager> logger;

        public ContentFileMongoManager(IOptions<ContentFileOptions> options, ISavedContentStorage savedContentStorage, IDictionary<ContentType, FileSaverBase> fileSavers,
            ILogger<ContentFileMongoManager> logger)
        {
            this.options = options;
            this.savedContentStorage = savedContentStorage;
            this.fileSavers = fileSavers;
            this.logger = logger;
        }

        public async Task<Result> RemoveFile(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var savedContent = await savedContentStorage.GetSavedContent(id);
                if (savedContent is null)
                    return Result.NotFound();
                cancellationToken.ThrowIfCancellationRequested();
                await savedContentStorage.DeleteSavedContent(id);
                // maybe should transfer delete logic in this class
                savedContent.RemoveFile();
                logger.LogInformation("File {fileName} deleted", savedContent.Path);
                return Result.Success();
            }
            catch(Exception ex)
            {
                logger.LogError("Can't delete file with id {id}, error: {ex}", id, ex);
                return Result.Error(ex.Message);
            }
        }

        public Task<Result<int>> RemoveUnusedSavedContentFiles(TimeSpan notUseMinTime, CancellationToken cancellationToken = default)
        {
            try
            {
                // should implement new class who by mongo collections find api path saved content in the end of 
                throw new NotImplementedException();
            }
            catch(Exception ex)
            {
                throw new NotImplementedException();
            }
        }

        public async Task<Result<SavedContentFileInfo>> SaveNewContentFile(string fileName, byte[] contentBytes, CancellationToken cancellationToken = default)
        {
            try
            {
                var contentType = GetContentTypeByFileName(fileName);
                if (!fileSavers.TryGetValue(contentType, out FileSaverBase? value))
                    return Result.Invalid(new ValidationError("Unknown content type"));
                var fileSaver = value;
                var newContent = await fileSaver.SaveFile(Guid.NewGuid(), Path.GetExtension(fileName), contentBytes, cancellationToken);
                await savedContentStorage.Add(newContent);
                logger.LogInformation("Saved file: {fileName}, id: {id}", newContent.Path, newContent.Id);
                return newContent;
            }
            catch(Exception ex)
            {
                logger.LogError("Can't save file: {fileName}, error: {ex}", fileName, ex);
                return Result.Error(ex.Message);
            }
        }
        public async Task<Result<ContentFile>> GetFile(Guid id, string baseUrlForSegmentManifest, CancellationToken cancellationToken = default)
        {
            try
            {
                var savedContentFile = await savedContentStorage.GetSavedContent(id);
                if (savedContentFile is null)
                    return Result.NotFound();
                var mimeType = MimeTypesMap.GetMimeType(savedContentFile.Path);
                if (savedContentFile.IsSegmented)
                    return await CreateSegmentedFile(savedContentFile, mimeType, baseUrlForSegmentManifest, cancellationToken);
                var fileBytes = await File.ReadAllBytesAsync(savedContentFile.Path, cancellationToken);
                return new ContentFile(fileBytes, savedContentFile.Path, mimeType);
            }
            catch(Exception ex)
            {
                logger.LogError("Can't get file with id: {id}, error: {ex}", id, ex);
                return Result.Error(ex.Message);
            }

        }

        public async Task<Result<ContentFile>> GetFileSegment(Guid id, string segmentName, CancellationToken cancellationToken = default)
        {
            try
            {
                var savedContentFile = await savedContentStorage.GetSavedContent(id);
                if (savedContentFile is null)
                    return Result.NotFound();
                if (!savedContentFile.IsSegmented)
                    return Result.Invalid(new ValidationError("File is not segmented"));

                var parentDirectoryPath = Directory.GetParent(savedContentFile.Path)!.ToString();
                var fileNames = Directory.GetFiles(parentDirectoryPath);
                var segmentFileName = fileNames.First(c => c.Contains(segmentName));
                var mimeType = MimeTypesMap.GetMimeType(segmentFileName);
                var fileBytes = await File.ReadAllBytesAsync(segmentFileName, cancellationToken);
                return new ContentFile(fileBytes, segmentFileName, mimeType);
            }
            catch (Exception ex)
            {
                logger.LogError("Can't get file with id: {id}, segment: {segment} error: {ex}", id, segmentName, ex);
                return Result.Error(ex.Message);
            } 

        }
        private static async Task<ContentFile> CreateSegmentedFile(SavedContentFileInfo savedContentFile, string mimeType, string baseUrlForSegmentManifest, CancellationToken cancellationToken)
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
