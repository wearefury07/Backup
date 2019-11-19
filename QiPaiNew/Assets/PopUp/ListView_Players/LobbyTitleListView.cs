using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LobbyTitleListView : MonoBehaviour
{
    public bool notShowSolo = true;
    public UIListView uiListView;
    public int items = 10;
    private List<Lobby> listData = new List<Lobby>();
    private List<LobbyTitleItemView> listView = new List<LobbyTitleItemView>();

    private void OnEnable()
    {
        if (listData != null && !listData.Any())
        {
            if (notShowSolo)
                listData = LobbyViewListView.listData.Where(x => x.lobbymode != LobbyMode.SOLO).ToList();
            else
                listData = LobbyViewListView.listData;
        }
        FillData();
    }

    public void FillData()
    {
        if (listView != null && !listView.Any() && listData != null && listData.Any())
        {
            var uiTopGold = uiListView.GetUIView<LobbyTitleItemView>(uiListView.GetDetailView());
            var lobbyTopGold = new Lobby { desc = "Đại gia", subname = GameBase.moneyGold.name, shotname = "Đại gia", id = (int)LobbyId.TOPGOLD };
            if (uiTopGold.FillData(lobbyTopGold))
                listView.Add(uiTopGold);

            var uiTopAllLevel = uiListView.GetUIView<LobbyTitleItemView>(uiListView.GetDetailView());
            var lobbyTopAllLevel = new Lobby { desc = "Cao thủ", subname = "Đệ nhất Thiên hạ", shotname = "Cao thủ", id = (int)LobbyId.TOPLEVEL };
            if (uiTopAllLevel.FillData(lobbyTopAllLevel))
                listView.Add(uiTopAllLevel);

            foreach (var i in listData)
            {
                try
                {
					if (i.id != (int)LobbyId.SLOT && i.id != (int)LobbyId.XENG_HOAQUA && i.id != (int)LobbyId.XOCDIA && i.id != (int)LobbyId.BAUCUA )
                    {
                        var ui = uiListView.GetUIView<LobbyTitleItemView>(uiListView.GetDetailView(()=> !string.IsNullOrEmpty(i.shotname)));
                        if (ui != null && ui.FillData(i))
                            listView.Add(ui);
                    }
                }
                catch (System.Exception ex)
                {
                    UILogView.Log("LobbyTitleListView FillData: " + "\n" + ex.Message + "\n" + ex.StackTrace, true);
                }
            }
        }
    }

    public void CheckLeaderBoard()
    {
        if (!listView.Any(x => x.uiToggle.GetComponent<Toggle>().isOn))
        {
            listView.FirstOrDefault().uiToggle.GetComponent<Toggle>().isOn = true;
            //listView.FirstOrDefault().GetLeaderBoard();
        }
    }
}
