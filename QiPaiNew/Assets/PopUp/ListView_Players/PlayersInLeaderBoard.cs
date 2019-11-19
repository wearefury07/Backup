using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersInLeaderBoard : MonoBehaviour
{
    public Text subTitle;
    public Lobby currentLobby;
    public UIListView uiListView;
    public int items = 10;
    private List<UserData> listData = new List<UserData>();
    private List<PlayersItemView> listView = new List<PlayersItemView>();

    private void OnEnable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnTopUserDone += Wc_OnTopUserDone;
        }
    }

    private void OnDisable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnTopUserDone -= Wc_OnTopUserDone;
        }
    }

    private void Wc_OnTopUserDone(WarpResponseResultCode status, RootTopUser data)
    {
        if (status == WarpResponseResultCode.SUCCESS && data != null)
        {
            if (currentLobby.id == (int)LobbyId.TOPGOLD)
                subTitle.text = "Đại gia " + "“" + GameBase.moneyGold.name + "„";
            else if (currentLobby.id == (int)LobbyId.TOPLEVEL)
                subTitle.text = "Cao thủ " + "“" + "Đệ nhất Thiên hạ" + "„";
            else
                subTitle.text = "Cao thủ " + "“" + currentLobby.desc + " " + currentLobby.subname + "„";

            if (!data.topUsers.Any(x => x.id == data.myPosition.id))
            {
                data.topUsers.Add(data.myPosition);
            }

            foreach (var i in data.topUsers)
            {
                if (i.exp > 0 && i.target > 0)
                    i.expPercent = (int)((float)i.exp / i.target * 100);

                if (i.archs == null)
                    i.archs = new List<Arch>();
                while (i.archs.Count < 3)
                    i.archs.Add(new Arch());

                if (i.archs != null && i.archs.Any())
                    i.archs = PlayersArchHelper.Update(i.archs);

                if (currentLobby.id == (int)LobbyId.TOPGOLD)
                    i.status = LongConverter.ToFull(i.gold) + " " + GameBase.moneyGold.name;
                else if (currentLobby.id == (int)LobbyId.TOPLEVEL)
                    i.status = "Cấp độ " + i.allLevel + " (" + i.expPercent + "%)";
                else
                    i.status = "Cấp độ " + i.level + " (" + i.expPercent + "%)";
            }

            listData = data.topUsers;

            FillData();
        }
    }

    public void Get_Data(Lobby _lobby)
    {
        currentLobby = _lobby;
        uiListView.ClearList();
        listData = new List<UserData>();
        listView = new List<PlayersItemView>();

        if (currentLobby.id == (int)LobbyId.TOPGOLD)
            WarpRequest.GetTopGold(OGUIM.me.id);
        else if (currentLobby.id == (int)LobbyId.TOPLEVEL)
		{            
			#if UNITY_EDITOR && UNITY_ANDROID 
            string content = "";
            content += "PRODUCT: " + Application.productName + "\n";
            content += "PROVIDER CODE: " + GameBase.providerCode + "\n";
            content += "HTTP REQUEST: " + OldWarpChannel.httpRequest + "\n";
            content += "SOCKET: " + OldWarpChannel.warp_host + "\n";
            content += "PACKAGE: " + Application.identifier + "\n";
            content += "FBID: " + Facebook.Unity.FB.AppId + "\n";
			content += "AppsFlyer: " + "ZWyX2eMzxGiZDqRziqTqBn" + "\n";
			//content += "Pushwoosh AppId: " + Pushwoosh.ApplicationCode + "\n";
			// += "GCM Number: " + Pushwoosh.GcmProjectNumber + "\n";
            content += "Version: " + Application.version + "\n";

            OGUIM.MessengerBox.Show ("CONFIG", content);
            #endif

            WarpRequest.GetTopLevel();
		}
        else
            WarpRequest.GetTopLevelByGame((LobbyId)currentLobby.id);
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

            uiListView.ScrollVerticalToTop();
        }
        OGUIM.Toast.Hide();
    }
}