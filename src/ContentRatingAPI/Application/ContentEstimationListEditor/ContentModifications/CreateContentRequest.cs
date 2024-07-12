using ContentRating.Domain.Shared.Content;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public class CreateContentRequest
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Path { get; set; }
        public required ContentType ContentType { get; set; }
    }
}
