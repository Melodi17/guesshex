namespace guesshex;

public class Utils
{
    
    public static string Color(string text, int r, int g, int b)
    {
        const string esc = "\x1b";
        return $"{esc}[38;2;{r};{g};{b}m{text}{esc}[0m";
    }

    public static string Color(string text, ConsoleColor color)
    {
        const string esc = "\x1b";
        
        int c = color switch
        {
            ConsoleColor.Black => 30,
            ConsoleColor.Red => 31,
            ConsoleColor.Green => 32,
            ConsoleColor.Yellow => 33,
            ConsoleColor.Blue => 34,
            ConsoleColor.Magenta => 35,
            ConsoleColor.Cyan => 36,
            ConsoleColor.White => 37,
            _ => 37
        };
        
        return $"{esc}[{c}m{text}{esc}[0m";
    }

    public static string Color(string text, Color color) => Color(text, color.R, color.G, color.B);

    public static string EraseLine()
    {
        const string esc = "\x1b";
        return $"{esc}[2K";
    }

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

    public static string ReadEmbeddedResource(string fileName)
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream($"guesshex.{fileName}");
        using var reader = new System.IO.StreamReader(stream);
        return reader.ReadToEnd();
    }
}