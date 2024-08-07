using Ardalis.Result.AspNetCore;
using ContentRatingAPI.Application.ContentFileManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentRatingAPI.Controllers
{

    [Route("api/content-files")]
    [ApiController]
    [Authorize]
    public class ContentFilesController : ControllerBase
    {
        private readonly IContentFileManager contentFileManager;

        public ContentFilesController(IContentFileManager contentFileManager)
        {
            this.contentFileManager = contentFileManager;
        }
        [HttpGet("{fileId:guid}")]
        public async Task<IActionResult> GetFile(Guid fileId)
        {
            var baseUrlForManifest = Url.Action(nameof(GetFile), null, null, HttpContext.Request.Scheme, Request.Host.ToUriComponent());
            var fileResult = await contentFileManager.GetFile(fileId, baseUrlForManifest);
            if (fileResult.IsSuccess)
                return File(fileResult.Value.Data, fileResult.Value.ContentType);
            if (fileResult.IsNotFound())
                return NotFound();
            if (fileResult.IsInvalid())
                return BadRequest(string.Join("\n\r", fileResult.ValidationErrors.Select(c => c.ErrorMessage)));
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        [AllowAnonymous]
        [HttpGet("{fileId:guid}/{segment}")]
        public async Task<IActionResult> GetFileSegment(Guid fileId, string segment)
        {

            var fileResult = await contentFileManager.GetFileSegment(fileId, segment);
            if (fileResult.IsSuccess)
                return File(fileResult.Value.Data, fileResult.Value.ContentType);
            if (fileResult.IsNotFound())
                return NotFound();
            if (fileResult.IsInvalid())
                return BadRequest(string.Join("\n\r", fileResult.ValidationErrors.Select(c => c.ErrorMessage)));
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);

        }
        [TranslateResultToActionResult]
        [RequestSizeLimit(200_000_000)]
        [HttpPost]
        public async Task<Result<SavedFileResponse>> AddFile(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer);
            var newFileInfoResult = await contentFileManager.SaveNewContentFile(file.FileName, buffer);
            if (newFileInfoResult.IsSuccess)
            {          
                var response = new SavedFileResponse
                {
                    Id = newFileInfoResult.Value.Id,
                    FileRoute = Url.Action(nameof(GetFile), null, new { fileId = newFileInfoResult.Value.Id }, HttpContext.Request.Scheme, Request.Host.ToUriComponent())!
                };
                return response;
            }
            if (newFileInfoResult.IsInvalid())
                return Result.Invalid(newFileInfoResult.ValidationErrors);
            return Result.Error(new ErrorList(newFileInfoResult.Errors));



        }
        [TranslateResultToActionResult]
        [HttpDelete("{fileId:guid}")]
        public async Task<Result> RemoveFile(Guid fileId)
        {
            return await contentFileManager.RemoveFile(fileId);
        }

    }
}
