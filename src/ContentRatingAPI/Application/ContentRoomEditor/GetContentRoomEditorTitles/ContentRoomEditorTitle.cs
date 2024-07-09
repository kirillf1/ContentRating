namespace ContentRatingAPI.Application.ContentRoomEditor.GetContentRoomEditorTitles
{
    public class ContentRoomEditorTitle
    {
        public ContentRoomEditorTitle(Guid id, string name, int addedContentCount, string creatorName)
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
