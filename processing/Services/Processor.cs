using System.Security.Cryptography.X509Certificates;
using System.Text;

using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Metadata;
namespace processing.Services;

public abstract class Processor {
    protected const int maxPixelLightness = byte.MaxValue * 3;

    public async Task<bool> IsImage(Stream source) {
        try{
            ImageInfo info =  await Image.IdentifyAsync(source);
        } catch (UnknownImageFormatException) {
            return false;
        }
        return true;
    }
    
    // 0 stands for the original size
    // if 0 is passed to only one of parameters, aspect ratio is preserved
    // if outPath is not provided, it is defaulted to savge under the same name in the same folder
    public Task Process(
        Stream source,
        out byte[] destination,
        int newWidth = 0, int newHeight = 0) {

        source.Seek(0, SeekOrigin.Begin);

        using (var image = Image.Load<Rgba32>(source)) {
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.UTF8, (int) source.Length, true);

            if (newWidth == 0) {
                newWidth = image.Width;
            } else if (newHeight == 0) {
                newHeight = image.Height;
            }
            
            makeAscii(writer, image, newWidth, newHeight);
            writer.Flush();
            destination = ms.ToArray();
        }
        return Task.CompletedTask;
    }   

    protected virtual void makeAscii(StreamWriter writer, Image<Rgba32> image, int newWidth, int newHeight) {
        Console.WriteLine(newWidth.ToString() + ' ' + newHeight.ToString());
        image.Mutate(accessor => {
            accessor.Resize(newWidth, newHeight);
        });
        image.ProcessPixelRows(accessor => {
            for (int y = 0; y < accessor.Height; y+=2) {
                var pixelRow = accessor.GetRowSpan(y);
                string row = ProcessPixelRow(pixelRow);
                writer.WriteLine(row);
            }
        });
    }

    protected string ProcessPixelRow(Span<Rgba32> pixelRow) {
        // pixelRow.Length has the same value as accessor.Width,
        // but using pixelRow.Length allows the JIT to optimize away bounds checks
        var sb = new StringBuilder();
        foreach (var pixel in pixelRow) {
            writeChar(pixel, sb);
        }
        sb.Append('\n');
        return sb.ToString();
    }

    protected abstract void writeChar (Rgba32 input, StringBuilder sb);
}
