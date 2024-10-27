using Newtonsoft.Json;

namespace guesshex;

class Program
{
    const int MaxGuesses = 5;

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        PInvoke.EnableVTMode();

        // Read ColorNames.json from Embedded Resource
        string json = Utils.ReadEmbeddedResource("ColorNames.json");
        Color.KnownColors = JsonConvert.DeserializeObject<List<ColorJsonObj>>(json)!;

        Random r = new();

        Color targetColor = r.NextColor();
        Color guessColor = new(128, 128, 128);

        List<Color> guesses = new();

        while (true)
        {
            Console.Clear();
            DisplayColors(targetColor, guessColor);
            Console.WriteLine();
            for (int i = 0; i < guesses.Count; i++)
            {
                DrawGuess(targetColor, guesses[i], i + 1);
                DrawLine();
            }

            if (guessColor.Hex == targetColor.Hex || guesses.Count >= MaxGuesses)
                break;

            Console.WriteLine();
            guessColor = InputColor();
            guesses.Add(guessColor);
        }

        Console.WriteLine();
        if (guessColor.Hex == targetColor.Hex)
        {
            Console.WriteLine(Utils.Color("You won in " + (guesses.Count) + " rounds!", ConsoleColor.Green));
        }
        else
            Console.WriteLine(Utils.Color("You lose!", ConsoleColor.Red));

        int accuracy = guesses.Select(g =>
                CalculateHexDiff(targetColor, g) // get hex diff array
                    .Select(x => Math.Abs(x)) // get abs, so -1 and 1 are the same
                    .Sum()) // sum all diffs
            .Sum(); // sum all rounds
        Console.Write(Utils.Color($"Inaccuracy: {accuracy} ", ConsoleColor.Cyan));
        
        (string text, ConsoleColor color) score = accuracy switch
        {
            0 => ("Perfect", ConsoleColor.Magenta),
            < 10 => ("Amazing", ConsoleColor.Cyan),
            < 30 => ("Great", ConsoleColor.Green),
            < 50 => ("Good", ConsoleColor.Yellow),
            < 100 => ("Not bad", ConsoleColor.Red),
            _ => ("Bad", ConsoleColor.Gray)
        };
        
        Console.WriteLine(Utils.Color($"[{score.text}]", score.color));

        Console.WriteLine(Utils.Color($"The color was {targetColor.Name} ({targetColor.Hex})", targetColor));
    }

    private static Color InputColor()
    {
        string text = "";
        string chars = "0123456789ABCDEFabcdef";
        Console.CursorLeft = 3;
        while (true)
        {
            ConsoleKeyInfo info = Console.ReadKey(true);

            if (info.Key == ConsoleKey.Backspace)
            {
                if (text.Length > 0)
                    text = text[..^1];
            }
            else if (chars.Contains(info.KeyChar))
                text += info.KeyChar.ToString().ToUpper();

            else continue;

            if (text.Length == 6)
                return new(text);

            Console.CursorLeft = 3;
            Console.Write(Utils.EraseLine());
            Console.Write(string.Join(" ", text.ToArray()) + " ");
        }
    }

    private static void DrawLine()
    {
        Console.WriteLine("  ------------------------------");
    }

    static void DisplayColors(Color targetColor, Color guessColor)
    {
        int y = Console.CursorTop;
        const int size = 7;

        DrawColoredSquare(targetColor, 1, y, size);
        DrawColoredSquare(guessColor, 1 + size + 1, y, size);

        Console.SetCursorPosition(1, y + size);
    }

    static void DrawGuess(Color targetColor, Color guessColor, int roundNumber)
    {
        int[] diff = CalculateHexDiff(targetColor, guessColor);

        const string spacing = "   ";

        Console.CursorLeft = 0;
        Console.Write(spacing);
        for (int i = 0; i < 6; i++)
        {
            if (diff[i] > 2)
                Console.Write(Utils.Color("= ", ConsoleColor.Red));
            else if (diff[i] > 0)
                Console.Write(Utils.Color("- ", ConsoleColor.Yellow));
            else
                Console.Write("  ");
        }

        Console.WriteLine();

        Console.Write(Utils.Color($"{roundNumber}.", ConsoleColor.Cyan));
        Console.Write(spacing[2..]);
        for (int i = 0; i < guessColor.Hex[1..].Length; i++)
        {
            string c = guessColor.Hex[1..][i].ToString();
            if (diff[i] == 0)
                c = Utils.Color(c, ConsoleColor.Green);
            Console.Write(c + " ");
        }

        Console.Write(new string(' ', 9));
        Console.Write(Utils.Color(new('\u2588', 2), guessColor));

        Console.WriteLine();

        Console.Write(spacing);
        for (int i = 0; i < 6; i++)
        {
            if (diff[i] < -2)
                Console.Write(Utils.Color("= ", ConsoleColor.Red));
            else if (diff[i] < 0)
                Console.Write(Utils.Color("- ", ConsoleColor.Yellow));
            else
                Console.Write("  ");
        }

        Console.WriteLine();
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
        int[] diff = new int[6];

        string targetHex = targetColor.Hex.TrimStart('#');
        string guessHex = guessColor.Hex.TrimStart('#');

        for (int i = 0; i < 6; i++)
        {
            int t = int.Parse(targetHex[i].ToString(), System.Globalization.NumberStyles.HexNumber);
            int g = int.Parse(guessHex[i].ToString(), System.Globalization.NumberStyles.HexNumber);
            diff[i] = t - g;
        }

        return diff;
    }
}