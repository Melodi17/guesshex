using Newtonsoft.Json;

namespace guesshex;

public class Round
{
    public Color CorrectColor { get; set; }
    public List<Color> Guesses { get; set; } = new();
    [JsonIgnore] public int RoundNumber => this.Guesses.Count;
    public bool Completed = false;
}