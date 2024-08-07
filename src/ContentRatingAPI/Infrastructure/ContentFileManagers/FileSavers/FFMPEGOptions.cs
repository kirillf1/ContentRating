namespace ContentRatingAPI.Infrastructure.ContentFileManagers.FileSavers
{
    public class FFMPEGOptions
    {
        public string FFMPEGPath { get; set; } = "ffmpeg";
        public double SegmentTime { get; set; } = 3;
    }
}
