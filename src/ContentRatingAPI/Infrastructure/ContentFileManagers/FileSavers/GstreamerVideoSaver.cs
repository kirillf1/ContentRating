using System.Globalization;
using System.Text.RegularExpressions;
using ContentRatingAPI.Application.ContentFileManager;
using GLib;
using Gst;
using HeyRed.Mime;
using Microsoft.Extensions.Options;
using GstApp = Gst.Application;
using SysDateTime = System.DateTime;

namespace ContentRatingAPI.Infrastructure.ContentFileManagers.FileSavers
{
    public class GstreamerVideoSaver : FileSaverBase
    {
        private readonly ILogger<GstreamerVideoSaver> _logger;

        public GstreamerVideoSaver(
            IOptions<ContentFileOptions> options,
            ILogger<GstreamerVideoSaver> logger
        )
            : base(options)
        {
            _logger = logger;

            // Инициализируем GStreamer
            GstApp.Init();
        }

        public override async System.Threading.Tasks.Task<SavedContentFileInfo> SaveFile(
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

            // Сохраняем исходный файл во временную папку
            var tempInputFile = Path.Combine(Path.GetTempPath(), $"{fileName}{fileExtension}");
            await File.WriteAllBytesAsync(tempInputFile, data, cancellationToken);

            try
            {
                await ConvertToHls(
                    tempInputFile,
                    Path.Combine(filePath, fileName),
                    cancellationToken
                );
            }
            finally
            {
                if (File.Exists(tempInputFile))
                {
                    File.Delete(tempInputFile);
                }
            }

            return new SavedContentFileInfo(
                fileId,
                SysDateTime.UtcNow,
                Path.Combine(filePath, fileName + ".m3u8"),
                ContentRating.Domain.Shared.Content.ContentType.Video
            );
        }

        private async System.Threading.Tasks.Task ConvertToHls(
            string inputFile,
            string outputPath,
            CancellationToken cancellationToken
        )
        {
            var tcs = new TaskCompletionSource<bool>();
            var cancellationRegistration = cancellationToken.Register(
                () => tcs.TrySetCanceled(cancellationToken)
            );

            try
            {
                _logger.LogInformation("Starting convert video to hls");

                var segmentLocation = $"{outputPath}%03d.ts";
                var playlistLocation = $"{outputPath}.m3u8";

                var formattedInputFile = FormatPathForGStreamer(inputFile);
                var formattedSegmentLocation = FormatPathForGStreamer(segmentLocation);
                var formattedPlaylistLocation = FormatPathForGStreamer(playlistLocation);

                // Простой пайплайн без перекодирования - просто разбиваем на сегменты
                var pipelineDescription =
                    $"filesrc location=\"{formattedInputFile}\" ! "
                    + "qtdemux name=demux ! "
                    + "h264parse ! queue ! mux. "
                    + "demux. ! aacparse ! queue ! mux. "
                    + "mpegtsmux name=mux ! "
                    + "hlssink max-files=0 playlist-length=0 target-duration=10 "
                    + $"location=\"{formattedSegmentLocation}\" playlist-location=\"{formattedPlaylistLocation}\"";

                _logger.LogDebug("GStreamer pipeline: {Pipeline}", pipelineDescription);

                var pipeline = Parse.Launch(pipelineDescription);
                var mainLoop = new GLib.MainLoop();
                var bus = pipeline.Bus;
                bus.AddSignalWatch();

                bus.Message += (sender, args) =>
                {
                    var message = args.Message;
                    _logger.LogDebug(
                        "GStreamer message: {MessageType} from {Source}",
                        message.Type,
                        message.Src?.Name ?? "unknown"
                    );

                    switch (message.Type)
                    {
                        case MessageType.Eos:
                            _logger.LogInformation(
                                "Video conversion completed successfully (EOS received)"
                            );
                            tcs.TrySetResult(true);
                            mainLoop.Quit();
                            break;
                        case MessageType.Error:
                            message.ParseError(out var gerror, out var debug);
                            _logger.LogError(
                                "GStreamer error: {Error}, Debug: {Debug}",
                                gerror,
                                debug
                            );
                            mainLoop.Quit();
                            tcs.TrySetException(new Exception($"GStreamer error: {debug}"));
                            break;
                        case MessageType.Warning:
                            message.ParseWarning(out var gwarning, out var wdebug);
                            _logger.LogWarning(
                                "GStreamer warning: {Warning}, Debug: {Debug}",
                                gwarning,
                                wdebug
                            );
                            break;
                        case MessageType.Info:
                            message.ParseInfo(out var ginfo, out var idebug);
                            _logger.LogInformation(
                                "GStreamer info: {Info}, Debug: {Debug}",
                                ginfo,
                                idebug
                            );
                            break;
                        case MessageType.StateChanged:
                            message.ParseStateChanged(
                                out var oldState,
                                out var newState,
                                out var pendingState
                            );
                            _logger.LogDebug(
                                "Pipeline state changed from {OldState} to {NewState} (element: {Element})",
                                oldState,
                                newState,
                                message.Src?.Name ?? "unknown"
                            );
                            break;
                        case MessageType.StreamStatus:
                            _logger.LogDebug(
                                "Stream status message from {Element}",
                                message.Src?.Name ?? "unknown"
                            );
                            break;
                    }
                };

                var stateChangeReturn = pipeline.SetState(State.Playing);
                if (stateChangeReturn == StateChangeReturn.Failure)
                {
                    throw new Exception("Failed to start GStreamer pipeline");
                }
                _logger.LogInformation("Pipeline started, waiting for completion...");
                mainLoop.Run();

                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
                using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken,
                    timeoutCts.Token
                );

                try
                {
                    await tcs.Task.WaitAsync(combinedCts.Token);
                }
                catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
                {
                    throw new TimeoutException("Video conversion timed out");
                }
                finally { }

                _logger.LogInformation("Pipeline completed, cleaning up...");

                pipeline.SetState(State.Null);
                bus.RemoveSignalWatch();
                pipeline.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to convert video file: {InputFile}", inputFile);
                throw;
            }
            finally
            {
                await cancellationRegistration.DisposeAsync();
            }
        }

        private static string FormatPathForGStreamer(string path)
        {
            if (OperatingSystem.IsWindows())
            {
                // На Windows экранируем обратные слеши для GStreamer
                return path.Replace("\\", "\\\\");
            }
            else
            {
                // На Linux/macOS используем пути как есть
                return path;
            }
        }
    }
}
