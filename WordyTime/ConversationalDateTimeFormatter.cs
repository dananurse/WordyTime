using System.Globalization;

namespace WordyTime;

public class ConversationalDateTimeFormatter
{
    private static readonly Random Random = new();
    private readonly IDateTimeProvider _dateTimeProvider;

    public ConversationalDateTimeFormatter(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public string Format(DateTime suppliedUtc, string timeZoneId = "GMT Standard Time")
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

    private List<string> GetFormats(DateTime suppliedUtc, string timeZoneId)
    {
        var formats = new List<string>();

        var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var suppliedLocal = TimeZoneInfo.ConvertTimeFromUtc(suppliedUtc, tz);
        var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(_dateTimeProvider.UtcNow, tz);
        var timeOfDay = suppliedLocal.TimeOfDay;

        var diff = suppliedLocal - nowLocal;
        bool isPast = diff.TotalSeconds < 0;

        var day = suppliedLocal.Day;
        var nowday = nowLocal.Day;
        
        if(day != nowday) formats.AddRange(OutsideDays(suppliedLocal, nowLocal));
        else
        {
            formats.AddRange(Difference(diff, isPast));
            formats.AddRange(Periods(suppliedLocal, timeOfDay, isPast, nowLocal));
            formats.AddRange(TimePoints(timeOfDay));
            formats.AddRange(StandardFormats(suppliedLocal));
            formats.AddRange(ConversationalFormats(suppliedLocal));   
        }

        return formats.Distinct().ToList();
    }

    private List<string> StandardFormats(DateTime suppliedLocal)
    {
        var formats = new List<string>();
        formats.Add($"at {suppliedLocal.ToString("HH:mm", CultureInfo.CurrentCulture)}");
        formats.Add($"at {suppliedLocal.ToString("hh:mm tt", CultureInfo.CurrentCulture)}");
        return formats;
    }
    private List<string> Difference(TimeSpan diff, bool isPast)
    {
        var formats = new List<string>();
        var absDiff = TimeSpan.FromSeconds(Math.Abs(diff.TotalSeconds));
        
        if (absDiff < TimeSpan.FromMinutes(1)) formats.Add("just now");

        if (absDiff == TimeSpan.FromMinutes(0)) formats.Add("just now");

        if (absDiff < TimeSpan.FromMinutes(60) && absDiff > TimeSpan.FromMinutes(0))
        {
            var mins = (int)Math.Round(absDiff.TotalMinutes);
            if (mins > 0)
            {
                if (isPast)
                    formats.Add(mins == 1 ? "a minute ago" : $"{mins} minutes ago");
                else
                    formats.Add(mins == 1 ? "in a minute" : $"in {mins} minutes");
            }
        }

        if (absDiff < TimeSpan.FromHours(12))
        {
            var hours = (int)Math.Round(absDiff.TotalHours);
            if (hours > 0)
            {
                if (isPast)
                    formats.Add(hours == 1 ? "an hour ago" : $"{hours} hours ago");
                else
                    formats.Add(hours == 1 ? "in an hour" : $"in {hours} hours");
            }
        }
        return formats;
    }
    private List<string> Periods(DateTime suppliedLocal, TimeSpan timeOfDay, bool isPast, DateTime nowLocal)
    {
        var formats = new List<string>();
        // 2. Day period-based
        var period = DayPeriods.Periods.FirstOrDefault(p => timeOfDay >= p.Start && timeOfDay <= p.End);
        if (period.Name != null)
        {
            formats.Add($"{period.prefix}{period.Name.ToLower()}");
            if (suppliedLocal.Date == nowLocal.Date)
            {
                formats.Add(isPast
                    ? $"earlier {period.prefix}{period.Name.ToLower()}"
                    : $"later {period.prefix}{period.Name.ToLower()}");
            }
        }
        
        return formats;
    }
    private List<string> TimePoints(TimeSpan timeOfDay)
    {
        var formats = new List<string>();
        
        // 3. Key time points
        var nearestPoint = KeyTimePoints.Points
            .OrderBy(p => Math.Abs((timeOfDay - p.Time).TotalMinutes))
            .First();
        
        var minutesToPoint = (timeOfDay - nearestPoint.Time).TotalMinutes;

        if (Math.Abs(minutesToPoint) < 10 && Math.Abs(minutesToPoint) > 0)
        {
            formats.Add(minutesToPoint < 0
                ? $"just before {nearestPoint.Name.ToLower()}"
                : $"just past {nearestPoint.Name.ToLower()}");
        }

        if (Math.Abs(minutesToPoint) < 20 && Math.Abs(minutesToPoint) > 0)
        {
            formats.Add(minutesToPoint < 0
                ? $"almost {nearestPoint.Name.ToLower()}"
                : $"shortly after {nearestPoint.Name.ToLower()}");
        }
        
        switch (minutesToPoint)
        {
            case 0:
                formats.Add($"{nearestPoint.Name.ToLower()} o'clock");
                break;
            case 5:
                formats.Add($"5 past {nearestPoint.Name.ToLower()}");
                break;
            case 10:
                formats.Add($"10 past {nearestPoint.Name.ToLower()}");
                break;
            case 15:
                formats.Add($"quarter past {nearestPoint.Name.ToLower()}");
                break;
            case 20:
                formats.Add($"20 past {nearestPoint.Name.ToLower()}");
                break;
            case 25:
                formats.Add($"25 past {nearestPoint.Name.ToLower()}");
                break;
            case 30:
                formats.Add($"half past {nearestPoint.Name.ToLower()}");
                break;
            case -25:
                formats.Add($"25 to {nearestPoint.Name.ToLower()}");
                break;
            case -20:
                formats.Add($"20 to {nearestPoint.Name.ToLower()}");
                break;
            case -15:
                formats.Add($"quarter to {nearestPoint.Name.ToLower()}");
                break;
            case -10:
                formats.Add($"10 to {nearestPoint.Name.ToLower()}");
                break;
            case -5:
                formats.Add($"5 to {nearestPoint.Name.ToLower()}");
                break;
        }
        
        return formats;
    }
    private List<string> ConversationalFormats(DateTime suppliedLocal)
    {
        var formats = new List<string>();
        int hour = suppliedLocal.Hour % 12; // 12-hour clock
        if (hour == 0) hour = 12;
        int minute = suppliedLocal.Minute;

        string hourString = hour.ToString();
        string nextHourString = (hour == 12 ? 1 : hour + 1).ToString();

        // Build appropriate conversational format
        switch (minute)
        {
            case 0:
                formats.Add($"{hourString} o'clock");
                break;
            case 5:
                formats.Add($"5 past {hourString}");
                break;
            case 10:
                formats.Add($"10 past {hourString}");
                break;
            case 15:
                formats.Add($"quarter past {hourString}");
                break;
            case 20:
                formats.Add($"20 past {hourString}");
                break;
            case 25:
                formats.Add($"25 past {hourString}");
                break;
            case 30:
                formats.Add($"half past {hourString}");
                break;
            case 35:
                formats.Add($"25 to {nextHourString}");
                break;
            case 40:
                formats.Add($"20 to {nextHourString}");
                break;
            case 45:
                formats.Add($"quarter to {nextHourString}");
                break;
            case 50:
                formats.Add($"10 to {nextHourString}");
                break;
            case 55:
                formats.Add($"5 to {nextHourString}");
                break;
            default:
                // Handle in-between times (optionally)
                if (minute < 30)
                    formats.Add($"{minute} past {hourString}");
                else
                    formats.Add($"{60 - minute} to {nextHourString}");
                break;
        }
        return formats;
    }

    private List<string> OutsideDays(DateTime suppliedLocal, DateTime nowLocal)
    {
        var formats = new List<string>();
        if(suppliedLocal.Date < nowLocal.Date) formats.Add("yesterday");
        if(suppliedLocal.Date > nowLocal.Date) formats.Add("tomorrow");
        return formats;
    }
}