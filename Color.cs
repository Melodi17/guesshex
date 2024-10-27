namespace guesshex;

public class Color
{
    public static List<ColorJsonObj> KnownColors { get; set; }

    public string Name => GetColorName(this.R, this.G, this.B);
    public string Hex { get; set; }

    public int R { get; set; }
    public int G { get; set; }
    public int B { get; set; }
    
    public Color(int r, int g, int b)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.Hex = Utils.RgbToHex(r, g, b);
    }
    
    public Color(string hex)
    {
        (this.R, this.G, this.B) = Utils.HexToRgb(hex);
        this.Hex = hex.ToUpper();
        if (this.Hex.Length == 6)
            this.Hex = "#" + this.Hex;
    }

    private static string GetColorName(int r, int g, int b)
    {
        return KnownColors.FirstOrDefault(c => c.r == r && c.g == g && c.b == b)?.name
               ?? KnownColors.OrderBy(c => Math.Abs(c.r - r) + Math.Abs(c.g - g) + Math.Abs(c.b - b))
                   .First().name;
    }
}