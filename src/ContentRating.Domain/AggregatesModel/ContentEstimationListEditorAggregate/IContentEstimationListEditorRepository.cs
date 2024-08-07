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
