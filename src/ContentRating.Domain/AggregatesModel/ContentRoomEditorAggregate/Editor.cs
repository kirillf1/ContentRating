namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate
{
    public class Editor : Entity
    {
        public Editor(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Name { get; }
        public Content CreateContent(ContentData contentData)
        {
            var contentCreator = new ContentModificationHistory(Id);
            return new Content(contentData.Id, contentData.Path, contentData.Name,
                contentData.Type, contentCreator);
        }
    }
}
