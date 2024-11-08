// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate
{
    public interface IContentEstimationListEditorRepository : IRepository<ContentEstimationListEditor>
    {
        ContentEstimationListEditor Add(ContentEstimationListEditor editor);
        ContentEstimationListEditor Update(ContentEstimationListEditor editor);
        void Delete(ContentEstimationListEditor editor);
        Task<ContentEstimationListEditor?> GetContentEstimationListEditor(Guid id);
        Task<bool> HasEditorInContentEstimationList(Guid listId, Guid editorId);
    }
}
