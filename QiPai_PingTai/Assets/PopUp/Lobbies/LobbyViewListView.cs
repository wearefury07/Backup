using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LobbyViewListView : MonoBehaviour
{
    public float scaleFakeData = 3f;
    public float timeFakeData = 3f;
    public UIListView uiListView;
    public static List<Lobby> listData = new List<Lobby>();
    public static List<Lobby> listSoloData = new List<Lobby>();
    public static List<Lobby> listFavoriteData = new List<Lobby>();
    public static List<LobbyView> listView = new List<LobbyView>();
	public static List<LobbyStatus> listLobbiesStatus = new List<LobbyStatus> ();

    private UIAnimation anim { get; set; }

    void Awake()
    {
        anim = gameObject.GetComponent<UIAnimation>();
    }

    void Start()
    {
        InvokeRepeating("FakeTablePlayer", 1.5f, timeFakeData);
    }

    private void OnEnable()
    {
        if (WarpClient.wc != null)
        {
			WarpClient.wc.OnListLobbiesDone += Wc_OnListLobbiesDone;
			WarpClient.wc.OnGetLobbyInfo += Wc_OnGetLobbyInfo;
        }
    }

    private void OnDisable()
    {
        if (WarpClient.wc != null)
        {
			WarpClient.wc.OnListLobbiesDone -= Wc_OnListLobbiesDone;
			WarpClient.wc.OnGetLobbyInfo -= Wc_OnGetLobbyInfo;
        }
    }

    public void Get_Data(bool reload)
    {
        if (anim != null)
            anim.Hide();
		if (!listView.Any () || reload) {
			OGUIM.Toast.ShowLoading("");
			//OGUIM.Toast.ShowLoading ("Đang tải dữ liệu, vui lòng chờ giây lát...");
			uiListView.ClearList ();
			listData = new List<Lobby> ();
			listView = new List<LobbyView> ();
			WarpRequest.ListLobbies ();
		} 
		else 
		{
			// Get list lobbies status
			WarpRequest.GetLobbyInfoConfig();
		}
    }

    private void Wc_OnListLobbiesDone(WarpResponseResultCode resultCode, List<Lobby> data)
    {
        if (resultCode == WarpResponseResultCode.SUCCESS)
        {
            foreach (var i in data)
            {
                var checkFavorite = listFavoriteData.FirstOrDefault(x => x.id == i.id);
                if (checkFavorite != null)
                    i.play = checkFavorite.play;

                switch ((LobbyId)i.id)
                {
                    case LobbyId.TLMNDL:
                        i.lobbymode = LobbyMode.MULTI;
                        i.desc = "Tiến lên";
                        i.shotname = "TLMN";
                        i.subname = "Miền Nam";
                        break;
					case LobbyId.TLMNDL_SOLO:
						if (GameBase.isLiteVersion) {
							i.status = 0;
							break;
						}
                        i.lobbymode = LobbyMode.SOLO;
                        i.desc = "Tiến lên";
                        i.shotname = "TLMN Solo";
                        i.subname = "Miền Nam Solo";
                        break;
                    case LobbyId.PHOM:
                        i.lobbymode = LobbyMode.MULTI;
                        i.desc = "Phỏm";
                        i.shotname = "Phỏm";
                        i.subname = "";
                        break;
					case LobbyId.PHOM_SOLO:
						if (GameBase.isLiteVersion) {
							i.status = 0;
							break;
						}
                        i.lobbymode = LobbyMode.SOLO;
                        i.desc = "Phỏm";
                        i.shotname = "Phỏm Solo";
                        i.subname = "Solo";
                        break;
                    case LobbyId.SAM:
                        i.lobbymode = LobbyMode.MULTI;
                        i.desc = "Sâm";
                        i.shotname = "Sâm";
                        i.subname = "";
                        break;
					case LobbyId.SAM_SOLO:
						if (GameBase.isLiteVersion) {
							i.status = 0;
							break;
						}
                        i.lobbymode = LobbyMode.SOLO;
                        i.desc = "Sâm";
                        i.shotname = "Sâm";
                        i.subname = "Solo";
                        break;
					case LobbyId.BACAY:
						if (GameBase.isLiteVersion) {
							i.status = 0;
							break;
						}
                        i.lobbymode = LobbyMode.CLASSIC;
                        i.desc = "Ba cây";
                        i.shotname = "Ba cây chương";
                        i.subname = "Chương";
                        break;
					case LobbyId.BACAY_GA:
						if (GameBase.isLiteVersion) {
							i.status = 0;
							break;
						}
                        i.lobbymode = LobbyMode.CLASSIC;
                        i.desc = "Ba cây";
                        i.shotname = "Ba cây Gà";
                        i.subname = "Gà";
                        break;
                    case LobbyId.MAUBINH:
                        i.lobbymode = LobbyMode.CLASSIC;
                        i.desc = "Mậu Binh";
                        i.shotname = "Mậu Binh";
                        i.subname = "";
                        break;
					case LobbyId.LIENG:
						if (GameBase.isLiteVersion) {
							i.status = 0;
							break;
						}
                        i.lobbymode = LobbyMode.CLASSIC;
                        i.desc = "Liêng";
                        i.shotname = "Liêng";
                        i.subname = "";
                        break;
					case LobbyId.BAUCUA:
						i.status = 0;
                        i.lobbymode = LobbyMode.CLASSIC;
                        i.desc = "Bầu cua mới";
                        i.shotname = "Bầu cua mới";
                        i.subname = "";
                        break;
                    case LobbyId.BAUCUA_OLD:
                        i.lobbymode = LobbyMode.CLASSIC;
                        i.desc = "Bầu cua";
                        i.shotname = "Bầu cua";
                        i.subname = "";
                        i.onlygold = true;
                        break;
					case LobbyId.XOCDIA:
						i.status = 0;
                        i.lobbymode = LobbyMode.CLASSIC;
						i.desc = "Xóc đĩa mới";
                        i.shotname = "Xóc đĩa mới";
                        i.subname = "";
                        break;
                    case LobbyId.XOCDIA_OLD:
                        i.lobbymode = LobbyMode.CLASSIC;
                        i.desc = "Xóc đĩa";
                        i.shotname = "Xóc đĩa";
                        i.subname = "";
                        i.onlygold = true;
                        break;
                    case LobbyId.SLOT:
                        i.lobbymode = LobbyMode.CLASSIC;
                        i.desc = "Đập hũ";
                        i.shotname = "Đập hũ";
                        i.subname = "";
                        i.onlygold = true;
                        break;
					case LobbyId.XENG_HOAQUA:
                        i.status = 0;
                        i.lobbymode = LobbyMode.CLASSIC;
                        i.desc = "Xèng";
                        i.shotname = "Xèng";
                        i.subname = "";
                        i.onlygold = true;
                        break;
					default:
						i.status = 0;
						break;
                }
            }
            listData = data.OrderByDescending(x => x.play).ToList();

            FillData();

			// Get list lobbies status
			WarpRequest.GetLobbyInfoConfig();
			// On Get Done --> listLobbiesStatus --> FakeTablePlayer
        }
    }

	private void Wc_OnGetLobbyInfo(WarpResponseResultCode resultCode, List<LobbyStatus> data)
	{
		if (resultCode == WarpResponseResultCode.SUCCESS) {
			listLobbiesStatus = data;
			TrueTablePlayer ();
		} 
		else {
			FakeTablePlayer ();
		}
	}


    public void FillData()
    {
        if (listData.Any())
        {
            //Fake data game mode
            listSoloData = new List<Lobby>();
            int count = 0;
            foreach (var i in listData)
            {
                if (i.status == 1)
                {
                    if (i.lobbymode == LobbyMode.SOLO)
                    {
                        listSoloData.Add(i);
                    }
                    else
                    {
                        try
                        {
                            var ui = uiListView.GetUIView<LobbyView>(uiListView.GetDetailView());
                            ui.FillData(i);
                            listView.Add(ui);

                            count++;
                        }
                        catch (System.Exception ex)
                        {
                            UILogView.Log("IconGameListView FillData: " + "\n" + ex.Message + "\n" + ex.StackTrace, true);
                        }
                    }
                }
            }
        }

        OGUIM.Toast.Hide();
        if (anim != null)
            anim.Show();
    }

    public void TrueTablePlayer()
    {
        if (listView.Any() && scaleFakeData > 0)
        {
            long totalTables = 0;
            long totalPlayers = 0;
			long ranTable = Random.Range(-3, 3);
			long ranPlayer = (long)(ranTable * scaleFakeData) + Random.Range(-3, 3);
            foreach (var i in listData)
            {
				var checkLobyStatus = listLobbiesStatus.FirstOrDefault(x => x.zoneId == i.id);
				if (checkLobyStatus == null)
					return;
				i.tables = checkLobyStatus.tables;
				i.players = checkLobyStatus.player;

//                if (i.tables == 0)
//                    ranTable = Random.Range(20, 99);
//                else if (i.tables > 4)
//                    ranTable = i.tables + Random.Range(-1, 2);
                if (i.id != (int)LobbyId.SLOT && i.id != (int)LobbyId.XENG_HOAQUA)
                {
                    i.tables += ranTable ;
					i.players += ranPlayer;
					totalTables += i.tables;
                }
                else
				{	
					i.tables += Random.Range (3, 9) * Random.Range (1, 3);;
                    i.players += Random.Range(-3, 3);
                }
                totalPlayers += i.players;

                var lobbyView = listView.FirstOrDefault(x => (int)x.id == i.id);
                if (lobbyView != null)
                {
                    lobbyView.lobbyData = i;
                    if (lobbyView.gameObject.activeSelf)
                        lobbyView.UpdateStatus();
                }

                if (i.id != (int)LobbyId.SLOT && i.id != (int)LobbyId.XENG_HOAQUA)
                {
					if (OGUIM.instance.lobbyViewInRooms != null && OGUIM.instance.lobbyViewInRooms.gameObject.activeSelf && (int)OGUIM.instance.lobbyViewInRooms.id == i.id)
                    {
                        OGUIM.instance.lobbyViewInRooms.lobbyData.tables += ranTable;
						OGUIM.instance.lobbyViewInRooms.lobbyData.players += ranPlayer;
                        OGUIM.instance.lobbyViewInRooms.UpdateStatus();
                    }
                    //else if (oguim.instance.lobbyviewinleader != null && oguim.instance.lobbyviewinleader.gameobject.activeself && (int)oguim.instance.lobbyviewinleader.id == i.id)
                    //{
                    //    oguim.instance.lobbyviewinleader.lobbydata.tables += rantable;
                    //    oguim.instance.lobbyviewinleader.lobbydata.players += ranplayer;
                    //    oguim.instance.lobbyviewinleader.updatestatus();
                    //}
                }
                //else if (OGUIM.instance.lobbyViewInLeader != null && OGUIM.instance.lobbyViewInLeader.gameObject.activeSelf
                //    && (OGUIM.instance.lobbyViewInLeader.id == LobbyId.TOPGOLD || OGUIM.instance.lobbyViewInLeader.id == LobbyId.TOPLEVEL))
                //{
                //    OGUIM.instance.lobbyViewInLeader.lobbyData.tables = totalTables;
                //    OGUIM.instance.lobbyViewInLeader.lobbyData.players = totalPlayers;
                //    OGUIM.instance.lobbyViewInLeader.UpdateStatus();
                //}
            }
        }
    }

	public void FakeTablePlayer()
	{
		// Neu da nhan duoc du lieu thi quay lai ham TrueTablePlayer
		if (listLobbiesStatus != null && listLobbiesStatus.Count > 0) {
			TrueTablePlayer ();
			return;
		}
		
		if (listView.Any() && scaleFakeData > 0)
		{
			long totalTables = 0;
			long totalPlayers = 0;
			long ranTable = Random.Range(20, 99);
			long ranUser = Random.Range(0, 99);
			foreach (var i in listData)
			{
				if (i.tables == 0)
					ranTable = Random.Range(20, 99);
				else if (i.tables > 4)
					ranTable = i.tables + Random.Range(-1, 2);

				if (i.id != (int)LobbyId.SLOT && i.id != (int)LobbyId.XENG_HOAQUA)
				{
					i.tables = ranTable;
					i.players = (long)(ranTable * scaleFakeData) + Random.Range(-3, 3);
				}
				else
				{
					i.tables += Random.Range(1, 9) * Random.Range(1, 9);
					i.players += Random.Range(-3, 3);
				}
				totalTables += i.tables;
				totalPlayers += i.players;

				var lobbyView = listView.FirstOrDefault(x => (int)x.id == i.id);
				if (lobbyView != null)
				{
					lobbyView.lobbyData = i;
					if (lobbyView.gameObject.activeSelf)
						lobbyView.UpdateStatus();
				}

				if (i.id != (int)LobbyId.SLOT && i.id != (int)LobbyId.XENG_HOAQUA)
				{
					if (OGUIM.instance.lobbyViewInRooms != null && OGUIM.instance.lobbyViewInRooms.gameObject.activeSelf && (int)OGUIM.instance.lobbyViewInRooms.id == i.id)
					{
						OGUIM.instance.lobbyViewInRooms.lobbyData.tables = ranTable;
						OGUIM.instance.lobbyViewInRooms.lobbyData.players = (long)(ranTable * scaleFakeData) + Random.Range(-3, 3);
						OGUIM.instance.lobbyViewInRooms.UpdateStatus();
					}
					//else if (OGUIM.instance.lobbyViewInLeader != null && OGUIM.instance.lobbyViewInLeader.gameObject.activeSelf && (int)OGUIM.instance.lobbyViewInLeader.id == i.id)
					//{
					//	OGUIM.instance.lobbyViewInLeader.lobbyData.tables = ranTable;
					//	OGUIM.instance.lobbyViewInLeader.lobbyData.players = (long)(ranTable * scaleFakeData) + Random.Range(-3, 3);
					//	OGUIM.instance.lobbyViewInLeader.UpdateStatus();
					//}
				}
				//else if (OGUIM.instance.lobbyViewInLeader != null && OGUIM.instance.lobbyViewInLeader.gameObject.activeSelf
				//	&& (OGUIM.instance.lobbyViewInLeader.id == LobbyId.TOPGOLD || OGUIM.instance.lobbyViewInLeader.id == LobbyId.TOPLEVEL))
				//{
				//	OGUIM.instance.lobbyViewInLeader.lobbyData.tables = totalTables;
				//	OGUIM.instance.lobbyViewInLeader.lobbyData.players = totalPlayers;
				//	OGUIM.instance.lobbyViewInLeader.UpdateStatus();
				//}
			}
		}
	}
}
	