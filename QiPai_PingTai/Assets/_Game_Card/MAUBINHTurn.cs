using System;
using System.Collections.Generic;
using System.Linq;

public enum MAUBINH_TurnKind
{
    NONE = 0,
    MT = 1,
    D = 2,
    HD = 3,
    SC = 4,
    S = 5,
    T = 6,
    CL = 7,
    TQ = 8,
    TPS = 9
}
public class MAUBINHTurn
{
    public static readonly int STRAIGHT_FLUSH = 8000000;
    // + valueHighCard()
    public static readonly int FOUR_OF_A_KIND = 7000000;
    // + Quads Card Rank
    public static readonly int FULL_HOUSE = 6000000;
    // + SET card rank
    public static readonly int FLUSH = 5000000;
    // + valueHighCard()
    public static readonly int STRAIGHT = 4000000;
    // + valueHighCard()
    public static readonly int SET = 3000000;
    // + Set card value
    public static readonly int TWO_PAIRS = 2000000;
    // + High2*15^4+ Low2*15^2 + card
    public static readonly int ONE_PAIR = 1000000;
    // + high*15^2 + high2*15^1 + low

    public string Name { get; set; }
    public MAUBINH_TurnKind Kind;
    private List<CardData> _dataOfCards = new List<CardData>();
    public int Value
    {
        get
        {
            switch (Kind)
            {
                case MAUBINH_TurnKind.MT:
                    return valueHighCard(DataOfCards, true);
                case MAUBINH_TurnKind.D:
                    return valueOnePair(DataOfCards);
                case MAUBINH_TurnKind.HD:
                    return valueTwoPairs(DataOfCards);
                case MAUBINH_TurnKind.SC:
                    return valueSet(DataOfCards);
                case MAUBINH_TurnKind.S:
                    return valueStraight(DataOfCards);
                case MAUBINH_TurnKind.T:
                    return valueFlush(DataOfCards);
                case MAUBINH_TurnKind.CL:
                    return valueFullHouse(DataOfCards);
                case MAUBINH_TurnKind.TQ:
                    return valueFourOfAKind(DataOfCards);
                case MAUBINH_TurnKind.TPS:
                    return valueStraightFlush(DataOfCards);
                default:
                    return 0;
            }
        }
    }
    public string FullName
    {
        get
        {
            switch (Name)
            {
                case "dach":
                    return "Dách";
                case "thu":
                    return "Thú";
                case "samco":
                    return "Sám Cô";
                case "sanh":
                    return "Sảnh";
                case "sanhha":
                    return "Sảnh Hạ";
                case "sanhthuong":
                    return "Sảnh Thượng";
                case "thung":
                    return "Thùng";
                case "culu":
                    return "Cù Lũ";
                case "tuquy":
                    return "Tứ quý";
                case "tuquyat":
                    return "Tứ Quý Át";
                case "thungphasanh":
                    return "Thùng Phá Sảnh";
                case "thungphasanhha":
                    return "Thùng Phá Sảnh Hạ";
                case "thungphasanhthuong":
                    return "Thùng Phá Sảnh Thượng";
                default:
                    return "Mậu Thầu";
            }
        }
    }
    public List<CardData> DataOfCards
    {
        get { return _dataOfCards; }
        set
        {
            _dataOfCards = value.OrderBy(x => x.card).ToList();
            Kind = GetKindByCards();
        }
    }
    public MAUBINHTurn()
    {
        Kind = MAUBINH_TurnKind.NONE;
        Name = "";
    }
    public MAUBINHTurn(List<CardData> cards) : this()
    {
        DataOfCards = cards;
    }
    public void Reset()
    {
        DataOfCards.Clear();
        Kind = MAUBINH_TurnKind.NONE;
        Name = "";
    }
    public override string ToString()
    {
        return Kind.ToString();
    }
    private MAUBINH_TurnKind GetKindByCards()
    {
        if (DataOfCards.Count == 5)
        {
            if (CardLogic.IsThungPhaSanh(DataOfCards.ToArray()))
            {
                if (DataOfCards.Any(x => x.card % 13 == 1))
                {
                    if (DataOfCards.Any(x => x.card == 13))
                        Name = "thungphasanhthuong";
                    else
                        Name = "thungphasanhha";
                }
                else
                    Name = "thungphasanh";
                return MAUBINH_TurnKind.TPS;
            }
            if (CardLogic.IsThung(DataOfCards.ToArray()))
            {
                Name = "thung";
                return MAUBINH_TurnKind.T;
            }
            if (CardLogic.IsSanh(DataOfCards.ToArray()))
            {
                if (DataOfCards.Any(x => x.card % 13 == 1))
                {
                    if (DataOfCards.Any(x => x.card == 13))
                        Name = "sanhthuong";
                    else
                        Name = "sanhha";
                }
                else
                    Name = "sanh";
                return MAUBINH_TurnKind.S;
            }
            if (CardLogic.IsTuQuy(DataOfCards.Take(4).ToArray()) || CardLogic.IsTuQuy(DataOfCards.Skip(1).Take(4).ToArray()))
            {
                var sum = DataOfCards.Sum(x => x.card);
                if (DataOfCards.Any(x => x.card % 13 == 1))
                    Name = "tuquyat";
                else
                    Name = "tuquy";
                return MAUBINH_TurnKind.TQ;
            }
            if (CardLogic.isSamCo(DataOfCards.Take(3).ToArray()))
            {
                if (CardLogic.IsDoi(DataOfCards[3], DataOfCards[4]))
                {
                    Name = "culu";
                    return MAUBINH_TurnKind.CL;
                }
                else
                {
                    Name = "samco";
                    return MAUBINH_TurnKind.SC;
                }
            }
            if (CardLogic.isSamCo(DataOfCards.Skip(2).Take(3).ToArray()))
            {
                if (CardLogic.IsDoi(DataOfCards[0], DataOfCards[1]))
                {
                    Name = "culu";
                    return MAUBINH_TurnKind.CL;
                }
                else
                {
                    Name = "samco";
                    return MAUBINH_TurnKind.SC;
                }
            }
            if (CardLogic.isSamCo(DataOfCards.Skip(1).Take(3).ToArray()))
            {
                Name = "samco";
                return MAUBINH_TurnKind.SC;
            }
            if (CardLogic.IsDoi(DataOfCards[0], DataOfCards[1]))
            {
                if (CardLogic.IsDoi(DataOfCards[2], DataOfCards[3]))
                {
                    Name = "thu";
                    return MAUBINH_TurnKind.HD;
                }
                if (CardLogic.IsDoi(DataOfCards[3], DataOfCards[4]))
                {
                    Name = "thu";
                    return MAUBINH_TurnKind.HD;
                }
                Name = "dach";
                return MAUBINH_TurnKind.D;
            }
            if (CardLogic.IsDoi(DataOfCards[1], DataOfCards[2]))
            {
                if (CardLogic.IsDoi(DataOfCards[3], DataOfCards[4]))
                {
                    Name = "thu";
                    return MAUBINH_TurnKind.HD;
                }
                Name = "dach";
                return MAUBINH_TurnKind.D;
            }
            if (CardLogic.IsDoi(DataOfCards[2], DataOfCards[3]) || CardLogic.IsDoi(DataOfCards[3], DataOfCards[4]))
            {
                Name = "dach";
                return MAUBINH_TurnKind.D;
            }
        }
        else if (DataOfCards.Count == 3)
        {
            if (CardLogic.isSamCo(DataOfCards.ToArray()))
            {
                Name = "samco";
                return MAUBINH_TurnKind.SC;
            }
            if (CardLogic.IsDoi(DataOfCards[0], DataOfCards[1]))
            {
                Name = "dach";
                return MAUBINH_TurnKind.D;
            }
            if (CardLogic.IsDoi(DataOfCards[1], DataOfCards[2]))
            {
                Name = "dach";
                return MAUBINH_TurnKind.D;
            }
        }
        Name = "mauthau";
        return MAUBINH_TurnKind.MT;
    }

