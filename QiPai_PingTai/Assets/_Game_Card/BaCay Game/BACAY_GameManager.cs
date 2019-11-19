using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

public class BACAY_GameManager : CardGameManager
{
    public static void SpawnChipEfx(int toId, int fromId)
    {
        try
        {
            var playersOnBoard = IGUIM.GetPlayersOnBoard();

            GameObject go = IGUIM.SpawnChipEfx();
            try
            {
                var fromPos = playersOnBoard[fromId].avatarView.imageAvatar.transform.position;
                var toPos = playersOnBoard[toId].avatarView.imageAvatar.transform.position;

                go.transform.localScale = Vector3.one * 0.5f;
                go.transform.position = fromPos;
                go.transform.DOMove(toPos, 1).SetDelay(0.5f).OnComplete(() => go.Recycle());
            }
            catch(System.Exception ex)
            {
                UILogView.Log(ex.Message + " " + ex.StackTrace);
                go.Recycle();
            }
        }
        catch(System.Exception ex)
        {
            UILogView.Log("BACAY_GameManager/SpawnChipEfx throw ex: " + ex.Message + " " + ex.StackTrace);
        }
    }
    Vector2 mouseBegin = Vector2.one * -1000;
    public override void AddListener()
    {
        base.AddListener();
        WarpClient.wc.OnNotifySetBet += _wc_OnNotifySetBet;
        WarpClient.wc.OnNotifyChickenBet += _wc_OnNotifyChickenBet;
    }

    public override void RemoveListener()
    {
        base.RemoveListener();
        WarpClient.wc.OnNotifySetBet -= _wc_OnNotifySetBet;
        WarpClient.wc.OnNotifyChickenBet -= _wc_OnNotifyChickenBet;
    }
    
    private void _wc_OnNotifyChickenBet(CasinoTurnData turn)
    {
        IGUIM.SetRoomData(turn.room);
    }

    private void _wc_OnNotifySetBet(CasinoTurnData turn)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (playersOnBoard.ContainsKey(turn.userId))
        {
            var str = "Đặt cửa " + turn.bet + " " + (OGUIM.currentMoney.name);
            IGUIM.SpawnTextEfx(str, playersOnBoard[turn.userId].avatarView.imageAvatar.transform.position, true);

            if (!playersOnBoard[turn.userId].userData.owner && OGUIM.currentLobby.id == (int)LobbyId.BACAY)
                playersOnBoard[turn.userId].SetBet(turn.bet);
            else
                playersOnBoard[turn.userId].SetBet(0);
        }

