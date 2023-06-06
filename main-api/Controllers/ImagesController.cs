using Microsoft.AspNetCore.Mvc;

using shared.DTOs;

using api.Services;

namespace api.Controllers;

[Route("[controller]")]
public class ImagesController : ControllerBase
{
    ImagesService _imagesService;
    CommunicationService _communicationService;
    public ImagesController(ImagesService imagesService, CommunicationService communicationService) {
        _imagesService = imagesService;
        _communicationService = communicationService;
    }

    [HttpPost("UploadAndProcess")]
    public async Task<ActionResult> UploadAndProcess(IFormFile file) {
        if (file is null) {
            return BadRequest("File not received");
        }
        file.FileName.Trim().ToLowerInvariant();
        string? userId = GetUserAsyncId();
        if (userId is null) {
            return BadRequest("No user-id query parameter specified");
        }
        userId.Trim().ToLowerInvariant();

        string? outPath = await _imagesService.SaveFileAsync(file, userId);
        if (outPath is null) {
            return BadRequest("Error saving file");
        }

        ImageToAsciiDTO dto = new ImageToAsciiDTO() {
            Path = outPath
        };
        string? processingResult = await _communicationService.ProcessImageAsync(dto);
        if (processingResult is null) {
            return BadRequest("Error processing file");
        }
        return NoContent();
    }

    [HttpGet()]
    public ActionResult Download() {
        string? fileName = Request.Query["file-name"];
        if (fileName is null) {
            return BadRequest("No file-name query parameter");
        }
        fileName.Trim().ToLowerInvariant();
        string? userId = GetUserAsyncId();
        if (userId is null) {
            return BadRequest("No user-id query parameter");
        }

        Stream? stream = _imagesService.GetFile(fileName, userId);
        if(stream == null) {
            return BadRequest("File not found"); 
        }

        string mimeType = "application/octet-stream";

        return File(stream, mimeType, fileName);
    }    
    private string? GetUserAsyncId() {
        return Request.Query["user-id"];
    }
}