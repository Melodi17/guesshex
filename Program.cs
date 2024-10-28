using Newtonsoft.Json;

namespace guesshex;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        PInvoke.EnableVTMode();

        // Read ColorNames.json from Embedded Resource
        string json = Utils.ReadEmbeddedResource("ColorNames.json");
        Color.KnownColors = JsonConvert.DeserializeObject<List<ColorJsonObj>>(json)!;

        Random r = new();
        Round round = new(targetColor: r.NextColor());

        while (true)
        {
            Console.Clear();
            DrawColors(round.TargetColor, round.GuessColor);
            Console.WriteLine();
            for (int i = 0; i < round.Guesses.Count; i++)
            {
                DrawGuess(round.TargetColor, round.Guesses[i], i + 1);
                DrawLine();
            }

            if (round.Completed)
                break;

            Console.WriteLine();
            round.GuessColor = InputColor();
        }

        Console.WriteLine();
        if (round.TargetColor.Hex == round.GuessColor.Hex)
            Console.WriteLine(Utils.Color("You won in " + round.RoundNumber + " rounds!", ConsoleColor.Green));
        else
            Console.WriteLine(Utils.Color("You lose!", ConsoleColor.Red));

        int inaccuracy = round.CalculateInaccuracy();
        int inaccuracy_w = round.CalculateWeightedInaccuracy();


        Console.Write(Utils.Color($"Inaccuracy: {inaccuracy} ({inaccuracy_w}w)  ", ConsoleColor.Cyan));

        (string text, ConsoleColor color) score = inaccuracy switch
        {
            0 => ("Perfect!", ConsoleColor.Magenta),
            < 5 => ("Insane", ConsoleColor.Cyan),
            < 10 => ("Amazing", ConsoleColor.DarkCyan),
            < 30 => ("Great", ConsoleColor.Green),
            < 50 => ("Good", ConsoleColor.Yellow),
            < 100 => ("Not bad", ConsoleColor.Red),
            _ => ("Bad", ConsoleColor.Gray)
        };

        Console.WriteLine(Utils.Color($"[{score.text}]", score.color));

        string filePath = Path.Join(Path.GetDirectoryName(Environment.ProcessPath), "data.json");
        List<(int s, string c)> scores = File.Exists(filePath)
            ? JsonConvert.DeserializeObject<List<(int, string)>>(File.ReadAllText(filePath))!
            : new();

        if (scores.Count > 0 && scores.Min(x => x.s) > inaccuracy)
        {
            Console.WriteLine(Utils.Color("New low score!", ConsoleColor.Magenta));
            Console.WriteLine(Utils.Color("Previous low score: " + scores.Min(x => x.s), ConsoleColor.DarkGray));
        }

        if (scores.Count > 0)
        {
            int avg = (int)scores.Average(x => x.s);

            scores.Add((inaccuracy, round.TargetColor.Hex));

            int avg_new = (int)scores.Average(x => x.s);

            ConsoleColor c = avg_new < avg ? ConsoleColor.Green :
                avg_new > avg ? ConsoleColor.Red : ConsoleColor.DarkGray;
            Console.WriteLine(Utils.Color(
                $"Average score: {Utils.Color(avg.ToString(), c)} -> {Utils.Color(avg_new.ToString(), c)}",
                ConsoleColor.DarkGray));
        }
        else
            scores.Add((inaccuracy, round.TargetColor.Hex));

        File.WriteAllText(filePath, JsonConvert.SerializeObject(scores));
        
        Console.WriteLine(Utils.Color($"The color was {round.TargetColor.Name} ({round.TargetColor.Hex})",
            round.TargetColor));
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

    private static void DrawLine() => Console.WriteLine("  ------------------------------");

    private static void DrawColors(Color targetColor, Color guessColor)
    {
        int y = Console.CursorTop;
        const int size = 7;

        DrawColoredSquare(targetColor, 1, y, size);
        DrawColoredSquare(guessColor, 1 + size + 1, y, size);

        Console.SetCursorPosition(1, y + size);
    }

    private static void DrawColoredSquare(Color color, int x, int y, int size)
    {
        for (int i = 0; i < size; i++)
        {
            Console.SetCursorPosition(x * 2, y + i);
            Console.Write(Utils.Color(new string('\u2588', size * 2), color));
        }
    }

    private static void DrawGuess(Color targetColor, Color guessColor, int roundNumber)
    {
        int[] diff = Round.CalculateHexDiff(targetColor, guessColor);

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
}