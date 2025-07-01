using DoeTech.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DoeTech.Controllers;

[ApiController]
[Route("api/files")]
public class FileController : ControllerBase
{
    private readonly FileService _fileService;

    public FileController(FileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("{type}/{id}")]
    public async Task<IActionResult> Upload(
        [FromRoute] FileType type,
        [FromRoute] Guid id,
        [FromForm] IFormFile file)
    {
        var url = await _fileService.UploadAsync(type, id, file);
        return Ok(new { url });
    }

    [HttpGet("{type}/{id}")]
    public IActionResult GetPresignedUrl(
        [FromRoute] FileType type,
        [FromRoute] Guid id)
    {
        var url = _fileService.GetPresignedUrl(type, id);
        return Ok(new { url });
    }
}