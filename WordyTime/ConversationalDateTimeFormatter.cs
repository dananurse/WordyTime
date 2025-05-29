using System.Globalization;

namespace WordyTime;

public class ConversationalDateTimeFormatter
{
    private static readonly Random Random = new();

    public static string Format(DateTime suppliedUtc, string timeZoneId = "GMT Standard Time")
    {
        var formats = GetFormats(suppliedUtc, timeZoneId);
        if (formats.Count == 0)
        {
            // If no formats generated, fall back to standard HH:mm
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var suppliedLocal = TimeZoneInfo.ConvertTimeFromUtc(suppliedUtc, tz);
            return suppliedLocal.ToString("HH:mm", CultureInfo.CurrentCulture);
        }
        // Pick one at random
        return formats[Random.Next(formats.Count)];
    }

        private static List<string> GetFormats(DateTime suppliedUtc, string timeZoneId)
    {
        var formats = new List<string>();

        var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var suppliedLocal = TimeZoneInfo.ConvertTimeFromUtc(suppliedUtc, tz);
        var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        var timeOfDay = suppliedLocal.TimeOfDay;

        var diff = suppliedLocal - nowLocal;
        bool isPast = diff.TotalSeconds < 0;
        var absDiff = TimeSpan.FromSeconds(Math.Abs(diff.TotalSeconds));

        // 1. Difference-based
        if (absDiff < TimeSpan.FromMinutes(1))
        {
            formats.Add("right now");
        }
        if (absDiff < TimeSpan.FromMinutes(60))
        {
            var mins = (int)Math.Round(absDiff.TotalMinutes);
            if (isPast)
                formats.Add(mins == 1 ? "a minute ago" : $"{mins} minutes ago");
            else
                formats.Add(mins == 1 ? "in a minute" : $"in {mins} minutes");
        }
        if (absDiff < TimeSpan.FromHours(3))
        {
            var hours = (int)Math.Round(absDiff.TotalHours);
            if (isPast)
                formats.Add(hours == 1 ? "an hour ago" : $"{hours} hours ago");
            else
                formats.Add(hours == 1 ? "in an hour" : $"in {hours} hours");
        }

        // 2. Day period-based
        var period = DayPeriods.Periods.FirstOrDefault(
            p => timeOfDay >= p.Start && timeOfDay <= p.End
        );
        if (period.Name != null)
        {
            if (suppliedLocal.Date == nowLocal.Date)
            {
                formats.Add(isPast
                    ? $"earlier this {period.Name.ToLower()}"
                    : $"later this {period.Name.ToLower()}");
            }
            formats.Add(suppliedLocal.Date < nowLocal.Date
                ? $"last {period.Name.ToLower()}"
                : $"next {period.Name.ToLower()}");
        }

        // 3. Key time points
        var nearestPoint = KeyTimePoints.Points
            .OrderBy(p => Math.Abs((timeOfDay - p.Time).TotalMinutes))
            .First();
        double minutesToPoint = (timeOfDay - nearestPoint.Time).TotalMinutes;

        if (Math.Abs(minutesToPoint) < 10)
        {
            formats.Add(minutesToPoint < 0
                ? $"just before {nearestPoint.Name.ToLower()}"
                : $"just after {nearestPoint.Name.ToLower()}");
        }
        if (Math.Abs(minutesToPoint) < 20)
        {
            formats.Add(minutesToPoint < 0
                ? $"almost {nearestPoint.Name.ToLower()}"
                : $"shortly after {nearestPoint.Name.ToLower()}");
        }
        if (Math.Abs(minutesToPoint) < 45)
        {
            formats.Add(minutesToPoint < 0
                ? $"{Math.Abs((int)minutesToPoint)} minutes before {nearestPoint.Name.ToLower()}"
                : $"{(int)minutesToPoint} minutes after {nearestPoint.Name.ToLower()}");
        }

        // 4. Fallback: Explicit time
        formats.Add($"at {suppliedLocal.ToString("HH:mm", CultureInfo.CurrentCulture)}");

        return formats.Distinct().ToList();
    }
}