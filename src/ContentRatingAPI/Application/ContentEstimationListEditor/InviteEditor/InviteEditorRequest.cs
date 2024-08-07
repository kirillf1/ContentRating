namespace ContentRatingAPI.Application.ContentEstimationListEditor.InviteEditor
{
    public class InviteEditorRequest
    {
        public required Guid EditorId { get; set; }
        public required string EditorName { get; set; }
    }
}
