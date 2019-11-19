using System;

[Serializable]
public class CardData
{
    public int card;
    public int face;
    
    public int pair;
    public float suite;
    public int index;

    public CardData() { }
    public CardData(int card, int face)
    {
        this.card = card;
        this.face = face;
    }

    public override string ToString()
    {
        return (card > 13 ? (card % 13) : card) + "_" + face;
    }
    public int ToNumber()
    {
        var tempCard = (card > 13 ? (card % 13) : card);
        return (tempCard - 1) * 4 + face - 1;
    }
    public int IndexNumber {
        get
        {
            var tempCard = (card > 13 ? (card % 13) : card);
            return (tempCard - 1) * 4 + face;
        }
    }
    public int IndexFace
    {
        get
        {
            var tempCard = (card > 13 ? (card % 13) : card);
            return face * 13 + (tempCard - 1);
        }
    }
    public bool EqualNumber(CardData cardData)
    {
        return card == cardData.card || card + 13 == cardData.card;
    }
    public bool EqualFace(CardData cardData)
    {
        return face == cardData.face;
    }
    public bool Equal(CardData cardData)
    {
        return EqualNumber(cardData) && EqualFace(cardData);
    }
}
