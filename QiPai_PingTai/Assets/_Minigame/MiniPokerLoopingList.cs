using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

public class MiniPokerLoopingList : ZenLoopingList
{
    public int startCard;
    [HideInInspector]
    public Vector2 spacing;
    MiniPokerLoopingListResult loopingResult;

    public override void UpdateUI(int roomId)
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            allItems[i].GetComponent<Card>().SetData(new CardData(startCard, i + 1));
        }
        if (loopingResult == null)
            loopingResult = resultPanel.GetComponent<MiniPokerLoopingListResult>();
        loopingResult.SetData(new CardData(startCard, 1), 
                                new CardData(startCard, 2), 
                                new CardData(startCard, 3));
    }
    public override void Start()
    {
        base.Start();

        spacing = GetComponent<GridLayoutGroup>().spacing;
        loopingResult = resultPanel.GetComponent<MiniPokerLoopingListResult>();

    }
    public override IEnumerator DoSpin(float time, int[] results)
    {
        //var offset = Random.Range(-50, 50);
        var count = results.Length;
        for (int i = 0; i < results.Length; i++)
        {
            allItems.Add(CreateItem(transform, Mathf.FloorToInt(results[i] / 4) + 1, results[i] % 4 + 1));
        }

        yield return new WaitForSeconds(0.1f);
        var origin = rect.localPosition;
        var yNext = rect.localPosition.y;
		yNext += -(count) * (cellSize.y + spacing.y); // + offset;
		rect.DOLocalMoveY(yNext, time).SetEase(Ease.Linear).OnComplete(() =>
        {
            var posCurrent = rect.localPosition;
				yNext = posCurrent.y;// - offset;
            //rect.DOLocalMoveY(yNext, Random.Range(0.3f, 0.6f)).SetEase(Ease.InOutSine).OnComplete(() =>
            //{
                var removedItems = new List<GameObject>();
                for (int i = 0; i < count; i++)
                {
                    removedItems.Add(allItems[i]);
                }
                //removedItems.Add(allItems.LastOrDefault());
                for (int i = 0; i < removedItems.Count; i++)
                {
                    allItems.Remove(removedItems[i]);
                    removedItems[i].Recycle();
                }


                gameObject.SetActive(false);
                rect.localPosition = origin;
                loopingResult.SetData(allItems.Select(x => x.GetComponent<Card>().cardData).ToArray());
                loopingResult.gameObject.SetActive(true);
            //}).SetId(MiniGames.miniGameTweenId);
        }).SetId(MiniGames.miniGameTweenId); ;
    }
    GameObject CreateItem(Transform transform, int card, int face)
    {
        var item = itemPrefab.Spawn();
        try
        {
            item.GetComponent<Card>().SetData(new CardData(card, face));
            item.transform.SetParent(transform, false);
        }
        catch (System.Exception ex)
        {
            UILogView.Log("LoopingList / CreateItem ex: " + ex.Message);
        }
        return item;
    }
}
