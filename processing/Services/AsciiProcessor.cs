using System.Text;
namespace processing.Services;

public class AsciiProcessor : PerRowProcessor {
    const string asciiRow = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/|()1{}[]?-_+~i!lI;:,\"^`\". ";
    protected override void writeChar(Rgba32 input, StringBuilder sb) {        
        float lightness = (input.R + input.G + input.B);
        float actualLightness = (lightness / maxPixelLightness);
        
        //Cuts lightness between 0.025 and 0.0925
        if (actualLightness < 0.025) {
            actualLightness = 0.1f;
        } else if (actualLightness > 0.925) {
            actualLightness = 0.99f;
        } else {
            actualLightness *= 0.95f; 
        }

        int index = (int)(asciiRow.Length * actualLightness) - 1;
        sb.Append(asciiRow[index].ToString());
    }
}