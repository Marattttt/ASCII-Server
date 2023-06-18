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

    UsersService _usersService;

    public MainController(UsersService imagesService) {
        _usersService = imagesService;
    }

    [HttpPost()]
    [Route("users/new/")]
    [Produces(MediaTypeNames.Text.Plain)]
    public async Task<ActionResult> CreateNewUser(FullUserInfoDTO dto) {
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

    //http://storage/user/123
    [HttpGet()]
    [Route("user/{id:int}")] 
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<FullUserInfoDTO?>> GetUserById(int id) {
        User? user = await _usersService.GetUserAsync(id);
        if (user is null) {
            return BadRequest("User not found");
        }
        FullUserInfoDTO dto = user.ToDto();
        return Ok(dto);
    }

    //http://storage/user/1/images/ + header="fileName:image.png"
    //Requires an http get request with id defined in route 
    //and header with the needed fileName
    [HttpGet()]
    [Route("user/{userId:int}/images/")]
    [Produces(MediaTypeNames.Application.Octet)]
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

    //http://user/1/images/processed
    [HttpGet()]
    [Route("user/{userId:int}/images/processed")]
    [Produces(MediaTypeNames.Text.Plain)]
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
    }
    
//http://storage/user/images/new
    [HttpPost()]
    [Route("user/images/new")]
    [Consumes("multipart/form-data")]
    [Produces(MediaTypeNames.Text.Plain)]
    public async Task<ActionResult> SaveImage(
        [FromHeader] int UserId,
        [FromHeader] string FileName,
        [FromHeader] string FileType,
        IFormFile image,
        IFormFile? processed) {

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

        UsersServiceResult result = await _usersService
            .SaveImageDataAsync(user, new ImageData(dto));

        if (result != UsersServiceResult.Success) {
            return BadRequest(result.ToString());
        }
        return NoContent();
    }

    //http://storage/user/1
    [HttpDelete()]
    [Route("user/{userId:int}")]
    [Produces(MediaTypeNames.Text.Plain)]
    public async Task<ActionResult> DeleteUser([FromRoute] int userId) {
        User? user = await _usersService.GetUserAsync(userId);
        if (user is null) {
            return BadRequest("User not found");
        }
        await _usersService.DeleteUser(user);
        return NoContent();
    }

    //http://storage/user/1/images
    [HttpDelete("user/{userId:int}/images/")]
    [Produces(MediaTypeNames.Text.Plain)]
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