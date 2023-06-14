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

    [HttpPost("user/new")]
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
}