using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class IGUIM : MonoBehaviour
{
    public static IGUIM instance;

    #region Game Static Method

    public static GameObject SpawnChipEfx()
    {
        GameObject go = instance.casinoGameUI.chipTypePrefab.Spawn();
        go.transform.SetParent(instance.transform, false);
        go.GetComponent<Image>().sprite = OGUIM.currentMoney.image;

        return go;
    }
    public static void SpawnTextEfx(string str, Vector3 pos, bool active = true)
    {
        var te = instance.textEfxPrefab.Spawn();
        te.gameObject.transform.SetParent(instance.transform, false);
        te.gameObject.transform.position = pos;
        te.gameObject.transform.localScale = Vector3.one * (pos == Vector3.zero ? 1 : 0.6f);

        te.SetData(str.ToUpper(), active ? Color.white : Color.black, 0, 0);
    }
    public static GameObject CreateCard(GameObject prefab, CardData cardData, Vector3 position)
    {
        var go = prefab.Spawn(position);
        //var go = Instantiate(prefab, position, Quaternion.identity) as GameObject;
        go.transform.SetParent(instance.transform, false);
        go.transform.position = (Vector2)position;
        var card = go.GetComponent<Card>();
        card.SetData(cardData);
        return go;
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
        switch (instance.gameMode)
        {
            case GameMode.CARD:
                instance.cardGameUI.SetAllButtons(active, excludes);
                break;
            case GameMode.CASINO:
                instance.casinoGameUI.SetAllButtons(active, excludes);
                break;
        }
    }

    public static void SetButtonActive(string btnName, bool active)
    {
        if (instance != null)
        {
            switch (instance.gameMode)
            {
                case GameMode.CARD:
                    instance.cardGameUI.SetButtonActive(btnName, active);
                    break;
                case GameMode.CASINO:
                    instance.casinoGameUI.SetButtonActive(btnName, active);
                    break;
            }
        }
        else
            UILogView.Log("SetButtonActive: Game UI Controller has been destroyed");
    }

    public static void SetButtonsActive(string[] btnNames, bool[] active)
    {
        switch (instance.gameMode)
        {
            case GameMode.CARD:
                instance.cardGameUI.SetButtonsActive(btnNames, active);
                break;
            case GameMode.CASINO:
                instance.casinoGameUI.SetButtonsActive(btnNames, active);
                break;
        }
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

    public static void SetRoomData(RoomInfo room)
    {
        if (room == null)
        {
            DOTween.Kill(instance.GetType().Name);
        }
        if (instance != null)
        {
            instance.UpdateRoom(room);
        }
        else
            UILogView.Log("SetRoomData: Game UI Controller has been destroyed");
    }

    public static void UpdateRoomState(CardRoomState state, int timeCountDown)
    {
        // downgrade
        if (GameBase.isOldVersion)
        {
            instance.readyText.text = "";
            return;
        }
        if (instance != null)
        {
            if (instance.roomData != null && instance.roomData.state != (int)state)
            {
                DOTween.Kill(instance.GetType().Name);
                if (state == CardRoomState.WAIT)
                {
                    var isSetBet = OGUIM.me.owner;
                    DOVirtual.Float(timeCountDown, 0, timeCountDown,
                        (x) =>
                        {
                            var time = Mathf.CeilToInt(x);
                            if (time == 2)
                            {
                                instance.ReadyBtn_Click();
                                if (instance.gameManager is BACAY_GameManager && !isSetBet && OGUIM.currentLobby.id == (int)LobbyId.BACAY)
                                {
                                    isSetBet = true;
                                    instance.BACAY_slider_value_changed();
                                }
                            }
                            if (time > 0)
                                instance.readyText.text = "Ván mới sẽ bắt đầu sau " + time + " giây";
                            else
                                instance.readyText.text = "Ván mới bắt đầu";
                        }).SetId(instance.GetType().Name).SetEase(Ease.Linear);
                }
                else
                {
                    instance.readyText.text = "";
                }
                instance.roomData.state = (int)state;
            }
        }
        else
            UILogView.Log("SetRoomData: Game UI Controller has been destroyed");
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

    public static void SetChiInfo(params string[] chis)
    {
        if (instance != null)
        {
            instance.fadeCover.gameObject.SetActive(chis.Length >= 3);
            instance.cardGameUI.SetChiInfo(chis);
        }
        else
            UILogView.Log("SetChiInfo: Game UI Controller has been destroyed");
    }
    #endregion  

    public PlayerView currentUser;
    public GameObject coinBusrtEfxPrefab, coinRainEfxPrefab;
    public TextEffext textEfxPrefab;
    public UIAnimation anim;
    public List<GameObject> gmPrefabs;
    public RoomInfo roomData;
    Dictionary<int, PlayerView> playersOnBoard;

    public GameMode gameMode = GameMode.CARD;
    public CardGameUI cardGameUI;
    public CasinoGameUI casinoGameUI;

    public Transform cardOnHandTransform;
    public Image fadeCover;
    public Text readyText;
    public Button inviteBtn;

    public PlayerView[] players
    {
        get
        {
            if (gameMode == GameMode.CARD)
                return cardGameUI.players;
            else
                return casinoGameUI.players;
        }
    }
    public int maxPlayerInGame
    {
        get
        {
            if (gameMode == GameMode.CARD)
                return cardGameUI.maxPlayerInGame;
            else
                return casinoGameUI.maxPlayerInGame;
        }
    }
    [HideInInspector]
    public CardGameManager gameManager;
    public void Awake()
    {
        instance = this;
        InitGameMode();

        playersOnBoard = new Dictionary<int, PlayerView>();

        var prefab = gmPrefabs.FirstOrDefault(x => x.name.Contains(OGUIM.currentLobby.name.Replace("_SOLO", "").Replace("_GA", "")));
        var gm = Instantiate(prefab) as GameObject;
        gm.gameObject.transform.SetParent(transform);
        gameManager = gm.GetComponent<CardGameManager>();

        coinBusrtEfxPrefab.CreatePool(10);
        coinRainEfxPrefab.CreatePool(4);
        textEfxPrefab.CreatePool(10);

        currentUser.FillData(OGUIM.me);
        for (int i = 0; i < players.Length; i++)
            players[i].GetComponent<PlayerView>().FillData(null);
    }
    public void OnHideCompleted()
    {
        Debug.Log("OnHideCompleted");
        Destroy(gameManager.gameObject);
    }
    private void InitGameMode()
    {
        if (OGUIM.currentLobby.id == (int)LobbyId.BACAY || OGUIM.currentLobby.id == (int)LobbyId.BACAY_GA
               || OGUIM.currentLobby.id == (int)LobbyId.LIENG)
        {
            gameMode = GameMode.CASINO;
            casinoGameUI.chipTypePrefab.CreatePool(30);
            if (OGUIM.currentLobby.id == (int)LobbyId.LIENG)
            {
                casinoGameUI.sliderBet.gameObject.SetActive(false);
            }
            else if (OGUIM.currentLobby.id == (int)LobbyId.BACAY_GA)
            {
                casinoGameUI.sliderBet.gameObject.SetActive(false);
            }
            else
            {
                casinoGameUI.sliderBet.gameObject.SetActive(true);
            }
        }
        else
        {
            gameMode = GameMode.CARD;
            SetChiInfo("");
        }
    }

    #region User Handler
    public void Avatar_Click(int id)
    {
        if (gameManager is MAUBINH_GameManager && ((gameManager as MAUBINH_GameManager).state == MAUBINH_State.SORTING) && OGUIM.me.isPlayer)
        {
            UILogView.Log("Sorting....");
            return;
        }
        userRequest _params = new userRequest();
        RootUserInfo data = new RootUserInfo();
        if (id != 0)
        {
            _params.userId = id;
            if (id == OGUIM.me.id)
                data.user = OGUIM.me;
            else if (playersOnBoard.ContainsKey(id))
                data.user = playersOnBoard[id].userData;
        }
        if (_params.userId > 0)
        {
            Debug.Log("IGUIM Get user info: " + _params.userId);
            UILogView.Log("IGUIM Get user info: " + _params.userId, true);
        }
    }
    public void ReadyBtn_Click()
    {
        gameManager.ClearCards();
        ResetBoard();
        OGUIM.Toast.ShowLoading("");
        BuildWarpHelper.ReadyRequest(true, () =>
            {
                UILogView.Log("UnreadyRequest is Time out");
                OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
            });
    }
    public void UnreadyBtn_Click()
    {
        OGUIM.Toast.ShowLoading("");
        BuildWarpHelper.ReadyRequest(false, () =>
        {
            UILogView.Log("UnreadyRequest is Time out");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }
    public void StartBtn_Click()
    {
        OGUIM.Toast.ShowLoading("");
        gameManager.ClearCards();
        ResetBoard();
        BuildWarpHelper.StartRequest(() =>
        {
            UILogView.Log("Start request is time out");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }
    public void inviteBtn_Click()
    {
        gameManager.ClearCards();
        ResetBoard();
        WarpRequest.InviteToPlay();
        inviteBtn.interactable = false;
        inviteBtn.transform.DORotate(Vector3.zero, 3).OnComplete(() => inviteBtn.interactable = true);

        OGUIM.Toast.Show("Mời thành công. Vui lòng đợi", UIToast.ToastType.Notification, 1);
    }
    public void SubmitBtn_Click()
    {
        var submitCards = gameManager.GetSubmitCards();
        if (submitCards.Any())
        {
            if (gameManager is PHOM_GameManager)
            {
                if (submitCards.Count > 1)
                {
                    OGUIM.Toast.Show("Bài đánh ko hợp lệ", UIToast.ToastType.Warning);
                    return;
                }
            }
            OGUIM.Toast.ShowLoading("");
            if (gameManager is SAM_GameManager)
            {
                var array = submitCards.ToArray();
                if ((submitCards.Count == 3 && !CardLogic.IsSanh(array) && !CardLogic.isSamCo(array)) ||
                    (submitCards.Count == 4 && !CardLogic.IsSanh(array) && !CardLogic.IsTuQuy(array)) ||
                    (submitCards.Count >= 5 && !CardLogic.IsSanh(array)))
                {
                    OGUIM.Toast.Show("Đánh bài không hợp lệ", UIToast.ToastType.Warning);
                    gameManager._wc_OnSubmitFailed();
                    return;
                }
            }
            BuildWarpHelper.TLMN_SubmitTurnRequest(submitCards, () =>
            {
                gameManager._wc_OnSubmitFailed();
                UILogView.Log("TLMN_SubmitTurn request is time out");
                OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
            });
        }
        else
        {
            OGUIM.Toast.Show("Hãy chọn bài đánh", UIToast.ToastType.Warning);
        }
    }
    public void PassBtn_Click()
    {
        OGUIM.Toast.ShowLoading("");
        BuildWarpHelper.TLMN_PassTurnRequest(() =>
        {
            UILogView.Log("TLMN_PassTurn request is time out");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }
    public void SAM_bao_Btn_Click()
    {
        OGUIM.Toast.ShowLoading("");
        BuildWarpHelper.SAM_CancelOrRequestTurn(true, () =>
        {
            UILogView.Log("SAM_RequestTurn request is time out");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }
    public void SAM_huy_Btn_Click()
    {
        OGUIM.Toast.ShowLoading("");
        BuildWarpHelper.SAM_CancelOrRequestTurn(false, () =>
        {
            UILogView.Log("SAM_CancelTurn request is time out");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }

    public void MAUBINH_HuyXepBtn_Click()
    {
        OGUIM.Toast.ShowLoading("");
        BuildWarpHelper.MAUBINH_CancelSubmitSuite(() =>
        {
            UILogView.Log("MAUBINH_CancelSubmitSuite request is time out");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }
    public void MAUBINH_XepXongBtn_Click()
    {
        OGUIM.Toast.ShowLoading("");
        var suites = (gameManager as MAUBINH_GameManager).GetSuites();
        BuildWarpHelper.MAUBINH_SubmitSuite(suites, () =>
        {
            UILogView.Log("MAUBINH_SubmitSuite request is time out");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }
    public void MAUBINH_DoiChiBtn_Click()
    {
        if (gameManager is MAUBINH_GameManager)
        {
            (gameManager as MAUBINH_GameManager).SwapChi();
        }
    }

    public void PHOM_anBtn_Click()
    {
        OGUIM.Toast.ShowLoading("");
        BuildWarpHelper.PHOM_GetOrTakeCard((int)GameRequestTypesCode_PHOM.TAKE_CARD, () =>
        {
            UILogView.Log("PHOM_anBtn_Click request is time out");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }
    public void PHOM_bocBtn_Click()
    {
        OGUIM.Toast.ShowLoading("");
        BuildWarpHelper.PHOM_GetOrTakeCard((int)GameRequestTypesCode_PHOM.GET_CARD, () =>
        {
            UILogView.Log("PHOM_bocBtn_Click request is time out");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }
    public void PHOM_guiBtn_Click()
    {
        if (gameManager is PHOM_GameManager)
        {
            var submitCards = gameManager.GetSubmitCards();
            if (submitCards.Any())
            {
                bool check = false;
                foreach (var suiteKey in (gameManager as PHOM_GameManager).suitesOnGame.Keys)
                {
                    var suite = (gameManager as PHOM_GameManager).suitesOnGame[suiteKey];
                    var firstCard = suite.FirstOrDefault();
                    var userId = 0;
                    if (firstCard != null && gameManager.cardsOnBoard.ContainsKey(firstCard.ToString()))
                    {
                        userId = gameManager.cardsOnBoard[firstCard.ToString()].userId;
                    }
                    var newSuite = suite.Concat(submitCards).ToArray();
                    if (CardLogic.IsTuQuy(newSuite) || CardLogic.IsThungPhaSanh(newSuite))
                    {
                        check = true;
                        var suiteData = new PHOM_SuiteData();
                        suiteData.cards = suite;
                        suiteData.userId = userId;
                        var turnData = new PHOM_TurnData();
                        turnData.suite = suiteData;
                        turnData.userId = OGUIM.me.id;
                        turnData.cards = submitCards;
                        OGUIM.Toast.ShowLoading("");
                        BuildWarpHelper.PHOM_AttachCardToSuite(turnData, () =>
                        {
                            UILogView.Log("PHOM_guiBtn_Click request is time out");
                            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
                        });
                        break;
                    }
                }
                if (!check)
                {
                    OGUIM.Toast.Show("Gửi bài không hợp lệ", UIToast.ToastType.Warning);
                }
            }
            else
            {
                OGUIM.Toast.Show("Hãy chọn bài gửi", UIToast.ToastType.Warning);
            }
        }
        else
        {
            UILogView.Log("Bạn đang không chơi phỏm");
        }
    }
    public void PHOM_haBtn_Click()
    {
        if (gameManager is PHOM_GameManager)
        {
            var submitCards = gameManager.GetSubmitCards();
            if (submitCards.Any())
            {
                var count = submitCards.Count;

                //var sortByFace = (gameManager as PHOM_GameManager).SortCard((int)SortingType.ByFace);
                //if(sortByFace.Any (c=>c.suite != 0 && gameManager.cardsOnHand.ContainsKey(c.ToString()) && !gameManager.cardsOnHand[c.ToString()].isSelected))
                //{
                //    (gameManager as PHOM_GameManager).SortCard((int)SortingType.ByCard);
                //}

                var results = from p in submitCards
                              group p by p.suite into g
                              select new { suiteId = g.Key, cards = g.ToList() };


                var ownerSuites = new List<List<CardData>>();
                foreach (var r in results)
                {
                    ownerSuites.Add(r.cards);
                }
                if (ownerSuites.Any())
                {
                    OGUIM.Toast.ShowLoading("");
                    BuildWarpHelper.PHOM_SubmitSuite(ownerSuites, () =>
                    {
                        UILogView.Log("PHOM_haBtn_Click request is time out");
                        OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
                    });
                }
                else
                {
                    BuildWarpHelper.PHOM_SubmitSuite(new List<List<CardData>> { submitCards }, () =>
                   {
                       UILogView.Log("PHOM_haBtn_Click request is time out");
                       OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
                   });
                }
            }
            else
            {
                (gameManager as PHOM_GameManager).SuggestSuite();
                OGUIM.Toast.Show("Hãy chọn phỏm để hạ!", UIToast.ToastType.Warning);
            }
        }
        else
        {
            UILogView.Log("Bạn đang không chơi phỏm!");
        }
    }
    public void PHOM_xepBtn_Click()
    {
        gameManager.ReorderCardOnHand();
    }

    public void BACAY_submit_btn_click()
    {
        foreach (var cKey in gameManager.cardsOnHand.Keys)
            gameManager.cardsOnHand[cKey].isSelected = true;
        var submitCards = gameManager.GetSubmitCards();
        if (submitCards.Any())
        {
            OGUIM.Toast.ShowLoading("");
            BuildWarpHelper.TLMN_SubmitTurnRequest(submitCards, () =>
            {
                UILogView.Log("BACAY_submit_btn_click request is time out");
                OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
            });
        }
        else
        {
            OGUIM.Toast.Show("Hạ bài không hợp lệ", UIToast.ToastType.Warning);
        }
    }
    public void BACAY_slider_value_changed()
    {
        var value = (int)casinoGameUI.sliderBet.GetValue();
        var bet = GetRoomData().bet;

        if (!(gameManager is LIENG_GameManager))
        {
            DOTween.Kill(CasinoGameUI.setBetTweenId);
            DOVirtual.DelayedCall(0.3f, () =>
            {
                BuildWarpHelper.BACAY_SetBet(value * bet, () =>
                {
                    UILogView.Log("BACAY_SetBet is timeout");
                    casinoGameUI.sliderBet.SetValue(casinoGameUI.currentBet / bet);
                });
            }).SetId(CasinoGameUI.setBetTweenId);
        }
    }

    public void LIENG_fold_btn_click()
    {
        OGUIM.Toast.ShowLoading("");
        BuildWarpHelper.LIENG_UserFold(() =>
        {
            UILogView.Log("LIENG_fold_btn_click request is time out");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }
    public void LIENG_call_btn_click()
    {
        OGUIM.Toast.ShowLoading("");
        BuildWarpHelper.LIENG_UserCall(() =>
        {
            UILogView.Log("LIENG_call_btn_click request is time out");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }
    public void LIENG_raise_btn_click()
    {
        var bet = Mathf.Clamp(casinoGameUI.sliderBet.GetValue(), 1, 10) * GetRoomData().bet;

        OGUIM.Toast.ShowLoading("");
        BuildWarpHelper.LIENG_UserRaise(bet, () =>
        {
            UILogView.Log("LIENG_raise_btn_click request is time out");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }


    #region menu
    private GameObject topupPopup;
    public void BackBtn_Click()
    {
        OGUIM.Toast.ShowLoading("");
        BuildWarpHelper.LeaveRoom(() =>
        {
            UILogView.Log("Leave room is timeout");
            OGUIM.Toast.Show("Rất tiếc. Kết nối thất bại", UIToast.ToastType.Warning);
        });
    }
    #endregion

    #endregion

    public void UpdateUIPlayers(List<UserData> users)
    {
        var me = users.Where(x => x.id == OGUIM.me.id).FirstOrDefault();
        if (me == null)
        {
            Debug.LogError("SetUIPlayers: Ban ko con trong phong!");
            OGUIM.Toast.Show("Bạn đã thoát khỏi phòng hiện tại", UIToast.ToastType.Warning);
            OGUIM.GoBack();
            return;
        }

        OGUIM.me = me;
        currentUser.FillData(me);
        currentUser.SetReady((me.isReady || me.owner || roomData.started) && me.isPlayer);
        AddOrUpdatePlayerOnBoard(me.id, currentUser);
        if (!roomData.started)
        {
            currentUser.Reset();
        }

        try
        {
            var listPlayer = new List<int>() { 0, 1, 2 };
            foreach (var i in users)
            {
                if (i.id == OGUIM.me.id)
                {
                    UILogView.Log("UpdateUIPlayers: This is current user!", false);
                    continue;
                }
                var fakeOrder = i.seatOrder - me.seatOrder;
                if (fakeOrder <= 0)
                    fakeOrder += maxPlayerInGame;
                if (OGUIM.currentLobby.lobbymode == LobbyMode.SOLO)
                    fakeOrder = 2;
                listPlayer.Remove(fakeOrder - 1);
                var playerInfo = instance.players[fakeOrder - 1].GetComponent<PlayerView>();

                playerInfo.FillData(i);
                playerInfo.SetReady((i.isReady || i.owner || roomData.started) && i.isPlayer);
                if (!i.isPlayer || !roomData.started)
                {
                    playerInfo.Reset();
                }
                if (roomData.started && i.remainCardCount > 0 && i.isPlayer && (gameManager is TLMN_GameManager || gameManager is SAM_GameManager))
                    playerInfo.SetRemainCard(i.remainCardCount);

                if (OGUIM.currentLobby.id == (int)LobbyId.BACAY && !i.owner && i.properties != null)
                    playerInfo.SetBet(i.properties.user_bet);
                else
                    playerInfo.SetBet(0);

                AddOrUpdatePlayerOnBoard(i.id, playerInfo);

               
            }

            ClearBoard(listPlayer);
        }
        catch (System.Exception ex)
        {
            UILogView.Log("GameUIController throw ex: " + ex.Message);
            OGUIM.Toast.Show("Có lỗi xảy ra. Vui lòng thử lại", UIToast.ToastType.Warning);
            BackBtn_Click();
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
            if (player.userData.id > 0)
            {
                playersOnBoard.Remove(player.userData.id);
            }
            player.FillData(null);
        }
    }

    public void ResetBoard()
    {
        foreach (var key in playersOnBoard.Keys)
            playersOnBoard[key].Reset();
    }

    public virtual void UpdateRoom(RoomInfo roomData)
    {
        this.roomData = roomData;
        if (roomData != null && OGUIM.currentLobby != null)
        {
            var nameLobby = OGUIM.currentLobby.desc + " " + OGUIM.currentLobby.subname;
            OGUIM.instance.lobbyName.text = nameLobby.ToUpper();
            OGUIM.instance.roomBetValue.text = Ultility.CoinToStringNoMark(roomData.bet) + " " + OGUIM.currentMoney.name;

            if (gameMode == GameMode.CARD)
                cardGameUI.UpdateRoom(roomData);
            else
                casinoGameUI.UpdateRoom(roomData);
        }
    }

    public bool FakeUserOnBoard()
    {
        if (players[0].gameObject.activeSelf)
            return false;
        var users = new List<UserData>();
        OGUIM.me.seatOrder = 0;
        OGUIM.me.isPlayer = true;
        OGUIM.me.isReady = true;
        users.Add(OGUIM.me);
        for (int i = 1; i <= 3; i++)
        {
            users.Add(new UserData { id = i, avatar = i + "", seatOrder = i, isPlayer = true, isReady = true });
        }
        IGUIM.SetUsers(users);
        return true;
    }
}

public enum GameMode
{
    CARD = 0,
    CASINO = 1
}

public enum CardRoomState
{
    STOP = -1,
    WAIT = 0,
    PLAY = 1
}