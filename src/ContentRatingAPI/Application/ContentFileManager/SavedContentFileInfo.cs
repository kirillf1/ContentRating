using ContentRating.Domain.Shared.Content;
using HeyRed.Mime;

namespace ContentRatingAPI.Application.ContentFileManager
{
    public class SavedContentFileInfo
    {
        private static readonly List<string> _segmentedMimeTypes = ["application/x-mpegURL", "application/vnd.apple.mpegurl", "application/dash+xml"];
        public SavedContentFileInfo(Guid id, DateTime creationDate, string path, ContentType contentType)
        {
            Id = id;
            CreationDate = creationDate;
            Path = path;
            ContentType = contentType;
        }
        public ContentType ContentType { get; private set; }
        public Guid Id { get; init; }
        public DateTime CreationDate { get; init; }
        public string Path { get; private set; }
        public DateTime? LastCheckDate { get; set; }
        public bool IsSegmented => _segmentedMimeTypes.Contains(MimeTypesMap.GetMimeType(Path));
        public bool RemoveFile()
        {
            var isSegmented = IsSegmented;
            var path = isSegmented ? Directory.GetParent(Path)?.ToString() : Path;
            if (isSegmented)
                return RemoveDirectory(path);
            return RemoveFile(path);
        }

        private static bool RemoveFile(string? path)
        {
            if (!File.Exists(path))
                return false;
            File.Delete(path);
            return true;
        }

        private static bool RemoveDirectory(string? path)
        {
            if (!Directory.Exists(path))
                return false;
            Directory.Delete(path, true);
            return true;
        }
    }
}
