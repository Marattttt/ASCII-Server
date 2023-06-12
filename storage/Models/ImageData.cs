using System.Diagnostics.CodeAnalysis;
using shared.DTOs;

namespace storage.Models;

public class ImageData {
    public uint ImageId { get; set; }
    public string FileName { get; set; } = String.Empty;    
    public string FileType { get; set; } = String.Empty;
    public required byte[] Content { get; set; }
    public char[] ASCII { get; set; } = new char[0];
    public int UserId { get; set; }
    public User? Owner { get; set; }

    public ImageData() { }
    
    [SetsRequiredMembers]
    public ImageData(ImageDataDTO dto) {
        if (dto.Content is null) {
            throw new ArgumentNullException (
                "Content not provided in ImageDataDTO when creating new ImageData object");
        }
        UserId = dto.UserId;
        FileName = dto.FileName;
        FileType = dto.FileType;
        ASCII = dto.ASCII ?? new char[0];
        Content = dto.Content;
    }
    public ImageDataDTO ToDTO() {
        return new ImageDataDTO() {
                    UserId = this.UserId,
                    FileName = this.FileName,
                    FileType = this.FileType,
                    ASCII = this.ASCII,
                    Content = this.Content
                };
    }
    public void CopyData(ImageData newData) {
        FileName = newData.FileName;
        FileType = newData.FileType;
        Content = newData.Content;
        ASCII = newData.ASCII;
    }
}