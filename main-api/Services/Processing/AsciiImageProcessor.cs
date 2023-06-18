using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

using shared.DTOs;
using shared.Config;

namespace api.Services.Processing;

public class AsciiImageProcessor : IImageProcessor {

    //Modifies the DTO
    public async Task ProcessImageAsync(
        ImageDataDTO dto, int newWidth = 0, int newHeight = 0) {

        if (dto.Content is null) {
            throw new ArgumentNullException(
                "ImageToAsciiDTO.Content cannot be null for a process request");
        }

        using (HttpClient client = new HttpClient()) {  
            const string url = CommunicationUrls.ProcessingUrl + "ascii/convert";

            var ms = new MemoryStream(dto.Content); 
            var content = new StreamContent(ms);

            var queryParams = new Dictionary<string, string?>() {
                { "newWidth", newWidth.ToString() },
                { "newHeight", newHeight.ToString() }
            };

            var newUrl = QueryHelpers.AddQueryString(url, queryParams);

            using (var response = await client.PostAsync(newUrl, content)) {
                if (!response.IsSuccessStatusCode) {
                    throw new HttpRequestException(await response.Content.ReadAsStringAsync() + response.StatusCode.ToString());
                }
                dto.Text = await response.Content.ReadAsByteArrayAsync();
            };
        }
        


    }
}