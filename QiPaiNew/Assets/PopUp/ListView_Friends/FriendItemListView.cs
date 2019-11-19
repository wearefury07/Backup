using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

public class FriendItemListView : MonoBehaviour
{
    public UIListView uiListView;
    public int items = 10;
    private List<UserData> listData = new List<UserData>();
    private List<PlayersItemView> listView = new List<PlayersItemView>();


    private void OnEnable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnGetFriendsDone += Wc_OnGetFriendsDone;
        }
    }

    private void OnDisable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnGetFriendsDone -= Wc_OnGetFriendsDone;
        }
    }

    private void Wc_OnGetFriendsDone(WarpResponseResultCode status, List<UserData> data)
    {
        OGUIM.Toast.Hide();
        if (data != null)
        {
            uiListView.ClearList();
            listData = new List<UserData>();
            listView = new List<PlayersItemView>();
            listData = data;

            FillData();
        }
    }
    public void Get_Data(bool reload)
    {
        if (reload)
        {
            uiListView.ClearList();
            listData = new List<UserData>();
            listView = new List<PlayersItemView>();
        }

        if (!listView.Any() || reload)
		{
			OGUIM.Toast.ShowLoading("");
			//OGUIM.Toast.ShowLoading("Đang tải dữ liệu, vui lòng chờ giây lát...");
            WarpRequest.GetFriends();
        }
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
