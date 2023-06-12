using Microsoft.AspNetCore.Mvc;

using shared.DTOs;
using shared.DataChecking;

using storage.Services;
using storage.Models;

namespace storage.Controllers;

[ApiController]
[Route("")] //https://storage/
public class MainController: ControllerBase {

    //up to 500 chars with the utf 8 max single char size of 4 bytes in mind
    const int MaxBufferSize = 500 * 4; 
    UsersService _usersService;

    public MainController(UsersService imagesService) {
        _usersService = imagesService;
    }

    [HttpPost("user/new/")]
    public async Task<ActionResult<User>> CreateNewUser(FullUserInfoDTO dto) {
        string dtoErrorMessage = UserDataChecker.CheckFullUserDto(dto);
        if (dtoErrorMessage != String.Empty) {
            return BadRequest(dtoErrorMessage);
        }
        
        User? existingUser = await _usersService.GetUserAsync(dto.UserId);
        if (existingUser is not null) {
            return BadRequest("User already exists");
        }
        User? newUser = await _usersService.CreateUserAsync(dto);
        if (newUser is null) {
            return BadRequest("Could not create user from DTO");
        }
        return NoContent();
    }

    [HttpGet("user/{id:int}")] //http://storage/user/123
    public async Task<ActionResult<User>> GetUserById(int id) {
        User? user = await _usersService.GetUserAsync(id);
        if (user is null) {
            return BadRequest("User not found");
        }
        return Ok(user);
    }

    //Requires FileName to save as and path to the temporary file for contents
    //http://storage/user/images/new
    [HttpPost("user/images/new")] 
    public async Task<ActionResult> SaveImageFromLocalStorage(
        [FromBody] ImageDataDTO dto) {
        User? user = await _usersService.GetUserAsync(dto.UserId);
        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(dto));

        if (user is null) {
            return BadRequest("User not found");
        }

        if (dto.Path is null) {
            return BadRequest("Path to content is not provided");
        }
    
        using (FileStream fs = new FileStream(dto.Path, FileMode.Open)) {
            dto.Content = new byte[fs.Length];
            await fs.ReadAsync(dto.Content, 0, (int)fs.Length);
        }
        
        ImageData imageData = new ImageData(dto);

        UsersServiceResult result = await _usersService.SaveImageDataAsync(user, imageData);
        if (result == UsersServiceResult.Success) {
            return NoContent();
        }
        return BadRequest(result.ToString());
    }


    //Requires an http get request with id defined in route and body containing
    //the needed file name in utf 8 encoding
    //Maximum body length is int32 max value
    //http://storage/user/1/images/ + body="image.png"
    [HttpGet("user/{userId:int}/images/")]
    [Consumes("text/plain")]
    public async Task<ActionResult<string>>  GetPathToImage(
        [FromRoute] int userId) {
        User? user = await _usersService.GetUserAsync(userId);
        if (user is null) {
            return BadRequest("User not found");
        }

        string fileName = String.Empty;
        try {
            fileName = await new StreamReader(Request.Body).ReadToEndAsync();
        } catch (ArgumentOutOfRangeException) {
            return BadRequest("Body longer than int32 max value");
        }

        var result = _usersService.GetImageData(user, fileName);
        if (result.imageData is null) {
            return BadRequest(result.result.ToString());
        }

        string tempPath = await FileStorage.SaveToTempFilePath(result.imageData.Content);

        return Ok(tempPath);
    }
}