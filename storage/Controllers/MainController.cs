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
    public async Task<ActionResult<User>> GetUserById(int id) {
        User? user = await _usersService.GetUserAsync(id);
        if (user is null) {
            return BadRequest("User not found");
        }
        return Ok(user);
    }

    //
    //http://storage/user/images/new
    [HttpPost("user/images/new")]
    [Consumes("application/octet-stream")]
    public async Task<ActionResult> SaveImage(
        [FromHeader] int UserId,
        [FromHeader] string FileName,
        [FromHeader] string FileType,
        [FromHeader] int Length) {
        ImageDataDTO dto = new ImageDataDTO() {
            UserId = UserId,
            FileName = FileName,
            FileType = FileType
        };
        if (Length == 0) {
            return BadRequest("Length header = 0");
        }

        User? user = await _usersService.GetUserAsync(dto.UserId);
        if (user is null) {
            return BadRequest("User not found");
        }

        dto.Content = new byte[Length];

        await Request.Body.ReadAsync(dto.Content, 0, Length);

        UsersServiceResult result = await _usersService.SaveImageDataAsync(user, new ImageData(dto));
        if (result != UsersServiceResult.Success) {
            return BadRequest(result.ToString());
        }
        return NoContent();
    }


    //Requires an http get request with id defined in route and body containing
    //the needed file name in utf 8 encoding
    //Maximum body length is int32 max value
    //http://storage/user/1/images/ + body="image.png"
    [HttpGet("user/{userId:int}/images/")]
    [Consumes("text/plain")]
    public async Task<ActionResult>  GetImage(
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

        return File(result.imageData.Content, MediaTypeNames.Application.Octet);
    }
}