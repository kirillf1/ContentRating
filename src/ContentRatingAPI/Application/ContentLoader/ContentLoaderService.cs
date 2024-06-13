using System.Diagnostics;

namespace ContentRatingAPI.Application.ContentLoader
{
    public class ContentLoaderService
    {
        public async Task SaveToHLS(byte[] videoBytes, string outPath, string name)
        {
            ConvertToHls(videoBytes, Path.Combine(outPath, "hls", name));
        }
        private void ConvertToHls(byte[] videoData, string outputPath)
        {
            var arguments = $"-i - -c:v libx264 -b:v 1M -hls_time 30 -hls_list_size 0 -r 20 -preset ultrafast -hls_segment_filename \"{outputPath}%03d.ts\" \"{outputPath}.m3u8\"";

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"ffmpeg",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = false,

                }
            };
            process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
            process.ErrorDataReceived += (sender, e) => Console.WriteLine("ERROR: " + e.Data);
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            using (Stream stdin = process.StandardInput.BaseStream)
            {
                stdin.Write(videoData, 0, videoData.Length);
            }
            process.WaitForExit();
        }
    }
}
