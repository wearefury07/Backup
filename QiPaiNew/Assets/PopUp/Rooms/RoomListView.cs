using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListView : MonoBehaviour
{
    public UIListView uiListView;
    public static RootRoom listData = new RootRoom();
    public static List<RoomView> listView = new List<RoomView>();
    public int maxBetRoom = 12;
    public Toggle normalToggle;
    public Toggle soloToggle;

    private UIAnimation anim { get; set; }

    void Awake()
    {
        anim = gameObject.GetComponent<UIAnimation>();
        if (WarpClient.wc != null)
            WarpClient.wc.OnSubLobbyDone += Wc_OnSubLobbyDone;
    }

    private void OnEnable()
    {
        if (WarpClient.wc != null)
            WarpClient.wc.OnInvite += Wc_OnInvite;
    }

    private void OnDisable()
    {
        if (WarpClient.wc != null)
            WarpClient.wc.OnInvite -= Wc_OnInvite;
    }

    private void Wc_OnSubLobbyDone(WarpResponseResultCode status, RootRoom data)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            Debug.Log("OnSubLobbyDone: on RoomListView " + (LobbyId)OGUIM.currentLobby.id + " " + OGUIM.currentLobby.lobbymode);

            if (OGUIM.currentLobby.lobbymode == LobbyMode.SOLO)
            {
                normalToggle.isOn = false;
                soloToggle.isOn = true;
            }
            else
            {
                normalToggle.isOn = true;
                soloToggle.isOn = false;
            }

            listData = data;
            FillData(true);
            OGUIM.Toast.Hide();
        }
    }

    private void Wc_OnInvite(RootConfig data)
    {
        if (OGUIM.instance.inviteToggleInMesBox != null && OGUIM.instance.inviteToggleInMesBox.isOn)
        {
            WarpClient.wc.OnInvite -= Wc_OnInvite;

            var nameLobby = OGUIM.currentLobby.desc + " " + OGUIM.currentLobby.subname;
            var nameMoney = data.type == 1 ? GameBase.moneyGold.name : GameBase.moneyKoin.name;
            OGUIM.MessengerBox.Show("Lời mời", "Bạn được mời chơi game " + "\n" + nameLobby.ToUpper() + "\n" + " bàn " + nameMoney + "\n\n" + "Tham gia ngay?",
                "Đồng ý", () =>
				{
					OGUIM.Toast.ShowLoading("");
                    // Change money type when join room
                    OGUIM.instance.ToggleChangeMoneyType(data.type);
                    WarpRequest.JoinRoom((MoneyType)data.type, -1);
                    WarpClient.wc.OnInvite += Wc_OnInvite;
                },
                "Lần khác", () =>
                {
                    WarpClient.wc.OnInvite += Wc_OnInvite;
                }, () =>
                {
                    WarpClient.wc.OnInvite += Wc_OnInvite;
                }, true);
        }
    }

    public void Get_Data(bool reload)
    {
        gameObject.SetActive(false);
        if (!listView.Any() || reload)
		{
			OGUIM.Toast.ShowLoading("");
            //OGUIM.Toast.ShowLoading("Đang tải danh sách phòng chơi...");
            uiListView.ClearList();
            listData = new RootRoom();
            listView = new List<RoomView>();

			if (OGUIM.currentLobby != null) {
				OGUIM.Toast.ShowLoading("");
				WarpRequest.SubLobby (OGUIM.currentLobby.id);
			}
        }
    }

    public void FillData(bool reload)
    {
        if (OGUIM.currentLobby != null && OGUIM.currentLobby.onlygold && OGUIM.currentMoney != null && OGUIM.currentMoney.type != MoneyType.Gold)
        {
            OGUIM.instance.ChangeMoneyType((int)MoneyType.Gold);
            OGUIM.Toast.ShowNotification(OGUIM.currentLobby.desc + " chỉ hỗ trợ chơi " + GameBase.moneyGold.name);
            return;
        }

        InteractableToggles(false);

        if (reload)
        {
            uiListView.ClearList();
        }

        if (listData != null && listData.listBet != null && listData.listBetAvailable != null)
        {
            List<int> listFill = new List<int>();
            List<int> listFillAvaiable = new List<int>();

            if (OGUIM.currentMoney.type == MoneyType.Gold)
            {
                listFill = listData.listBet.gold.Take(maxBetRoom).ToList();
                listFillAvaiable = listData.listBetAvailable.gold.Take(maxBetRoom).ToList();
            }
            else
            {
                listFill = listData.listBet.koin.Take(maxBetRoom).ToList();
                listFillAvaiable = listData.listBetAvailable.koin.Take(maxBetRoom).ToList();
            }

            foreach (var i in listFill)
            {
                try
                {
                    int roomValue = i;
                    bool canJoin = listFillAvaiable.Any(ii => ii == i);

                    var ui = uiListView.GetUIView<RoomView>(uiListView.GetDetailView());
                    ui.FillData(roomValue, canJoin, OGUIM.currentMoney.type);
                    listView.Add(ui);
                }
                catch (System.Exception ex)
                {
                    UILogView.Log("RoomListView FillData: " + "\n" + ex.Message + "\n" + ex.StackTrace, true);
                }
            }

            if (anim != null)
                anim.Show(() => InteractableToggles(true));
        }
        else
        {
            InteractableToggles(true);
        }
    }

    public void InteractableToggles(bool interactable)
    {
        if (OGUIM.instance.toggleKoin != null)
            OGUIM.instance.toggleKoin.interactable = interactable;
        if (OGUIM.instance.toggleGold != null)
            OGUIM.instance.toggleGold.interactable = interactable;
    }

    public static int GetQuickRoomAvaible(RootRoom data)
    {
        if (data != null && data.listBetAvailable != null)
        {
            if (OGUIM.currentMoney.type == MoneyType.Gold)
            {
                if (data.listBetAvailable.gold != null && data.listBetAvailable.gold.Any())
                {
                    var betIndex = Random.Range(0, data.listBetAvailable.gold.Take(12).Count());
                    return data.listBetAvailable.gold[betIndex];
                }
            }
            else
            {
                if (data.listBetAvailable.koin != null && data.listBetAvailable.koin.Any())
                {
                    var betIndex = Random.Range(0, data.listBetAvailable.koin.Take(12).Count());
                    return data.listBetAvailable.koin[betIndex];
                }
            }
            return 0;
        }
        return -1;
    }
}
