using Microsoft.AspNetCore.Mvc;
using api.Services;

namespace api.Controllers;

[Route("[controller]/[action]")]
public class ImagesController 
{
    ImagesService _imagesService;
    public ImagesController(ImagesService imagesService)
    {
        _imagesService = imagesService;
    }

    [HttpGet()]
    public string Test() 
    {
        return "Success!";
    }

    [HttpPost()]
    public async Task<int> Upload(IFormFile file)
    {
        
        return await _imagesService.SaveFileAsync(file);
    }
}