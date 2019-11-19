using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

public static class LongConverter
{
    public static string ToK(object value)
    {
        var longValue = long.Parse(value.ToString());
        if (longValue > 9999999)
            return (longValue / 1000000).ToString("N0", new CultureInfo("vi-VN")) + "M";
        else if (longValue > 9999)
            return (longValue / 1000).ToString("N0", new CultureInfo("vi-VN")) + "K";
        else
            return longValue.ToString("N0", new CultureInfo("vi-VN"));
    }

    public static string ToM(object value)
    {
        var longValue = long.Parse(value.ToString());
        if (longValue > 99999999)
            return (longValue / 1000000).ToString("N0", new CultureInfo("vi-VN")) + "M";
        else if (longValue > 99999)
            return (longValue / 1000).ToString("N0", new CultureInfo("vi-VN")) + "K";
        else
            return longValue.ToString("N0", new CultureInfo("vi-VN"));
    }

    public static string ToFull(object value)
    {
        var longValue = long.Parse(value.ToString());
        return longValue.ToString("N0", new CultureInfo("vi-VN"));
    }
}

public static class FaceConverter
{
    public static string ToString(CardData data)
    {
        string returnFace;
        string returnCard;

        switch (data.card)
        {
            case 1:
                returnCard = "A";
                break;
            case 11:
                returnCard = "J";
                break;
            case 12:
                returnCard = "Q";
                break;
            case 13:
                returnCard = "K";
                break;
            default:
                returnCard = data.card.ToString();
                break;
        }
        switch (data.face)
        {
            case 1:
                returnFace = "♠";
                break;
            case 2:
                returnFace = "♣";
                break;
            case 3:
                returnFace = "♦";
                break;
            case 4:
                returnFace = "♥";
                break;
            default:
                returnFace = "";
                break;
        }
        return returnCard + returnFace;
    }
}