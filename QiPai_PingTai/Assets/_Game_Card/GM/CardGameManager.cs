using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class CardGameManager : GameManager
{
    public static Vector2 CalculateSizeOfCards(int count, float distance, Vector3 direction, float cardSize)
    {
        float w = ((count - 1) * distance + cardSize) * (1 - Mathf.Abs(direction.x));
        float h = ((count - 1) * distance + cardSize) * (1 - Mathf.Abs(direction.y));
        return new Vector2(w, h);
    }
    public GameObject cardPrefab;
    public Dictionary<string, Card> cardsOnBoard;
    public Dictionary<string, Card> cardsOnHand;
    public List<CardData> lastSubmitCards;
    public float interval = 30f;
    public float distanceCards = 0.7f;
    public float cardOnHandScale = 0.75f;
    public float cardOnBoardScale = 0.6f;

    [HideInInspector]
    public int cardsEachRow = 5;
    public int startTimeCountDown;

    //float lastReorderCard = 0;
    //bool inGame = false;
    public override void Awake()
    {
        cardsOnBoard = new Dictionary<string, Card>();
        cardsOnHand = new Dictionary<string, Card>();
        lastSubmitCards = new List<CardData>();
        cardPrefab.CreatePool(50);

        base.Awake();
    }
    public override void LoadScene()
    {
        base.LoadScene();

        if (OGUIM.currentRoom.room != null)
        {
            OGUIM.currentRoom.room.state = 0;
            IGUIM.SetRoomData(OGUIM.currentRoom.room);
            if (IGUIM.GetRoomData().started)
                IGUIM.SetButtonsActive(new string[] { "ready_btn", "start_btn", "invite_btn" }, new bool[] { false, false, false });
            else
                IGUIM.SetButtonsActive(new string[3] { "ready_btn", "start_btn", "invite_btn" },
                                                    new bool[3] { !OGUIM.me.owner, OGUIM.me.owner, false });
        }
    }
    public override void Update()
    {
#if UNITY_EDITOR
        //if (Input.GetButtonDown("Jump"))
        //{
        //    FakeGetRoomInfo();
        //}
            //{
            //    if (IGUIM.instance.FakeUserOnBoard()) return;
            //    if (cardsOnBoard.Any())
            //    {
            //        foreach (var key in cardsOnBoard.Keys)
            //            cardsOnBoard[key].Recycle();
            //        cardsOnBoard.Clear();
            //        lastSubmitCards.Clear();
            //    }

            //    //Test Start match
            //    var cards = new List<CardData> { new CardData(6,1), new CardData(6, 2) , new CardData(6, 3) , new CardData(5, 1),
            //                                     new CardData(7,1), new CardData(1, 4) , new CardData(3, 4) , new CardData(9, 2),
            //                                     new CardData(7,2), new CardData(1, 1) , new CardData(3, 1) , new CardData(9, 1),
            //                                     new CardData(12,4)};
            //    StartMatch(cards);

            //    //Test Submit card
            //    //var playersOnBoard = IGUIM.GetPlayersOnBoard();
            //    //var p = playersOnBoard.OrderBy(x => System.Guid.NewGuid()).FirstOrDefault();
            //    //var submitCards = new List<CardData> { new CardData(6, 1), new CardData(6, 2), new CardData(6, 3), new CardData(5, 1) };
            //    //DOVirtual.DelayedCall(2, () =>
            //    //{
            //    //    Instance_OnSubmitTurn(new TurnData { userId = p.Value.userData.id, cards = submitCards });
            //    //});



            //    //////Test Efx
            //    //IGUIM.SpawnTextEfx("THẮNG", Vector3.zero);
            //    //var playersOnBoard = IGUIM.GetPlayersOnBoard();
            //    //var p = playersOnBoard.OrderBy(x => System.Guid.NewGuid()).FirstOrDefault();
            //    //IGUIM.SpawnTextEfx("THẮNG", p.Value.avatarView.transform.position);
            //    //IGUIM.SpawnTextEfx(Ultility.CoinToString(Random.Range(-1000000,1000000)) + " " + OGUIM.currentMoney.name, p.Value.avatarView.imageAvatar.transform.position);


            //    //Test Show Cards Each Player
            //    //var index = Random.Range(0, 4);
            //    //var cards = new List<CardData> { new CardData(6,1), new CardData(6, 2) , new CardData(6, 3) , new CardData(5, 1),
            //    //                                 new CardData(7,1), new CardData(1, 4) , new CardData(3, 4) , new CardData(9, 2),
            //    //                                 new CardData(12,4)};
            //    //var cards1 = new List<CardData> { new CardData(1,3), new CardData(2, 3) , new CardData(3, 3) , new CardData(4, 3),
            //    //                                 new CardData(5,3), new CardData(6, 3) , new CardData(7, 3) , new CardData(8, 3),
            //    //                                 new CardData(9,3), new CardData(10, 3) , new CardData(11, 3) , new CardData(12, 3), new CardData(13, 3),};
            //    //var cards2 = new List<CardData> { new CardData(1,4), new CardData(2, 4) , new CardData(3, 4) , new CardData(4, 4),
            //    //                                 new CardData(5,4), new CardData(6, 4) , new CardData(7, 4) , new CardData(8, 4),
            //    //                                 new CardData(9,4), new CardData(10, 4) , new CardData(11, 4) , new CardData(12, 4), new CardData(13, 4),};
            //    //ShowCardsEachPlayer(1, cards);
            //    //ShowCardsEachPlayer(2, cards1);
            //    //ShowCardsEachPlayer(3, cards2);


            //    //DOVirtual.DelayedCall(2, () => Instance_OnKickUser(GameUIManager.me.id));
            //    //GameUIController.CreateCoinBurstEfx(index, 0);
            //    //DOVirtual.DelayedCall(0.7f, () => GameUIController.CreateCoinRainEfx(index));
            //    //DOVirtual.DelayedCall(1.4f, () => GameUIController.CreateCoinRainEfx(index));
            //    //Instance_OnStartMatch(new TurnData
            //    //{
            //    //    userId = 0
            //    //});
            //    //            StartMatch(cards);
            //    //foreach (var p in GameUIController.Instance.players)
            //    //{
            //    //    p.SetChipChange(-1000000, GameUIManager.me, 1);
            //    //    p.SetStatus("aaaaaaaaaaaaaaaaa");
            //    //}
            //    //currentUserUI.SetChipChange(100000, GameUIManager.me, 1);
            //    //currentUserUI.SetEffect("efx_thang", true);


            //    //Test set turn
            //    //var playersOnBoard = IGUIM.GetPlayersOnBoard();
            //    //var p = playersOnBoard.OrderBy(x => System.Guid.NewGuid()).FirstOrDefault();
            //    //Instance_OnSetTurn(new TurnData { userId = p.Value.userData.id });

            //    //Test submit turn
            //    //var cards = new List<CardData> { new CardData(6,1), new CardData(6, 2) , new CardData(6, 3)};
            //    //var playersOnBoard = IGUIM.GetPlayersOnBoard();
            //    //var p = playersOnBoard.OrderBy(x => System.Guid.NewGuid()).FirstOrDefault();
            //    //Instance_OnSubmitTurn(new TurnData {
            //    //    userId = p.Value.userData.id,
            //    //    remainCardCount = 4,
            //    //    cards = cards
            //    //});
            //}
#endif

            if (Input.GetMouseButtonDown(0))
        {
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(point, Vector2.zero, 1000);
            if (hits.Any())
            {
                var colliders = hits.Where(x => x.collider != null).Select(x => x.collider).OrderBy(c => c.gameObject.transform.position.x).ToArray();
                if (colliders.Any(x => x.gameObject.tag == "button"))
                    return;
                var col = colliders.LastOrDefault();
                if (col.gameObject.tag == "card")
                {
                    var cardData = col.gameObject.GetComponent<Card>().cardData;
                    var cardStr = cardData.ToString();
                    if (cardsOnHand.ContainsKey(cardStr))
                    {
                        if (cardsOnHand[cardStr].isSelected)
                        {
                            var suite = cardData.suite;
                            foreach (var cd in cardsOnHand.Select(x => x.Value.cardData).Where(xx => xx.suite == suite))
                            {
                                cd.suite = 0;
                            }
                        }
                        cardsOnHand[cardStr].SetSelected(!cardsOnHand[cardStr].isSelected);
                    }
                }
            }
        }
    }
    public override void AddListener()
    {
        WarpClient.wc.OnReconnected += Instance_OnReconnected;
        WarpClient.wc.OnJoinRoom += Instance_OnJoinRoom;
        WarpClient.wc.OnLeaveRoom += Instance_OnLeaveRoom;
        WarpClient.wc.OnReady += Instance_OnReady;
        WarpClient.wc.OnUnready += Instance_OnUnready;
        WarpClient.wc.OnKickUser += Instance_OnKickUser;
        WarpClient.wc.OnAutoKick += Instance_OnAutoKick;
        WarpClient.wc.OnGetRoomInfo += Instance_OnGetRoomInfo;
        WarpClient.wc.OnPassOwner += Instance_OnPassOwner;
        WarpClient.wc.OnStartMatch += Instance_OnStartMatch;
        WarpClient.wc.OnEndMatch += Instance_OnEndMatch;
        WarpClient.wc.OnSetTurn += Instance_OnSetTurn;
        WarpClient.wc.OnPassTurn += Instance_OnPassTurn;
        WarpClient.wc.OnSubmitTurn += Instance_OnSubmitTurn;
        WarpClient.wc.OnWhiteWin += Instance_OnWhiteWin;
        WarpClient.wc.OnChipChanged += Instance_OnChipChanged;
        WarpClient.wc.OnSubmitFailed += _wc_OnSubmitFailed;
        WarpClient.wc.OnRoomStateChanged += Wc_OnRoomStateChanged;

        UIManager.instance.OnSizeChanged += Instance_OnSizeChanged;


        try
        {
            var objs = GameObject.FindGameObjectsWithTag("card");
            if (objs.Any())
            {
                foreach (var o in objs)
                    Destroy(o);
            }
        }
        catch (System.Exception ex)
        {
            UILogView.Log("CardGameManager/ClearCards: " + ex.Message);
        }
    }

    public override void RemoveListener()
    {
        WarpClient.wc.OnReconnected -= Instance_OnReconnected;
        WarpClient.wc.OnLeaveRoom -= Instance_OnLeaveRoom;
        WarpClient.wc.OnReady -= Instance_OnReady;
        WarpClient.wc.OnUnready -= Instance_OnUnready;
        WarpClient.wc.OnKickUser -= Instance_OnKickUser;
        WarpClient.wc.OnAutoKick -= Instance_OnAutoKick;
        WarpClient.wc.OnGetRoomInfo -= Instance_OnGetRoomInfo;
        WarpClient.wc.OnStartMatch -= Instance_OnStartMatch;
        WarpClient.wc.OnEndMatch -= Instance_OnEndMatch;
        WarpClient.wc.OnSetTurn -= Instance_OnSetTurn;
        WarpClient.wc.OnPassTurn -= Instance_OnPassTurn;
        WarpClient.wc.OnSubmitTurn -= Instance_OnSubmitTurn;
        WarpClient.wc.OnWhiteWin -= Instance_OnWhiteWin;
        WarpClient.wc.OnChipChanged -= Instance_OnChipChanged;
        WarpClient.wc.OnPassOwner -= Instance_OnPassOwner;
        WarpClient.wc.OnSubmitFailed -= _wc_OnSubmitFailed;
        WarpClient.wc.OnRoomStateChanged -= Wc_OnRoomStateChanged;

        UIManager.instance.OnSizeChanged -= Instance_OnSizeChanged;
        ClearCards();
    }

    public virtual void _wc_OnSubmitFailed()
    {
        ReorderCardOnHand();
        foreach (var key in cardsOnHand.Keys)
            cardsOnHand[key].SetSelected(false);
    }
    public virtual void Instance_OnSizeChanged()
    {
    }

    public override void OnUnloadScene()
    {
        OGUIM.me.owner = false;
        OGUIM.currentRoom.room.id = 0;
        IGUIM.SetRoomData(null);
        RemoveListener();
    }
    public virtual void StartMatch(List<CardData> cards, bool skipAnimate = false)
    {
        ClearCards();
        float time = 0.3f;
        float delay = 0.1f;
        int i = 0;
        Vector2 sizeOfCards = CalculateSizeOfCards(cards.Count, distanceCards, Vector3.down, cardOnHandScale);
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        var offset = this is LIENG_GameManager ? Vector2.up : Vector2.zero;
        var currentPos = (Vector2)IGUIM.instance.cardOnHandTransform.position + offset;
        foreach (var cardData in cards)
        {
            var go = IGUIM.CreateCard(cardPrefab, cardData, Vector3.zero);
            var card = go.GetComponent<Card>();
            go.transform.SetScale(cardOnBoardScale);
            if (!skipAnimate)
			{
                foreach (var key in playersOnBoard.Keys)
                {
                    if (key != OGUIM.me.id && playersOnBoard.ContainsKey(key)) // Dont need check isPlayer cuz everyone must be player if in gamestart && playersOnBoard[key].userData.isPlayer)
                    {
                        var player = playersOnBoard[key];
                        int remainCard = i + 1;
                        var go1 = IGUIM.CreateCard(cardPrefab, cardData, Vector3.zero);
                        var card1 = go1.GetComponent<Card>();
                        go1.transform.SetScale(cardOnBoardScale);
                        //if (remainCard % 2 == 1)
                        //    go1.SetActive(false);
                        //else
                        {
                            card1.DoAnimate(time, delay * i, player.cardView.transform.position, 360, 0.5f).OnComplete(() =>
                            {
                                go1.Recycle();
                            });
									
							if (this is TLMN_GameManager || this is SAM_GameManager)
								player.SetRemainCard(remainCard);
                        }
                    }
                    else
                    {
                        float x = i * distanceCards - (sizeOfCards.x * 0.5f - 0.5f);
                        float y = currentPos.y;
                        var tween = card.DoAnimate(time, delay * i, new Vector3(x, y, 0), 360, cardOnHandScale).SetDelay(delay * i).OnComplete(() =>
                        {
                            card.SetFlip(true);
                            card.canTouch = true;
                        });
                        if (i == cards.Count - 1)
                            tween.OnComplete(() => {
                                card.SetFlip(true);
                                card.canTouch = true;
                                DOVirtual.DelayedCall(0.5f, () => ReorderCardOnHand());
                            });
                    }
                }
            }
            else
            {
                go.transform.position = currentPos;
                go.transform.localScale = cardOnHandScale * Vector3.one;
                card.SetFlip(true, 10);
                card.canTouch = true;
                DOVirtual.DelayedCall(1, ReorderCardOnHand);
            }
            card.SetCoord(i, 0);
            card.SetSortingOrder(i);
            cardsOnHand.Add(cardData.ToString(), card);
            i++;
        }
    }
    public virtual void ReorderCardOnHand()
    {
        float time = 0.3f;
        int i = 0;
        Vector2 sizeOfCards = CalculateSizeOfCards(cardsOnHand.Count, distanceCards, Vector3.down, cardOnHandScale);

        var tempCards = cardsOnHand.Values.Select(x => x.cardData).OrderBy(x =>
        {
            var card = x.card % 13 <= 2 ? (x.card % 13 + 13) : x.card;
            return (card - 1) * 4 + x.face - 1;
        }).ToList();

        var offset = this is LIENG_GameManager ? Vector2.up : Vector2.zero;
        var currentPos = (Vector2)IGUIM.instance.cardOnHandTransform.position + offset;
        foreach (var card in tempCards)
        {
            var id = card.ToString();
            if (cardsOnHand.ContainsKey(id))
            {
                float x = i * distanceCards - (sizeOfCards.x * 0.5f - 0.5f);
                float y = currentPos.y;
                cardsOnHand[id].gameObject.transform.localScale = cardOnHandScale * Vector3.one;
                cardsOnHand[id].canTouch = true;
                cardsOnHand[id].SetFlip(true);
                cardsOnHand[id].SetSortingOrder(i);
                cardsOnHand[id].DoAnimate(time, 0, new Vector3(x, y, 0), 0, cardOnHandScale);
                cardsOnHand[id].SetCoord(i, 0);
            }
            i++;
        }
    }
    public void ShowCardsEachPlayer(int id, List<CardData> cards, bool cardDisable = false)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (!playersOnBoard.ContainsKey(id) || id == OGUIM.me.id)
        {
            Debug.Log("----------------ShowCardsEachPlayer: Player have been leave room.");
            return;
        }
        float time = 0.05f;
        float delay = 0.03f;
        Vector3 size = Vector3.one * cardOnBoardScale;
        int i = 0;
        var player = playersOnBoard[id];
        player.SetRemainCard(0);



        var yOffset = Mathf.Clamp(Mathf.FloorToInt(cards.Count / (cardsEachRow + 0.1f)) * 0.5f, 0, 0.9f);
        foreach (var cardData in cards)
        {
            var go = IGUIM.CreateCard(cardPrefab, cardData, player.cardView.transform.position);
            go.transform.localScale = size;
            var card = go.GetComponent<Card>();
            float x = player.showCardDirection.x * (i % cardsEachRow) * 0.4f + player.cardView.transform.position.x;
            float y = player.showCardDirection.y * Mathf.FloorToInt(i / cardsEachRow) * 0.6f + player.cardView.transform.position.y - yOffset;
            card.DoAnimate(time, 0, new Vector3(x, y, 0), 0, cardOnBoardScale).SetDelay(delay * i);
            card.SetFlip(true, 20);
            card.SetCoord(i, 0);
            card.SetSortingOrder((i % cardsEachRow) * (int)player.showCardDirection.x - (i / cardsEachRow) * cardsEachRow * (int)player.showCardDirection.y + 150);

            AddCardToBoard(card);
            i++;

            if (cardDisable)
                card.SetDisable();
        }
    }
    public void ClearCards()
    {
        try
        {
            List<Card> removedObj = new List<Card>();
            foreach (var c in cardsOnBoard.Keys)
            {
                removedObj.Add(cardsOnBoard[c]);
            }
            foreach (var key in cardsOnHand.Keys)
                removedObj.Add(cardsOnHand[key]);

            for (int i = 0; i < removedObj.Count; i++)
            {
                if (removedObj[i] != null && removedObj[i].gameObject != null)
                {
                    removedObj[i].transform.DOKill();
                    removedObj[i].gameObject.Recycle();
            }
            }

            cardPrefab.RecycleAll();
        }
        catch (System.Exception ex)
        {
            UILogView.Log("CardGameManager/ClearCards/ removeObj: " + ex.Message + " " + ex.StackTrace);
        }

        cardsOnHand.Clear();
        cardsOnBoard.Clear();
        try
        {
            var objs = GameObject.FindGameObjectsWithTag("card");
            if (objs.Any())
            {
                for (int i = 0; i < objs.Length; i++)
                    objs[i].Recycle();
            }
        }
        catch (System.Exception ex)
        {
            UILogView.Log("CardGameManager/ClearCards: " + ex.Message);
        }
    }
    public List<CardData> GetSubmitCards()
    {
        return cardsOnHand.Values.Where(x => x.isSelected).Select(x => x.cardData).ToList();
    }
    public void AddCardToBoard(Card card)
    {
        var cardKey = card.cardData.ToString();
        if (!cardsOnBoard.ContainsKey(cardKey))
            cardsOnBoard.Add(cardKey, card);
        else
        {
            var oldCard = cardsOnBoard[cardKey];
            cardsOnBoard.Add(cardKey + Time.time, oldCard);
            cardsOnBoard[cardKey] = card;
        }
    }

    #region notification from server



    private void Wc_OnRoomStateChanged(byte[] payloads, WarpContentTypeCode payloadType)
    {
        var data = ZenMessagePack.DeserializeObject<Room>(payloads, WarpContentTypeCode.MESSAGE_PACK);
        if (data != null && data.room != null)
            IGUIM.UpdateRoomState((CardRoomState)data.room.state, data.room.timeCountDown);
    }
    private void Instance_OnReconnected(int resultCode)
    {
        if (resultCode == (int)WarpResponseResultCode.INVALID_SESSION)
        {
            UILogView.Log("CardGameManager  Instance_OnReconnected  resultCode:" + resultCode);

            Debug.LogError("Instance_OnReconnected: fail -> go to login");

            return;
        }
        if (OGUIM.currentRoom != null)
        {
            BuildWarpHelper.GetRoomInfo(OGUIM.currentRoom, () =>
            {
                UILogView.Log("GetRoomInfo is time out.");
            });
        }
        else
        {
            UILogView.Log("Instance_OnReconnected: You have left this room.");
        }
    }
    public void Instance_OnUnready(TurnData data)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (OGUIM.me.id == data.userId)
        {
            OGUIM.me.isReady = false;
            IGUIM.SetButtonsActive(new string[2] { "unready_btn", "ready_btn" },
                                                new bool[2] { false, true });
        }
        if (playersOnBoard.ContainsKey(data.userId))
        {
            playersOnBoard[data.userId].userData.isReady = false;
            playersOnBoard[data.userId].SetReady(false);
        }
    }

    public void Instance_OnReady(TurnData data)
	{
		UIManager.PlaySound ("ready");
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (OGUIM.me.id == data.userId)
        {
            OGUIM.me.isReady = true;
            IGUIM.SetButtonsActive(new string[2] { "unready_btn", "ready_btn" },
                                                new bool[2] { true, false });
        }
        if (playersOnBoard.ContainsKey(data.userId))
        {
            playersOnBoard[data.userId].Reset();
            playersOnBoard[data.userId].SetReady(true);
            playersOnBoard[data.userId].userData.isReady = true;
        }

        if (IGUIM.instance.readyText != null)
            IGUIM.instance.readyText.gameObject.SetActive(false);

    }
    public void Instance_OnJoinRoom(Room data)
	{
		UIManager.PlaySound ("knock");
        if (data != null && data.users != null)
        {
            UILogView.Log("join room - Users count: " + data.users.Count, false);
            IGUIM.SetUsers(data.users);
        }
		if (data != null && data.room != null && !GameBase.isOldVersion)
        {
            IGUIM.UpdateRoomState((CardRoomState)data.room.state, data.room.timeCountDown);
            IGUIM.SetRoomData(data.room);
        }
        var room = IGUIM.GetRoomData();
        if (room != null && !room.started)
        {
            IGUIM.SetAllButtons(false);

            // Nut Unready hien khi user chua ss va ko phai la chu phong
            // Nut Ready hien khi user da ss va khong phai la chu phong
            // Nut Start hien khi la chu phong
            // Nut Invite hien khi co du lieu tra ve, so ng trong phong it hon so ng toi da, ban choi loai tien 368vipEdited 
            IGUIM.SetButtonsActive(new string[] { "unready_btn", "ready_btn", "start_btn", "invite_btn" },
				new bool[] { OGUIM.me.isReady && !OGUIM.me.owner, !OGUIM.me.isReady && !OGUIM.me.owner, OGUIM.me.owner,
                    data != null && data.users != null && data.users.Count < IGUIM.instance.maxPlayerInGame && OGUIM.currentMoney.type == MoneyType.Gold });
        }
    }

    public void Instance_OnKickUser(int userId)
	{
		UIManager.PlaySound ("close");
		//ClearCards ();
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (OGUIM.me.id == userId)
        {
            //DOVirtual.DelayedCall(3f, () =>
            //{
                ClearCards();
                OGUIM.MessengerBox.Show("Thông báo", "Bạn đã bị kích khỏi phòng", "Đồng ý", () =>
                {
                    OGUIM.UnLoadGameScene();
                });
            //});
        }
        else if (playersOnBoard.ContainsKey(userId))
        {
            OGUIM.Toast.ShowNotification(playersOnBoard[userId].userData.displayName + " bị kích khỏi phòng chơi");
            playersOnBoard[userId].FillData(null);
            playersOnBoard.Remove(userId);
            //if (OGUIM.currentLobby.id == (int)LobbyId.XIDACH)
            //{
            //    // ????
            //    OGUIM.me.isReady = false;
            //    // Nut Unready hien khi user chua ss va ko phai la chu phong
            //    // Nut Ready hien khi user da ss va khong phai la chu phong
            //    // Nut Start hien khi la chu phong
            //    // Nut Invite hien khi co du lieu tra ve, so ng trong phong it hon so ng toi da, ban choi loai tien 368vipEdited 
            //    IGUIM.SetButtonsActive(new string[] { "unready_btn", "ready_btn", "start_btn", "invite_btn" },
            //        new bool[] { OGUIM.me.isReady && !OGUIM.me.owner, !OGUIM.me.isReady && !OGUIM.me.owner, OGUIM.me.owner,
            //        playersOnBoard.Count < IGUIM.instance.maxPlayerInGame && OGUIM.currentMoney.type == MoneyType.Gold });
            //}
        }
    }

    private void Instance_OnAutoKick(Room data)
    {

        UILogView.Log("On auto kick User");
        var playerOnboard = IGUIM.GetPlayersOnBoard();
        
        if(OGUIM.me.id == data.userId)
        {
            // show time countdown

            if (!IGUIM.instance.readyText.gameObject.activeInHierarchy)
            {
                IGUIM.instance.readyText.gameObject.SetActive(true);
            }
            DOVirtual.Float(data.countDown, 0, data.countDown, (t) => {

                var time = Mathf.CeilToInt(t);

                if (time > 0)
                {

                    if (OGUIM.me.owner)
                    {
                        IGUIM.instance.readyText.text = "Bạn sẽ bị kích nếu không bắt đầu sau " + time + " giây";
                    }
                    else
                    {
                        IGUIM.instance.readyText.text = "Bạn sẽ bị kích nếu không sẵn sàng sau " + time + " giây";
                    }
                    
                }
                else
                {
                    IGUIM.instance.readyText.text = "";
                }

            });
            
        }
    }

    public void Instance_OnLeaveRoom(Room data, int resultCode)
    {
        if (resultCode == (int)WarpResponseResultCode.SUCCESS)
        {
			UIManager.PlaySound ("close");
            var playersOnBoard = IGUIM.GetPlayersOnBoard();
            if (data.userId == OGUIM.me.id)
            {
                Debug.Log("CardGameManager  Instance_OnLeaveRoom  success -> go to list room");
            }
            else
            {
                var name = playersOnBoard[data.userId].userData.displayName;
                playersOnBoard[data.userId].FillData(null);
                playersOnBoard.Remove(data.userId);
                OGUIM.Toast.ShowNotification(name + " đã thoát khỏi phòng");
            }
        }
    }

    public void Instance_OnGetRoomInfo(int requestType, int resultCode, byte[] payLoad)
    {
        var data = ZenMessagePack.DeserializeObject<GetRoomInfoData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
        if (data == null || data.users == null || !data.users.Any())
        {
            UILogView.Log("Instance_OnGetRoomInfo: data is null!");
            IGUIM.instance.BackBtn_Click();
        }
        else
        {
            IGUIM.SetUsers(data.users);
            if (data.room != null)
            {
                IGUIM.GetRoomData().state = (int)CardRoomState.STOP;
                IGUIM.UpdateRoomState((CardRoomState)data.room.state, data.room.timeCountDown);
                interval = OGUIM.currentRoom.intervalPlay;
                IGUIM.SetRoomData(data.room);
            }
            else
            {
                UILogView.Log("Instance_OnGetRoomInfo: Room is null!");
            }

            var playersOnBoard = IGUIM.GetPlayersOnBoard();
            if (data.room.started)
            {
                StartMatch(data.cards, true);
                foreach (var user in data.users)
                {
                    if (playersOnBoard.ContainsKey(user.id))
                    {
                        if (this is TLMN_GameManager || this is SAM_GameManager)
                            playersOnBoard[user.id].SetRemainCard(user.remainCardCount);
                        playersOnBoard[user.id].SetReady(user.isReady);


                        if (OGUIM.currentLobby.id == (int)LobbyId.MAUBINH)
                            playersOnBoard[user.id].SetTurn(user.isPlayer, interval, 90);
                        else if (OGUIM.currentLobby.id == (int)LobbyId.BACAY || OGUIM.currentLobby.id == (int)LobbyId.BACAY_GA)
                            playersOnBoard[user.id].SetTurn(user.isPlayer, interval, 30);
                            
                        if (user.lastPlayedCards != null && user.lastPlayedCards.Any())
                        {
                            Instance_OnSubmitTurn(new TurnData
                            {
                                userId = user.id,
                                cards = user.lastPlayedCards,
                                remainCardCount = user.remainCardCount
                            });
                        }
                        if (user.playedCards != null && user.playedCards.Any())
                        {
                            Instance_OnSubmitTurn(new TurnData
                            {
                                userId = user.id,
                                cards = user.playedCards,
                                playedCards = user.playedCards
                            });
                        }
                        if (user.acquiredCards != null)
                        {
                            if (playersOnBoard[user.id].userData.acquiredCards != null)
                                playersOnBoard[user.id].userData.acquiredCards.Clear();
                            if (this is PHOM_GameManager)
                            {
                                foreach (var cardData in user.acquiredCards)
                                {
                                    var turnData = new PHOM_TurnData();
                                    turnData.card = cardData;
                                    turnData.userId = user.id;
                                    (this as PHOM_GameManager)._wc_OnUserTakeCard(turnData);
                                }
                            }
                        }
                        if (user.properties != null)
                        {
                            if (this is BACAY_GameManager)
                            {
                                if (user.properties.isShowCard && user.properties.showCards.Any())
                                {
                                    var turnData = new TurnData();
                                    turnData.cards = user.properties.showCards;
                                    turnData.userId = user.id;
                                    Instance_OnSubmitTurn(turnData);
                                }
                                if (!user.owner && OGUIM.currentLobby.id == (int)LobbyId.BACAY)
                                    playersOnBoard[user.id].SetBet(user.properties.user_bet);
                                else
                                    playersOnBoard[user.id].SetBet(0);
                            }
                            else if (this is LIENG_GameManager)
                            {
                                if (user.isPlayer)
                                {
                                    playersOnBoard[user.id].SetBet(user.properties.user_bet);
                                    if (user.properties.user_allin)
                                        IGUIM.SpawnTextEfx("Xả láng", playersOnBoard[user.id].avatarView.imageAvatar.transform.position);
                                }
                            }
                        }
                    }
                }
                if (data.currTurnUser > 0 && OGUIM.currentLobby.id != (int)LobbyId.MAUBINH && OGUIM.currentLobby.id != (int)LobbyId.BACAY && OGUIM.currentLobby.id != (int)LobbyId.BACAY_GA)
                    Instance_OnSetTurn(new TurnData { userId = data.currTurnUser, newTurn = data.isNewTurn });

                IGUIM.SetButtonsActive(new string[] { "ready_btn", "start_btn", "invite_btn" }, new bool[] { false, false, false });
            }
            else
            {
                IGUIM.SetButtonsActive(new string[3] { "ready_btn", "start_btn", "invite_btn" },
                                                    new bool[3] { !OGUIM.me.owner, OGUIM.me.owner,
                                                        data.users != null && data.users.Count < IGUIM.instance.maxPlayerInGame && OGUIM.currentMoney.type == MoneyType.Gold });
            }

            IGUIM.instance.anim.Show();
        }
        
    }

    public virtual void Instance_OnPassOwner(TurnData data)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        var currentUser = playersOnBoard[OGUIM.me.id];
        if (OGUIM.me.owner && data.userId != OGUIM.me.id)
        {
            currentUser.SetReady(false);
            currentUser.SetOwner(false);
            currentUser.userData.isReady = false;
        }
        foreach (var key in playersOnBoard.Keys)
        {
            playersOnBoard[key].userData.owner = data.userId == key;
            if (key == data.userId)
            {
                playersOnBoard[key].userData.isReady = true;
                playersOnBoard[key].SetOwner(true);
                playersOnBoard[key].SetReady(true);
            }
        }

        if (data.userId == OGUIM.me.id)
        {
            OGUIM.me.owner = true;
            IGUIM.SetButtonsActive(new string[] { "unready_btn", "ready_btn", "start_btn" },
                                                new bool[] { false, !OGUIM.me.owner, OGUIM.me.owner });
        }
    }

    public void Instance_OnWhiteWin(TurnData data)
    {
		UIManager.PlaySound ("white_win");
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        IGUIM.SpawnTextEfx("THẮNG TRẮNG", Vector3.zero);
        if (data.userId == OGUIM.me.id)
            IGUIM.CreateCoinRainEfx();
        else
            IGUIM.CreateCoinBurstEfx(data.userId);
		foreach(var id in playersOnBoard.Keys)
		{
			if (id == data.userId) {
				IGUIM.SpawnTextEfx ("THẮNG TRẮNG", playersOnBoard [data.userId].avatarView.imageAvatar.transform.position);
				ShowCardsEachPlayer (data.userId, data.cards);
			} else
				playersOnBoard [id].SetRemainCard (0);
		}
        IGUIM.SetButtonsActive(IGUIM.instance.cardGameUI.buttons.Keys.ToArray(),
                                           IGUIM.instance.cardGameUI.buttons.Keys.Select(x => false).ToArray());

        IGUIM.SetButtonsActive(new string[] { "ready_btn", "start_btn"},
                                            new bool[] { !OGUIM.me.owner, OGUIM.me.owner });
    }

    public void Instance_OnPassTurn(TurnData data)
	{
		UIManager.PlaySound ("pass");
        UILogView.Log("pass turn - userId: " + data.userId, false);
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (OGUIM.me.id == data.userId)
        {
            foreach (var key in cardsOnHand.Keys)
                cardsOnHand[key].SetSelected(false);
        }
        if (playersOnBoard.ContainsKey(data.userId))
        {
            playersOnBoard[data.userId].SetStatus("Bỏ lượt");
        }
    }

    public virtual void Instance_OnSetTurn(TurnData data)
    {
        UILogView.Log("set turn - userId: " + data.userId, false);
        if (data.newTurn)
        {
            var removedCard = new List<Card>();
            foreach (var key in cardsOnBoard.Keys)
                removedCard.Add(cardsOnBoard[key]);
            foreach (var c in removedCard)
            {
                cardsOnBoard.Remove(c.cardData.ToString());
                c.gameObject.Recycle();
            }
            lastSubmitCards.Clear();
        }
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        foreach (var key in playersOnBoard.Keys)
        {
            if (data.newTurn)
                playersOnBoard[key].SetStatus("");

            playersOnBoard[key].SetTurn(data.userId == key, interval);
        }

        IGUIM.SetAllButtons(false);
        IGUIM.SetButtonsActive(new string[2] { "submit_btn", "pass_btn" },
                                            new bool[2] { OGUIM.me.id == data.userId, OGUIM.me.id == data.userId && !data.newTurn });
    }

    public void Instance_OnChipChanged(List<UserData> data)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        var roomData = IGUIM.GetRoomData();
        foreach (var user in data)
        {
            if (playersOnBoard.ContainsKey(user.id))
            {
                if (user.chipChange != 0)
                {
                    var str = Ultility.CoinToString(user.chipChange) + " " + OGUIM.currentMoney.name;
                    IGUIM.SpawnTextEfx(str, playersOnBoard[user.id].avatarView.imageAvatar.transform.position, user.chipChange > 0);
                }
                
                playersOnBoard[user.id].avatarView.FillData(user);
                if (user.chipChange > 0)
                {

					UIManager.PlaySound ("winchip");
                    if (user.id == OGUIM.me.id)
                        IGUIM.CreateCoinRainEfx();
                }
            }
        }
    }

    public virtual void Instance_OnSubmitTurn(TurnData data)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        float time = 0.5f;
        float distanceCard = 0.5f;
        float y = Random.Range(-0.25f, 0.5f);
        float offset = Random.Range(-0.5f, 0.5f);
        Vector2 sizeOfCards = CalculateSizeOfCards(data.cards.Count, distanceCard, Vector3.down, cardOnBoardScale);

        var tempCards = data.cards.OrderBy(x =>
        {
            var card = x.card % 13 <= 2 ? (x.card % 13 + 13) : x.card;
            return (card - 1) * 4 + x.face - 1;
        }).ToList();
        for (var i = 0; i < tempCards.Count; i++)
        {
            CardData cardData = tempCards[i];
            string cardStr = cardData.ToString();
            Card card = null;
            if (cardsOnHand.ContainsKey(cardStr))
            {
                card = cardsOnHand[cardStr];
                cardsOnHand.Remove(cardStr);

                ReorderCardOnHand();
            }
            else if (playersOnBoard.ContainsKey(data.userId))
            {
                var go = IGUIM.CreateCard(cardPrefab, cardData, playersOnBoard[data.userId].Position);
                go.transform.localScale = cardOnHandScale * Vector3.one;
                card = go.GetComponent<Card>();
                if (this is TLMN_GameManager || this is SAM_GameManager)
                    playersOnBoard[data.userId].SetRemainCard(data.remainCardCount);
            }
            if (card != null)
            {
                AddCardToBoard(card);

                float x = i * distanceCard - (sizeOfCards.x * 0.5f - 0.5f) + offset;
                card.SetSortingOrder(cardsOnBoard.Count - 200);
                var pos = new Vector3(x, y, 0);
#if !UNITY_WEBGL
                var rotate = Random.Range(-15, 15);
#else
                var rotate = 0;
#endif
                card.DoAnimate(time, 0, pos, rotate, cardOnBoardScale)
                    .OnStart(() => card.SetFlip(true, 10));
                card.canTouch = false;
            }
        }
    }

    public virtual void Instance_OnEndMatch(byte[] payLoadBytes)
	{
		UIManager.PlaySound ("winchip");
        IGUIM.GetRoomData().started = false;

        if (OGUIM.autoLeaveRoom && IGUIM.instance != null)
        {
            OGUIM.Toast.ShowLoading("");
            DOVirtual.DelayedCall(3.8f, () => IGUIM.instance.BackBtn_Click());
        }
    }

    public virtual void Instance_OnStartMatch(byte[] payLoadBytes)
	{
		UIManager.PlaySound ("deal_13");
        IGUIM.SetButtonsActive(new string[] { "ready_btn", "unready_btn", "start_btn", "invite_btn" },
                                            new bool[] { false, false, false, false });
        IGUIM.GetRoomData().started = true;
        OGUIM.autoLeaveRoom = false;

        var data = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
        if (data != null)
        {
            IGUIM.UpdateRoomState(CardRoomState.PLAY, 3);
            StartMatch(data.cards);
            if (data.user != null && data.user.properties != null)
                OGUIM.me.properties = data.user.properties;
            else if (data.properties != null)
                OGUIM.me.properties = data.properties;
            else
                OGUIM.me.properties = null;
        }
    }


    #endregion

    private void FakeGetRoomInfo()
    {
        var raw = "{ \"cards\" : [], \"lastTurnUser\" : 1267847, \"currTurnUser\" : 1225681, \"room\" : { \"id\" : 36, \"name\" : null, \"type\" : null, \"chipType\" : 0, \"bet\" : 5000, \"maxUsers\" : 0, \"max_player\" : 4, \"curr_num_of_player\" : 4, \"locked\" : false, \"started\" : true, \"quickplay\" : false, \"funds\" : 0 }, \"users\" : [ { \"id\" : 717023, \"avatar\" : \"1\", \"faceBookId\" : null, \"userName\" : \"trangthaiid1511\", \"displayName\" : \"Sát thủ mủ\", \"koin\" : 4434598, \"gold\" : 0, \"allLevel\" : 10, \"level\" : 0, \"chipChange\" : 0, \"order\" : 0, \"seatOrder\" : 0, \"owner\" : false, \"isPlayer\" : false, \"isReady\" : false, \"remainCardCount\" : 0, \"properties\" : {}, \"playedCards\" : [], \"acquiredCards\" : [], \"lastPlayedCards\" : [] }, { \"id\" : 1225681, \"avatar\" : \"1\", \"faceBookId\" : \"1745160169107384\", \"userName\" : \"fb_1745160169107384\", \"displayName\" : \"Phạm Toàn\", \"koin\" : 331238, \"gold\" : 370, \"allLevel\" : 5, \"level\" : 0, \"chipChange\" : 0, \"order\" : 1, \"seatOrder\" : 1, \"owner\" : true, \"isPlayer\" : true, \"isReady\" : true, \"remainCardCount\" : 3, \"properties\" : {}, \"playedCards\" : [], \"acquiredCards\" : [], \"lastPlayedCards\" : [] }, { \"id\" : 3487, \"avatar\" : \"1\", \"faceBookId\" : null, \"userName\" : \"thinhpt\", \"displayName\" : \"thinhpt\", \"koin\" : 17501480, \"gold\" : 0, \"allLevel\" : 7, \"level\" : 0, \"chipChange\" : 0, \"order\" : 2, \"seatOrder\" : 2, \"owner\" : false, \"isPlayer\" : false, \"isReady\" : false, \"remainCardCount\" : 0, \"properties\" : {}, \"playedCards\" : [], \"acquiredCards\" : [], \"lastPlayedCards\" : [] }, { \"id\" : 1267847, \"avatar\" : \"1\", \"faceBookId\" : null, \"userName\" : \"guest_1267847\", \"displayName\" : \"Khách_1267847\", \"koin\" : 2384848, \"gold\" : 9, \"allLevel\" : 5, \"level\" : 0, \"chipChange\" : 0, \"order\" : 3, \"seatOrder\" : 3, \"owner\" : false, \"isPlayer\" : true, \"isReady\" : true, \"remainCardCount\" : 11, \"properties\" : {}, \"playedCards\" : [], \"acquiredCards\" : [], \"lastPlayedCards\" : [ { \"card\" : 5, \"face\" : 3 } ] } ], \"isNewTurn\" : false, \"turnStartTime\" : 1491466611489 }";
        var data = JsonUtility.FromJson<GetRoomInfoData>(raw);

        if (data.users != null && data.users.Any())
        {
            IGUIM.SetUsers(data.users);
        }
        else
        {
            UILogView.Log("Instance_OnGetRoomInfo: Users is null or empty!");
        }
        if (data.room != null)
        {
            interval = OGUIM.currentRoom.intervalPlay;
            IGUIM.SetRoomData(data.room);
        }
        else
        {
            UILogView.Log("Instance_OnGetRoomInfo: Room is null!");
        }

        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (data.room.started)
        {
            StartMatch(data.cards, true);
            foreach (var user in data.users)
            {
                if (playersOnBoard.ContainsKey(user.id))
                {
                    if (this is TLMN_GameManager || this is SAM_GameManager)
                        playersOnBoard[user.id].SetRemainCard(user.remainCardCount);
                    playersOnBoard[user.id].SetReady(user.isReady);


                    if (OGUIM.currentLobby.id == (int)LobbyId.MAUBINH)
                        playersOnBoard[user.id].SetTurn(user.isPlayer, interval, 90);
                    else if (OGUIM.currentLobby.id == (int)LobbyId.BACAY || OGUIM.currentLobby.id == (int)LobbyId.BACAY_GA)
                        playersOnBoard[user.id].SetTurn(user.isPlayer, interval, 30);

                    if (user.lastPlayedCards != null && user.lastPlayedCards.Any())
                    {
                        Instance_OnSubmitTurn(new TurnData
                        {
                            userId = user.id,
                            cards = user.lastPlayedCards,
                            remainCardCount = user.remainCardCount
                        });
                    }
                    if (user.playedCards != null && user.playedCards.Any())
                    {
                        Instance_OnSubmitTurn(new TurnData
                        {
                            userId = user.id,
                            cards = user.playedCards,
                            playedCards = user.playedCards
                        });
                    }
                    if (user.acquiredCards != null)
                    {
                        if (playersOnBoard[user.id].userData.acquiredCards != null)
                            playersOnBoard[user.id].userData.acquiredCards.Clear();
                        if (this is PHOM_GameManager)
                        {
                            foreach (var cardData in user.acquiredCards)
                            {
                                var turnData = new PHOM_TurnData();
                                turnData.card = cardData;
                                turnData.userId = user.id;
                                (this as PHOM_GameManager)._wc_OnUserTakeCard(turnData);
                            }
                        }
                    }
                    if (user.properties != null)
                    {
                        if (this is BACAY_GameManager)
                        {
                            if (user.properties.isShowCard && user.properties.showCards.Any())
                            {
                                var turnData = new TurnData();
                                turnData.cards = user.properties.showCards;
                                turnData.userId = user.id;
                                Instance_OnSubmitTurn(turnData);
                            }
                            if (!user.owner && OGUIM.currentLobby.id == (int)LobbyId.BACAY)
                                playersOnBoard[user.id].SetBet(user.properties.user_bet);
                            else
                                playersOnBoard[user.id].SetBet(0);
                        }
                        else if (this is LIENG_GameManager)
                        {
                            if (user.isPlayer)
                            {
                                playersOnBoard[user.id].SetBet(user.properties.user_bet);
                                if (user.properties.user_allin)
                                    IGUIM.SpawnTextEfx("Xả láng", playersOnBoard[user.id].avatarView.imageAvatar.transform.position);
                            }
                        }
                    }
                }
            }
            if (data.currTurnUser > 0 && OGUIM.currentLobby.id != (int)LobbyId.MAUBINH && OGUIM.currentLobby.id != (int)LobbyId.BACAY && OGUIM.currentLobby.id != (int)LobbyId.BACAY_GA)
                Instance_OnSetTurn(new TurnData { userId = data.currTurnUser, newTurn = data.isNewTurn });

            IGUIM.SetButtonsActive(new string[] { "ready_btn", "start_btn" }, new bool[] { false, false });
        }
        else
        {
            IGUIM.SetButtonsActive(new string[2] { "ready_btn", "start_btn" },
                                                new bool[2] { !OGUIM.me.owner, OGUIM.me.owner });
        }

        IGUIM.instance.anim.Show();
    }
}

