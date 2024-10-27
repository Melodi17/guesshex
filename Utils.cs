namespace guesshex;

public class Utils
{
    
    public static string Color(string text, int r, int g, int b)
    {
        const string esc = "\x1b";
        return $"{esc}[38;2;{r};{g};{b}m{text}{esc}[0m";
    }

    public static string Color(string text, Color color) => Color(text, color.R, color.G, color.B);

    public static string RgbToHex(int r, int g, int b)
    {
        return $"#{r:X2}{g:X2}{b:X2}";
    }
    
    public static (int r, int g, int b) HexToRgb(string hex)
    {
        if (hex[0] == '#')
            hex = hex.Substring(1);
        
        return (int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
            int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
            int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
    }
}