    #region Calculate Value
    public static int valueStraightFlush(List<CardData> h)
    {
        return STRAIGHT_FLUSH + valueHighCard(h, true);
    }

    public static int valueFlush(List<CardData> h)
    {
        return FLUSH + valueHighCard(h, true);
    }

    public static int valueStraight(List<CardData> h)
    {
        return STRAIGHT + valueHighCard(h, true);
    }
    public static int valueFourOfAKind(List<CardData> h)
    {
        var card = h[2].card;
        if (card == 1)
            card += 13;
        return FOUR_OF_A_KIND + card;
    }

    public static int valueFullHouse(List<CardData> h)
    {
        var card = h[2].card;
        if (card == 1)
            card += 13;
        return FULL_HOUSE + card;
    }

    public static int valueSet(List<CardData> h)
    {
        var card = h[2].card;
        if (card == 1)
            card += 13;
        return SET + card;
    }

    public static int valueTwoPairs(List<CardData> cards)
    {
        int val = 0;
        var h = new List<CardData>(cards);
        foreach (var c in h)
            if (c.card == 1)
                c.card += 13;
        h = h.OrderBy(x => x.card).ToList();

        if (h[0].card == h[1].card &&
             h[2].card == h[3].card)
            val = 15 * 15 * h[2].card + 15 * h[0].card + h[4].card;
        else if (h[0].card == h[1].card &&
                  h[3].card == h[4].card)
            val = 15 * 15 * h[3].card + 15 * h[0].card + h[2].card;
        else
            val = 15 * 15 * h[3].card + 15 * h[1].card + h[0].card;

        return TWO_PAIRS + val;
    }

