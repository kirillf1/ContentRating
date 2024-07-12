using ContentRating.Domain.Shared.Content;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public class UpdateContentRequest
    {
        public required string Name { get; set; }
        public required string Path { get; set; }
        public required ContentType ContentType { get; set; }
    }
}
