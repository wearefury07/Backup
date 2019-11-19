using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class LoopingList : ZenLoopingList {
    
    public Vector2 spacing;
    LoopingListResult loopingResult;
    int roomId;
    public override void Awake()
    {
        base.Awake();
        spacing = GetComponent<GridLayoutGroup>().spacing;
    }
    public override void UpdateUI(int roomId)
    {
        base.UpdateUI(roomId);
        this.roomId = roomId;
        for (int i = 0; i < allItems.Count; i++)
        {
            var key = string.Format("slot_{0}_{1}", roomId, i + 1);
            allItems[i].GetComponent<Image>().sprite = ImageSheet.Instance.resourcesDics[key];
        }
        if (loopingResult == null)
            loopingResult = resultPanel.GetComponent<LoopingListResult>();
        loopingResult.SetData(roomId, 1, 2, 3);
    }
    public override IEnumerator DoSpin(float time, int[] results)
    {
        //var offset = Random.Range(-50, 50);
		//var offset = 200;
        //var count = Random.Range(20, 20);
		var count = 30;
		for (int i = 0; i < count; i++)
        {
            allItems.Add(CreateItem(transform, "slot_" + roomId + "_" + Random.Range(1,8)));
        }
        foreach (var r in results)
            allItems.Add(CreateItem(transform, "slot_" + roomId + "_" + r));
        allItems.Add(CreateItem(transform, "slot_" + roomId + "_" + Random.Range(1, 8)));
        count += results.Length;

        yield return new WaitForSeconds(0.1f);
        var origin = rect.localPosition;
        var yNext = rect.localPosition.y;
        yNext += -count * (cellSize.y + spacing.y); // + offset;
		rect.DOLocalMoveY(yNext, time).SetEase(Ease.Linear).OnComplete(() =>
        {
            var posCurrent = rect.localPosition;
            yNext = posCurrent.y; // - offset;
			//rect.DOLocalMoveY(yNext, 1f).SetEase(Ease.OutBounce).OnComplete(() =>
            //{
                var removedItems = new List<GameObject>();
                for (int i = 0; i < count; i++)
                {
                    removedItems.Add(allItems[i]);
                }
                removedItems.Add(allItems.LastOrDefault());
                for (int i = 0; i < removedItems.Count; i++)
                {
                    allItems.Remove(removedItems[i]);
                    removedItems[i].Recycle();
                }


                gameObject.SetActive(false);
                rect.localPosition = origin;
                loopingResult.SetData(allItems.Select(x => x.GetComponent<Image>().sprite).ToArray());
                loopingResult.gameObject.SetActive(true);
            //});
        });
    }

    GameObject CreateItem(Transform transform, string name)
    {
        var item = itemPrefab.Spawn();
        try
        {
            if (ImageSheet.Instance.resourcesDics.ContainsKey(name))
            {
                Sprite sprite = ImageSheet.Instance.resourcesDics[name];
                item.GetComponent<Image>().sprite = sprite;
            }
            else
            {
                UILogView.Log("LoopingList / CreateItem: " + name + " is not found in resourcesDics");
            }
            item.transform.SetParent(transform, false);
        }
        catch (System.Exception ex)
        {
            UILogView.Log("LoopingList / CreateItem ex: " + ex.Message);
        }
        return item;
    }
}
