namespace shared.DTOs;
public class ImageDataDTO
{
    public int UserId { get; set; }
    public string FileName { get; set; } = String.Empty;
    public string FileType { get; set; } = String.Empty;
    public string? Path { get; set; }
    public byte[]? Text { get; set; }
    public byte[]? Content { get; set; }
}
