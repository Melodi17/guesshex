using Newtonsoft.Json;

namespace guesshex;

public class Round
{
    private const int MaxGuesses = 5;

    public Round(Color targetColor)
    {
        TargetColor = targetColor;
    }

    public Color TargetColor { get; set; }

    [JsonIgnore]
    public Color GuessColor
    {
        get => Guesses.Count > 0 ? Guesses[^1] : new(128, 128, 128);
        set => Guesses.Add(value);
    }

    public List<Color> Guesses { get; set; } = new();
    [JsonIgnore] public int RoundNumber => this.Guesses.Count;
    [JsonIgnore] public bool Completed => this.RoundNumber == MaxGuesses || this.TargetColor.Hex == this.GuessColor.Hex;
    
    public int CalculateInaccuracy()
    {
        return this.Guesses.Select(g =>
                CalculateHexDiff(this.TargetColor, g) // get hex diff array
                    .Select(x => Math.Abs(x)) // get abs, so -1 and 1 are the same
                    .Sum()) // sum all diffs
            .Sum();
    }

    public int CalculateWeightedInaccuracy()
    {
        return this.Guesses.Select((g, i) =>
                ApplyWeights(
                        CalculateHexDiff(this.TargetColor, g), // get hex diff array
                        [
                            1, 0.5f, 1, 0.5f, 1, 0.5f
                        ]) // weight according to position, 1, 3 and 5 being the most significant digits
                    .Select(x => Math.Abs(x)) // get abs, so -1 and 1 are the same
                    .Sum()
            ) // sum all diffs
            .Sum();
    }

    private static int[] ApplyWeights(int[] source, float[] weights)
    {
        if (source.Length != weights.Length)
            throw new InvalidOperationException("Source and weights must have the same length");

        int[] output = new int[source.Length];
        for (int i = 0; i < output.Length; i++)
            output[i] = (int)(source[i] * weights[i]);

        return output;
    }

    public static int[] CalculateHexDiff(Color targetColor, Color guessColor)
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