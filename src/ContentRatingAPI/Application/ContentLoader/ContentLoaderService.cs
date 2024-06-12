using SharpCompress.Common;
using System.Diagnostics;

namespace ContentRatingAPI.Application.ContentLoader
{
    public class ContentLoaderService
    {
        public async Task SaveToHLS(string path, string outPath, string name)
        {
            ConvertToHls(path, Path.Combine(outPath, "hls", name));
        }
        private void ConvertToHls(string inputPath, string outputPath)
        {

            var arguments = $"-i \"{inputPath}\" -c:v libx264 -b:v 1M -hls_time 10 -hls_list_size 0 -hls_segment_filename \"{outputPath}%03d.ts\" \"{outputPath}.m3u8\"";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"ffmpeg.exe",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            process.WaitForExit();
        }
    }
}
