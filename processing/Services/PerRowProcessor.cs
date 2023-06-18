using System.Text;
namespace processing.Services;

public abstract class PerRowProcessor : Processor {
    protected override void makeText(StreamWriter writer, Image<Rgba32> image, int newWidth, int newHeight) {
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