
using Microsoft.AspNetCore.Mvc;

using api.Services;
using api.Models;

namespace api.Controllers;

[Route("[controller]/[action]")]
public class ImagesController : ControllerBase
{
    ImagesService _imagesService;
    CommunicationService _communicationService;
    public ImagesController(ImagesService imagesService, CommunicationService communicationService)
    {
        _imagesService = imagesService;
        _communicationService = communicationService;
    }

    [HttpPost()]
    public async Task<ActionResult<string>> Upload(IFormFile file)
    {
        string? outPath = await _imagesService.SaveFileAsync(
            file, 
            HttpContext.Request.Host.Value);

        if (outPath is null)
            return BadRequest();

        ImageToAsciiDTO dto = new ImageToAsciiDTO() {
            Path = outPath
        };
        return await _communicationService.ProcessImageAsync(dto);

    }
}