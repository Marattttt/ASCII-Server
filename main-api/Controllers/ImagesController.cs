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
    public async Task<ActionResult> UploadAndProcess(IFormFile file)
    {
        string? outPath = await _imagesService.SaveFileAsync(
            file, 
            HttpContext.Request.Host.Value);

        if (outPath is null)
            return BadRequest("Error saving file");

        ImageToAsciiDTO dto = new ImageToAsciiDTO() {
            Path = outPath
        };
        string? processingResult = await _communicationService.ProcessImageAsync(dto);
        if (processingResult is null)
            return BadRequest("Error processing file");
        return NoContent();
    }
}