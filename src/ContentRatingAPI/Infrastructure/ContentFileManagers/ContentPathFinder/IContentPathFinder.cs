
namespace ContentRatingAPI.Infrastructure.ContentFileManagers.ContentFilePathFinder
{
    public interface IContentPathFinder
    {
        Task<bool> HasFileIdInContent(Guid savedFileInfoId);
    }
}