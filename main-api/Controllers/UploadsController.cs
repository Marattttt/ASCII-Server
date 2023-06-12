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

    //https://uploads/1/new + body
    [HttpPost("{userId:int}/new")]
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
            FileName = fileName
        };
        
        byte[] buffer = new byte[file.Length];
        using (var stream = file.OpenReadStream()) {
            await stream.ReadAsync(buffer, 0, (int)file.Length);
        }

        dto.Path = await _filesManager.SaveDataAsync(buffer, Path.GetExtension(fileName));
        string? uploadErrorMessage = await _uploadsManager.UploadImageAsync(dto);
        if (uploadErrorMessage is not null) {
            return BadRequest(uploadErrorMessage);
        }
        return NoContent();
    }
}