// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;
using ContentRating.Domain.Shared;
using ContentRatingAPI.Infrastructure.Data.Caching;

namespace ContentRatingAPI.Infrastructure.Data.Repositories
{
    public class CachingContentEstimationListEditorRepository : IContentEstimationListEditorRepository
    {
        private readonly IContentEstimationListEditorRepository baseRepository;
        private readonly GenericCacheBase<ContentEstimationListEditor> cache;

        public CachingContentEstimationListEditorRepository(
            IContentEstimationListEditorRepository baseRepository,
            GenericCacheBase<ContentEstimationListEditor> cacheBase
        )
        {
            this.baseRepository = baseRepository;
            cache = cacheBase;
        }

        public IUnitOfWork UnitOfWork => baseRepository.UnitOfWork;

        public ContentEstimationListEditor Add(ContentEstimationListEditor editor)
        {
            cache.Set(editor.Id, editor);
            return baseRepository.Add(editor);
        }

        public void Delete(ContentEstimationListEditor editor)
        {
            cache.Remove(editor.Id);
            baseRepository.Delete(editor);
        }

        public async Task<ContentEstimationListEditor?> GetContentEstimationListEditor(Guid id)
        {
            if (cache.TryGetValue(id, out var listEditor) && listEditor is not null)
            {
                return listEditor;
            }

            listEditor = await baseRepository.GetContentEstimationListEditor(id);

            if (listEditor is not null)
            {
                cache.Set(id, listEditor);
            }

            return listEditor;
        }

        public async Task<bool> HasEditorInContentEstimationList(Guid listId, Guid editorId)
        {
            if (cache.TryGetValue(listId, out var contentEstimationListEditor) && contentEstimationListEditor is not null)
            {
                return contentEstimationListEditor!.ContentListCreator.Id == editorId
                    || contentEstimationListEditor.InvitedEditors.Any(c => c.Id == editorId);
            }
            return await baseRepository.HasEditorInContentEstimationList(listId, editorId);
        }

        public ContentEstimationListEditor Update(ContentEstimationListEditor editor)
        {
            return baseRepository.Update(editor);
        }
    }
}
