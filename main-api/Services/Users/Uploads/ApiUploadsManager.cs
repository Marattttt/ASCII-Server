using System.Net;

using shared.DTOs;
using shared.Config;

namespace api.Services;

public class ApiUploadsManager : IUploadsManager {
    public async Task<string?> UploadImageAsync(ImageDataDTO dto) {
        if (dto.Content is null) {
            throw new ArgumentNullException("DTO content is null");
        }
        using (HttpClient client = new HttpClient()) {
            HttpContent content = new StreamContent(
                new MemoryStream(dto.Content)
            );
            content.Headers.Add(nameof(dto.UserId), dto.UserId.ToString());
            content.Headers.Add(nameof(dto.FileName), dto.FileName);
            content.Headers.Add(nameof(dto.FileType), dto.FileType);
            content.Headers.Add("Length", dto.Content.Length.ToString());
            
            HttpResponseMessage responseMessage = await client.PostAsync(
                CommunicationUrls.StorageUrl + "user/images/new",
                content
            );

            if (responseMessage.StatusCode == HttpStatusCode.NoContent) {
                return null;
            }

            return await responseMessage.Content.ReadAsStringAsync();
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
}