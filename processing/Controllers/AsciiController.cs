using System.Text;
using Microsoft.AspNetCore.Mvc;

using shared.DTOs;
using processing.Services;
using shared.Config;

namespace processing.Controllers;

[ApiController]
[Route("ascii")]
public class AsciiController : ControllerBase
{
    Processor _processor;

    public AsciiController(AsciiProcessor asciiProcessor) {
        _processor = asciiProcessor;
    }

    //Response contains body ascii text int UTF-8 encoding
    //Max file length is specified in the file config class
    //http://processing/ascii/convert + body with file
    [HttpPost("convert")]
    [Consumes("image/png", "image/webp", "image/bmp", "image/pbm", "image/tga", "image/jpeg", "image/tiff")]
    public async  Task<ActionResult> Convert(
        [FromQuery] int newWidth = 0,
        [FromQuery] int newHeight = 0) {

        if (Request.ContentLength is null || Request.ContentLength == 0) {
            return BadRequest("Body empty");
        }
        if (Request.ContentLength > FileConfig.MaxFileSizeBytes) {
            return BadRequest("Body too large");
        }

        byte[] result = new byte[(int)Request.ContentLength];
        using (var memoryStream = new MemoryStream()) {
            await HttpContext.Request.Body.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            bool isImage = await _processor.IsImage(memoryStream);
            if (!isImage) {
                return BadRequest("Not an image");
            }
            memoryStream.Seek(0, SeekOrigin.Begin);

            await Task.Run(() => 
                    _processor.Process(
                        memoryStream,
                        out result,
                        newWidth, newHeight));
            memoryStream.Seek(0, SeekOrigin.Begin);
        }
        
        await Response.Body.WriteAsync(result);
        return new EmptyResult();

    }
}
