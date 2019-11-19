using System;
using System.Globalization;
using UnityEngine;

public class DateTimeConverter
{
    public static string ToNow(string timeString, string stringFormat)
    {
        var tempString = "--";
        var time = DateTime.Now;
        if (!string.IsNullOrEmpty(timeString) && !string.IsNullOrEmpty(timeString) && DateTime.TryParseExact(timeString, stringFormat, new CultureInfo("en-US"), DateTimeStyles.None, out time))
            return ToNow(time);
        else if (!string.IsNullOrEmpty(timeString) && DateTime.TryParse(timeString, out time))
            return ToNow(time);
        return tempString;
    }

    public static string ToNow(DateTime time)
    {
        var tempString = "--";
        if (time.Date == DateTime.Today.Date)
            tempString = time.ToString("HH:mm") + " Hôm nay";
        else if (time.AddDays(1).Date == DateTime.Today.Date)
            tempString = time.ToString("HH:mm") + " Hôm qua";
        else
            tempString = time.ToString("HH:mm dd/MM/yyyy");
        return tempString;
    }

    public static string ToRelativeTime(object time)
    {
        var temp = DateTime.Now;
        if (DateTime.TryParse(time.ToString(), out temp))
        {
            return ToRelativeTime(temp);
        }
        else
        {
            Debug.LogError("ToRelativeTime: " + time.ToString());
            return time.ToString();
        }
    }

    public static string ToRelativeTime(DateTime time)
    {
        var tempString = "--";
        var timeSpam = DateTime.Now - time;

        if (timeSpam.Days >= 7)
            tempString = time.ToString("dd.MM.yyyy", new CultureInfo("vi-VN").DateTimeFormat);
        else if (timeSpam.Days > 0 && timeSpam.Days <= 7)
            tempString = time.ToString("dddd", new CultureInfo("vi-VN").DateTimeFormat);
        else if (timeSpam.Days == 0 && timeSpam.Hours >= 0 && timeSpam.Minutes > 1)
            tempString = time.ToString("HH:mm", new CultureInfo("vi-VN").DateTimeFormat);
        else if (timeSpam.Days == 0 && timeSpam.Hours == 0 && timeSpam.Minutes >= 1)
            tempString = timeSpam.Minutes + " " + "phút" + " " + "trước";
        else
            tempString = "vài giây trước";

        return tempString;
    }
}
