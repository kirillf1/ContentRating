namespace ContentRatingAPI.Application.ContentFileManager
{
    public class SavedFileResponse
    {
        public required Guid Id { get; set; }
        public required string FileRoute { get; set; }
    }
}
