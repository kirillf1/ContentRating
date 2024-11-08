// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using ContentRatingAPI.Application.ContentFileManager;
using HeyRed.Mime;
using Microsoft.Extensions.Options;

namespace ContentRatingAPI.Infrastructure.ContentFileManagers.FileSavers
{
    public partial class HLSVideoSaver : FileSaverBase
    {
        private const double DefaultFPS = 30;
        private const double DefaultVideoBitrateKbs = 256;
        private const double DefaultAudioBitrateKbs = 128;

        private record VideoInfo(double VideoBitrate, double FPS, double AudioBitrate);

        private readonly IOptions<FFMPEGOptions> fFMPEGOptions;

        public HLSVideoSaver(IOptions<ContentFileOptions> options, IOptions<FFMPEGOptions> FFMPEGOptions)
            : base(options)
        {
            fFMPEGOptions = FFMPEGOptions;
        }

        public override async Task<SavedContentFileInfo> SaveFile(
            Guid fileId,
            string fileExtension,
            byte[] data,
            CancellationToken cancellationToken = default
        )
        {
            var mimeType = MimeTypesMap.GetMimeType(fileExtension);
            if (!mimeType.StartsWith("video"))
            {
                throw new ArgumentException("File extension must be video");
            }

            var fileName = fileId.ToString();
            var filePath = Path.Combine(options.Value.Directory, "videos", $"segments_{fileName}");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var videoInfo = await GetVideoInfo(data, cancellationToken);
            await ConvertToHls(data, Path.Combine(filePath, fileName), videoInfo);
            return new SavedContentFileInfo(
                fileId,
                DateTime.UtcNow,
                Path.Combine(filePath, fileName + ".m3u8"),
                ContentRating.Domain.Shared.Content.ContentType.Video
            );
        }

        private async Task ConvertToHls(byte[] videoData, string outputPath, VideoInfo videoInfo)
        {
            var arguments =
                $"-i - -c:v libx264 -b:v {videoInfo.VideoBitrate}k -b:a {videoInfo.AudioBitrate}k -hls_time {fFMPEGOptions.Value.SegmentTime} -hls_list_size 0 -r {videoInfo.FPS} -hls_segment_filename \"{outputPath}%03d.ts\" \"{outputPath}.m3u8\"";
            using CancellationTokenSource cts = new(TimeSpan.FromMinutes(5));
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fFMPEGOptions.Value.FFMPEGPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                },
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            using (var stdin = process.StandardInput.BaseStream)
            {
                stdin.Write(videoData, 0, videoData.Length);
            }
            await process.WaitForExitAsync(cts.Token);
        }

        private async Task<VideoInfo> GetVideoInfo(byte[] videoData, CancellationToken cancellationToken = default)
        {
            try
            {
                var arguments = $"-i - -f null -";

                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = fFMPEGOptions.Value.FFMPEGPath,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = false,
                    },
                };
                var output = new StringBuilder();
                process.OutputDataReceived += (sender, e) => output.Append(e.Data);
                process.ErrorDataReceived += (sender, e) => output.Append(e.Data);
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                using (var stdin = process.StandardInput.BaseStream)
                {
                    await stdin.WriteAsync(videoData, 0, videoData.Length);
                }

                await process.WaitForExitAsync(cancellationToken);
                var regexVideoBitrate = VideoRegex();
                var regexFps = FPSRegex();
                var regexAudioBitrate = AudioRegex();

                var stringResult = output.ToString();

                var matchVideoBitrate = regexVideoBitrate.Match(stringResult);
                if (!double.TryParse(matchVideoBitrate.Groups[1].Value, CultureInfo.InvariantCulture, out var bitrateVideo))
                {
                    bitrateVideo = DefaultVideoBitrateKbs;
                }
                else
                {
                    var bitrateUnitVideo = matchVideoBitrate.Groups[2].Value;
                    if (bitrateUnitVideo == "Mb/s")
                    {
                        bitrateVideo *= 1000;
                    }
                }
                var matchFps = regexFps.Match(stringResult);
                if (!double.TryParse(matchFps.Groups[1].Value, CultureInfo.InvariantCulture, out var fps))
                {
                    fps = DefaultFPS;
                }

                var matchAudioBitrate = regexAudioBitrate.Match(stringResult);
                if (!double.TryParse(matchAudioBitrate.Groups[1].Value, CultureInfo.InvariantCulture, out var bitrateAudio))
                {
                    bitrateAudio = DefaultAudioBitrateKbs;
                }
                else
                {
                    var bitrateUnitAudio = matchAudioBitrate.Groups[2].Value;
                    if (bitrateUnitAudio == "Mb/s")
                    {
                        bitrateAudio *= 1000;
                    }
                }
                return new VideoInfo(bitrateVideo, fps, bitrateAudio);
            }
            catch (Exception ex)
            {
                return new VideoInfo(DefaultVideoBitrateKbs, DefaultFPS, DefaultAudioBitrateKbs);
            }
        }

        [GeneratedRegex(@"Audio:\s.+?,\s(\d+)\s+(kb/s|Mb/s)")]
        private static partial Regex AudioRegex();

        [GeneratedRegex(@"\s.+?,\s(\d+)\s+fps")]
        private static partial Regex FPSRegex();

        [GeneratedRegex(@"Video:\s.+?,\s(\d+)\s+(kb/s|Mb/s)")]
        private static partial Regex VideoRegex();
    }
}
