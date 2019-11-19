using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerChatListView : MonoBehaviour
{
    public UIListView uiListView;
    public static List<PlayerChatView> listView = new List<PlayerChatView>();

    public void Start()
    {
        InvokeRepeating("UpdateChatTime", 1f, 1f);
    }

    public void UpdateChatTime()
    {
        if (listView != null && listView.Any())
        {
            foreach (var i in listView)
            {
                i.UpdateTime();
            }
        }
    }


    public void FillData(RootChatData data)
    {
        var ui = uiListView.GetUIView<PlayerChatView>(uiListView.GetDetailView());
        ui.FillData(data);
        listView.Add(ui);
        DOVirtual.DelayedCall(0.3f, () => uiListView.ScrollVerticalToBottom());
    }

    public void FillData(List<RootChatData> listData)
    {
        listView.Clear();
        uiListView.ClearList();

        foreach (var i in listData)
        {
            var ui = uiListView.GetUIView<PlayerChatView>(uiListView.GetDetailView());
            ui.FillData(i);
            listView.Add(ui);
        }
        DOVirtual.DelayedCall(0.3f, () => uiListView.ScrollVerticalToBottom());
    }
}
