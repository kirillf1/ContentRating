namespace ContentRatingAPI.Application.ContentFileManager
{
    public interface IContentFileManager
    {
        Task<Result<SavedContentFileInfo>> SaveNewContentFile(string fileName, byte[] contentBytes, CancellationToken cancellationToken = default);
        Task<Result<int>> RemoveUnusedSavedContentFiles(TimeSpan notCheckedTime, CancellationToken cancellationToken = default);
        Task<Result<ContentFile>> GetFile(Guid id, string baseUrlForSegmentManifest, CancellationToken cancellationToken = default);
        Task<Result<ContentFile>> GetFileSegment(Guid id, string segmentName, CancellationToken cancellationToken = default);
        Task<Result> RemoveFile(Guid id, CancellationToken cancellationToken = default);
    }
}
