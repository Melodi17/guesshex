namespace guesshex;

public static class RandomExtensions
{
    public static Color NextColor(this Random r)
    {
        return new Color(r.Next(256), r.Next(256), r.Next(256));
    }
}