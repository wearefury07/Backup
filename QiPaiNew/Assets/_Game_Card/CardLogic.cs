using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class CardLogic
{
    public static bool IsDoi(CardData card1, CardData card2)
    {
        return card1.card == card2.card && card1.face != card2.face;
    }
    public static bool IsSanh(params CardData[] cards)
    {
        if (cards.Length < 3)
            return false;
        var is2Existed = cards.Any(x => x.card == 2);
        var tempCards = cards.Select(x=> {
            var c = x.card;
            if (c >= 14)
                c -= 13;
            var card = new CardData(!is2Existed && c % 13 == 1 ? 14 : c, x.face);
            return card;
        }).OrderBy(c => c.card).ToList();
        var firstCard = tempCards.FirstOrDefault();
        for (int i = 0; i < tempCards.Count(); i++)
        {
            if (tempCards.ElementAtOrDefault(i).card != i + firstCard.card)
                return false;
        }
        return true;
    }
    public static bool isSamCo(params CardData[] cards)
    {
        return cards.Length == 3 && IsDoi(cards[0], cards[1]) && IsDoi(cards[0], cards[2]);
    }
    public static bool IsTuQuy(params CardData[] cards)
    {
        return cards.Length == 4 && IsDoi(cards[0], cards[1]) && IsDoi(cards[0], cards[2]) && IsDoi(cards[0], cards[3]);
    }
    public static bool IsThung(params CardData[] cards)
    {
        if (cards.Length < 3)
            return false;
        var first = cards.FirstOrDefault();
        return !cards.Any(x => x.face != first.face);
    }
    public static bool IsThungPhaSanh(params CardData[] cards)
    {
        return IsThung(cards) && IsSanh(cards);
    }

    public static bool Is3DoiThong(params CardData[] cards)
    {
        if (cards.Length != 6)
            return false;
        var tempCards = cards.OrderBy(x => x.card).ToArray();
        return IsSanh(tempCards[0], tempCards[2], tempCards[4]);
    }
    public static bool Is4DoiThong(params CardData[] cards)
    {
        if (cards.Length != 8)
            return false;
        var tempCards = cards.OrderBy(x => x.card).ToArray();
        return IsSanh(tempCards[0], tempCards[2], tempCards[4], tempCards[6]);
    }



    public static void callPair(List<CardData> cards)
    {
        foreach (var c1 in cards)
        {
            foreach (var c2 in cards)
            {
                if ((c1.card == c2.card && c1.face != c2.face) ||
                   (c1.card != c2.card && c1.face == c2.face && Math.Abs(c1.card - c2.card) <= 2))
                {
                    c1.pair = 2;
                }
            }
        }
    }
    public static void sortFace(List<CardData> cards, int pair, int id)
    {
        List<List<CardData>> pairList = new List<List<CardData>>();

        #region create pairList
        for (int i = 0; i < cards.Count; i++)
        {
            var c1 = cards[i];
            if (c1.pair == pair)
            {
                for (int j = 0; j < cards.Count; j++)
                {
                    var c2 = cards[j];
                    if (c2.pair == pair)
                    {
                        if (c1.card != c2.card && c1.face == c2.face && Math.Abs(c1.card - c2.card) <= 1)
                        {
                            if (pairList.Any())
                            {
                                var hasPair = false;
                                foreach (var pairChild in pairList)
                                {
                                    if (!pairChild.Any(x => x.Equal(c1)) && !pairChild.Any(x => x.Equal(c2)))
                                    {

                                    }
                                    else if (!pairChild.Any(x => x.Equal(c1)) && pairChild.Any(x => x.Equal(c2)))
                                    {
                                        c1.index = i;
                                        pairChild.Add(c1);
                                        hasPair = true;
                                    }
                                    else if (pairChild.Any(x => x.Equal(c1)) && pairChild.Any(x => x.Equal(c2)))
                                    {
                                        hasPair = true;
                                    }
                                    else if (pairChild.Any(x => x.Equal(c1)) && !pairChild.Any(x => x.Equal(c2)))
                                    {
                                        c2.index = j;
                                        pairChild.Add(c2);
                                        hasPair = true;
                                    }
                                }
                                if (!hasPair)
                                {
                                    c1.index = i;
                                    pairList.Add(new List<CardData> { c1 });
                                }
                            }
                            else
                            {
                                c1.index = i;
                                pairList.Add(new List<CardData> { c1 });
                            }
                        }
                    }
                }
            }
        }
        #endregion

        for (int i = 0; i < pairList.Count; i++)
        {
            var pairChild = pairList[i];
            if (pairChild.Count >= 3)
            {
                if (pairChild.Count >= 9)
                {
                    for (int j = 0; j < pairChild.Count; j++)
                    {
                        cards[pairChild[j].index].pair = 1;
                        cards[pairChild[j].index].suite = i + id + (j >= 6 ? 0.6f : j >= 3 ? 0.3f : 0);
                    }
                }
                else if (pairChild.Count >= 6)
                {
                    for (int j = 0; j < pairChild.Count; j++)
                    {
                        cards[pairChild[j].index].pair = 1;
                        cards[pairChild[j].index].suite = i + id + (j >= 3 ? 0.3f : 0);
                    }
                }
                else
                {
                    for (int j = 0; j < pairChild.Count; j++)
                    {
                        cards[pairChild[j].index].pair = 1;
                        cards[pairChild[j].index].suite = i + id;
                    }
                }
            }
        }
    }
    public static void sortNumber(List<CardData> cards, int pair, int id)
    {
        List<List<CardData>> pairList = new List<List<CardData>>();

        #region create pairList
        for (int i = 0; i < cards.Count; i++)
        {
            var c1 = cards[i];
            if (c1.pair == pair)
            {
                for (int j = 0; j < cards.Count; j++)
                {
                    var c2 = cards[j];
                    if (c2.pair == pair)
                    {
                        if (c1.card == c2.card && c1.face != c2.face)
                        {
                            if (pairList.Any())
                            {
                                var hasPair = false;
                                foreach (var pairChild in pairList)
                                {
                                    if (!pairChild.Any(x => x.Equal(c1)) && !pairChild.Any(x => x.Equal(c2)))
                                    {

                                    }
                                    else if (!pairChild.Any(x => x.Equal(c1)) && pairChild.Any(x => x.Equal(c2)))
                                    {
                                        c1.index = i;
                                        pairChild.Add(c1);
                                        hasPair = true;
                                    }
                                    else if (pairChild.Any(x => x.Equal(c1)) && pairChild.Any(x => x.Equal(c2)))
                                    {
                                        hasPair = true;
                                    }
                                    else if (pairChild.Any(x => x.Equal(c1)) && !pairChild.Any(x => x.Equal(c2)))
                                    {
                                        c2.index = j;
                                        pairChild.Add(c2);
                                        hasPair = true;
                                    }
                                }
                                if (!hasPair)
                                {
                                    c1.index = i;
                                    pairList.Add(new List<CardData> { c1 });
                                }
                            }
                            else
                            {
                                c1.index = i;
                                pairList.Add(new List<CardData> { c1 });
                            }
                        }
                    }
                }
            }
        }
        #endregion

        for (int i = 0; i < pairList.Count; i++)
        {
            var pairChild = pairList[i];
            if (pairChild.Count >= 3)
            {
                for (int j = 0; j < pairChild.Count; j++)
                {
                    cards[pairChild[j].index].pair = 1;
                    cards[pairChild[j].index].suite = i + id;
                }
            }
        }
    }
    public static List<CardData> sort(List<CardData> cards)
    {
		return cards.OrderBy(x => x.pair).ThenBy(y => y.suite).ThenBy(z =>z.card).ThenBy(t => t.face).ToList();
    }
}