namespace shared.DTOs;
public class ImageDataDTO {
    public int UserId { get; set; } = 0;
    public string FileName { get; set; } = String.Empty;
    public string FileType { get; set; } = String.Empty;
    public string? ASCII;
    public byte[]? Content;
}