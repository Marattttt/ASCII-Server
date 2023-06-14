using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

using shared.DTOs;
using shared.Config;
using shared.DataChecking;
using api.Services;

[ApiController]
[Route("uploads")]
public class UploadsController : ControllerBase {
    IFilesManager _filesManager;
    IUploadsManager _uploadsManager;
    IUsersManager _usersManager;
    public UploadsController(
        LocalFilesManager filesManager, 
        ApiUploadsManager uploadsManager, 
        StorageUsersManager storageUsersManager) {
        
        _filesManager = filesManager;
        _uploadsManager = uploadsManager;
        _usersManager = storageUsersManager;
    }

    //https://api/uploads/user/1/images/new
    [HttpPost("user/{userId:int}/images/new")]
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

    //https://api/uploads/user/1/images
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

        byte[] content = new byte[result.stream.Length];
        await result.stream.ReadAsync(content, 0, (int)result.stream.Length);
        return File(content, MediaTypeNames.Application.Octet);
    }

    //https://api/uploads/delete
    //All data is from form
    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteImage(
        [FromForm] FullUserInfoDTO dto,
        [FromForm] string fileName) {

        string dtoErrorMessage = UserDataChecker.CheckFullUserDto(dto);
        if (dtoErrorMessage != String.Empty) {
            return BadRequest(dtoErrorMessage);
        }
        var existingUser = await _usersManager.GetUserInfoAsync(dto.UserId);
        if (existingUser is null) {
            return BadRequest("User not found");
        }

        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(existingUser));

        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(dto));

        if (existingUser.Password != dto.Password || existingUser.UserName != dto.UserName) {
            return Unauthorized();
        }

        string? errorMessage = await _uploadsManager.DeleteImageAsync(dto.UserId, fileName);
        
        if (errorMessage is not null) {
            return BadRequest(errorMessage);
        }
        return NoContent();
    }
}