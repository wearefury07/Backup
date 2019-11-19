using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsListView : MonoBehaviour
{

    public UIListView uiListView;
    public int items = 20;
    public UIAnimation anim;
    private List<Stat> listData = new List<Stat>();
    private List<StatisticsItemView> listView = new List<StatisticsItemView>();

    public void Show(List<Stat> _listData)
    {
        if (_listData != null)
        {
            uiListView.ClearList();
            listView = new List<StatisticsItemView>();
            listData = _listData;
        }
        anim.Show(() => FillData());
    }

    public void FillData()
    {
        if (listData != null && listData.Any())
        {
            int count = 0;
            foreach (var i in listData)
            {
                if (i.zoneId != (int)LobbyId.PHOM_SOLO && i.zoneId != (int)LobbyId.TLMNDL_SOLO && i.zoneId != (int)LobbyId.SAM_SOLO)
                {
                    try
                    {
                        var ui = uiListView.GetUIView<StatisticsItemView>(uiListView.GetDetailView());
                        if (count % 2 != 0)
                            ui.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

                        if (ui.FillData(i))
                            listView.Add(ui);

                        count++;
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("FillData: " + ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }
        }
    }
}
