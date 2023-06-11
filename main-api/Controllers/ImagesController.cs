using Microsoft.AspNetCore.Mvc;

using shared.DTOs;
using shared.DataChecking;

using api.Services;

namespace api.Controllers;

[Route("")]
public class ImagesController : ControllerBase {
    IImageProcessor _imgProcessor;
    IUsersManager _usersManager;
    public ImagesController(StorageUsersManager manager, ApiImageProcessor processor) {
        _imgProcessor = processor;
        _usersManager = manager;
    }

    [HttpPost("user/new")]
    public async Task<ActionResult> CreateUserAsync(FullUserInfoDTO dto) {
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