using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class UISlot : MonoBehaviour
{
    public static UISlot instance;

    public bool selectAllAtStart = false;
    public static void SpawnTextEfx(string str, Vector3 pos)
    {
        var te = instance.textEfxPrefab.Spawn();
        te.gameObject.transform.SetParent(instance.transform, false);
        te.gameObject.transform.position = pos;
        te.gameObject.transform.localScale = Vector3.one * (pos == Vector3.zero ? 1 : 0.6f);

        te.SetData(str.ToUpper(), 0.5f, 3.5f);
    }

    public List<int> listLineSelected;
    public List<Toggle> listToggleLineOnGame;
    public List<Toggle> listToggleLineOnTab;
    public List<NumberAddEffect> listJackpotLabel;
    public List<GameObject> listJackpotView;
    public List<LobbyView> listSlotRoomView;

    public string soundOnLineSelect;
    public string soundOffLineSelect;
    public string soundSpin;
    public string soundMegaWin;
    public string soundJackpot;

    public NumberAddEffect moneyLabel;
    public NumberAddEffect currentBetLabel;
    public NumberAddEffect totalBetLabel;
    public NumberAddEffect lastWinLabel;
    public Text totalLineLabelInButton;
    public Text totalLineLableInPopup;
    public Text buttonSpinStatusLabel;
    public int betRoomValue;
    public int freeValue;

    public Button buttonSpin;

    public UIToggle buttonAuto;
    public UIToggle buttonTrial;

    public SlotMachine slotMachine;
    public UIPopupTabs popUp;
    public UIAnimation popUpSelectRoom;
    public GameObject coinRainEfxPrefab;
    public TextEffext textEfxPrefab;
    public Effect_Jackpot jackpotEfx;
    public bool isSpining = false;
    public bool isAutoJoinRoom = false;
    public int totalLine;
    public int totalBet;

    public GameObject gmPrefab;

    SlotGameManager gameManager;
    public int nextBetRoomValue;
    public void Awake()
    {
        instance = this;

        var gm = Instantiate(gmPrefab) as GameObject;
        gm.gameObject.transform.SetParent(transform);
        gameManager = gm.GetComponent<SlotGameManager>();

        coinRainEfxPrefab.CreatePool(4);
        textEfxPrefab.CreatePool(10);
        slotMachine.UpdateUIByRoom(794);
		Select_Tat ();
    }
    public void OnEnable()
    {
        moneyLabel.FillData(0);
        lastWinLabel.FillData(0);
        if (selectAllAtStart)
            Select_Tat();
        UpdateUI();

        OGUIM.instance.lobbyName.text = "";
        OGUIM.instance.roomBetValue.text = "";

        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnGetRoomsDone += Wc_OnGetRoomsDone;
            WarpClient.wc.OnJoinRoomDone += Wc_OnJoinRoomDone;
            WarpClient.wc.OnLeaveRoom += Wc_OnLeaveRoom;
			OGUIM.listRoomSlot = null;
            AutoCheckAndJoinRoom(true);
        }
    }

    public void OnDisable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnGetRoomsDone -= Wc_OnGetRoomsDone;
            WarpClient.wc.OnJoinRoomDone -= Wc_OnJoinRoomDone;
            WarpClient.wc.OnLeaveRoom -= Wc_OnLeaveRoom;
        }
    }

    public void AutoCheckAndJoinRoom(bool autoJoinRoom)
    {
        isAutoJoinRoom = autoJoinRoom;
        if (OGUIM.listRoomSlot == null || !OGUIM.listRoomSlot.Any())
		{
			OGUIM.Toast.ShowLoading("");
			//OGUIM.Toast.ShowLoading("Đang tải dữ liệu, vui lòng chờ giây lát...");
            OGUIM.isListen = false;
            WarpRequest.GetRooms(OGUIM.currentLobby.id);
            return;
        }
        else
        {
            OGUIM.Toast.ShowLoading("");
            for (int i = 0; i < OGUIM.listRoomSlot.Count; i++)
            {
                listJackpotLabel[i].FillData(OGUIM.listRoomSlot[i].funds);
                listJackpotLabel[i].name = OGUIM.listRoomSlot[i].id.ToString();
                listJackpotView[i].name = OGUIM.listRoomSlot[i].id.ToString();

                FillDataInPopUp();
            }

            var checkRoom = OGUIM.listRoomSlot.OrderBy(x => x.bet).FirstOrDefault();
            if (checkRoom != null && isAutoJoinRoom)
            {
                OGUIM.isListen = false;
                WarpRequest.JoinRoom(OGUIM.currentLobby.onlygold ? GameBase.moneyGold.type : OGUIM.currentMoney.type, checkRoom.bet);
            }
        }
    }

    private void Wc_OnGetRoomsDone(WarpResponseResultCode status, List<RoomSlot> data)
    {
        if (status == WarpResponseResultCode.SUCCESS && data.Any())
        {
            OGUIM.listRoomSlot = data;
            AutoCheckAndJoinRoom(isAutoJoinRoom);
        }
        else
        {
            OGUIM.Toast.Show("Có lỗi xảy ra. Vui lòng thử lại", UIToast.ToastType.Warning);
        }
    }

    private void Wc_OnJoinRoomDone(WarpResponseResultCode status, Room data)
    {
        if (status == WarpResponseResultCode.SUCCESS && data != null && data.room != null)
        {
            Debug.Log("SLOT ROOM: " + data.room.id);

            for (int i = 0; i < listJackpotView.Count; i++)
            {
                if (listJackpotView[i].name != data.room.id.ToString())
                    listJackpotView[i].SetActive(false);
                else
                    listJackpotView[i].SetActive(true);
            }

            OGUIM.instance.currentLobbyId = LobbyId.SLOT;
            OGUIM.currentRoom = data;
            OGUIM.autoLeaveRoom = false;
            currentBetLabel.FillData(data.room.bet, "", 50);
            UpdateUI();
            OGUIM.Toast.Hide();

            if (popUpSelectRoom.state == UIAnimation.State.IsShow)
                popUpSelectRoom.Hide();

            OGUIM.instance.lobbyName.text = OGUIM.currentLobby.desc.ToUpper();
            OGUIM.instance.roomBetValue.text = LongConverter.ToFull(betRoomValue) + " " + GameBase.moneyGold.name;
        }
        else
        {
            OGUIM.Toast.Show("Có lỗi xảy ra. Vui lòng thử lại", UIToast.ToastType.Warning);
            //OGUIM.UnLoadGameScene();
            OGUIM.Toast.Hide();
        }
        OGUIM.isListen = true;
    }

    private void Wc_OnLeaveRoom(Room room, int status)
    {
        if (status == (int)WarpResponseResultCode.SUCCESS && nextBetRoomValue != 0)
        {
            OGUIM.Toast.ShowLoading("");
            OGUIM.isListen = false;
            WarpRequest.JoinRoom(OGUIM.currentLobby.onlygold ? GameBase.moneyGold.type : OGUIM.currentMoney.type, nextBetRoomValue);
            nextBetRoomValue = 0;
        }
        else
            OGUIM.Toast.Hide();
    }

    void Update()
    {

    }

    public void ChangeBetRoom(int roomIndex = -1)
    {
        if (slotMachine.IsSpinning)
        {
            OGUIM.Toast.ShowNotification("Vui lòng đợi lượt quay kết thúc...!");
            buttonAuto.toggle.isOn = false;
            return;
        }

        OGUIM.Toast.ShowLoading("");
        if (OGUIM.listRoomSlot == null || !OGUIM.listRoomSlot.Any())
        {
            WarpRequest.GetRooms(OGUIM.currentLobby.id);
        }
        else if (OGUIM.listRoomSlot.Count >= 3)
        {
            if (roomIndex == -1)
            {
                nextBetRoomValue = 0;
                if (betRoomValue == OGUIM.listRoomSlot[0].bet)
                {
                    nextBetRoomValue = OGUIM.listRoomSlot[1].bet;
                }
                else if (betRoomValue == OGUIM.listRoomSlot[1].bet)
                {
                    nextBetRoomValue = OGUIM.listRoomSlot[2].bet;
                }
                else if (betRoomValue == OGUIM.listRoomSlot[2].bet)
                {
                    nextBetRoomValue = OGUIM.listRoomSlot[0].bet;
                }
            }
            else
            {
                nextBetRoomValue = OGUIM.listRoomSlot[roomIndex].bet;
                if (nextBetRoomValue == betRoomValue)
                {
                    popUpSelectRoom.Hide();
                    return;
                }
            }
            OGUIM.isListen = false;
            BuildWarpHelper.LeaveRoom(null);
        }
    }

    public void FillDataInPopUp()
	{
		var lobbySlotStatus = LobbyViewListView.listLobbiesStatus.FirstOrDefault(x => (int)x.zoneId == (int)LobbyId.SLOT);
		long players = 100;
		if (lobbySlotStatus != null)
			players = lobbySlotStatus.player + UnityEngine.Random.Range(-10, 20);
		
        for (int i = 0; i < OGUIM.listRoomSlot.Count; i++)
        {
            listJackpotLabel[i].FillData(OGUIM.listRoomSlot[i].funds);
            listJackpotLabel[i].name = OGUIM.listRoomSlot[i].id.ToString();

            if (listSlotRoomView[i].lobbyData == null)
            {
                //listSlotRoomView[i].nameText.text = LongConverter.ToFull(OGUIM.listRoomSlot[i].bet);
                listSlotRoomView[i].lobbyData = new Lobby
                {
                    tables = OGUIM.listRoomSlot[i].funds,
                    id = OGUIM.listRoomSlot[i].id
                };
            }

            if (listSlotRoomView[i].lobbyData != null)
            {
                listSlotRoomView[i].lobbyData.tables = OGUIM.listRoomSlot[i].funds;
				if (i == 0)
					listSlotRoomView [i].lobbyData.players = (long)(players * 0.6);
				else if (i == 1)
					listSlotRoomView [i].lobbyData.players = (long)(players * 0.3);
				else if (i == 2)
					listSlotRoomView [i].lobbyData.players = (long)(players * 0.1);
            }
            listSlotRoomView[i].UpdateStatus();
        }
    }

    public void Spin()
    {
		// isSpining 			  : check by Logic (client/server)
		// slotMachine.IsSpinning : check by Animation (client spin)
		if (isSpining || slotMachine.IsSpinning) {
			Debug.Log ("SPINNING");
			//if (OGUIM.Toast != null)
			//	OGUIM.Toast.ShowNotification ("Vui lòng đợi lượt quay kết thúc...!");
			return;
		}
		
        if (totalLine <= 0)
        {
            if (OGUIM.Toast != null)
                OGUIM.Toast.Show("Vui lòng chọn dòng cược...!", UIToast.ToastType.Warning);
            popUp.Show(1);
        }
        else
        {
            OGUIM.Toast.ShowLoading("");
            //Check Trial
            if (buttonTrial.toggle.isOn)
            {
                //is Trial
                if (buttonAuto.toggle.isOn)
                {
                    if (OGUIM.Toast != null)
                        OGUIM.Toast.Show("Đang ở chế độ quay thử và tự quay...", UIToast.ToastType.Warning);
                }
                else
                {
                    if (OGUIM.Toast != null)
                        OGUIM.Toast.Show("Đang ở chế độ quay thử...", UIToast.ToastType.Warning);
                }
            }

            //Send request
			isSpining = true;
            BuildWarpHelper.SLOT_Spin(buttonTrial.toggle.isOn ? 0 : 1, listLineSelected, () =>
            {
                UILogView.Log("Spin is timeout");
                isSpining = false;
                OGUIM.Toast.Hide();
            });
        }
    }

    public void AutoSpinToggle()
    {
        //AutoSpin
        if (buttonAuto.toggle.isOn)
        {
            //is Auto
            buttonAuto.UpdateTextContent("BẬT");
            Spin();
        }
        else
        {
            buttonAuto.UpdateTextContent("TẮT");
        }

        //UnLock spin button
        //buttonSpin.animator.SetTrigger("Normal");
    }

    public void ShowLine(int line, float delay)
    {
        var buttonLine = listToggleLineOnGame.FirstOrDefault(x => x.transform.parent.name == "Select_Line (" + line + ")");
        if (buttonLine != null)
        {
            if (buttonLine.isOn)
                UIManager.PlaySound(soundOnLineSelect);
            else
                UIManager.PlaySound(soundOffLineSelect);

            DOVirtual.DelayedCall(delay, () =>
            {
                buttonLine.transform.parent.gameObject.GetComponent<Select_Line>().ShowLine();
            }).SetId(slotMachine.doTweenId);
        }
    }

    public void UpdateStatusFromMainGame()
    {


        for (int i = 0; i < listToggleLineOnGame.Count; i++)
        {
            if (i < listToggleLineOnTab.Count)
            {
                listToggleLineOnTab[i].isOn = listToggleLineOnGame[i].isOn;
            }
        }

        buttonAuto.toggle.isOn = false;
        UpdateUI();
    }

    public void UpdateStatusFromTabGame()
    {
        for (int i = 0; i < listToggleLineOnTab.Count; i++)
        {
            if (i < listToggleLineOnGame.Count)
            {
                listToggleLineOnGame[i].isOn = listToggleLineOnTab[i].isOn;
            }
        }
        buttonAuto.toggle.isOn = false;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (OGUIM.currentRoom != null && OGUIM.currentRoom.room != null)
            betRoomValue = OGUIM.currentRoom.room.bet;

        moneyLabel.FillData(OGUIM.me.gold);

        totalLine = listToggleLineOnGame.Count(x => x.isOn);
        totalBet = totalLine * betRoomValue;

        totalLineLabelInButton.text = totalLine + "";
        totalBetLabel.FillData(totalBet);

        listLineSelected = new List<int>();
        foreach (var i in listToggleLineOnGame.Where(x => x.isOn))
        {
            var index = listToggleLineOnGame.IndexOf(i) + 1;
            listLineSelected.Add(index);
        }

        if (totalLine > 0)
            totalLineLableInPopup.text = "Bạn đã chọn " + "<b><color=#FFC800FF>" + totalLine + "</color></b>" + " dòng, tổng cược là " + "<b><color=#FFC800FF>" + LongConverter.ToK(totalBet) + "</color></b>";// + " 368vipEdited";
        else
            totalLineLableInPopup.text = "Vui lòng chọn dòng cược! Chúc bạn may mắn!";

        if (freeValue > 0)
        {
            buttonSpinStatusLabel.text = "MIỄN PHÍ" + " " + "X" + freeValue;
        }
        else
        {
            if (buttonTrial.toggle.isOn)
            {
                buttonTrial.UpdateTextContent("THỬ");
                buttonSpinStatusLabel.text = "- THỬ -";
            }
            else
            {
                buttonTrial.UpdateTextContent("THẬT");
                buttonSpinStatusLabel.text = "";
            }
        }
    }

    public void Select_Tat()
    {
        for (int i = 0; i < listToggleLineOnTab.Count; i++)
        {
            listToggleLineOnTab[i].isOn = true;
        }
        UpdateStatusFromTabGame();
    }

    public void Select_Le()
    {
        for (int i = 0; i < listToggleLineOnTab.Count; i++)
        {
            if (i % 2 == 0)
                listToggleLineOnTab[i].isOn = true;
            else
                listToggleLineOnTab[i].isOn = false;
        }
        UpdateStatusFromTabGame();
    }

    public void Select_Chan()
    {
        for (int i = 0; i < listToggleLineOnTab.Count; i++)
        {
            if (i % 2 != 0)
                listToggleLineOnTab[i].isOn = true;
            else
                listToggleLineOnTab[i].isOn = false;
        }
        UpdateStatusFromTabGame();
    }

    public void Select_Bo()
    {
        for (int i = 0; i < listToggleLineOnTab.Count; i++)
        {
            listToggleLineOnTab[i].isOn = false;
        }
        UpdateStatusFromTabGame();
    }

    public void ShowTopUp()
	{
		if (GameBase.underReview)
		{
			#if UNITY_ANDROID || UNITY_IOS
			OGUIM.instance.popupIAP.Show();
			return;
			#endif
		}
        //OGUIM.instance.popupTopUp.Show(0);
        OGUIM.MessengerBox.Show("Thông Báo", "Mọi giao dịch nạp tiền của Win360 đều được thực hiện trên cổng thanh toán win360.club");
    }

    public void ShowUserHistory()
    {
        if (OGUIM.instance != null)
            OGUIM.instance.popupHistory.Show(0);
    }

    public void ShowJacpotHistory()
    {
        if (OGUIM.instance != null)
            OGUIM.instance.popupHistory.Show(1);

    }

    public void UpdateJackpotValue(int id, long values)
    {
        var checkRoom = OGUIM.listRoomSlot.FirstOrDefault(x => x.id == id);
        if (checkRoom != null)
            checkRoom.funds = values;

        var checkJackpotLabel = listJackpotLabel.FirstOrDefault(x => x.name == id.ToString());
        if (checkJackpotLabel != null)
            checkJackpotLabel.FillData(values);

        var checkRoomView = listSlotRoomView.FirstOrDefault(x => x.lobbyData != null && x.lobbyData.id == id);
        if (checkRoomView != null)
        {
            checkRoomView.lobbyData.tables = values;
            checkRoomView.UpdateStatus();
        }
    }

    public void UpdateJackpotValue(List<long> values)
    {
        for (int i = 0; i < listJackpotLabel.Count; i++)
        {
            listJackpotLabel[i].FillData(values[i]);
            listSlotRoomView[i].lobbyData.tables = values[i];
            listSlotRoomView[i].UpdateStatus();
        }
    }

    public void CreateSpecialEfx(int jackpotType)
    {
        jackpotEfx.Active(jackpotType);
    }

    public void CreateCoinRainEfx()
    {
        //var pos = new Vector3(0, -1.5f, 0);
        ////for (int i = 0; i < Random.Range(30, 50); i++)
        //{
        //    var go = coinRainEfxPrefab.Spawn();
        //    go.transform.SetParent(transform, false);
        //    go.transform.rotation = Quaternion.Euler(Vector3.left * 60);
        //    go.transform.position = pos;
        //    go.transform.DOMoveZ(0, 2.5f).OnComplete(() => go.Recycle());
        //    //go.GetComponent<ChipEffect>().Run(Instance.userCoinTransform.position);
        //}
    }

    public void SetWinCoinAndSpinAndUserCoin(int chip, int freeSpin, long userCoin)
    {
        lastWinLabel.FillData(chip);
        freeValue = freeSpin;
        moneyLabel.FillData(userCoin);
        OGUIM.instance.meView.moneyView.FillData(MoneyType.Gold, userCoin);
    }

    public void DoSpin(List<int[]> faces)
    {
        isSpining = true;
		slotMachine.Spin(faces);
		UIManager.PlaySound ("reverse");

		for (int i = 0; i < 20; i++)
		{
			int index = i + 1;
			if (index > 12)
				index = 11;
			DOVirtual.DelayedCall (0.15f * (i+1), () => UIManager.PlaySound ("combo_" + index));
		}
    }

    public void ShowLine(List<PayLine> payLines)
    {
		UIManager.PlaySound ("slow_down");
        for (int i = 0; i < payLines.Count; i++)
        {
            ShowLine(payLines[i].line, i * 0.7f);
            slotMachine.ShowLine(payLines[i].matched, i * 0.7f);
        }
        slotMachine.ShowLine(null, payLines.Count * 0.7f);
        DOVirtual.DelayedCall(Mathf.Min(payLines.Count + 1, 3), () =>
		{
			UIManager.PlaySound ("stage_clear");
            OnSpinCompleted();
        }).SetId(slotMachine.doTweenId);
    }

    public void OnSpinCompleted()
    {
        isSpining = false;

        Debug.Log("OnSpinCompleted autoLeaveRoom: " + OGUIM.autoLeaveRoom);
        if (OGUIM.autoLeaveRoom)
        {
            OGUIM.GoBack();
        }
        else
        {
            AutoSpinToggle();
            lastWinLabel.FillData(0);
            OGUIM.Toast.Hide();
        }
    }

    public void ResetSpin()
    {
        buttonSpinStatusLabel.text = "";
        DOTween.Kill(slotMachine.doTweenId);
        slotMachine.ShowLine(null);
        lastWinLabel.FillData(0);
    }

    public class LineData
    {
        public int index;
    }
}
