using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviour
{
    public ZenLoopingList[] columns;
    public Image[] resultImages;
    public string doTweenId;

    public bool IsSpinning
    {
        get
        {
            return columns.Any(x => x.IsSpinning);
        }
    }

    public void UpdateUIByRoom(int roomId)
    {
        for(var i = 0; i< columns.Length; i++)
        {
            columns[i].UpdateUI(roomId);
        }
    }
    public void SpinTest()
    {
        if (IsSpinning)
        {
            Debug.Log("Spinning!!!");
        }
        else
        {
            try
            {
                for (int i = 0; i < columns.Length; i++)
                {
                    columns[i].Spin(i * 0.75f + 3f + Random.Range(0, 6) * 0.25f, 1, 2, 3);
                }
            }
            catch (System.Exception ex)
            {
                UILogView.Log("SlotMachine: Spin throw: " + ex.Message);
            }
        }
    }
    public void Spin(List<int[]> faces)
    {
        if (IsSpinning)
        {
            Debug.Log("Spinning!!!");
        }
        else
        {
            try
            {
                for (int i = 0; i < columns.Length; i++)
                {
                    columns[i].Spin(i * 0.33f + 2f, faces[2][i], faces[1][i], faces[0][i]);
                }
            }
            catch (System.Exception ex)
            {
                UILogView.Log("SlotMachine: Spin throw: " + ex.Message);
            }
        }
    }
    public void Spin(List<CardData> cardDatas)
    {
        if (IsSpinning)
        {
            Debug.Log("Spinning!!!");
        }
        else
        {
            try
            {
                for (int i = 0; i < columns.Length; i++)
                {
                    var cards = cardDatas.Skip(i * 10).Take(10).Select(x => x.ToNumber()).ToArray();
                    columns[i].Spin(i * 0.4f + 1.5f, cards.Concat(cards).ToArray());
                }
            }
            catch (System.Exception ex)
            {
                UILogView.Log("SlotMachine: Spin throw: " + ex.Message);
            }
        }
    }
    public void ShowLine(List<int> line = null, float delay = 0)
    {
        TweenCallback callback = () =>
        {
            for (int i = 0; i < 15; i++)
            {
                var ii = i;
                var color = resultImages[ii].color;
                if (line == null || !line.Any() || line.Contains(ii))
				{
					UIManager.PlaySound("combo_" + ii);
                    color.a = 1;
				}
                else
                    color.a = 0.3f;

                resultImages[ii].color = color;
            }
        };
        if (delay > 0)
            DOVirtual.DelayedCall(delay, callback).SetId(doTweenId);
        else
            callback();
    }
}
