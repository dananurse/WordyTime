namespace WordyTime;

public static class KeyTimePoints
{
    public static readonly (string Name, TimeSpan Time)[] Points = new[]
    {
        ("Getting Up",  new TimeSpan(7, 0, 0)),
        ("Breakfast",   new TimeSpan(8, 0, 0)),
        ("Brunch",      new TimeSpan(11, 0, 0)),
        ("Lunch",       new TimeSpan(13, 0, 0)),
        ("Teatime",     new TimeSpan(16, 0, 0)),
        ("Dinner",      new TimeSpan(18, 30, 0)),
        ("Bath",        new TimeSpan(19, 30, 0)),
        ("Supper",      new TimeSpan(21, 0, 0)),
        ("Bedtime",     new TimeSpan(22, 30, 0)),
    };
}