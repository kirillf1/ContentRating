using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;
using ContentRating.Domain.Shared;
using Microsoft.Extensions.Caching.Memory;

namespace ContentRatingAPI.Infrastructure.Data.Repositories
{
    public class CachingContentEstimationListEditorRepository : IContentEstimationListEditorRepository
    {
        public readonly static string ContentListEditorKey = nameof(ContentEstimationListEditor);
        private readonly IContentEstimationListEditorRepository baseRepository;
        private readonly IMemoryCache memoryCache;
        private readonly MemoryCacheEntryOptions cacheEntryOptions;

        public CachingContentEstimationListEditorRepository(IContentEstimationListEditorRepository baseRepository, IMemoryCache memoryCache)
        {
            this.baseRepository = baseRepository;
            this.memoryCache = memoryCache;
            cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        }
        public IUnitOfWork UnitOfWork => baseRepository.UnitOfWork;

        public ContentEstimationListEditor Add(ContentEstimationListEditor editor)
        {
            return baseRepository.Add(editor);
        }

        public void Delete(ContentEstimationListEditor editor)
        {
            memoryCache.Remove(GetKeyById(editor.Id));
            baseRepository.Delete(editor);
        }

        public async Task<ContentEstimationListEditor?> GetContentEstimationListEditor(Guid id)
        {
            return await memoryCache.GetOrCreateAsync(GetKeyById(id), async entry =>
                {
                    entry.SetOptions(cacheEntryOptions);
                    return await baseRepository.GetContentEstimationListEditor(id);
                });
        }

        public async Task<bool> HasEditorInContentEstimationList(Guid listId, Guid editorId)
        {
            if (memoryCache.TryGetValue(GetKeyById(listId), out ContentEstimationListEditor? contentEstimationListEditor))
            {
                return contentEstimationListEditor!.RoomCreator.Id == editorId || contentEstimationListEditor.InvitedEditors.Any(c => c.Id == editorId);
            }
            return await baseRepository.HasEditorInContentEstimationList(listId, editorId);
        }

        public ContentEstimationListEditor Update(ContentEstimationListEditor editor)
        {
            return baseRepository.Update(editor);
        }
        private static string GetKeyById(Guid id)
        {
            return ContentListEditorKey + id.ToString();
        }
    }
}
