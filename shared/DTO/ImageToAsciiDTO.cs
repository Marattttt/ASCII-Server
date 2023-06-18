namespace shared.DTOs;
public class ImageToAsciiDTO
{
    public string FileName;
    public int NewWidth = 0;
    public int NewHeight = 0;
    public byte[]? Content;
    public byte[]? ProcessingResult;

    public ImageToAsciiDTO(string fileName) {
        FileName = fileName;
    }

}

