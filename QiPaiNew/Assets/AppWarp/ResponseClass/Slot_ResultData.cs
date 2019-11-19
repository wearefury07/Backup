using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class SLOT_ResultData
{
    public Result result;
    public int chips;
    public int spinID;
    public long jackpot;
    public int type;
    public int roomId;
    public List<HistoryBase> slotHistory;
}
[Serializable]
public class Slots
{
    public List<int[]> faces;

    public void GetRandom()
    {
        faces = new List<int[]>();
        for (int i = 0; i < 3; i++)
        {
            var f = new int[5] { UnityEngine.Random.Range(1,8), UnityEngine.Random.Range(1, 8), UnityEngine.Random.Range(1, 8), UnityEngine.Random.Range(1, 8), UnityEngine.Random.Range(1, 8) };
            faces.Add(f);
        }
    }
}

[Serializable]
public class PayLine
{
    public int line;
    public int face;
    public List<int> matched;
}

[Serializable]
public class Result
{
    public Slots slots;
    public int jackpotNum;
    public int freeSpins;
    public int winChips;
    public List<PayLine> payLines;
    public int betChips;
    public int bonusGame;
    public bool isWinJackpot;

    
    public void GenarateResult(int bet, params int[] selectedLines)
    {
        slots = new Slots();
        slots.GetRandom();

        betChips = bet;

        payLines = new List<PayLine>();
        CalculateResult(selectedLines);

    }

    void CalculateResult(int[] selectedLines)
    {
        for (int i = 1; i <= 20; i++)
        {
            var pl = new PayLine() { line = i, matched = new List<int>() };
            var line = SlotLine.GetLine(i);
            var takenFaces = line.Select(l => new { k = slots.faces[l.x][l.y], v = l.x * 5 + l.y }).ToList();
            var orderedFaces = takenFaces.OrderBy(x => x.k).ToList();
            var midleFace = orderedFaces[2].k;
            if (takenFaces.Count(f => f.k == midleFace) >= 3)
            {
                for (int ii = 0; ii < takenFaces.Count; ii++)
                {
                    if (takenFaces[ii].k == midleFace)
                    {
                        pl.matched.Add(takenFaces[ii].v);
                        pl.face = midleFace;
                    }
                }
            }
            if (pl.matched.Count >= 3 && selectedLines.Any(sl => sl == pl.line))
            {
                payLines.Add(pl);
                int bonus = GetBonus(pl.matched.Count, pl.face);
                if(bonus == int.MaxValue)
                {
                    winChips += int.MaxValue;
                    jackpotNum = pl.line;
                    isWinJackpot = true;
                }
                else if(bonus < 0)
                {
                    freeSpins++;
                }
                else if(bonus > 0)
                {
                    winChips += betChips * bonus;
                }
            }
        }
    }

    int GetBonus(int count, int symbol)
    {
        switch (symbol)
        {
            case 1:
                if (count == 5)
                {
                    return int.MaxValue;
                }
                else if (count == 4)
                {
                    return 30;
                }
                else if (count == 3)
                {
                    return 5;
                }
                return 0;
            case 2:
                if (count == 5)
                {
                    return 8000;
                }
                else if (count == 4)
                {
                    return 25;
                }
                else if (count == 3)
                {
                    return 4;
                }
                return 0;
            case 3:
                if (count == 5)
                {
                    return -15;
                }
                else if (count == 4)
                {
                    return -3;
                }
                else if (count == 3)
                {
                    return -1;
                }
                return 0;
            case 4:
                if (count == 5)
                {
                    return 500;
                }
                else if (count == 4)
                {
                    return 20;
                }
                else if (count == 3)
                {
                    return 4;
                }
                return 0;
            case 5:
                if (count == 5)
                {
                    return 200;
                }
                else if (count == 4)
                {
                    return 15;
                }
                else if (count == 3)
                {
                    return 3;
                }
                return 0;
            case 6:
                if (count == 5)
                {
                    return 75;
                }
                else if (count == 4)
                {
                    return 10;
                }
                else if (count == 3)
                {
                    return 2;
                }
                return 0;
            case 7:
                if (count == 5)
                {
                    return 30;
                }
                else if (count == 4)
                {
                    return 6;
                }
                return 0;
            default:
                return 0;
        }
    }
}

[Serializable]
public class HistoryBase
{
    public int id;
    public string displayName;
    public string time;
    public int bet;
    public int win;
    public string desc;
    public int add;
    public int sub;
    public int total;
    public int code;
    public int gold;
    public int koin;

    //MiniSpin
    public int largFace;
    public int smallFace;

    //Taixiu
    public int betMax;
	public int betMin;
	public CardData result;
	public int payback;
    public List<int> faces;
    public int point;

