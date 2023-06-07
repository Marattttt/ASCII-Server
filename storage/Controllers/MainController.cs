using System.Xml;
using Microsoft.AspNetCore.Mvc;

using shared.DTOs;
using storage.Services;
using storage.Models;

namespace storage.Controllers;

[ApiController]
[Route("")] //https://storage/
public class MainController: ControllerBase {
    UsersService _usersService;

    public MainController(UsersService imagesService) {
        _usersService = imagesService;
    }

    [HttpGet("user/new/")]
    public async Task<ActionResult<User>> CreateNewUser(FullUserInfoDTO dto) {
        User? existingUser = await _usersService.GetUserAsync(dto.UserId);
        if (existingUser is not null) {
            return BadRequest("User already exists");
        }
        User? newUser = await _usersService.CreateUserAsync(dto);
        if (newUser is null) {
            return BadRequest("Could not create user from DTO");
        }
        return newUser;
    }

    [HttpGet("user/{id:int}")] //http://storage/user/123
    public async Task<ActionResult<User>> GetUserAsyncById(int id) {
        User? user = await _usersService.GetUserAsync(id);
        if (user is null) {
            return BadRequest("User not found");
        }
        return Ok(user);
    }

    [HttpPost("user/images/new")] //http:storage/user/images/new + form with image data DTO
    public async Task<ActionResult> SaveImageAsync(
        [FromForm] ImageDataDTO dto) {
        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(dto));

        User? user = await _usersService.GetUserAsync(dto.UserId);

        if (user is null) {
            return BadRequest("User not found");
        }

        using (MemoryStream memoryStream = new MemoryStream()) {
            Stream? stream = Request.Form.Files[0]?.OpenReadStream();
            if (stream is null || stream.Length == 0) {
                return BadRequest("Cannot find file");
            }
            
            await stream.CopyToAsync(memoryStream);
            dto.Content = memoryStream.ToArray();
        }
        ImageData imageData = new ImageData() {
            Content = new byte[0]
        };
        try {

            imageData = new ImageData(dto);
        }
        catch (ArgumentNullException) {
            return  BadRequest("Cannot read image data");
        }

        UsersServiceResult result = await _usersService.SaveImageDataAsync(user, imageData);
        if (result == UsersServiceResult.Success) {
            return NoContent();
        }
        return BadRequest(result);
    }

    [HttpGet("user/{userId:int}/images/{fileName:alpha}")]
    public async Task<ActionResult<string>>  GetPathToImage(int userId, string fileName) {
        User? user = await _usersService.GetUserAsync(userId);
        if (user is null) {
            return BadRequest("User not found");
        }

        var result = _usersService.GetImageData(user, fileName);
        if (result.imageData is null) {
            return BadRequest(result.result.ToString());
        }

        string tempPath = await FileStorageService.SaveToTempFilePath(result.imageData.Content);

        return Ok(tempPath);
    }
}