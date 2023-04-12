using System.Security.AccessControl;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize()]
    public async Task<int> Upload(IFormFile file, HttpContext context)
    {
        try { 
            return await _imagesService.SaveFileAsync(
                file, 
                context.Connection.RemoteIpAddress!.ToString());
        }
        catch (ArgumentNullException e) 
        {
            Console.WriteLine("IP adress inaccessible");
            Console.WriteLine(e.Message);
            return 0;
        }

    }
}