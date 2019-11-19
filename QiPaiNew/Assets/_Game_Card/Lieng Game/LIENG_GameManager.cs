using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;

public class LIENG_GameManager : CardGameManager {
    List<int> foldUserIds = new List<int>();
    public override void AddListener()
    {
        WarpClient.wc.OnUserCall += _wc_OnUserCall;
        WarpClient.wc.OnUserFold += _wc_OnUserFold;
        WarpClient.wc.OnUserRaise += _wc_OnUserRaise;
        WarpClient.wc.OnCalculatePot += _wc_OnCalculatePot;
        WarpClient.wc.OnNotifyChickenBet += _wc_OnNotifyChickenBet;
        base.AddListener();
    }

    public override void RemoveListener()
    {
        WarpClient.wc.OnUserCall -= _wc_OnUserCall;
        WarpClient.wc.OnUserFold -= _wc_OnUserFold;
        WarpClient.wc.OnUserRaise -= _wc_OnUserRaise;
        WarpClient.wc.OnCalculatePot -= _wc_OnCalculatePot;
        WarpClient.wc.OnNotifyChickenBet -= _wc_OnNotifyChickenBet;
        base.RemoveListener();
    }

    public override void Update()
    {
        //base.Update();
        //if (Input.GetButtonDown("Jump"))
        //{
        //    var turn = new TurnData();
        //    turn.userId = OGUIM.me.id;
        //    turn.cards = new System.Collections.Generic.List<CardData>()
        //    {
        //        new CardData(12,1),
        //        new CardData(1,1),
        //        new CardData(2,1),
        //    };
        //    Instance_OnSubmitTurn(turn);
        //    //turn.pots = new System.Collections.Generic.List<LIENGPot> {
        //    //    new LIENGPot { order = 0, takenUserId = OGUIM.me.id, totalKoins = 1000 },
        //    //    new LIENGPot { order = 1, takenUserId = OGUIM.me.id, totalKoins = 10000 },
        //    //    new LIENGPot { order = 2, takenUserId = OGUIM.me.id, totalKoins = 100000 },
        //    //    new LIENGPot { order = 3, takenUserId = OGUIM.me.id, totalKoins = 1000000 },
        //    //    new LIENGPot { order = 4, takenUserId = OGUIM.me.id, totalKoins = 10000000 },
        //    //    new LIENGPot { order = 5, takenUserId = OGUIM.me.id, totalKoins = 10000000 },
        //    //    //new LIENGPot { order = 6, takenUserId = GameUIManager.me.id, totalKoins = 10000000 },
        //    //    //new LIENGPot { order = 7, takenUserId = GameUIManager.me.id, totalKoins = 10000000 },
        //    //};

        //    //_wc_OnCalculatePot(turn);
        //}
    }

