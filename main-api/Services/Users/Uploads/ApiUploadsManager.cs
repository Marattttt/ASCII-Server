using System.Reflection.Metadata;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Text.Json;

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
}