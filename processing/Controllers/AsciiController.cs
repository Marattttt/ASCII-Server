using Microsoft.AspNetCore.Mvc;

using shared.DTOs;
using processing.Services;

namespace processing.Controllers;

[ApiController]
[Route("[action]")]
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
        Response.StatusCode = 201;
        return createdPath;
    }
}
