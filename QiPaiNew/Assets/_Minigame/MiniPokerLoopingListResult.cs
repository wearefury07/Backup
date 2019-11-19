using UnityEngine;
using System.Collections;

public class MiniPokerLoopingListResult : MonoBehaviour {
    public Card[] cards;
	void Start ()
    {
	}

    public void SetData(params CardData[] cardDatas)
    {
        for (int i = 0; i < cardDatas.Length; i++)
            cards[i].SetData(cardDatas[i]);
    }
}
