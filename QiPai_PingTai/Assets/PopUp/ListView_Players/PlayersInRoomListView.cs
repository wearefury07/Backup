using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayersInRoomListView : MonoBehaviour
{
    public UIListView uiListView;
    public int items = 10;
    private List<UserData> listData = new List<UserData>();
    private List<PlayersItemView> listView = new List<PlayersItemView>();

    private void OnEnable()
    {
        if (WarpClient.wc != null)
        {
            uiListView.ClearList();
            listData = new List<UserData>();
            listView = new List<PlayersItemView>();
            WarpClient.wc.OnGetPlayers += Wc_OnGetPlayers;
        }
    }

    private void OnDisable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnGetPlayers -= Wc_OnGetPlayers;
        }
    }

    private void Wc_OnGetPlayers(List<UserData> users)
    {
        if (users != null)
        {
            listData = users;
            FillData();
        }
    }

    public void Get_Data()
    {
        uiListView.ClearList();
        listData = new List<UserData>();
        listView = new List<PlayersItemView>();
        BuildWarpHelper.CASINO_GetPlayerInRoom(null);
    }

    public void Clean_Data()
    {
        uiListView.ClearList();
        listData = new List<UserData>();
        listView = new List<PlayersItemView>();
    }


    public void FillData()
    {
        if (listData != null && listData.Any())
        {
            int count = 0;
            foreach (var i in listData)
            {
                try
                {
                    var ui = uiListView.GetUIView<PlayersItemView>(uiListView.GetDetailView());
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
        OGUIM.Toast.Hide();
    }
}
