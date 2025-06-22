using ContentRating.Web.UI.ViewModels.Content;

namespace ContentRating.Web.UI.Services.Content
{
    public class ContentItemEditingService
    {
        private readonly Dictionary<Guid, ContentItemBackup> _editingItems = new();

        public void StartEditing(ContentItemViewModel item)
        {
            // Сохраняем резервную копию оригинальных значений
            _editingItems[item.Id] = new ContentItemBackup
            {
                OriginalName = item.Name,
                OriginalUrl = item.Url,
                OriginalContentType = item.ContentType,
            };

            item.IsEditing = true;
        }

        public void StopEditing(ContentItemViewModel item)
        {
            item.IsEditing = false;
            _editingItems.Remove(item.Id);
        }

        public void CancelEditing(ContentItemViewModel item)
        {
            if (_editingItems.TryGetValue(item.Id, out var backup))
            {
                item.Name = backup.OriginalName;
                item.Url = backup.OriginalUrl;
                item.ContentType = backup.OriginalContentType;
            }

            StopEditing(item);
        }

        public ContentItemViewModel? GetCurrentEditingItem(IEnumerable<ContentItemViewModel> items)
        {
            return items.FirstOrDefault(x => x.IsEditing);
        }

        public void CancelAllEditing(IEnumerable<ContentItemViewModel> items)
        {
            foreach (var item in items.Where(x => x.IsEditing))
            {
                CancelEditing(item);
            }
        }

        private class ContentItemBackup
        {
            public string OriginalName { get; set; } = string.Empty;
            public string OriginalUrl { get; set; } = string.Empty;
            public Domain.Shared.Content.ContentType OriginalContentType { get; set; }
        }
    }
}
