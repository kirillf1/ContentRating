namespace ContentRatingAPI.Application.ContentRoomEditor.GetContentRoomEditor
{
    public class InvitedEditorResponse
    {
        public InvitedEditorResponse(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }
        public string Name { get; }
    }
}