        if(turn.userId == OGUIM.me.id)
        {
            IGUIM.instance.casinoGameUI.sliderBet.SetValue(turn.bet / OGUIM.currentRoom.room.bet);
            IGUIM.instance.casinoGameUI.currentBet = turn.bet;
        }
    }
    
    public override void Update()
    {
        //base.Update();
        //if (Input.GetButtonDown("Jump"))
        //{
        //    FakeStart();

        //    //var index = Random.Range(0, 8);
        //    //GameUIController.CreateCoinBurstEfx(0, index);
        //}
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
                    var card = col.gameObject.GetComponent<Card>();
                    card.SetFlip(true);
                }
            }
        }
    }
    public override void Instance_OnPassOwner(TurnData data)
    {
        base.Instance_OnPassOwner(data);

        IGUIM.instance.casinoGameUI.sliderBet.gameObject.SetActive(!OGUIM.me.owner);
    }
    public override void Instance_OnStartMatch(byte[] payLoadBytes)
    {
        base.Instance_OnStartMatch(payLoadBytes);
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        foreach (var key in playersOnBoard.Keys)
        {
            playersOnBoard[key].Reset();
            if (playersOnBoard[key].userData.isPlayer)
            {
                playersOnBoard[key].SetTurn(true, interval, 30);
                if (key == OGUIM.me.id)
                {
                    IGUIM.SetAllButtons(false, "BACAY_submit_btn");
                    IGUIM.instance.casinoGameUI.sliderBet.gameObject.SetActive(false);
                }
            }
        }
    }
    public override void Instance_OnSubmitTurn(TurnData data)
    {
		UIManager.PlaySound ("flip");
        if (OGUIM.me.id == data.userId)
        {
            IGUIM.SetButtonActive("BACAY_submit_btn", false);
        }

        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        float time = 0.5f;
        float distanceCard = 0.4f;
        Vector2 sizeOfCards = CalculateSizeOfCards(data.cards.Count, distanceCard, Vector3.down, cardOnBoardScale);

        var tempCards = data.cards.OrderBy(c => c.card % 13 == 1 ? 14 : c.card).ToList();
        var sumOfCard = tempCards.Sum(c => c.card) % 10;
        var valueOnCard = CardLogic.isSamCo(tempCards.ToArray()) ? "SÁP" : sumOfCard == 0 ? "10" : sumOfCard.ToString();
        float x = 0;
        float y = 0;
        var cardPos = IGUIM.instance.cardOnHandTransform.position + Vector3.up * 1.2f;
        var currentPlayer = IGUIM.instance.currentUser;

        for (var i = 0; i < tempCards.Count; i++)
        {
            CardData cardData = tempCards[i];
            Card card = null;
            if (OGUIM.me.id == data.userId)
            {
                string cardStr = cardData.ToString();
                if (cardsOnHand.ContainsKey(cardStr))
                {
                    card = cardsOnHand[cardStr];
                    cardsOnHand.Remove(cardStr);

                    card.SetSortingOrder(i - 100);

                    x = i * distanceCard - (sizeOfCards.x * 0.5f - 0.5f);
                    y = cardPos.y;
                    currentPlayer.SetTurn(false);
                }
                ReorderCardOnHand();
            }
            else if (playersOnBoard.ContainsKey(data.userId))
            {
                var player = playersOnBoard[data.userId];
                player.SetRemainCard(0);
                player.SetTurn(false);
                var go = IGUIM.CreateCard(cardPrefab, cardData, playersOnBoard[data.userId].Position);
                go.transform.localScale = cardOnHandScale * Vector3.one;
                card = go.GetComponent<Card>();
                x = player.showCardDirection.x * ((i - 1) * distanceCard + 0.3f) + player.cardView.transform.position.x;
                y = player.cardView.transform.position.y;
                card.SetSortingOrder(i * (int)player.showCardDirection.x - 100 - 999);
            }
            if (card != null)
            {
                //card.SetSortingOrder(cardsOnBoard.Count, boardLayerName);
                card.SetFlip(true, 20);
                card.canTouch = false;


                var xx = x;
                var yy = y - 0.3f;
                TweenCallback action = () => {
                    var valueTxt = Instantiate(IGUIM.instance.casinoGameUI.valueCardTxt) as Text;
                    valueTxt.gameObject.transform.SetParent(IGUIM.instance.transform, false);
                    valueTxt.gameObject.transform.position = new Vector3(xx, yy, 0);
                    valueTxt.text = valueOnCard + "";
                };

                var pos = new Vector3(x, y, 0);
#if !UNITY_WEBGL
                var rotate = Random.Range(-15, 15);
#else
                var rotate = 0;
#endif
                var tween = card.DoAnimate(time, 0, pos, rotate, cardOnBoardScale);

                if (i == 1)
                    tween.OnComplete(action);

                AddCardToBoard(card);
            }
        }
    }
    public override void Instance_OnEndMatch(byte[] payLoadBytes)
    {
        base.Instance_OnEndMatch(payLoadBytes);

        var data = ZenMessagePack.DeserializeObject<CardEndMatchData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
        if (data == null)
        {
            Debug.LogError(this.GetType().Name + " - Instance_OnEndMatch: data is null");
            return;
        }

        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        IGUIM.UpdateRoomState(data.users.Count > 1 ? CardRoomState.WAIT : CardRoomState.STOP, 5);

        var loserUser = new List<int>();
        var winUser = new List<int>();
        var hostId = data.users.Where(u => {
            if (OGUIM.currentLobby.id == (int)LobbyId.BACAY)
                return u.owner;
            else
                return u.chipChange > 0;
        }).Select(u => u.id).FirstOrDefault();

        foreach (var user in data.users)
        {
            if (user.extra != null && user.extra.cards != null)
                ShowCardsEachPlayer(user.id, user.extra.cards);
            if (playersOnBoard.ContainsKey(user.id))
            {
                if (data.specialty == 0 && user.chipChange != 0 && user.isPlayer)
                    IGUIM.SpawnTextEfx(user.chipChange > 0 ? "Thắng" : "Thua", 
                        playersOnBoard[user.id].avatarView.imageAvatar.transform.position, user.chipChange > 0);


                playersOnBoard[user.id].avatarView.FillMoney(user);
                if (data.specialty == 0)
                {
                    if (user.id != hostId)
                    {
                        if (user.chipChange > 0)
                            winUser.Add(user.id);
                        else if (user.chipChange != 0)
                            loserUser.Add(user.id);
                    }
                    
                    if (OGUIM.currentLobby.id == (int)LobbyId.BACAY)
                        playersOnBoard[user.id].userData.isReady = false;
                }
                else if (data.specialty == 1)
                {
                    playersOnBoard[user.id].userData.isReady = false;
                    if (user.chipChange > 0)
                        winUser.Add(user.id);
                    else if (user.chipChange != 0)
                        loserUser.Add(user.id);
                }
                
                playersOnBoard[user.id].userData.isPlayer = true;
                playersOnBoard[user.id].userData.owner = user.owner;
                playersOnBoard[user.id].SetOwner(playersOnBoard[user.id].userData.owner);
                playersOnBoard[user.id].SetReady(playersOnBoard[user.id].userData.owner || playersOnBoard[user.id].userData.isReady);
                playersOnBoard[user.id].SetTurn(false);
                if (user.properties != null)
                {
                    playersOnBoard[user.id].SetBet((!user.owner && OGUIM.currentLobby.id == (int)LobbyId.BACAY) ? user.properties.user_bet : 0);
                    if (user.id == OGUIM.me.id && OGUIM.currentLobby.id == (int)LobbyId.BACAY)
                    {
                        IGUIM.instance.casinoGameUI.sliderBet.gameObject.SetActive(!playersOnBoard[user.id].userData.owner);
                        IGUIM.instance.casinoGameUI.sliderBet.SetValue(user.properties.user_bet / OGUIM.currentRoom.room.bet);   
                        IGUIM.instance.casinoGameUI.currentBet = user.properties.user_bet;
                    }
                }

                if (user.chipChange != 0)
                {
                    var str = Ultility.CoinToString(user.chipChange) + " " + OGUIM.currentMoney.name;
                    IGUIM.SpawnTextEfx(str, playersOnBoard[user.id].avatarView.imageAvatar.transform.position + Vector3.down * 0.75f, user.chipChange > 0);
                }
            }
        }

        if (hostId != 0)
        {
            for (int i = 0; i < loserUser.Count; i++)
            {
                SpawnChipEfx(hostId, loserUser[i]);
            }
            DOVirtual.DelayedCall(1.5f, () =>
            {
                if (playersOnBoard.ContainsKey(hostId) && OGUIM.currentLobby.id == (int)LobbyId.BACAY)
                {
                    for (int i = 0; i < winUser.Count; i++)
                    {
                        SpawnChipEfx(winUser[i], hostId);
                    }
                }
            });
        }

        if(data.specialty == 1)
        {
            var room = IGUIM.GetRoomData();
            room.koinGA = 0;
            IGUIM.instance.UpdateRoom(room);

            var takeChickenUser = data.users.Where(x => x.id > 0 && x.isPlayer && x.order == 0 && x.chipChange > 0).FirstOrDefault();
            if (playersOnBoard.ContainsKey(takeChickenUser.id))
            {
                IGUIM.SpawnTextEfx("Ăn gà", Vector3.zero);
                IGUIM.SpawnTextEfx("Ăn gà", playersOnBoard[takeChickenUser.id].avatarView.imageAvatar.transform.position);
                if (takeChickenUser.id == OGUIM.me.id)
                {
                    IGUIM.CreateCoinRainEfx();
                }
                else
                {
                    IGUIM.CreateCoinBurstEfx(takeChickenUser.id, 0);
                    DOVirtual.DelayedCall(0.7f, () =>
                    {
                        IGUIM.CreateCoinBurstEfx(takeChickenUser.id, 0);
                    });
                }
            }
        }
        IGUIM.SetButtonActive("BACAY_submit_btn", false);
        if (data.specialty == 1 || OGUIM.currentLobby.id != (int)LobbyId.BACAY_GA)
        {
            IGUIM.SetButtonsActive(new string[] { "unready_btn", "ready_btn", "start_btn", "invite_btn" },
                                                new bool[] { false, !OGUIM.me.owner, OGUIM.me.owner,
                                                    data.users.Count < IGUIM.instance.maxPlayerInGame && OGUIM.currentMoney.type == MoneyType.Gold });

			if (!OGUIM.me.owner && !OGUIM.me.isReady && GameBase.isOldVersion)
			{
				DOVirtual.DelayedCall(2, () =>
					{OGUIM.Toast.Show("Sẵn sàng để chơi ván mới", UIToast.ToastType.Notification, 2);});
			}

        }
    }
    public override void Instance_OnSetTurn(TurnData data)
    {
        //var playersOnBoard = GameUIController.GetPlayersOnBoard();
        //foreach (var key in playersOnBoard.Keys)
        //{
        //    playersOnBoard[key].SetTurn(data.userId == key, interval);
        //}
    }

    public override void StartMatch(List<CardData> cards, bool skipAnimate = false)
    {
        ClearCards();
        float time = 0.3f;
        float delay = 0.1f;
        int i = 0;
        Vector2 sizeOfCards = CalculateSizeOfCards(cards.Count, distanceCards, Vector3.down, cardOnHandScale);
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        var currentPos = IGUIM.instance.cardOnHandTransform.position;
        foreach (var cardData in cards)
        {
            var go = IGUIM.CreateCard(cardPrefab, cardData, Vector3.zero);
            var card = go.GetComponent<Card>();
            if (!skipAnimate)
            {
                foreach (var key in playersOnBoard.Keys)
                {
                    if (key != OGUIM.me.id && playersOnBoard.ContainsKey(key) && playersOnBoard[key].userData.isPlayer)
                    {
                        var player = playersOnBoard[key];
                        int remainCard = i + 1;
                        var go1 = IGUIM.CreateCard(cardPrefab, cardData, Vector3.zero);
                        go1.transform.DOMove(player.cardView.transform.position, time).SetDelay(delay * i);
                        DOVirtual.Float(0, 360, time, (value) => { go1.transform.rotation = Quaternion.Euler(Vector3.forward * value); }).SetDelay(delay * i);
                        go1.transform.DOScale(Vector3.one * 0.3f, time).SetDelay(delay * i).OnComplete(() =>
                        {
                            go1.Recycle();
                        });
                    }
                    else
                    {
                        float x = i * distanceCards - (sizeOfCards.x * 0.5f - 0.5f);
                        float y = currentPos.y;
                        var index = i;
                        go.transform.DOMove(new Vector3(x, y, 0), time).SetDelay(delay * i).OnComplete(() =>
                        {
                            card.cardData.index = index;
                            card.canTouch = true;
                        });
                        go.transform.DOScale(cardOnHandScale, time).SetDelay(delay * i);
                        go.transform.DORotate(Vector3.forward * 360, time, RotateMode.FastBeyond360).SetDelay(delay * i);
                    }
                }
            }
            else
            {
                go.transform.position = currentPos;
                go.transform.localScale = cardOnHandScale * Vector3.one;
                card.SetFlip(true, 10);
                card.canTouch = true;
            }
            card.SetCoord(i, 0);
            card.SetSortingOrder(i);
            cardsOnHand.Add(cardData.ToString(), card);
            i++;
        }
        if (IGUIM.instance.gameManager.cardsOnHand.Any())
            IGUIM.SetButtonActive("BACAY_submit_btn", true);
    }
    private void FakeStart()
    {
        //Debug.Log("Test");
        //var index = Random.Range(1, 4);
        //GameUIController.CreateCoinRainEfx(index);
        //DOVirtual.DelayedCall(0.7f, () => GameUIController.CreateCoinRainEfx(index));
        //DOVirtual.DelayedCall(1.4f, () => GameUIController.CreateCoinRainEfx(index));
        var cards = new List<CardData> { new CardData(6, 1), new CardData(6, 2), new CardData(6, 3) };
        //var cards = new List<CardData> { new CardData(1,1), new CardData(2, 1) , new CardData(3, 1) , new CardData(4, 1),
        //                                 new CardData(5,1), new CardData(6, 1) , new CardData(7, 1) , new CardData(8, 1),
        //                                 new CardData(9,1), new CardData(10, 1) , new CardData(11, 1) , new CardData(12, 1), new CardData(13, 1),};
        //var cards1 = new List<CardData> { new CardData(1,3), new CardData(2, 3) , new CardData(3, 3) , new CardData(4, 3),
        //                                 new CardData(5,3), new CardData(6, 3) , new CardData(7, 3) , new CardData(8, 3),
        //                                 new CardData(9,3), new CardData(10, 3) , new CardData(11, 3) , new CardData(12, 3), new CardData(13, 3),};
        //var cards2 = new List<CardData> { new CardData(1,4), new CardData(2, 4) , new CardData(3, 4) , new CardData(4, 4),
        //                                 new CardData(5,4), new CardData(6, 4) , new CardData(7, 4) , new CardData(8, 4),
        //                                 new CardData(9,4), new CardData(10, 4) , new CardData(11, 4) , new CardData(12, 4), new CardData(13, 4),};
        StartMatch(cards);
        //DOVirtual.DelayedCall(1, () => (this as PHOM_GameManager).SuggestSuite());
        //ShowCardsEachPlayer(1, cards);
        //ShowCardsEachPlayer(2, cards1);
        //ShowCardsEachPlayer(3, cards2);
        //Instance_OnStartMatch(new TurnData
        //{
        //    userId = 0
        //});
        //            StartMatch(cards);
        //foreach (var p in GameUIController.Instance.players)
        //{
        //    p.SetChipChange(-1000000, GameUIManager.me, 1);
        //    p.SetStatus("aaaaaaaaaaaaaaaaa");
        //}
        //currentUserUI.SetChipChange(100000, GameUIManager.me, 1);
        //currentUserUI.SetEffect("efx_thang", true);
    }
}
