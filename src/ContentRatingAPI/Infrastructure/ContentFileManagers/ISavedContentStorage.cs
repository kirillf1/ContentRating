using ContentRatingAPI.Application.ContentFileManager;

namespace ContentRatingAPI.Infrastructure.ContentFileManagers
{
    public interface ISavedContentStorage
    {
        Task<SavedContentFileInfo> GetSavedContent(Guid Id);
        Task<IEnumerable<SavedContentFileInfo>> GetOldCheckedOrUncheckedContent(TimeSpan checkInterval);
        Task Update(SavedContentFileInfo file);
        Task DeleteSavedContent(Guid Id);
        Task Add(SavedContentFileInfo file);
    }
}
