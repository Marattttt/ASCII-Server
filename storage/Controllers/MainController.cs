using Microsoft.AspNetCore.Mvc;

using storage.Services;
using storage.Models;

namespace storage.Controllers;

[ApiController]
[Route("")] //https://storage/
public class MainController: ControllerBase {
    ImagesService _imagesService;

    public MainController(ImagesService imagesService) {
        _imagesService = imagesService;
    }

    [HttpGet("user/{id}")] //https://storage/user/123
    public async Task<ActionResult<User>> GetUserById(int id) {
        User? user = await _imagesService.GetUser(id);
        if (user is null) {
            return BadRequest("User not found");
        }
        return Ok(user);
    }
}