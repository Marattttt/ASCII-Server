using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;

using shared.DTOs;
using shared.DataChecking;

using api.Services;

namespace api.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase {
    IUsersManager _usersManager;
    public UsersController(
        StorageUsersManager usersManager) {
        _usersManager = usersManager;
    }

    //https://api/users/new
    [HttpPost("new")]
    public async Task<ActionResult> CreateUser([FromForm] FullUserInfoDTO dto) {
        string dtoErrorMessage = UserDataChecker.CheckFullUserDto(dto);
        if (dtoErrorMessage != String.Empty) {
            return BadRequest(dtoErrorMessage);
        }
        string createUserErrorMessage = await _usersManager.CreateUserAsync(dto);
        if (createUserErrorMessage != String.Empty) {
            return BadRequest(createUserErrorMessage);
        }
        return NoContent();
    }

    //https://api/users/delete
    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteUser ([FromForm] FullUserInfoDTO dto) {
        string dtoErrorMessage = UserDataChecker.CheckFullUserDto(dto);
        if (dtoErrorMessage != String.Empty) {
            return BadRequest(dtoErrorMessage);
        }
        var existingUser = await _usersManager.GetUserInfoAsync(dto.UserId);
        if (existingUser is null) {
            return BadRequest("User not found");
        }
        
        if (existingUser.Password == dto.Password && existingUser.UserName == dto.UserName) {
            await _usersManager.DeleteUserAsync(dto.UserId);
            return NoContent();
        }
        
        return Unauthorized();
    }
}