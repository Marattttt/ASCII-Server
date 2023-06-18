using System.Text;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

using shared.DTOs;
using shared.Config;
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
    public async Task<ActionResult<FullUserInfoDTO?>> GetUserById(int id) {
        User? user = await _usersService.GetUserAsync(id);
        if (user is null) {
            return BadRequest("User not found");
        }
        FullUserInfoDTO dto = user.ToDto();
        return Ok(dto);
    }

    //http://storage/user/images/new
    [HttpPost("user/images/new")]
    public async Task<ActionResult> SaveImage(
        [FromHeader] int UserId,
        [FromHeader] string FileName,
        [FromHeader] string FileType,
        [FromForm] IFormFile image,
        [FromForm] IFormFile? processed) {
            Console.WriteLine("SaveProcessedImage invoked");

        ImageDataDTO dto = new ImageDataDTO() {
            UserId = UserId,
            FileName = FileName,
            FileType = FileType
        };

        if (Request.Form.Files.Count() > 2 || Request.Form.Files.Count() == 0) {
            return BadRequest(
                "This endpoint needs 1-2 file:" + 
                "1 for the image itself (name:image) and " +
                "1 (optional) for the processing result (name:processed)");
        }
        if (Request.Form.Files.Any(file => file.Length <= 1)) {
            return BadRequest("File length too short");
        }
        if (Request.Form.Files.Any(file => file.Length >= Int32.MaxValue)) {
            return BadRequest("File length bigger than or equal to Int32.MaxValue");
        }

        User? user = await _usersService.GetUserAsync(dto.UserId);
        if (user is null) {
            return BadRequest("User not found");
        }

        int imageLength = (int)image.Length;
        dto.Content = new byte[imageLength];
        await image.OpenReadStream().ReadAsync(dto.Content);
        
        if (processed is not null) {
            int processedLength = (int)processed.Length;
            dto.Text = new byte[processedLength];
            await processed.OpenReadStream().ReadAsync(dto.Text);
        }

        using (var ms = new MemoryStream(dto.Text!)) {
            var reader = new StreamReader(ms, System.Text.Encoding.UTF8);
            string? test = reader.ReadLine();
            Console.WriteLine(test);
        }

        UsersServiceResult result = await _usersService
            .SaveImageDataAsync(user, new ImageData(dto));

        if (result != UsersServiceResult.Success) {
            return BadRequest(result.ToString());
        }
        return NoContent();
    }


    //Requires an http get request with id defined in route 
    //and header with the needed fileName
    //http://storage/user/1/images/ + header="fileName:image.png"
    [HttpGet("user/{userId:int}/images/")]
    public async Task<ActionResult> GetImage(
        [FromRoute] int userId,
        [FromHeader] string fileName) {
            
        Console.WriteLine(fileName);
        User? user = await _usersService.GetUserAsync(userId);
        if (user is null) {
            return BadRequest("User not found");
        }

        var result = _usersService.GetImageData(user, fileName);
        if (result.imageData is null) {
            return BadRequest(result.result.ToString());
        }

        return File(result.imageData.Content, MediaTypeNames.Application.Octet);
    }

    [HttpGet("user/{userId:int}/images/processed")]
    public async Task<ActionResult> GetProcessed (
        [FromRoute] int userId,
        [FromHeader] string fileName) {

        User? user = await _usersService.GetUserAsync(userId);
        if (user is null) {
            return BadRequest("User not found");
        }

        var result = _usersService.GetImageData(user, fileName);
        if (result.imageData is null) {
            return BadRequest(result.result.ToString());
        }
        if (result.imageData.Text is null ||
            result.imageData.Text.Count() == 0) {
            return BadRequest("No saved text");
        }
        using (var ms = new MemoryStream(result.imageData.Text)) {
            var reader = new StreamReader(ms, new UTF8Encoding(true));
            Response.ContentType = MediaTypeNames.Text.Plain;
            return Ok(await reader.ReadToEndAsync());
        }

        // return File(result.imageData.Text, MediaTypeNames.Text.Plain);
    }
    
    
    [HttpDelete("user/{userId:int}")]
    public async Task<ActionResult> DeleteUser([FromRoute] int userId) {
        User? user = await _usersService.GetUserAsync(userId);
        if (user is null) {
            return BadRequest("User not found");
        }
        await _usersService.DeleteUser(user);
        return NoContent();
    }

    [HttpDelete("user/{userId:int}/images/")]
    public async Task<ActionResult> DeleteImage(
        [FromRoute] int userId,
        [FromHeader] string fileName) {
        
        User? user = await _usersService.GetUserAsync(userId, true);
        if (user is null) {
            return BadRequest("User not found");
        }
        await _usersService.DeleteImage(user, fileName);
        return NoContent();
    }
}