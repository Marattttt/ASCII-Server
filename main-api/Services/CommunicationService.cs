using System.Text;
using System.Text.Json;

using api.Models;

namespace api.Services;

public class CommunicationService 
{
    //Needs changing
    string _processingUrl = "http://localhost:5001/ascii/converttotxtfile";
    public CommunicationService()
    {
    }
    public async Task<string?> ProcessImageAsync(ImageToAsciiDTO dto)
    {
        string? result = null;        
        using (HttpClient client = new HttpClient())
        {   
            StringContent content = new StringContent(
                JsonSerializer.Serialize<ImageToAsciiDTO>(dto),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage responseMessage = await client.PostAsync(_processingUrl, content);
            if (responseMessage.ToString().Length < 1)
                return null;
            Console.WriteLine(responseMessage);
        }
        return result;
    } 
}