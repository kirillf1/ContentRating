namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate
{
    public class ContentModificationHistory : ValueObject
    {
        public ContentModificationHistory(Guid editorId)
        {
            EditorId = editorId;
            LastContentModifiedDate = DateTime.UtcNow;
        }

        public Guid EditorId { get; }
        public DateTime LastContentModifiedDate { get; }
        public ContentModificationHistory MarkContentModification()
        {
            return new ContentModificationHistory(EditorId);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return EditorId;
            yield return LastContentModifiedDate;
        }
    }
}
