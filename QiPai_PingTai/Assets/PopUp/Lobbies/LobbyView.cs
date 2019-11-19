using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyView : MonoBehaviour
{
    public LobbyId id;
    public LobbyMode lobbyMode;
    public Image icon;
   // public Text nameText;
   // public Text subNameText;
    public Text tablesText;
    public Text playersText;
    public ParticleSystem particleEfx;
    private int status { get; set; }
    public string tableString { get; set; }
    public Lobby lobbyData { get; set; }

    public UIAnimation roomListView;

    private NumberAddEffect tablesNum;
    private NumberAddEffect playersNum;

    public void GoToRooms()
    {
        if (OGUIM.instance != null)
        {
            if ((LobbyId)lobbyData.id == LobbyId.XENG_HOAQUA)
            {
                QuickPlay();
            }
            else if ((LobbyId)lobbyData.id == LobbyId.SLOT)
            {
                QuickPlay();
            }
            else if ((LobbyId)lobbyData.id == LobbyId.XOCDIA_OLD || (LobbyId)lobbyData.id == LobbyId.BAUCUA_OLD)
            {
                QuickPlay();
            }
            else
            {
                lobbyData.playmode = (int)PlayMode.NORMAL;

                if (OGUIM.instance.lobbyViewInRooms != null)
                    OGUIM.instance.lobbyViewInRooms.FillData(lobbyData);
                OGUIM.instance.SubLobby(lobbyData);
            }
        }
        else
        {
            UILogView.Log("OGUIM.instance: NULL!???????????");
        }
    }

    public void OnEnable()
    {
        if (UIManager.instance != null && UIManager.instance.particleToggle != null)
            particleEfx.gameObject.SetActive(UIManager.instance.particleToggle.isOn);

        tablesNum = tablesText.GetComponent<NumberAddEffect>();
        playersNum = playersText.GetComponent<NumberAddEffect>();

    }

	void Awake()
	{
		#if UNITY_WEBGL
		particleEfx.Stop();
		particleEfx.gameObject.SetActive(false);
		#endif
	}

    public void QuickPlay()
    {

        if (OGUIM.currentLobby == null || OGUIM.instance.currentLobbyId == LobbyId.NONE || OGUIM.currentLobby.id != lobbyData.id)
        {
            Debug.LogWarning("QuickPlay from LOBBIES");
            lobbyData.playmode = (int)PlayMode.QUICK;
            if (OGUIM.instance.lobbyViewInRooms != null)
                OGUIM.instance.lobbyViewInRooms.FillData(lobbyData);

            if (lobbyData.onlygold && OGUIM.currentMoney.type == MoneyType.Koin)
            {
                OGUIM.instance.ChangeMoneyType((int)MoneyType.Gold);
                OGUIM.Toast.ShowNotification(lobbyData.desc + " chỉ hỗ trợ chơi " + GameBase.moneyGold.name);
            }
            else
            {
				OGUIM.Toast.ShowLoading ("");
                OGUIM.instance.SubLobby(lobbyData);
            }
        }
        else
        {
            Debug.LogWarning("QuickPlay from " + OGUIM.currentLobby.desc);
            lobbyData.playmode = (int)PlayMode.NORMAL;
            QuickJoinRoom(RoomListView.listData);
        }
    }

    public static void QuickJoinRoom(RootRoom data)
    {
        if ((LobbyId)OGUIM.currentLobby.id == LobbyId.SLOT || (LobbyId)OGUIM.currentLobby.id == LobbyId.XENG_HOAQUA)
        {
            OGUIM.GoToIngame();
        }
        else
        {
            var checkBet = RoomListView.GetQuickRoomAvaible(data);
            if (checkBet < 0 && OGUIM.currentLobby != null)
			{
				OGUIM.Toast.ShowLoading("");
                OGUIM.instance.SubLobby(OGUIM.currentLobby);
            }
            else if (checkBet == 0 && ((LobbyId)OGUIM.currentLobby.id != LobbyId.BAUCUA_OLD && (LobbyId)OGUIM.currentLobby.id != LobbyId.XOCDIA_OLD))
            {
                OGUIM.Toast.Show("Rất tiếc, cần thêm " + OGUIM.currentMoney.name + " để trải nghiệm", UIToast.ToastType.Warning);
            }
            else
            {
                OGUIM.Toast.ShowLoading("");
                WarpRequest.JoinRoom(OGUIM.currentMoney.type, -1);
            }
        }
    }

    public void ChangeLobbyMode(int _lobbyMode)
    {
        if (lobbyMode == (LobbyMode)_lobbyMode)
            return;

        if (roomListView != null)
            roomListView.Hide();

        var checkLobby = LobbyViewListView.listData.Where(x => x.desc.Contains(lobbyData.desc) && x.lobbymode == (LobbyMode)_lobbyMode).FirstOrDefault();
        if (checkLobby != null)
        {
            FillData(checkLobby);
            OGUIM.instance.SubLobby(lobbyData);
        }
    }

    public void FillData(Lobby i)
    {
        try
        {
            lobbyData = i;

            id = (LobbyId)i.id;
            lobbyMode = i.lobbymode;
            //nameText.text = i.desc.ToUpper();
            //if (!string.IsNullOrEmpty(i.subname))
            //{
            //    subNameText.text = i.subname.ToUpper();
            //    subNameText.gameObject.SetActive(true);
            //}
            //else
            //{
            //    subNameText.text = "";
            //    subNameText.gameObject.SetActive(false);
            //}

            //0 = hide
            //1 = show
            //2 = hot
            //9 = coming soon

            if (i.id != (int)LobbyId.XENG_HOAQUA && i.id != (int)LobbyId.SLOT && i.id != (int)LobbyId.TOPGOLD && i.id != (int)LobbyId.TOPLEVEL)
            {
                tableString = " BÀN";
            }
            else
            {
                tableString = "";
                if (lobbyData.tables == 0)
                    lobbyData.tables = DateTime.Now.Ticks / 100000000000;
                if (lobbyData.players == 0)
                    lobbyData.players = Math.Abs((long)(lobbyData.tables / 10000 * 0.5));
                if (lobbyData.players < 100)
                    lobbyData.players = 100;
            }

            status = i.status;

            if (i.status == 1)
            {
                particleEfx.gameObject.SetActive(true);
            }
            else if (i.status == 2)
            {
                particleEfx.gameObject.SetActive(true);
            }
            else if (i.status == 9)
            {
                particleEfx.gameObject.SetActive(false);
                //nameText.color = new Color32(50, 0, 0, 255);
                //subNameText.color = new Color32(50, 0, 0, 200);
                icon.color = new Color32(50, 50, 50, 255);
                tablesText.text = System.DateTime.Now.AddDays(10).ToString("dd.MM.yyyy");
                playersText.text = "COMMING SOON";
            }

            //SenceName = i.senceName;
            if (ImageSheet.Instance.resourcesDics.ContainsKey("icon_lobby_" + i.id))
            {
                icon.sprite = ImageSheet.Instance.resourcesDics["icon_lobby_" + i.id];
            }

            UpdateStatus();
        }
        catch (System.Exception ex)
        {
            UILogView.Log("IconGameView FillData: " + i.name + "\n" + ex.Message + "\n" + ex.StackTrace, true);
        }
    }

    public void UpdateStatus()
    {
        if (status != 9 && tablesNum != null && playersNum != null)
        {
            if (lobbyData.tables == 0 || lobbyData.players == 0)
            {
                tablesText.text = "LOADING";
                playersText.text = "Please wait...";
            }
            else if (lobbyData.id == (int)LobbyId.XENG_HOAQUA)
            {
				tablesText.gameObject.SetActive(false);
				lobbyData.players = lobbyData.players > 0 ? lobbyData.players : 13;
				playersNum.FillData(lobbyData.players, "NGƯỜI");
                return;
            }
            else
            {
                // check if table/player < 0 then set to 6/13
                lobbyData.tables = lobbyData.tables > 0 ? lobbyData.tables : 6;
                lobbyData.players = lobbyData.players > 0 ? lobbyData.players : 13;

                tablesNum.FillData(lobbyData.tables, tableString);
                playersNum.FillData(lobbyData.players, "NGƯỜI");
            }
            
        }
    }
}
