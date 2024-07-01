namespace ContentRatingAPI.Application.ContentFileManager
{
    public interface IContentFileManager
    {
        Task<SavedContentFileInfo> SaveNewContentFile(string fileName, byte[] contentBytes, CancellationToken cancellationToken = default);
        Task<int> RemoveUnusedSavedContentFiles(TimeSpan notCheckedTime, CancellationToken cancellationToken = default);
        Task<ContentFile> GetFile(Guid id, string baseUrlForSegmentManifest, CancellationToken cancellationToken = default);
        Task<ContentFile> GetFileSegment(Guid id, string segmentName, CancellationToken cancellationToken = default);
        Task RemoveFile(Guid id, CancellationToken cancellationToken = default);
    }
}
