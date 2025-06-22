namespace ContentRating.Web.UI.ViewModels.Shared
{
    public class RaterViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsMock => Name.StartsWith("Mock:");
        public string DisplayName => IsMock ? Name.Substring(5) : Name; // Убираем префикс "Mock:" для отображения
    }
}
