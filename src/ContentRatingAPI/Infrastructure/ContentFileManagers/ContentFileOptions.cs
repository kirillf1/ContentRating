namespace ContentRatingAPI.Infrastructure.ContentFileManagers
{
    public class ContentFileOptions
    {
        public string Directory { get; set; } = "";
        public double OldCheckedFileTakeIntervalInHours { get; set; } = 24;
        public double CheckUnusedFilesIntervalInHours { get; set; } = 24;
    }
}
