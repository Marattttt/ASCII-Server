using System.Net;
using System.Net.Http.Headers;

using shared.DTOs;
using shared.Config;

namespace api.Services;

public class ApiUploadsManager : IUploadsManager {
    public async Task<string?> UploadImageAsync(ImageDataDTO dto) {
        if (dto.Content is null) {
            throw new ArgumentNullException("DTO content is null");
        }
        using (HttpClient client = new HttpClient()) {
            MultipartFormDataContent content = new MultipartFormDataContent();

            content.Headers.Add(nameof(dto.UserId), dto.UserId.ToString());
            content.Headers.Add(nameof(dto.FileName), dto.FileName);
            content.Headers.Add(nameof(dto.FileType), dto.FileType);

            // var imageContentStream = new MemoryStream(dto.Content);
            var imageContent = new ByteArrayContent(dto.Content);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            content.Add(
                imageContent, "image", "image"
            );

            if (dto.Text?.Count() > 0) {
                var processedContent = new ByteArrayContent(dto.Text);
                processedContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Add(
                    processedContent, "processed", "processed"
                );
            }
            
            HttpResponseMessage responseMessage = await client.PostAsync(
                CommunicationUrls.StorageUrl + "user/images/new",
                content
            );

            if (!responseMessage.IsSuccessStatusCode) {
                return await responseMessage.Content.ReadAsStringAsync();
            }

            return null;
        }
    }
    public async Task<(Stream? stream, string? errorMessage)> GetImageAsync(
        int userId, 
        string fileName) {

        using (var client = new HttpClient()) {
            
            client.DefaultRequestHeaders.Add("fileName", fileName);
            string endPoint = $"user/{userId}/images";

            var response = await client.GetAsync(
                CommunicationUrls.StorageUrl + endPoint);

            if (!response.IsSuccessStatusCode) {
                return (null, await response.Content.ReadAsStringAsync());
            }

            var ms = await response.Content.ReadAsStreamAsync();
            return (ms, null);
        }
    }

    public async Task<(Stream? stream, string? errorMessage)> GetProcessedImageAsync (
        int userId, 
        string fileName){
        
        using (var client = new HttpClient()) {
            client.DefaultRequestHeaders.Add("fileName", fileName);
            string endPoint = $"user/{userId}/images/processed";

            var response = await client.GetAsync(
                CommunicationUrls.StorageUrl + endPoint);

            if (!response.IsSuccessStatusCode) {
                return (null, await response.Content.ReadAsStringAsync());
            }

            var ms = await response.Content.ReadAsStreamAsync();
            return (ms, null);
        }
    }

    public async Task<string?> DeleteImageAsync(
        int userId,
        string fileName) {
        
        using (var client = new HttpClient()) {
            
            client.DefaultRequestHeaders.Add("fileName", fileName);

            string endPoint = $"user/{userId}/images/";

            var response = await client.DeleteAsync(
                CommunicationUrls.StorageUrl + endPoint);
            
            if (!response.IsSuccessStatusCode) {
                Console.WriteLine(await response.Content.ReadAsStringAsync() + response.StatusCode);
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }
    }
}