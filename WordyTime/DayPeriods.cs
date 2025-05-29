namespace WordyTime;

public static class DayPeriods
{
    public static readonly (string Name, TimeSpan Start, TimeSpan End)[] Periods = new[]
    {
        ("Night",      new TimeSpan(0, 0, 0),   new TimeSpan(5, 59, 59)),
        ("Morning",    new TimeSpan(6, 0, 0),   new TimeSpan(11, 59, 59)),
        ("Afternoon",  new TimeSpan(12, 0, 0),  new TimeSpan(16, 59, 59)),
        ("Evening",    new TimeSpan(17, 0, 0),  new TimeSpan(20, 59, 59)),
        ("Nighttime",  new TimeSpan(21, 0, 0),  new TimeSpan(23, 59, 59))
    };
}