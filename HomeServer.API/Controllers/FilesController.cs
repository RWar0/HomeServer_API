using HomeServer.API.Common.Extensions;
using HomeServer.Application.Interfaces.MyFiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;
        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet("image/{photoId}")]
        public async Task<IActionResult> GetImage([FromRoute] Guid photoId)
        {
            var result = await _fileService.GetImage(photoId);

            if (result.IsFailure)
            {
                return result.ToActionResult(this);
            }

            return PhysicalFile(result.Value.RelativePath, result.Value.ContentType);
        }
    }
}
