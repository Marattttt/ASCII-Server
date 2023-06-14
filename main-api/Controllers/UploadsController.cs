using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

using shared.DTOs;
using shared.Config;
using api.Services;

[ApiController]
[Route("uploads")]
public class UploadsController : ControllerBase {
    IFilesManager _filesManager;
    IUploadsManager _uploadsManager;
    public UploadsController(
        LocalFilesManager filesManager, 
        ApiUploadsManager uploadsManager) {
        
        _filesManager = filesManager;
        _uploadsManager = uploadsManager;
    }

    //https://uploads/user/1/new + body
    [HttpPost("user/{userId:int}/new")]
    public async Task<ActionResult> UploadImage(
        [FromForm] IFormFile file,
        [FromForm] string fileName,
        [FromRoute] int userId) {

        if (file.Length > FileConfig.MaxFileSizeBytes) {
            return BadRequest("File too large");
        }
        if (file.Length == 0) {
            return BadRequest("File length 0");
        }

        ImageDataDTO dto = new ImageDataDTO(){
            UserId = userId,
            FileName = fileName,
            FileType = Path.GetExtension(file.FileName)
        };
        
        dto.Content = new byte[file.Length];
        using (var stream = file.OpenReadStream()) {
            await stream.ReadAsync(dto.Content, 0, (int)file.Length);
        }

        string? uploadErrorMessage = await _uploadsManager.UploadImageAsync(dto);
        if (uploadErrorMessage is not null) {
            return BadRequest(uploadErrorMessage);
        }
        return NoContent();
    }

    [HttpGet("user/{userId:int}/images/")]
    public async Task<ActionResult> GetImage(
        [FromRoute] int userId,
        [FromHeader] string fileName) {

        var result = await _uploadsManager.GetImageAsync(userId, fileName);
        if (result.errorMessage is not null) {
            return BadRequest(result.errorMessage.ToString());
        }
        if (result.stream is null) {
            return BadRequest("Unknown error");
        }
        // string contentType = ContentType.
        byte[] content = new byte[result.stream.Length];
        await result.stream.ReadAsync(content, 0, (int)result.stream.Length);
        return File(content, MediaTypeNames.Application.Octet);
    }
}