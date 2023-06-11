using System.Net;
using System.Text;
using System.Text.Json;

using shared.DTOs;
using shared.Config;

namespace api.Services;

public class ApiImageProcessor : IImageProcessor {

    // Sends an http post request to url defined in CommunicationUrls class
    public async Task<string?> ProcessImageAsync(ImageToAsciiDTO dto) {
        string? result = null;        
        using (HttpClient client = new HttpClient()) {   
            StringContent content = new StringContent(
                JsonSerializer.Serialize<ImageToAsciiDTO>(dto),
                Encoding.UTF8,
                "application/json"
            );
            HttpResponseMessage responseMessage = await client.PostAsync(
                CommunicationUrls.ProcessingUrl, 
                content
            );
            if (responseMessage.StatusCode != HttpStatusCode.Created)
                result = null;
            else 
                result = responseMessage.Content.ToString();
            Console.WriteLine(responseMessage);
        }
        return result;
    }
}