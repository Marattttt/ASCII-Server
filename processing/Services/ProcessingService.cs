namespace processing.Services;

public class ProcessingService
{
    private const string asciiRow = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~i!lI;:,\"^`\". ";
    private const int maxPixelLightness = byte.MaxValue * 3;

    public int Process(string path, 
        int width = 0,
        int height = 0,
        string? outPath = null)
    {
        if (outPath is null)
            outPath = getDefaultOutPath(path);

        if (File.Exists(outPath))
            File.Delete(outPath);

        int result = 0;
        using (Image<Rgba32> image = Image.Load<Rgba32>(path))
        {
            bool isResizeNeeded = true;
            if (width == 0 && height == 0)
            {
                width = image.Width;
                height = image.Height;
                isResizeNeeded = false;
            }

            result = makeAsciiTxtFile(
                image, 
                width, 
                height, 
                outPath,
                isResizeNeeded);
        };
        return result;
    }   

    private int makeAsciiTxtFile(
        Image<Rgba32> image, 
        int width, 
        int height, 
        string outPath, 
        bool isResizeNeeded) 
    {
        Console.WriteLine(outPath + "\n\n\n");
        FileStream fstream = File.Create(outPath);
        
        if (isResizeNeeded)
        image.Mutate(accessor => {
            accessor.Resize(width, height);
        });

        bool isSuccess = true;
        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y+=2)
            {
                
                Span<Rgba32> pixelRow = accessor.GetRowSpan(y);

                // pixelRow.Length has the same value as accessor.Width,
                // but using pixelRow.Length allows the JIT to optimize away bounds checks:
                for (int x = 0; x < pixelRow.Length; x++)
                {
                    char ascii = '\0';
                    try 
                    {
                        ascii = getAscii(pixelRow[x]);
                    }
                    catch(Exception)
                    {

                        Console.WriteLine("Exception while processing pixel: ");
                        Console.WriteLine(pixelRow[x].R + " " + pixelRow[x].G + " " + pixelRow[x].B + " " + pixelRow[x].A);
                        Console.WriteLine();
                        isSuccess = false;
                    }
                    if (isSuccess)
                        fstream.Write(new byte[] { Convert.ToByte(ascii) });
                }
                if (isSuccess)
                    fstream.Write(new byte[] { Convert.ToByte('\n') });
            }
        });
        fstream.Dispose();
        return isSuccess ? 1 : 0;
    }

    private string getDefaultOutPath(string path) 
    {
        int extensionStart = path.LastIndexOf('.');
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