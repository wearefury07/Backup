using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistoryItemView : MonoBehaviour
{
    public List<Text> labels;
    public Button buttonDetail;
    public List<string> data;

    public bool FillData(List<string> _data)
    {
        if (labels == null || !labels.Any())
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var label = transform.GetChild(i).GetComponent<Text>();
                if (label != null)
                    labels.Add(label);
            }
        }

        if (labels != null && _data != null)
        {
            data = _data;
            for (int i = 0; i < labels.Count; i++)
            {
                try
                {
                    labels[i].text = data[i];
                }
                catch (System.Exception ex)
                {
                    UILogView.Log("HistoryItemView: " + ex.Message, true);
                    return false;
                }
            }
            return true;
        }
        else
            return false;
    }
}

public class HistoryData
{
    public List<string> data;
}

public enum HistoryType
{
    USER,
    JACKPOT,
    CASHOUTUSER,
    CASHOUTHISTORY,
    PAYLOAD
}