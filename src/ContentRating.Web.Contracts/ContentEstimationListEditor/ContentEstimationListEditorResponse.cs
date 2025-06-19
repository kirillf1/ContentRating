using System.Text.Json.Serialization;

namespace ContentRating.Web.Contracts.ContentEstimationListEditor
{
    public class ContentEstimationListEditorResponse
    {
        public ContentEstimationListEditorResponse(
            Guid id,
            string name,
            string creatorName,
            Guid creatorId,
            IEnumerable<ContentResponse> content,
            IEnumerable<InvitedEditorResponse> invitedEditors
        )
        {
            Id = id;
            Name = name;
            CreatorName = creatorName;
            CreatorId = creatorId;
            Content = content;
            InvitedEditors = invitedEditors;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CreatorName { get; set; }
        public Guid CreatorId { get; set; }
        public IEnumerable<ContentResponse> Content { get; set; }
        public IEnumerable<InvitedEditorResponse> InvitedEditors { get; set; }
    }
}
