using System.ComponentModel.DataAnnotations;
using ContentRating.Domain.Shared.Content;

namespace ContentRating.Web.UI.ViewModels.Content
{
    public class ContentItemViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(
            300,
            MinimumLength = 1,
            ErrorMessage = "Название должно содержать от 1 до 300 символов"
        )]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "URL обязателен")]
        [Url(ErrorMessage = "Введите корректный URL")]
        public string Url { get; set; } = string.Empty;

        public ContentType ContentType { get; set; } = ContentType.Video;
        public Guid CreatorId { get; set; }
        public DateTime LastModificationDate { get; set; }
        public bool IsEditing { get; set; } = false;

        // Для отмены изменений
        public string OriginalName { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;
        public ContentType OriginalContentType { get; set; } = ContentType.Video;

        public string ContentTypeDisplayName =>
            ContentType switch
            {
                ContentType.Audio => "Аудио",
                ContentType.Video => "Видео",
                ContentType.Image => "Изображение",
                _ => "Неизвестно",
            };

        public void RestoreOriginalValues()
        {
            Name = OriginalName;
            Url = OriginalUrl;
            ContentType = OriginalContentType;
        }
    }
}
