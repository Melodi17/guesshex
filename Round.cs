namespace guesshex;

public class Round
{
    public Color CorrectColor { get; set; }
    public List<Color> Guesses { get; set; }
    public int RoundNumber => this.Guesses.Count;
}