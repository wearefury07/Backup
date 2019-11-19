using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public static class Ultility
{

	public static string FormatNumber(long value)
	{
		string formatted = value.ToString();
		if (value < 10000) {
			formatted = value.ToString ("N0", new CultureInfo ("vi-VN"));
		} else if (value >= 10000) {
			formatted = (value / 1000).ToString ("N0", new CultureInfo ("vi-VN")) + "K";
		} else if (value >= 1000000) {
			formatted = (value / 1000000).ToString ("N0", new CultureInfo ("vi-VN")) + "M";
		}

		return formatted;
	}

    public static string CoinToString(long coin)
    {
        if (coin == 0)
            return "+0";
        string str = coin >= 0 ? "+" : "-";
        long tempCoin = coin >= 0 ? coin : -coin;
        str += tempCoin.ToString("N0", new CultureInfo("vi-VN"));
        //bool check = false;
        //if (tempCoin >= 1000000000)
        //{
        //    str += Mathf.FloorToInt(tempCoin / 1000000000) + "";
        //    str += ".";
        //    tempCoin = tempCoin % 1000000000;
        //    check = true;
        //}
        //if(tempCoin == 0)
        //{
        //    str += "000.000.000";
        //}
        //else
        //{
        //    if (tempCoin >= 1000000)
        //    {
        //        str += Mathf.FloorToInt(tempCoin / 1000000).ToString(check ? "000" : "");
        //        str += ".";
        //        tempCoin = tempCoin % 1000000;
        //        check = true;
        //    }
        //    if (tempCoin == 0)
        //    {
        //        str += "000.000";
        //    }
        //    else
        //    {
        //        if (tempCoin >= 1000)
        //        {
        //            str += Mathf.FloorToInt(tempCoin / 1000).ToString(check ? "000" : "");
        //            str += ".";
        //            tempCoin = tempCoin % 1000;
        //            check = true;
        //        }
        //        str += tempCoin.ToString(check ? "000" : "");
        //    }
        //}
        return str;
    }

	public static readonly int[] listMoney = { 5000, 20000, 100000, 500000 };

    public static List<int> ConvertBet2Chip(int bet, int chipType)
    {
        int betValid = (bet * (chipType == 1 ? 10 : 1) / 5000) * 5000;
        List<int> coins = new List<int>();
        for (int i = listMoney.Length - 1; i >= 0; i--)
        {
            while (betValid >= listMoney[i])
            {
                betValid -= listMoney[i];
                coins.Add(i);
            }
        }
        return coins;
    }

	public static string CoinToStringNoMark(long coin)
	{
		return CoinToString (coin).Replace ("+", "");
	}

    public static byte[] TrimEnd(byte[] array)
	{
		int lastIndex = Array.FindLastIndex(array, b => b != 0);

		Array.Resize(ref array, lastIndex + 1);

		return array;
	}
		
}

