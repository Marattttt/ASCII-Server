using System.Dynamic;
using System.ComponentModel.DataAnnotations;

namespace storage.Models;

public class ImageData {
    public uint ImageId { get; set; }
    public string FileName { get; set; } = String.Empty;
    public required byte[] Content { get; set; }
    public string FileType { get; set; } = String.Empty;
    public string? ASCII { get; set; }
    public int UserId { get; set; }
    public required User Owner { get; set; }
}