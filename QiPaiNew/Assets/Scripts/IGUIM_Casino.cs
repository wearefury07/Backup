using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class IGUIM_Casino : MonoBehaviour {
    public static int[] betLevels = { 1, 5, 10, 20 };
    public static string[] XOCDIA_namePots = { "Chẵn", "Lẻ", "4Đ", "4T", "3T1Đ", "3Đ1T" };
    public static string[] BAUCUA_namePots = { "Hươu", "Bầu", "Gà", "Cá", "Cua", "Tôm" };
    public static IGUIM_Casino instance;

    #region Game Static Method

    public static void SpawnTextEfx(string str, Vector3 pos, bool active = true)
    {
        var te = instance.textEfxPrefab.Spawn();
        te.gameObject.transform.SetParent(instance.transform, false);
        te.gameObject.transform.position = pos;
        te.gameObject.transform.localScale = Vector3.one * (pos == Vector3.zero ? 1 : 0.6f);

        te.SetData(str.ToUpper(), active ? Color.white : Color.black, 0, 0);
    }
    public static void SpawnChipSprite(int fromUser, int toPot, int fakeUserFrom = -2)
    {
        var x = Random.Range(-0.5f, 0.5f);
        var y = Random.Range(-0.5f, 0.5f);
        Vector3 fromPos = Vector3.zero;
        Vector3 toPos = instance.allPots[toPot - 1].transform.position + new Vector3(x, y);
        if (instance.playersOnBoard.ContainsKey(fromUser))
        {
            var player = instance.playersOnBoard[fromUser];
            fromPos = player.avatarView.imageAvatar.transform.position;
        }
        else
        {
            fromPos = instance.anotherPlayers.transform.position;   
        }

        var chip = instance.chipSpritePrefab.Spawn();
        chip.transform.SetParent(instance.transform, false);
        chip.transform.position = fromPos;
        chip.SetPosition(toPos, true);
        chip.userId = fakeUserFrom >= -1 ? fakeUserFrom : fromUser;
        chip.pot = toPot;
        instance.allChipsOnBoard.Add(chip);
    }

    public static void CreateCoinBurstEfx(int userId = 0, int fromUserId = 0)
    {
        var pos = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-0.5f, 1f), 0);
        //var coinCountTemp = coinCount == 0 ? Random.Range(35, 60) : coinCount;
        //for (int i = 0; i < coinCountTemp; i++)
        {
            GameObject go = instance.coinBusrtEfxPrefab.Spawn();
            go.transform.SetParent(instance.transform, false);
            go.transform.position = fromUserId == 0 ? pos : instance.playersOnBoard[fromUserId].avatarView.imageAvatar.transform.position;
            var coinBurst = go.GetComponent<CoinBurst>();
            if (userId == 0)
                coinBurst.SetDestination(instance.currentUser.avatarView.moneyView.moneyImage.transform.position);
            else if (instance.playersOnBoard.ContainsKey(userId))
                coinBurst.SetDestination(instance.playersOnBoard[userId].avatarView.imageAvatar.transform.position);
        }
    }
    public static void CreateCoinRainEfx()
    {
        var pos = new Vector3(0, -1.5f, 0);
        //for (int i = 0; i < Random.Range(30, 50); i++)
        {
            var go = instance.coinRainEfxPrefab.Spawn();
            go.transform.SetParent(instance.transform, false);
            go.transform.rotation = Quaternion.Euler(Vector3.left * 60);
            go.transform.position = pos;
            go.transform.DOMoveZ(0, 2.5f).OnComplete(() => go.Recycle());
            //go.GetComponent<ChipEffect>().Run(Instance.userCoinTransform.position);
        }
    }

    public static void SetAllButtons(bool active, params string[] excludes)
    {
        foreach (var key in instance.buttons.Keys)
        {
            if (active)
                instance.buttons[key].SetActive(!excludes.Any(x => x == key));
            else
                instance.buttons[key].SetActive(excludes.Any(x => x == key));
        }
    }
    public static void SetButtonActive(string btnName, bool active)
    {
        if (instance.buttons.ContainsKey(btnName))
        {
            instance.buttons[btnName].SetActive(active);
        }
        else
        {
            UILogView.Log("SetButtonActive: " + btnName + " is not existed");
        }
    }
    public static void SetButtonsActive(string[] btnNames, bool[] active)
    {
        for (int i = 0; i < btnNames.Length; i++)
        {
            SetButtonActive(btnNames[i], active[i]);
        }
    }
    public static void SetResult(params int[] pot)
    {
        if (instance != null)
        {
            for (int i = 0; i < instance.allPots.Length; i++)
            {
                if (pot.Contains(i + 1))
                {
                    var PotInfo = instance.allPots[i];
                    PotInfo.SetWin(true);
                }
            }
        }
        else
            UILogView.Log("SetResult: Xocdia UI Controller has been destroyed");
    }
	public static void SetLucky(int pot, int rate, bool isAnimate = true)
	{
		if (instance != null)
		{
			if (isAnimate)
			{
				for (int i = 0; i < 8; i++)
				{
					DOVirtual.DelayedCall(0.4f * i, () =>
						{
							var index = Random.Range(0, 6);
							instance.allPots[index].SetLucky(true, rate, 0.4f);
						});
				}
				DOVirtual.DelayedCall(3.5f, () => { instance.allPots[pot].SetLucky(true, rate); });
			}
			else
			{
				instance.allPots[pot].SetLucky(true, rate);
			}
		}
		else
			Debug.Log("SetLucky: Xocdia UI Controller has been destroyed");
	}
    public static void SetUsers(List<UserData> users)
    {
        if (instance == null)
        {
            Debug.Log("SetUIPlayers: Game UI Controller has been destroyed");
            return;
        }
        instance.UpdateUIPlayers(users);

    }
    public static void SetRoomData(RoomInfo roomData)
	{
		// For using stateNew + timeCountDownNew in XOCDIA, BAUCUA new
		if (OGUIM.currentLobby.id == (int)LobbyId.XOCDIA || OGUIM.currentLobby.id == (int)LobbyId.BAUCUA) {
			roomData.state = roomData.stateNew;
			roomData.timeCountDown = roomData.timeCountDownNew;
		}

        if (instance != null)
        {
            instance.UpdateRoom(roomData);
        }
        else
            UILogView.Log("SetRoomData: Game UI Controller has been destroyed");
    }

    public static string GetPotName(int pot)
    {
        try
        {
            if (instance.casinoMode == CasinoMode.XOCDIA)
                return XOCDIA_namePots[pot - 1];
            else
                return BAUCUA_namePots[pot - 1];
        }
        catch(System.Exception ex)
        {
            UILogView.Log(ex.Message + " " + ex.StackTrace + " " + pot);
            return "";
        }
    }
    public static RoomInfo GetRoomData()
    {
        if (instance == null)
        {
            UILogView.Log("GetRoomData: Game UI Controller has been destroyed");
            return null;
        }
        return instance.roomData;
    }

    public static Dictionary<int, PlayerView> GetPlayersOnBoard()
    {
        if (instance == null)
        {
            UILogView.Log("GetPlayersOnBoard: Game UI Controller has been destroyed");
            return null;
        }
        return instance.playersOnBoard;
    }

    public static void SetTime(int time)
    {
        instance.batdia.SetTime(time);
    }
    public static void StartShake()
    {
        instance.batdia.StartShake();
    }
    public static void StopShake()
    {
        instance.batdia.StopShake();
    }
    public static void Open()
    {
        instance.batdia.Open();
    }
    public static void Close()
    {
        instance.batdia.Close();
    }
    public static void SetHistory(List<CasinoVi> his)
    {
        instance.history = his;
        if (instance.casinoMode == CasinoMode.XOCDIA)
        {
            instance.XOCDIA_historyView.Get_Data(true);
            instance.XOCDIA_historyView.listData = instance.history;
            instance.XOCDIA_historyView.FillData();
            DOVirtual.DelayedCall(0.5f, () =>
            {
                instance.XOCDIA_historyView.uiListView.ScrollVerticalToRight();
            });
        }
        else
        {
            instance.BAUCUA_historyView.Get_Data(true);
            instance.BAUCUA_historyView.listData = instance.history;
            instance.BAUCUA_historyView.FillData();
            DOVirtual.DelayedCall(0.5f, () =>
            {
                instance.BAUCUA_historyView.uiListView.ScrollVerticalToRight();
            });
        }
    }
    public static void AddHistory(CasinoVi vi = null)
    {
        if (vi != null)
            instance.history.Add(vi);
        SetHistory(instance.history);
    }
    public static void SetVis(List<int> vi)
    {
        instance.batdia.SetVis(vi);
    }
    public static void SetUIWithEachOwnerRule()
    {
        if (instance.roomData == null)
        {
            UILogView.Log("SetUIWithEachOwnerRule: roomData is null");
            return;
        }
        if(OGUIM.me.seatOrder == -2 || OGUIM.me.owner)
        {
            for (int i = 0; i < instance.allPots.Length; i++)
            {
                instance.allPots[i].SetOwnerBet(-1);
            }
        }

        if (instance.isOldVersion)
        {
            UILogView.Log("Version cũ chỉ quan tâm nút hủy cược");
			SetButtonActive("CASINO_huycuoc_btn", (CASINOGameStateOld)instance.roomData.state == CASINOGameStateOld.DEAL || 
				(CASINOGameStateOld)instance.roomData.state == CASINOGameStateOld.BET);
            return;
        }
        switch ((CASINOGameState)instance.roomData.state)
        {
            case CASINOGameState.STOP:
            case CASINOGameState.WAIT:
                SetButtonActive("CASINO_huycuoc_btn", false);
                SetButtonActive("CASINO_cancua_btn", false);
                if (OGUIM.me.seatOrder == -2 || OGUIM.me.owner)
                {
                    instance.betToggelGroup.gameObject.SetActive(false);
                    SetButtonActive("CASINO_bo_btn", true);
                    SetButtonActive("CASINO_lamcai_btn", false);
                    SetButtonActive("CASINO_seat_btn", false);
                }
                else
                {
                    instance.betToggelGroup.gameObject.SetActive(true);
                    SetButtonActive("CASINO_bo_btn", false);
                    SetButtonActive("CASINO_lamcai_btn", !instance.allUsers.Any(x => x.seatOrder == -2 || x.owner));
                    SetButtonActive("CASINO_seat_btn", true);
                    instance.CASINO_seatToggle.isOn = OGUIM.me.seatOrder != -1;
                }
                break;
            case CASINOGameState.DEAL:

                SetButtonActive("CASINO_bo_btn", false);
                SetButtonActive("CASINO_cancua_btn", false);
                SetButtonActive("CASINO_lamcai_btn", false);
                SetButtonActive("CASINO_seat_btn", false);
                if (OGUIM.me.seatOrder == -2 || OGUIM.me.owner)
                {
                    SetButtonActive("CASINO_huycuoc_btn", false);
                }
                else
                {
                    instance.betToggelGroup.gameObject.SetActive(true);
                    //SetButtonActive("CASINO_huycuoc_btn", true);
                }
                break;
            case CASINOGameState.BET:
                SetButtonActive("CASINO_bo_btn", false);
                SetButtonActive("CASINO_cancua_btn", false);
                SetButtonActive("CASINO_lamcai_btn", false);
                SetButtonActive("CASINO_seat_btn", false);
                if (OGUIM.me.seatOrder == -2 || OGUIM.me.owner)
                {
                    SetButtonActive("CASINO_huycuoc_btn", false);
                }
                else
                {
                    instance.betToggelGroup.gameObject.SetActive(true);
                    SetButtonActive("CASINO_huycuoc_btn", true);
                }
                break;
            case CASINOGameState.STOP_SET_BET:
				SetButtonActive("CASINO_cancua_btn", OGUIM.me.seatOrder == -2 || OGUIM.me.owner);
				SetButtonActive("CASINO_bo_btn", false);
				SetButtonActive("CASINO_lamcai_btn", false);
				SetButtonActive("CASINO_seat_btn", false);
                if (OGUIM.me.seatOrder == -2 || OGUIM.me.owner)
                {
                    if (instance.casinoMode == CasinoMode.XOCDIA)
                        instance.sellBuyPotsToggle.Show(false, 1, 2);
                    else
                        instance.sellBuyPotsToggle.ShowAll(false);
                }
                else
                    instance.sellBuyPotsToggle.HideAll();
                break;
			case CASINOGameState.END:
				SetButtonActive("CASINO_huycuoc_btn", false);
				SetButtonActive("CASINO_cancua_btn", false);
				SetButtonActive("CASINO_bo_btn", false);
				SetButtonActive("CASINO_lamcai_btn", false);
				if (OGUIM.me.seatOrder == -2 || OGUIM.me.owner)
					instance.betToggelGroup.gameObject.SetActive(false);
				else
					instance.betToggelGroup.gameObject.SetActive(true);
                break;
            case CASINOGameState.CALCULATE:
                SetButtonActive("CASINO_cancua_btn", false);
				SetButtonActive("CASINO_huycuoc_btn", false);
                if (OGUIM.me.seatOrder == -2 || OGUIM.me.owner)
                {
                    SetButtonActive("CASINO_bo_btn", true);
                    SetButtonActive("CASINO_seat_btn", false);
                }
                else
                {
                    instance.betToggelGroup.gameObject.SetActive(true);
                    SetButtonActive("CASINO_bo_btn", false);
					SetButtonActive("CASINO_seat_btn", true);
					SetButtonActive("CASINO_lamcai_btn", !instance.allUsers.Any(x => x.seatOrder == -2 || x.owner));
                    instance.CASINO_seatToggle.isOn = OGUIM.me.seatOrder != -1;
                }
                break;
            case CASINOGameState.BUY_POT:
				SetButtonActive("CASINO_cancua_btn", false);
				SetButtonActive("CASINO_bo_btn", false);
				SetButtonActive("CASINO_lamcai_btn", false);
				SetButtonActive("CASINO_seat_btn", false);
                if (OGUIM.me.seatOrder == -2 || OGUIM.me.owner)
                    instance.sellBuyPotsToggle.HideAll();
                break;
        }
    }

	public static void UpdateButton(int type, int action)
	{
		if (type == 1)
		{
			SetButtonActive ("CASINO_bo_btn", false);
			SetButtonActive ("CASINO_lamcai_btn", false);

			if (action == 2)
				SetButtonActive ("CASINO_lamcai_btn", true);
			else if (action == 3)
				SetButtonActive ("CASINO_bo_btn", true);
		}

		if (type == 2) {
			
		}
	}

    public static void SetRoomBet(List<Slot> pots, UserData user = null)
    {
        instance.UpdateBet(pots, user);
    }
    public static void ClearUserBet(int userId)
    {
        var removeChips = new List<ChipSprite>();
        var userChips = instance.allChipsOnBoard.Where(x => x.userId == userId).ToList();
        var toPos = Vector3.zero;
        if (OGUIM.me.id == userId && OGUIM.me.seatOrder == -1)
        {
            toPos = instance.playersOnBoard[userId].avatarView.moneyView.moneyImage.transform.position;
        }
        else if (instance.playersOnBoard.ContainsKey(userId))
        {
            toPos = instance.playersOnBoard[userId].avatarView.imageAvatar.transform.position;
        }
        else
        {
            toPos = instance.anotherPlayers.position;
        }

        foreach (var c in userChips)
        {
            c.SetPosition(toPos, true, 0, true);
            removeChips.Add(c);
        }

        foreach (var c in removeChips)
            instance.allChipsOnBoard.Remove(c);
    }
    public static void ClearPotBet(int pot)
    {
        TakeChipToUser(false, pot);
        instance.allPots[pot - 1].Reset(OGUIM.me.seatOrder == -2 || OGUIM.me.owner);
    }
    public static void AddChipFromHost(int pot)
    {
        if (pot > instance.allPots.Length)
        {
            Debug.Log("AddChipFromHost pot + " + pot + " is not exist!");
            return;
        }

        var fromId = instance.players[0].userData.id;
        if (instance.sellPotsAndUser.ContainsKey(pot))
            fromId = instance.sellPotsAndUser[pot];

        var userChips = instance.allChipsOnBoard.Where(x => x.pot == pot).ToList();
        foreach(var c in userChips)
        {
            SpawnChipSprite(fromId, c.pot, c.userId);
        }
    }
    public static void TakeChipToUser(bool hasDelay, params int[] pots)
    {
        var removeChips = new List<ChipSprite>();
        for (int i = 0; i < instance.allPots.Length; i++)
        {
            if (!pots.Contains(i + 1))
                continue;
            
            var userChips = instance.allChipsOnBoard.Where(x => x.pot == i + 1).ToList();
            int ii = 0;
            float delayStep = hasDelay ? (2f / Mathf.Max(userChips.Count, 10)) : 0;
            foreach (var c in userChips)
            {
                var toPos = Vector3.zero;
                if(OGUIM.me.id == c.userId)
                {
                    toPos = instance.playersOnBoard[c.userId].avatarView.moneyView.moneyImage.transform.position;
                }
                else if (instance.playersOnBoard.ContainsKey(c.userId))
                {
                    toPos = instance.playersOnBoard[c.userId].avatarView.imageAvatar.transform.position;
                }
                else
                {
                    toPos = instance.anotherPlayers.position;
                }
                c.SetPosition(toPos, true, ii * delayStep, true);
                removeChips.Add(c);
                ii++;
            }
        }

        foreach (var c in removeChips)
            instance.allChipsOnBoard.Remove(c);
    }
    public static void TakeChipToHost(params int[] pots)
    {
        var removeChips = new List<ChipSprite>();
        for (int i = 0; i < instance.allPots.Length; i++)
        {
            if (pots.Contains(i + 1))
                continue;

            var toId = -1;
            if (instance.sellPotsAndUser.ContainsKey(i + 1))
                toId = instance.sellPotsAndUser[i + 1];

            var toPos = toId != -1 ? instance.playersOnBoard[toId].avatarView.imageAvatar.transform.position : instance.players[0].avatarView.imageAvatar.transform.position;
            var userChips = instance.allChipsOnBoard.Where(x => x.pot == i + 1).ToList();
            foreach (var c in userChips)
            {
                c.SetPosition(toPos, true, 0, true);
                removeChips.Add(c);
            }
            instance.allPots[i].Reset(OGUIM.me.seatOrder == -2 || OGUIM.me.owner);
        }

        foreach (var c in removeChips)
            instance.allChipsOnBoard.Remove(c);
    }
    
    public static void UserBuyPot(int pot, int userId)
    {
        instance.sellBuyPotsToggle.Hide(pot - 1);
        if (instance.sellPotsAndUser.ContainsKey(pot))
            instance.sellPotsAndUser[pot] = userId;
        else
            instance.sellPotsAndUser.Add(pot, userId);

        if (instance.playersOnBoard.ContainsKey(userId))
        {
            var pos = instance.playersOnBoard[userId].avatarView.imageAvatar.transform.position;
            SpawnTextEfx("Mua cửa " + GetPotName(pot), pos);
        }
    }
    #endregion  

    public PlayerView currentUser;
    public GameObject coinBusrtEfxPrefab, coinRainEfxPrefab;
    public TextEffext textEfxPrefab;
    public UIAnimation anim;
    public RoomInfo roomData;
    public List<UserData> allUsers;
    Dictionary<int, PlayerView> playersOnBoard;
    public Button[] buttonsInGame;
    public Dictionary<string, GameObject> buttons;
    public Toggle CASINO_seatToggle;
    public UIToggleGroup betToggelGroup;

    public XOCDIA_HistoryListView XOCDIA_historyView;
    public BAUCUA_HistoryListView BAUCUA_historyView;

    public UISellPot XOCDIA_sellBuyPotsToggle;
    public UISellPot BAUCUA_sellBuyPotsToggle;

    bool isOldVersion;
    public UISellPot sellBuyPotsToggle
    {
        get
        {
            if (casinoMode == CasinoMode.XOCDIA)
                return XOCDIA_sellBuyPotsToggle;
            else
                return BAUCUA_sellBuyPotsToggle;
        }
    }
    public BatDia batdia;
    public PotInfo[] XOCDIA_pots;
    public PotInfo[] BAUCUA_pots;
    public ChipSprite chipSpritePrefab;
    public List<ChipSprite> allChipsOnBoard;
    public Transform anotherPlayers;
    public PotInfo[] allPots
    {
        get
        {
            if (casinoMode == CasinoMode.XOCDIA)
                return XOCDIA_pots;
            return BAUCUA_pots;
        }
    }

    public PlayerView[] players;
    public Dictionary<int, int> sellPotsAndUser;
    public int maxPlayerInGame = 8;
    public GameObject tableOnXoc, tabkeOnBau;
    public CasinoMode casinoMode = CasinoMode.XOCDIA;
    public List<CasinoGameManager> gmPrefabs;
    [HideInInspector]
    public CasinoGameManager gameManager;

    int currentBet;
    public List<CasinoVi> history;
    // Use this for initialization
    void Awake () {

        instance = this;

        playersOnBoard = new Dictionary<int, PlayerView>();
        allChipsOnBoard = new List<ChipSprite>();
        sellPotsAndUser = new Dictionary<int, int>();

        coinBusrtEfxPrefab.CreatePool(10);
        coinRainEfxPrefab.CreatePool(4);
        textEfxPrefab.CreatePool(10);
        chipSpritePrefab.CreatePool(100);

        buttons = new Dictionary<string, GameObject>();
        foreach (var btn in buttonsInGame)
            buttons.Add(btn.name, btn.gameObject);

        currentUser.FillData(OGUIM.me);
        for (int i = 0; i < players.Length; i++)
            players[i].GetComponent<PlayerView>().FillData(null);

        //anim.Show();

        if (OGUIM.currentLobby.id == (int)LobbyId.XOCDIA || OGUIM.currentLobby.id == (int)LobbyId.XOCDIA_OLD)
        {
            isOldVersion = OGUIM.currentLobby.id == (int)LobbyId.XOCDIA_OLD;
            casinoMode = CasinoMode.XOCDIA;
            gameManager = Instantiate(gmPrefabs[0]);

            if(isOldVersion)
            {
                UILogView.Log("Current casino: " +  (LobbyId)OGUIM.currentLobby.id);
            }
        }
		else if (OGUIM.currentLobby.id == (int)LobbyId.BAUCUA || OGUIM.currentLobby.id == (int)LobbyId.BAUCUA_OLD)
		{
			isOldVersion = OGUIM.currentLobby.id == (int)LobbyId.BAUCUA_OLD;
            casinoMode = CasinoMode.BAUCUA;
            gameManager = Instantiate(gmPrefabs[1]);
        }
        gameManager.gameObject.transform.SetParent(transform);
        ResetBoard();
    }
    void Update()
    {
    }
    public void OnHideCompleted()
    {
        Debug.Log("OnHideCompleted");
        Destroy(gameManager.gameObject);
    }

    private void UpdateUIPlayers(List<UserData> users)
    {
        allUsers = users;
        var me = users.Where(x => x.id == OGUIM.me.id).FirstOrDefault();
        if (me == null)
        {
            Debug.LogError("SetUIPlayers: Ban ko con trong phong!");
            OGUIM.Toast.Show("Bạn đã thoát khỏi phòng hiện tại", UIToast.ToastType.Warning);
            return;
        }

        OGUIM.me = me;
        currentUser.FillData(me);
        currentUser.SetReady((me.isReady || me.owner || roomData.started) && me.isPlayer);
        AddOrUpdatePlayerOnBoard(me.id, currentUser);

        if (isOldVersion)
        {
            UILogView.Log("Version cũ không cần update vị trí user");
            currentUser.SetReady(true);
            return;
        }

        try
        {
            var listPlayer = new List<int>() { 0, 1, 2, 3, 4, 5, 6 };
            foreach (var i in users)
            {
                if (i.seatOrder == -1)
                    continue;
                var fakeOrder = i.seatOrder;
                if (fakeOrder == -2)
                    fakeOrder = 0;
                else
                    fakeOrder++;

                listPlayer.Remove(fakeOrder);
                var playerInfo = instance.players[fakeOrder].GetComponent<PlayerView>();

                playerInfo.FillData(i);
                playerInfo.SetReady(true);
                if (!i.isPlayer || !roomData.started)
                {
                    playerInfo.Reset();
                }

                AddOrUpdatePlayerOnBoard(i.id, playerInfo);
            }
            ClearBoard(listPlayer);
        }
        catch (System.Exception ex)
        {
            Debug.Log("UpdateUIPlayers throw ex: " + ex.Message);
            UILogView.Log("UpdateUIPlayers throw ex: " + ex.Message, true);
            OGUIM.Toast.Show("Có lỗi xảy ra. Vui lòng thử lại", UIToast.ToastType.Warning);
            BackBtn_Click();
        }
    }

    private void UpdateRoom(RoomInfo roomData)
    {
        this.roomData = roomData;
        UpdateBet(roomData.slots);

        // BET FOR OLD XOCDIA/BAUCUA
        if (OGUIM.currentLobby.id == (int)LobbyId.XOCDIA_OLD || OGUIM.currentLobby.id == (int)LobbyId.BAUCUA_OLD)
        {
            roomData.bet = 500;
            betLevels[1] = 20;
            betLevels[2] = 100;
            betLevels[3] = 1000;
        }

        var betValues = new List<object> { LongConverter.ToK(roomData.bet * betLevels[0]),
            LongConverter.ToK(roomData.bet * betLevels[1]),
            LongConverter.ToK(roomData.bet * betLevels[2]),
            LongConverter.ToK(roomData.bet * betLevels[3]) };
        betToggelGroup.UpdateTextContent(betValues);
        
        if(currentBet == 0)
        currentBet = roomData.bet;

        if (roomData != null)
        {
            OGUIM.instance.lobbyName.text = OGUIM.currentLobby.desc.ToUpper();

            // HIDDEN ROOM-BET FOR OLD XOCDIA/BAUCUA
            if (OGUIM.currentLobby.id == (int)LobbyId.XOCDIA_OLD || OGUIM.currentLobby.id == (int)LobbyId.BAUCUA_OLD)
                OGUIM.instance.roomBetValue.text = "";
            else
                OGUIM.instance.roomBetValue.text = Ultility.CoinToStringNoMark(roomData.bet) + " " + OGUIM.currentMoney.name;
        }
        if(casinoMode == CasinoMode.XOCDIA)
        {
            tableOnXoc.SetActive(true);
            XOCDIA_sellBuyPotsToggle.gameObject.SetActive(true);
            tabkeOnBau.SetActive(false);
            BAUCUA_sellBuyPotsToggle.gameObject.SetActive(false);
        }
        else
        {
            tableOnXoc.SetActive(false);
            XOCDIA_sellBuyPotsToggle.gameObject.SetActive(false);
            tabkeOnBau.SetActive(true);
            BAUCUA_sellBuyPotsToggle.gameObject.SetActive(true);
        }
    }
    private void UpdateBet(List<Slot> slots, UserData user = null)
    {
        if(slots == null || !slots.Any())
        {
            ResetBoard();
            UILogView.Log("UpdateBet: pots is null or empty");
            return;
        }

        foreach(var s in slots)
        {
            if(s.pot > allPots.Length)
            {
                UILogView.Log("UpdateBet: pot is out of array PotInfos");
            }
            allPots[s.pot - 1].SetAllBet(s.bet);
        }
        if (user != null && user.id == OGUIM.me.id)
        {

            if (user.seatOrder == -2)
            {
                for (int i = 0; i < allPots.Length; i++)
                {
                    allPots[i].SetOwnerBet(-1);
                }
            }
            else if (user.properties != null)
            {
                allPots[0].SetOwnerBet(user.properties.pot_bet_1);
                allPots[1].SetOwnerBet(user.properties.pot_bet_2);
                allPots[2].SetOwnerBet(user.properties.pot_bet_3);
                allPots[3].SetOwnerBet(user.properties.pot_bet_4);
                allPots[4].SetOwnerBet(user.properties.pot_bet_5);
                allPots[5].SetOwnerBet(user.properties.pot_bet_6);
            }
            else
            {
                for (int i = 0; i < allPots.Length; i++)
                {
                    allPots[i].SetOwnerBet(0);
                }
            }
        }
    }
    void AddOrUpdatePlayerOnBoard(int id, PlayerView player)
    {
        if (playersOnBoard.ContainsKey(id))
        {
            playersOnBoard[id] = player;
        }
        else
            playersOnBoard.Add(id, player);
    }

    public void ClearBoard(List<int> listPlayer)
    {
        foreach (int i in listPlayer)
        {
            var player = players[i].GetComponent<PlayerView>();
            player.FillData(null);
        }
    }

    public void ResetBoard()
    {
        sellBuyPotsToggle.HideAll();
        foreach (var key in playersOnBoard.Keys)
            playersOnBoard[key].Reset();
        for (int i = 0; i < allPots.Length; i++)
            allPots[i].Reset(OGUIM.me.seatOrder == -2 || OGUIM.me.owner);

        chipSpritePrefab.RecycleAll();
        allChipsOnBoard.Clear();
        sellPotsAndUser.Clear();
    }

    public void CASINO_betSelect(int index)
    {
        currentBet = betLevels[index - 1] * roomData.bet;
        Debug.Log("curent bet: " + currentBet);
    }
    public void CASINO_potBtn_Click(int index)
    {
        if(OGUIM.me.seatOrder == -2)
        {
            Debug.Log("Chu phong ko dc dat cua -_-");
            return;
        }
        BuildWarpHelper.CASINO_SetBet(index, currentBet, () =>
        {
            UILogView.Log("CASINO_SetBet is Time out");
        });
    }
    public void CASINO_lamcaiBtn_click()
    {
        OGUIM.Toast.ShowLoading("");
        BuildWarpHelper.CASINO_GetOwner(() =>
        {
            UILogView.Log("CASINO_ClearBet is Time out");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }

    public void CASINO_seatBtn_Click()
    {
        OGUIM.Toast.ShowLoading("");
        BuildWarpHelper.CASINO_StandOrSit(OGUIM.me.isStanding(), () =>
        {
            UILogView.Log("CASINO_seatBtn_Click is Time out");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }
    public void CASINO_huycuocBtn_click()
    {
        OGUIM.Toast.ShowLoading("");
        BuildWarpHelper.CASINO_ClearBet(() =>
        {
            UILogView.Log("CASINO_ClearBet is Time out");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }
    public void CASINO_boBtn_click()
    {
        BuildWarpHelper.CASINO_PassOwner(() =>
        {
            UILogView.Log("CASINO_cancuaBtn_click is Time out");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }
    public void CASINO_cancuaBtn_click()
    {
        OGUIM.Toast.ShowLoading("");
        BuildWarpHelper.CASINO_HandleAll(() =>
        {
            UILogView.Log("CASINO_cancuaBtn_click is Time out");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }
    public void CASINO_sellBuyPotBtn_click(int index)
    {
        OGUIM.Toast.ShowLoading("");
        if (OGUIM.me.seatOrder == -2)
        {
            BuildWarpHelper.CASINO_SellPot(index, () =>
            {
                UILogView.Log("CASINO_sellBuyPotBtn_click is Time out");
                OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
            });
        }
        else
        {
            BuildWarpHelper.CASINO_BuyPot(index, () =>
            {
                UILogView.Log("CASINO_sellBuyPotBtn_click is Time out");
                OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
            });
        }
    }
    public void BackBtn_Click()
    {
        OGUIM.Toast.ShowLoading("");
        BuildWarpHelper.LeaveRoom(() =>
        {
            UILogView.Log("Leave room is timeout");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }

    public void ShowPlayersInRoom()
    {
        if (OGUIM.instance != null && OGUIM.instance.popUpPlayersInRoom != null)
        {
            OGUIM.instance.popUpPlayersInRoom.Show();
        }
    }

}

public enum CasinoMode
{
    XOCDIA = 0,
    BAUCUA = 1
}