    //Caothap
    public int isHigh;
    public int turn;
}

public class SlotLine
{
    public static readonly int[] symbols = new int[] { 1, 2, 3, 4, 5, 6, 7 };
    public static readonly Point[] line1 = new Point[] { new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(1, 3), new Point(1, 4) };
    public static readonly Point[] line2 = new Point[] { new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(0, 3), new Point(0, 4) };
    public static readonly Point[] line3 = new Point[] { new Point(2, 0), new Point(2, 1), new Point(2, 2), new Point(2, 3), new Point(2, 4) };
    public static readonly Point[] line4 = new Point[] { new Point(1, 0), new Point(1, 1), new Point(0, 2), new Point(1, 3), new Point(1, 4) };
    public static readonly Point[] line5 = new Point[] { new Point(1, 0), new Point(1, 1), new Point(2, 2), new Point(1, 3), new Point(1, 4) };
    public static readonly Point[] line6 = new Point[] { new Point(0, 0), new Point(0, 1), new Point(1, 2), new Point(0, 3), new Point(0, 4) };
    public static readonly Point[] line7 = new Point[] { new Point(2, 0), new Point(2, 1), new Point(1, 2), new Point(2, 3), new Point(2, 4) };
    public static readonly Point[] line8 = new Point[] { new Point(0, 0), new Point(2, 1), new Point(0, 2), new Point(2, 3), new Point(0, 4) };
    public static readonly Point[] line9 = new Point[] { new Point(2, 0), new Point(0, 1), new Point(2, 2), new Point(0, 3), new Point(2, 4) };
    public static readonly Point[] line10 = new Point[] { new Point(1, 0), new Point(0, 1), new Point(2, 2), new Point(0, 3), new Point(1, 4) };
    public static readonly Point[] line11 = new Point[] { new Point(2, 0), new Point(1, 1), new Point(0, 2), new Point(1, 3), new Point(2, 4) };
    public static readonly Point[] line12 = new Point[] { new Point(0, 0), new Point(1, 1), new Point(2, 2), new Point(1, 3), new Point(0, 4) };
    public static readonly Point[] line13 = new Point[] { new Point(1, 0), new Point(2, 1), new Point(1, 2), new Point(0, 3), new Point(1, 4) };
    public static readonly Point[] line14 = new Point[] { new Point(1, 0), new Point(0, 1), new Point(1, 2), new Point(2, 3), new Point(1, 4) };
    public static readonly Point[] line15 = new Point[] { new Point(2, 0), new Point(1, 1), new Point(1, 2), new Point(1, 3), new Point(2, 4) };
    public static readonly Point[] line16 = new Point[] { new Point(0, 0), new Point(1, 1), new Point(1, 2), new Point(1, 3), new Point(0, 4) };
    public static readonly Point[] line17 = new Point[] { new Point(1, 0), new Point(2, 1), new Point(2, 2), new Point(2, 3), new Point(1, 4) };
    public static readonly Point[] line18 = new Point[] { new Point(1, 0), new Point(0, 1), new Point(0, 2), new Point(0, 3), new Point(1, 4) };
    public static readonly Point[] line19 = new Point[] { new Point(2, 0), new Point(2, 1), new Point(1, 2), new Point(0, 3), new Point(0, 4) };
    public static readonly Point[] line20 = new Point[] { new Point(0, 0), new Point(0, 1), new Point(1, 2), new Point(2, 3), new Point(2, 4) };
    public static Point[] GetLine(int num)
    {
        switch (num)
        {
            case 1:
                return line1;
            case 2:
                return line2;
            case 3:
                return line3;
            case 4:
                return line4;
            case 5:
                return line5;
            case 6:
                return line6;
            case 7:
                return line7;
            case 8:
                return line8;
            case 9:
                return line9;
            case 10:
                return line10;
            case 11:
                return line11;
            case 12:
                return line12;
            case 13:
                return line13;
            case 14:
                return line14;
            case 15:
                return line15;
            case 16:
                return line16;
            case 17:
                return line17;
            case 18:
                return line18;
            case 19:
                return line19;
            default:
                return line20;
        }
    }

    Point[] line;

    public SlotLine()
    {
        line = new Point[5];
    }
    public Point[] GetLine()
    {
        return line;
    }

    public void SetLine(Point[] line)
    {
        this.line = line;
    }
}

public class Point
{
    public int x;
    public int y;
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class SlotVi
{
    public int[][] faces;
    private const int rows = 3;
    private const int cols = 5;
    public SlotVi()
    {
        faces = new int[rows][];
    }

    public List<PayLine> GetValidLines()
    {
        return null;
    }

}



