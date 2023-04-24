using Microsoft.AspNetCore.Mvc;

using processing.Services;
using processing.Models;

namespace processing.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AsciiController : ControllerBase
{
    ProcessingService _processingService;
    public AsciiController(ProcessingService service)
    {
        _processingService = service;
    }

    [HttpPost]
    public string Convert(ImageToAsciiDTO dto)
    {
        string createdPath = _processingService.Process(
            dto.Path, dto.Width, dto.Height, dto.OutPath);
        return createdPath;
    }
}