    private void _wc_OnNotifyChickenBet(CasinoTurnData turn)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        foreach (var user in turn.users)
        {
            if (user.isPlayer && user.id > 0 && playersOnBoard.ContainsKey(user.id))
            {
                playersOnBoard[user.id].userData.total_chips = user.total_chips;
                playersOnBoard[user.id].userData.properties = user.properties;
                playersOnBoard[user.id].SetBet(user.total_chips);
            }
        }
    }
    private void _wc_OnCalculatePot(TurnData turn)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if(turn == null || turn.pots == null || turn.pots.Count <= 0)
        {
            UILogView.Log("_wc_OnCalculatePot: no pots");
            return;
        }
        int count = turn.pots.Count;
        var widthPerPot = 1.5f;
        float i = -(Mathf.Min(count, 4) - 1) * 0.5f;
        var y = count <= 4 ? 0f : 0.6f;
        foreach (var pot in turn.pots)
        {
            var takenId = pot.takenUserId;
            var totalKoins = pot.totalKoins;
            var x = 0f;
            foreach(var pKey in playersOnBoard.Keys)
            {
                GameObject go = IGUIM.SpawnChipEfx();
                go.transform.SetParent(IGUIM.instance.transform, false);
                go.transform.position = playersOnBoard[pKey].avatarView.imageAvatar.transform.position;
                go.transform.localScale = Vector3.one * 0.5f;

                if (count > 4 && i > 1.5f)
                {
                    i = -(Mathf.Min(count - 4, 4) - 1) * 0.5f;
                    y = -0.8f;
                }
                x = i * widthPerPot;

                go.transform.DOMove(new Vector3(x, y, 0), 0.5f).OnComplete(() =>
                {
                    if (takenId == OGUIM.me.id)
                        go.transform.DOMove(IGUIM.instance.currentUser.avatarView.moneyView.moneyImage.transform.position, 0.5f)
                                    .SetDelay(2).OnComplete(()=> go.Recycle());
                    else
                        go.transform.DOMove(playersOnBoard[takenId].avatarView.imageAvatar.transform.position, 0.5f)
                                    .SetDelay(2).OnComplete(() => go.Recycle());
                });
            }
            var yy = y;
            DOVirtual.DelayedCall(0.5f, () => {
                var valueTxt = Instantiate(IGUIM.instance.casinoGameUI.valueCardTxt) as Text;
                valueTxt.gameObject.transform.SetParent(IGUIM.instance.transform, false);
                valueTxt.gameObject.transform.position = new Vector3(x, yy - 0.5f, 0);
                valueTxt.gameObject.transform.localScale = Vector3.one * 0.5f;
                valueTxt.text = LongConverter.ToK(totalKoins);
                Destroy(valueTxt, 2f);
            });
            i++;
        }
    }

    private void _wc_OnUserRaise(TurnData turn)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (playersOnBoard.ContainsKey(turn.userId))
        {
            playersOnBoard[turn.userId].userData.total_chips = turn.total_chips;
            playersOnBoard[turn.userId].userData.properties = turn.properties;
            playersOnBoard[turn.userId].SetBet(turn.total_chips);

            var alertStr = "";
            if (turn.properties != null && turn.properties.user_allin)
                alertStr = "Xả láng";
            else
            {
                alertStr = "Tố";
                playersOnBoard[turn.userId].SetStatus("Tố thêm " + Ultility.CoinToString(turn.bet));
            }
            IGUIM.SpawnTextEfx(alertStr, playersOnBoard[turn.userId].avatarView.imageAvatar.transform.position);
        }
    }

    private void _wc_OnUserFold(TurnData turn)
    {
        foldUserIds.Add(turn.userId);
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (playersOnBoard.ContainsKey(turn.userId))
        {
            playersOnBoard[turn.userId].SetStatus("Úp bỏ");
            IGUIM.SpawnTextEfx("Úp bỏ", playersOnBoard[turn.userId].avatarView.imageAvatar.transform.position, false);
        }
        if(turn.userId == OGUIM.me.id)
        {
            foreach (var cKey in cardsOnHand.Keys)
                cardsOnHand[cKey].SetDisable();
        }
    }

    private void _wc_OnUserCall(TurnData turn)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (playersOnBoard.ContainsKey(turn.userId))
        {
            playersOnBoard[turn.userId].userData.total_chips = turn.total_chips;
            playersOnBoard[turn.userId].userData.properties = turn.properties;
            playersOnBoard[turn.userId].SetBet(turn.total_chips);

            var alertStr = "";
            if (turn.properties != null && turn.properties.user_allin)
                alertStr = "Xả láng";
            else
                alertStr = "Theo";

            playersOnBoard[turn.userId].SetStatus(alertStr);
            IGUIM.SpawnTextEfx(alertStr, playersOnBoard[turn.userId].avatarView.imageAvatar.transform.position);
        }
    }

    public override void Instance_OnSubmitTurn(TurnData data)
    {
        if (OGUIM.me.id == data.userId)
            IGUIM.SetButtonActive("BACAY_submit_btn", false);

        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        float time = 0.5f;
        float distanceCard = 0.4f;
        Vector2 sizeOfCards = CalculateSizeOfCards(data.cards.Count, distanceCard, Vector3.down, cardOnBoardScale);

        var tempCards = data.cards.OrderBy(c => c.card >= 10 ? 0 : c.card).ToList();
        var sumOfCard = tempCards.Sum(c => Mathf.Min(c.card, 10)) % 10;
        var valueOnCard = CardLogic.IsSanh(tempCards.ToArray()) ? "LIÊNG" : CardLogic.isSamCo(tempCards.ToArray()) ? "SÁP" : !tempCards.Any(c => c.card <= 10) ? "ẢNH" : sumOfCard.ToString();
        float x = 0;
        float y = 0;
        var cardPos = IGUIM.instance.cardOnHandTransform.position;
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

                    card.SetSortingOrder(cardsOnBoard.Count + 100);

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
                card.SetSortingOrder((i % 5) * (int)player.showCardDirection.x + 150);
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
                    if (IGUIM.instance == null || IGUIM.instance.transform == null)
                    {
                        Destroy(valueTxt);
                        return;
                    }
                    if (foldUserIds.Contains(data.userId))
                        valueOnCard = "ÚP BỎ";
                    valueTxt.gameObject.transform.SetParent(IGUIM.instance.transform, false);
                    valueTxt.gameObject.transform.position = new Vector3(xx, yy, 0);
                    valueTxt.text = valueOnCard + "";
                };

                var pos = new Vector3(x, y, 0);
