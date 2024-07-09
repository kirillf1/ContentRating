namespace ContentRatingAPI.Application.ContentRoomEditor.GetContentRoomEditor
{
    public class ContentRoomEditorResponse
    {
        public ContentRoomEditorResponse(Guid id, string name, string creatorName, Guid creatorId, IEnumerable<ContentResponse> content,
            IEnumerable<InvitedEditorResponse> invitedEditors)
        {
            Id = id;
            Name = name;
            CreatorName = creatorName;
            CreatorId = creatorId;
            Content = content;
            InvitedEditors = invitedEditors;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string CreatorName { get; }
        public Guid CreatorId { get; }
        public IEnumerable<ContentResponse> Content { get; }
        public IEnumerable<InvitedEditorResponse> InvitedEditors { get; }
    }
}
