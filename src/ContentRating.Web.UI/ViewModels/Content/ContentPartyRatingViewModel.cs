using ContentRating.Domain.Shared.Content;
using ContentRating.Web.Contracts.ContentPartyEstimationRoom;

namespace ContentRating.Web.UI.ViewModels.Content
{
    public class ContentPartyRatingViewModel
    {
        public Guid RatingId { get; set; }
        public Guid ContentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public ContentType ContentType { get; set; }
        public Guid CreatorId { get; set; }
        public double AverageRating { get; set; }
        public List<RaterRatingViewModel> Ratings { get; set; } = new();

        // UI состояние
        public string ErrorMessage { get; set; } = string.Empty;
        public bool HasError { get; set; } = false;

        public string ContentTypeDisplayName =>
            ContentType switch
            {
                ContentType.Video => "Видео",
                ContentType.Audio => "Аудио",
                ContentType.Image => "Изображение",
                _ => "Неизвестно",
            };

        public static ContentPartyRatingViewModel FromResponse(ContentRatingResponse response)
        {
            return new ContentPartyRatingViewModel
            {
                RatingId = response.RatingId,
                ContentId = response.ContentId,
                Name = response.Name,
                Address = response.Address,
                ContentType = response.ContentType,
                CreatorId = response.CreatorId,
                AverageRating = response.AverageRating,
                Ratings = response
                    .Ratings.Select(r => new RaterRatingViewModel
                    {
                        RaterId = r.RaterId,
                        Rating = r.Rating,
                    })
                    .ToList(),
            };
        }
    }

    public class RaterRatingViewModel
    {
        public Guid RaterId { get; set; }
        public double Rating { get; set; }
        public string RaterName { get; set; } = string.Empty;
    }
}
