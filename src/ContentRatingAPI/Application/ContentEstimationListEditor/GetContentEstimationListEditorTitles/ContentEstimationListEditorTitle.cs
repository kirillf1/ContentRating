namespace ContentRatingAPI.Application.ContentEstimationListEditor.GetContentEstimationListEditorTitles
{
    public class ContentEstimationListEditorTitle
    {
        public ContentEstimationListEditorTitle(Guid id, string name, int addedContentCount, string creatorName)
        {
            Id = id;
            Name = name;
            AddedContentCount = addedContentCount;
            CreatorName = creatorName;
        }

        public Guid Id { get; }
        public string Name { get; }
        public int AddedContentCount { get; }
        public string CreatorName { get; }
    }
}