#if !UNITY_WEBGL
                var rotate = Random.Range(-10, 10);
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
    public override void Instance_OnSetTurn(TurnData data)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        foreach (var key in playersOnBoard.Keys)
        {
            playersOnBoard[key].SetTurn(data.userId == key, interval);
        }

        IGUIM.SetAllButtons(false);
        IGUIM.SetButtonsActive(new string[] { "LIENG_call_btn", "LIENG_raise_btn", "LIENG_fold_btn" },
                                            new bool[] { data.userId == OGUIM.me.id, data.userId == OGUIM.me.id, data.userId == OGUIM.me.id && !data.newTurn });
        IGUIM.instance.casinoGameUI.sliderBet.gameObject.SetActive(data.userId == OGUIM.me.id);
    }
    public override void Instance_OnStartMatch(byte[] payLoadBytes)
    {
        base.Instance_OnStartMatch(payLoadBytes);
        foldUserIds.Clear();
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        foreach (var key in playersOnBoard.Keys)
        {
            playersOnBoard[key].Reset();
            if (playersOnBoard[key].userData.isPlayer && playersOnBoard[key].userData.isReady)
            {
                playersOnBoard[key].SetTurn(true, interval, 30);
                if (key == OGUIM.me.id)
                {
                    IGUIM.SetAllButtons(false);
                    IGUIM.instance.casinoGameUI.sliderBet.gameObject.SetActive(false);
                }
            }
        }
    }
    public override void Instance_OnEndMatch(byte[] payLoadBytes)
    {
        base.Instance_OnEndMatch(payLoadBytes);

        var data = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
        if (data == null)
        {
            Debug.LogError(this.GetType().Name + " - Instance_OnEndMatch: data is null");
            return;
        }

        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        IGUIM.UpdateRoomState(data.users.Count > 1 ? CardRoomState.WAIT : CardRoomState.STOP, 5);

        foreach (var user in data.users)
        {
            if (user.extra != null && user.extra.cards != null && user.extra.cards.Any() && user.id != OGUIM.me.id)
                Instance_OnSubmitTurn(new TurnData { userId = user.id, cards = user.extra.cards });
            if (playersOnBoard.ContainsKey(user.id))
            {
				user.isPlayer = true;
				if (GameBase.isOldVersion) 
				{
					user.isReady = false;
					playersOnBoard [user.id].SetReady (user.isReady);
				}

                playersOnBoard[user.id].SetTurn(false);
                playersOnBoard[user.id].Reset();
                playersOnBoard[user.id].FillData(user);

                if (data.specialty == 0 && user.chipChange != 0 && user.isPlayer)
                {
                    var str = Ultility.CoinToString(user.chipChange) + " " + OGUIM.currentMoney.name;
                    IGUIM.SpawnTextEfx(str, playersOnBoard[user.id].avatarView.imageAvatar.transform.position, user.chipChange > 0);
                }
            }
        }

        IGUIM.SetAllButtons(false);
		IGUIM.SetButtonsActive(new string[] {"unready_btn", "ready_btn", "start_btn", "invite_btn" },
								new bool[] {false, !OGUIM.me.owner, OGUIM.me.owner,
                                                data.users.Count < IGUIM.instance.maxPlayerInGame && OGUIM.currentMoney.type == MoneyType.Gold });

		if (!OGUIM.me.owner && !OGUIM.me.isReady && GameBase.isOldVersion)
		{
			DOVirtual.DelayedCall(2, () =>
				{OGUIM.Toast.Show("Sẵn sàng để chơi ván mới", UIToast.ToastType.Notification, 2);});
		}
    }

    public override void ReorderCardOnHand()
    {
        base.ReorderCardOnHand();
        DOVirtual.DelayedCall(1, () => {

            if (cardsOnHand.Any())
            {
                var y = IGUIM.instance.cardOnHandTransform.position.y + 0.8f;

                var tempCards = cardsOnHand.Select(x => x.Value.cardData);
                var sumOfCard = tempCards.Sum(c => c.card >= 10 ? 0 : c.card) % 10;
                var valueOnCard = CardLogic.IsSanh(tempCards.ToArray()) ? "LIÊNG" : CardLogic.isSamCo(tempCards.ToArray()) ? "SÁP" : !tempCards.Any(x => x.card <= 10) ? "ẢNH" : sumOfCard.ToString();
                var valueTxt = Instantiate(IGUIM.instance.casinoGameUI.valueCardTxt) as Text;
                valueTxt.gameObject.transform.SetParent(IGUIM.instance.transform, false);
                valueTxt.gameObject.transform.position = new Vector3(0, y, 0);
                if (foldUserIds.Contains(OGUIM.me.id))
                    valueOnCard = "ÚP BỎ";
                valueTxt.text = valueOnCard + "";
            }
        });
    }
}
