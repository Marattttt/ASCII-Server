using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("[controller]/[action]")]
public class ImagesController 
{
    public ImagesController()
    {
    }

    [HttpGet()]
    public string Test() 
    {
        return "Success!";
    }
}