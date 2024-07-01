using ContentRatingAPI.Application.ContentFileManager;
using ContentRatingAPI.Models.ContentFiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContentRatingAPI.Controllers
{
    
    [Route($"api/content-files")]
    [ApiController]
    [Authorize]
    public class ContentFilesController : ControllerBase
    {
        private readonly IContentFileManager contentFileManager;

        public ContentFilesController(IContentFileManager contentFileManager)
        {
            this.contentFileManager = contentFileManager;
        }
        [AllowAnonymous]
        [HttpGet("{fileId:guid}")]
        public async Task<IActionResult> GetFile(Guid fileId)
        {
            try
            {
                var baseUrlForManifest = Url.Action(nameof(GetFile), null, null, HttpContext.Request.Scheme, Request.Host.ToUriComponent());
                var file = await contentFileManager.GetFile(fileId, baseUrlForManifest);
                return File(file.Data, file.ContentType);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        [AllowAnonymous]
        [HttpGet("{fileId:guid}/{segment}")]
        public async Task<IActionResult> GetFileSegment(Guid fileId, string segment)
        {
            try
            {
                var fileSegment = await contentFileManager.GetFileSegment(fileId, segment);
                return File(fileSegment.Data, fileSegment.ContentType);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        [RequestSizeLimit(200_000_000)]
        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer);
            var newFileInfo = await contentFileManager.SaveNewContentFile(file.FileName, buffer);
            var response = new SavedFileResponse
            {
                Id = newFileInfo.Id,
                FileRoute = Url.Action(nameof(GetFile), null, new { fileId = newFileInfo.Id }, HttpContext.Request.Scheme, Request.Host.ToUriComponent())!
            };
            return Ok(response);
        }
        [HttpDelete("{fileId:guid}")]
        public async Task<IActionResult> RemoveFile(Guid fileId)
        {
            await contentFileManager.RemoveFile(fileId);
            return Ok();
        }

    }
}
