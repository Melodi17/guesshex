namespace guesshex;

class Program
{
    static void Main(string[] args)
    {
        PInvoke.EnableVTMode();

        Random r = new();        
        
        Color targetColor = r.NextColor();
        Color guessColor = new Color(0, 0, 0);
        
        DisplayColors(targetColor, guessColor);
    }
    
    static void DisplayColors(Color targetColor, Color guessColor)
    {
        int y = Console.CursorTop;
        
        DrawColoredSquare(targetColor, 1, y, 6);
        DrawColoredSquare(guessColor, 1 + 6 + 1, y, 6);
        
        Console.SetCursorPosition(1, y + 6);
    }
    
    static void DrawColoredSquare(Color color, int x, int y, int size)
    {
        for (int i = 0; i < size; i++)
        {
            Console.SetCursorPosition(x * 2, y + i);
            Console.Write(Utils.Color(new string('\u2588', size * 2), color));
        }
    }
    
    static int[] CalculateHexDiff(Color targetColor, Color guessColor)
    {
        
    }
}