    public static int valueOnePair(List<CardData> cards)
    {
        int val = 0;
        var h = new List<CardData>(cards);
        foreach (var c in h)
            if (c.card == 1)
                c.card += 13;
        h = h.OrderBy(x => x.card).ToList();


        if (h.Count == 3)
        {
            if (h[0].card == h[1].card)
                val = 15 * 15 * 15 * h[0].card + 15 * 15 * h[2].card;
            if (h[1].card == h[2].card)
                val = 15 * 15 * 15 * h[1].card + 15 * 15 * h[0].card;
        }
        else
        {
            if (h[0].card == h[1].card)
                val = 15 * 15 * 15 * h[0].card + h[2].card + 15 * h[3].card + 15 * 15 * h[4].card;
            else if (h[1].card == h[2].card)
                val = 15 * 15 * 15 * h[1].card + h[0].card + 15 * h[3].card + 15 * 15 * h[4].card;
            else if (h[2].card == h[3].card)
                val = 15 * 15 * 15 * h[2].card + h[0].card + 15 * h[1].card + 15 * 15 * h[4].card;
            else
                val = 15 * 15 * 15 * h[3].card + h[0].card + 15 * h[1].card + 15 * 15 * h[2].card;
        }

        return ONE_PAIR + val;
    }

    public static int valueHighCard(List<CardData> cards, bool checkAce)
    {
        int val = 0;
        var h = cards.Select(x =>
        {
            var tempCard = x.card;
            if (tempCard == 1 && checkAce)
                tempCard += 13;
            return new CardData(tempCard, x.face);
        }).OrderBy(x => x.card).ToList();


        for (int i = 0; i < h.Count; i++)
        {
            var card = h[i].card;
            if (card == 1 && checkAce)
                card += 13;
            val += (int)Math.Pow(15, i + (h.Count == 3 ? 2 : 0)) * card;
        }

        return val;
    }
    #endregion
}
