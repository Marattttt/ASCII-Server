using shared.DTOs;

namespace processing.Services;

public class ProcessingService
{
    private const string asciiRow = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~i!lI;:,\"^`\". ";
    private const int maxPixelLightness = byte.MaxValue * 3;

    // 0 stands for the original size
    // if 0 is passed to only one of parameters, aspect ratio is preserved
    // if outPath is not provided, it is defaulted to savge under the same name in the same folder
    public string Process(string path, 
        int width = 0,
        int height = 0,
        string? outPath = null)
    {
        if (outPath is null)
            outPath = getDefaultOutPath(path);

        if (File.Exists(outPath))
            File.Delete(outPath);

        string result = String.Empty;
        using (Image<Rgba32> image = Image.Load<Rgba32>(path))
        {
            if (width == 0)
                width = image.Width;
            else if (height == 0)
                height = image.Height;

            result = makeAsciiTxtFile(
                image, 
                width, 
                height, 
                outPath);
        };
        return result;
    }   

    private string makeAsciiTxtFile(
        Image<Rgba32> image, 
        int width, 
        int height, 
        string outPath) 
    {
        Console.WriteLine(outPath + "\n\n\n");
        FileStream fstream = File.Create(outPath);
        
        image.Mutate(accessor => {
            accessor.Resize(width, height);
        });

        bool isSuccess = true;
        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y+=2)
            {
                Span<Rgba32> pixelRow = accessor.GetRowSpan(y);
                WritePixelRow(pixelRow, fstream);
            }
        });
        fstream.Dispose();
        return isSuccess ? outPath : String.Empty;
    }

    private void WritePixelRow(Span<Rgba32> pixelRow, FileStream fileStream)
    {
        // pixelRow.Length has the same value as accessor.Width,
        // but using pixelRow.Length allows the JIT to optimize away bounds checks: 
        for (int x = 0; x < pixelRow.Length; x++)
        {
            char ascii = '\0';
            ascii = getAscii(pixelRow[x]);
            fileStream.Write(new byte[] { Convert.ToByte(ascii) });
        }
        fileStream.Write(new byte[] { Convert.ToByte('\n') });
    }

    private string getDefaultOutPath(string path) 
    {
        int extensionStart = path.LastIndexOf('.');
        Console.WriteLine(extensionStart + '\n' + path);
        string outPath = path.Substring(0, extensionStart) + ".txt";
        return outPath;
    }

    private char getAscii (Rgba32 input) 
    {
        int lightness = (input.R + input.G + input.B) * input.A / 255;
        float actualLightness = ((float)lightness / maxPixelLightness) * 0.99f;
        int index = (int)(asciiRow.Length * actualLightness);
        return asciiRow[index];
    }
}