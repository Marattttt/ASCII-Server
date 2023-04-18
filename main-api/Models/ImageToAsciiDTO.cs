using System.Reflection.Metadata.Ecma335;
namespace api.Models;

public class ImageToAsciiDTO
{
    public string Path { get; set; } = String.Empty;
    public int Width { get; set; } = 0;
    public int Height { get; set; } = 0;
    public string? OutPath { get; set; } = null;
}