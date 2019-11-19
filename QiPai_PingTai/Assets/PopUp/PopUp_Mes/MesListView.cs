using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIListView))]
[DisallowMultipleComponent]
public class MesListView : MonoBehaviour
{
    public MesType mesType;
    public int tabIndex;
    public UIListView uiListView;
    public int items = 10;
    private List<Message> listData = new List<Message>();
    private List<MesItemView> listView = new List<MesItemView>();

    private void OnEnable()
    {
        tabIndex = -1;
    }

    public void Get_Data(int index)
    {
        if (tabIndex != index)
        {
            uiListView.ClearList();
            listData = new List<Message>();
            listView = new List<MesItemView>();
        }

        if (!listData.Any() || !listView.Any())
        {
            if (mesType == MesType.ADMIN)
                PopupAllMes.GetPromoAndAdminMes();
            else
                PopupAllMes.GetFriendAndGiftMes();
        }

        tabIndex = index;
    }

    public void FillData(List<Message> _data)
    {
        listData = _data;

        if (listData != null)
        {
            uiListView.ClearList();
            listView = new List<MesItemView>();
            int count = 0;
            foreach (var i in listData)
            {
                try
                {
                    var ui = uiListView.GetUIView<MesItemView>(uiListView.GetDetailView());
                    if (count % 2 != 0)
                        ui.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

                    if (ui.FillData(i))
                    {
                        listView.Add(ui);
                        count++;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("FillData: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
    }

}
