using System.Net;
using System.Text;
using System.Text.Json;

using api.Models;

namespace api.Services;

public class CommunicationService 
{
    //Needs changing
    string _processingUrl = "http://localhost:5001/convert";
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
            if (responseMessage.StatusCode != HttpStatusCode.Created)
                result = null;
            else 
                result = responseMessage.Content.ToString();
            Console.WriteLine(responseMessage);
        }
        return result;
    }
}