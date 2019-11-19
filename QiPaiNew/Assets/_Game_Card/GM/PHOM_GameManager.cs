using UnityEngine;
using System.Collections;
using System.Linq;
using DG.Tweening;
using System.Collections.Generic;

public enum SortingType
{
    ByCard = 0,
    ByFace = 1,
}
public class PHOM_GameManager : CardGameManager
{
    public Dictionary<string, List<CardData>> suitesOnGame;
    public override void AddListener()
    {
        base.AddListener();
        WarpClient.wc.OnUserGetCard += _wc_OnUserGetCard;
        WarpClient.wc.OnUserTakeCard += _wc_OnUserTakeCard;
        WarpClient.wc.OnAttachCardToSuite += _wc_OnAttachCardToSuite;
        WarpClient.wc.OnPHOMUserSubmitSuite += _wc_OnPHOMUserSubmitSuite;
        WarpClient.wc.OnCardsChanged += _wc_OnCardsChanged;
        WarpClient.wc.OnSetSubmitSuite += _wc_OnSetSubmitSuite;
        WarpClient.wc.OnSetKeyTurn += _wc_OnSetKeyTurn;
        WarpClient.wc.OnCancelKeyTurn += _wc_OnCancelKeyTurn;
        WarpClient.wc.OnUK += _wc_OnUK;
        WarpClient.wc.OnShowHand += _wc_OnShowHand;


        cardsEachRow = 3;
        suitesOnGame = new Dictionary<string, List<CardData>>();
    }

    public override void RemoveListener()
    {
        base.RemoveListener();
        WarpClient.wc.OnUserGetCard -= _wc_OnUserGetCard;
        WarpClient.wc.OnUserTakeCard -= _wc_OnUserTakeCard;
        WarpClient.wc.OnAttachCardToSuite -= _wc_OnAttachCardToSuite;
        WarpClient.wc.OnPHOMUserSubmitSuite -= _wc_OnPHOMUserSubmitSuite;
        WarpClient.wc.OnCardsChanged -= _wc_OnCardsChanged;
        WarpClient.wc.OnSetSubmitSuite -= _wc_OnSetSubmitSuite;
        WarpClient.wc.OnSetKeyTurn -= _wc_OnSetKeyTurn;
        WarpClient.wc.OnCancelKeyTurn -= _wc_OnCancelKeyTurn;
        WarpClient.wc.OnUK -= _wc_OnUK;
        WarpClient.wc.OnShowHand -= _wc_OnShowHand;
    }

    public override void Instance_OnSizeChanged()
    {
        DOVirtual.DelayedCall(0.1f, () =>
        {
            ReorderCardOnHandNotChangeSortType();
            ChangeCardsOnBoard();
        });
    }

    private void _wc_OnShowHand(TurnData turn)
    {
        if (OGUIM.me.id == turn.userId)
        {
            UILogView.Log("Show hand");
            SuggestSuite();
            IGUIM.SetAllButtons(false, "PHOM_ha_btn");
        }
        //var playersOnBoard = GameUIController.GetPlayersOnBoard();
        //if (playersOnBoard.ContainsKey(turn.userId))
        //{
        //    GameUIManager.Toast.Show(playersOnBoard[turn.userId].userData.displayName + " show bài trên tay!!!");
        //}
    }

    private void _wc_OnUK(TurnData turn)
    {
        UIManager.PlaySound("white_win");
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (playersOnBoard.ContainsKey(turn.userId))
            IGUIM.SpawnTextEfx("Ù K", playersOnBoard[turn.userId].avatarView.imageAvatar.transform.position);
    }

    private void _wc_OnCancelKeyTurn(TurnData turn)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
    }

    private void _wc_OnSetKeyTurn(TurnData data)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        foreach (var key in playersOnBoard.Keys)
        {
            if (key == data.userId)
            {
                playersOnBoard[key].SetStatus("Chốt");
            }
            //playersOnBoard[key].userData.isHaPhom = key == data.userId;
        }
    }

    private void _wc_OnSetSubmitSuite(TurnData turn)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (playersOnBoard.ContainsKey(turn.userId))
        {
            playersOnBoard[turn.userId].userData.isHaPhom = true;
            playersOnBoard[turn.userId].SetStatus("Hạ phỏm");
        }

        if (turn.userId == OGUIM.me.id)
        {
            IGUIM.SetAllButtons(false, "submit_btn", "PHOM_ha_btn", "PHOM_gui_btn", "PHOM_xep_btn");
            SuggestSuite();
        }
    }

    private void _wc_OnCardsChanged(TurnData turn)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        foreach (var user in turn.users)
        {
            if (playersOnBoard.ContainsKey(user.id))
            {
                playersOnBoard[user.id].userData.playedCards = user.playedCards;
                playersOnBoard[user.id].userData.acquiredCards = user.acquiredCards;
            }
        }
        ChangeCardsOnBoard();
    }

    private void _wc_OnPHOMUserSubmitSuite(PHOM_TurnData turn)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        float time = 0.4f;
        float distanceCard = 0.3f;
        float offset = 1.2f;
        if (playersOnBoard.ContainsKey(turn.userId))
        {
            var player = playersOnBoard[turn.userId];
            player.userData.isHaPhom = turn.suites.Any();

            foreach (var suite in turn.suites)
            {
                AddSuiteOnGame(suite);
                Vector2 sizeOfCards = CalculateSizeOfCards(suite.Count, distanceCard, Vector3.down, cardOnBoardScale * offset);
                for (var i = 0; i < suite.Count; i++)
                {
                    CardData cardData = suite[i];
                    Card card = null;
                    var cardKey = cardData.ToString();
                    if (OGUIM.me.id == turn.userId)
                    {
                        if (cardsOnHand.ContainsKey(cardKey))
                        {
                            card = cardsOnHand[cardKey];
                            cardsOnHand.Remove(cardKey);
                            AddCardToBoard(card);
                        }

                        ReorderCardOnHandNotChangeSortType();

                        IGUIM.SetButtonActive("PHOM_ha_btn", cardsOnHand.Any(c => c.Value.cardData.pair == 1));
                    }
                    else if (playersOnBoard.ContainsKey(turn.userId))
                    {
                        if (cardsOnBoard.ContainsKey(cardKey))
                        {
                            card = cardsOnBoard[cardKey];
                        }
                        else
                        {
                            var go = IGUIM.CreateCard(cardPrefab, cardData, player.Position);
                            go.transform.localScale = cardOnHandScale * Vector3.one;
                            card = go.GetComponent<Card>();
                            AddCardToBoard(card);
                        }
                        player.SetRemainCard(0);
                    }
                    if (card != null)
                    {
                        card.userId = turn.userId;
                        card.SetFlip(true, 20);
                        card.canTouch = false;

                        float x = i * distanceCard - (sizeOfCards.x * 0.5f - distanceCard * 0.5f);
                        card.SetSortingOrder(i - player.suiteCount * 50 + 300);

                        var pos = new Vector3(x, 0, 0) + player.PHOM_suitesCardsTransform[player.suiteCount].position;
                        // Quan bai di chuyen
                        card.DoAnimate(time, 0, pos, 0, cardOnBoardScale * offset);
                    }
                }
                player.suiteCount++;
                player.suiteCount = player.suiteCount % 3;
            }
        }
    }

    private void _wc_OnAttachCardToSuite(PHOM_TurnData data)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        float time = 0.5f;
        float distanceCard = 0.3f;

        var orderOnCanvas = 0;
        var yOffset = -100f;
        var offset = 1.2f;


        var key = AddSuiteOnGame(data.suite.cards, data.cards);
        if (!string.IsNullOrEmpty(key))
        {

            var newSuite = suitesOnGame[key];
            var tempCards = newSuite.OrderBy(x => x.card % 13 == 1 ? 1 : x.card).ToList();
            Vector2 sizeOfCards = CalculateSizeOfCards(tempCards.Count, distanceCard, Vector3.down, cardOnBoardScale);

            var suiteKey = data.suite.cards.Where(x =>
            {
                if (cardsOnBoard.ContainsKey(x.ToString()) && cardsOnBoard[x.ToString()].orderOnCanvas != 0)
                    return true;
                return false;
            }).FirstOrDefault();
            for (var i = 0; i < tempCards.Count; i++)
            {
                CardData cardData = tempCards[i];
                Card card = null;
                string cardStr = cardData.ToString();

                if (cardsOnHand.ContainsKey(cardStr))
                {
                    card = cardsOnHand[cardStr];
                    cardsOnHand.Remove(cardStr);
                    AddCardToBoard(card);
                    ReorderCardOnHandNotChangeSortType();
                }
                else if (cardsOnBoard.ContainsKey(cardStr))
                {
                    card = cardsOnBoard[cardStr];
                }
                else
                {
                    var go = IGUIM.CreateCard(cardPrefab, cardData, playersOnBoard[data.userId].Position);
                    go.transform.localScale = cardOnHandScale * Vector3.one;
                    card = go.GetComponent<Card>();
                    AddCardToBoard(card);
                }


                if (card != null)
                {
                    if (!playersOnBoard.ContainsKey(data.suite.userId))
                    {
                        UILogView.Log("_wc_OnAttachCardToSuite: user được gửi ko tồn tại");
                        Destroy(card.gameObject);
                        cardsOnBoard.Remove(cardStr);
                        return;
                    }
                    if (suiteKey == null)
                    {
                        UILogView.Log("_wc_OnAttachCardToSuite: phom ko ton tai");
                        Destroy(card.gameObject);
                        cardsOnBoard.Remove(cardStr);
                        return;
                    }

                    var player = playersOnBoard[data.suite.userId];
                    orderOnCanvas = cardsOnBoard[suiteKey.ToString()].orderOnCanvas;
                    yOffset = cardsOnBoard[suiteKey.ToString()].gameObject.transform.position.y;

                    card.SetFlip(true, 20);
                    card.canTouch = false;

                    float x = i * distanceCard - (sizeOfCards.x * 0.5f - distanceCard * 0.5f);
                    card.SetSortingOrder(orderOnCanvas + i);

                    var pos = new Vector3(x + player.PHOM_suitesCardsTransform[player.suiteCount % 3].position.x, yOffset, 0);
                    card.DoAnimate(time, 0, pos, 0, cardOnBoardScale * offset);

                }
            }
        }
    }

    public void _wc_OnUserTakeCard(PHOM_TurnData data)
    {
        float time = 0.3f;
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        var currentPos = IGUIM.instance.cardOnHandTransform.position;
        if (data.card != null)
        {
            var key = data.card.ToString();
            Card card = null;
            if (cardsOnBoard.ContainsKey(key))
                card = cardsOnBoard[key];
            else if (playersOnBoard.ContainsKey(data.userId))
            {
                var go = IGUIM.CreateCard(cardPrefab, data.card, playersOnBoard[data.userId].PHOM_submitCardsTransform.position);
                card = go.GetComponent<Card>();
                AddCardToBoard(card);
                card.SetFlip(true, 20);
            }
            if (OGUIM.me.id == data.userId)
            {
                float x = 0;
                float y = currentPos.y;

                var pos = new Vector3(x, y, 0);
                card.SetCoord(4, 0);
                card.SetSortingOrder(4 + 200);
                card.DoAnimate(time, 0, pos, 0, cardOnHandScale).OnComplete(() =>
                {
                    card.canTouch = true;
                    card.SetHighLight(true);
                    DOVirtual.DelayedCall(0.1f, ReorderCardOnHandNotChangeSortType);
                });
                cardsOnBoard.Remove(key);
                cardsOnHand.Add(data.card.ToString(), card);


                IGUIM.SetAllButtons(false, "submit_btn", "PHOM_xep_btn");
            }
            else if (playersOnBoard.ContainsKey(data.userId))
            {
                var player = playersOnBoard[data.userId];

                if (player.userData.acquiredCards == null)
                    player.userData.acquiredCards = new List<CardData>();
                player.userData.acquiredCards.Add(data.card);

                var y = (player.userData.acquiredCards.Count - 1) * 0.4f;

                card.SetHighLight(true);
                card.SetSortingOrder(-player.userData.acquiredCards.Count);


                var pos = player.cardView.transform.position + Vector3.up * y;
                card.DoAnimate(time, 0, pos, 0, cardOnBoardScale).OnComplete(() =>
                {
                    card.SetHighLight(true);
                });

                if (player.userData.acquiredCards == null)
                    player.userData.acquiredCards = new List<CardData>();
                player.userData.acquiredCards.Add(data.card);
            }
        }
    }

    private void _wc_OnUserGetCard(PHOM_TurnData data)
    {
        float time = 0.5f;
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        var currentPos = IGUIM.instance.cardOnHandTransform.position;
        if (data.card != null)
        {
            if (OGUIM.me.id == data.userId)
            {

                var go = IGUIM.CreateCard(cardPrefab, data.card, Vector3.zero);
                var card = go.GetComponent<Card>();
                float x = 0;
                float y = currentPos.y;
                card.SetCoord(4, 0);
                card.SetSortingOrder(4 + 200);
                go.transform.DOMove(new Vector3(x, y, 0), time).OnComplete(() =>
                {
                    card.SetFlip(true);
                    card.canTouch = true;
                    DOVirtual.DelayedCall(0.1f, ReorderCardOnHandNotChangeSortType);
                });
                go.transform.DOScale(cardOnHandScale, time);
                cardsOnHand.Add(data.card.ToString(), card);

                IGUIM.SetAllButtons(false, "submit_btn", "PHOM_xep_btn");
            }
            else if (playersOnBoard.ContainsKey(data.userId))
            {
                var player = playersOnBoard[data.userId];
                var go = IGUIM.CreateCard(cardPrefab, data.card, Vector3.zero);
                go.transform.DOMove(player.cardView.transform.position, time);
                go.transform.DOScale(cardOnBoardScale, time).OnComplete(() =>
                {
                    Destroy(go);
                });
            }
        }
    }

    public override void _wc_OnSubmitFailed()
    {
        ReorderCardOnHandNotChangeSortType();
        foreach (var key in cardsOnHand.Keys)
            cardsOnHand[key].SetSelected(false);
    }
    public override void Instance_OnSetTurn(TurnData data)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        foreach (var key in playersOnBoard.Keys)
        {
            playersOnBoard[key].SetTurn(data.userId == key, interval);
            playersOnBoard[key].SetStatus((key == data.userId && data.keyTurn) ? "CHỐT" : "");
            playersOnBoard[key].SetRemainCard(0);
        }
        if (OGUIM.me.id == data.userId)
        {
            if (data.newTurn)
                IGUIM.SetAllButtons(false, "submit_btn", "PHOM_xep_btn");
            else
                IGUIM.SetAllButtons(false, "PHOM_an_btn", "PHOM_boc_btn");

            // Auto-fix me.isPlayer = true if set turn to me
            OGUIM.me.isPlayer = true;
        }
        else if (OGUIM.me.isPlayer)
            IGUIM.SetAllButtons(false, "PHOM_xep_btn");
    }

    public override void Instance_OnStartMatch(byte[] payLoadBytes)
    {
        base.Instance_OnStartMatch(payLoadBytes);
        suitesOnGame.Clear();

        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        foreach (var key in playersOnBoard.Keys)
        {
            playersOnBoard[key].suiteCount = 0;
            playersOnBoard[key].userData.acquiredCards = new List<CardData>();
            playersOnBoard[key].SetRemainCard(0);
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

        foreach (var user in data.users)
        {
            if (user.extra != null && user.extra.cards != null)
                ShowCardsEachPlayer(user.id, user.extra.cards, true);
            if (playersOnBoard.ContainsKey(user.id))
            {
                foreach (var c in playersOnBoard[user.id].userData.playedCards)
                {
                    var cardStr = c.ToString();
                    if (cardsOnBoard.ContainsKey(c.ToString()))
                        cardsOnBoard[cardStr].SetDisable();
                }

                playersOnBoard[user.id].userData.order = user.order;
                playersOnBoard[user.id].userData.properties = user.properties;
                playersOnBoard[user.id].userData.owner = user.owner;
                playersOnBoard[user.id].userData.acquiredCards = new List<CardData>();
                playersOnBoard[user.id].suiteCount = 0;
                playersOnBoard[user.id].SetTurn(false);
                if (GameBase.isOldVersion)
                    playersOnBoard[user.id].SetReady(user.owner);
                playersOnBoard[user.id].FillData(user);

                if (playersOnBoard[user.id].userData.isPlayer)
                {
                    if (data.specialty == 6)
                    {
                        if (user.extra.cards.Count >= 9)
                        {
                            IGUIM.SpawnTextEfx("Cháy", playersOnBoard[user.id].avatarView.imageAvatar.transform.position, false);
                        }
                        else if (user.chipChange != 0)
                            IGUIM.SpawnTextEfx(GetRankByOrder(user.order),
                                playersOnBoard[user.id].avatarView.imageAvatar.transform.position,
                                user.chipChange > 0);
                    }
                    else if (data.specialty == 5)
                    {
                        if (user.chipChange > 0)
                        {
                            UIManager.PlaySound("white_win");
                            IGUIM.SpawnTextEfx("Ù tròn", playersOnBoard[user.id].avatarView.imageAvatar.transform.position);
                            if (user.id == OGUIM.me.id)
                            {
                                IGUIM.SpawnTextEfx("Ù tròn", Vector3.zero);
                            }
                        }
                        else if (user.properties.punished_uhhhh)
                        {
                            IGUIM.SpawnTextEfx("Đền", playersOnBoard[user.id].avatarView.imageAvatar.transform.position);
                        }
                    }
                    else if (data.specialty == 4)
                    {
                        if (user.chipChange > 0)
                        {
                            UIManager.PlaySound("white_win");
                            IGUIM.SpawnTextEfx("Ù", playersOnBoard[user.id].avatarView.imageAvatar.transform.position);
                            if (user.id == OGUIM.me.id)
                            {
                                IGUIM.SpawnTextEfx("Ù", Vector3.zero);
                            }
                        }
                        else if (user.properties.punished_uhhhh)
                        {
                            IGUIM.SpawnTextEfx("Đền", playersOnBoard[user.id].avatarView.imageAvatar.transform.position, false);
                        }
                    }
                    else if (data.specialty == 3)
                    {
                        if (user.chipChange < 0)
                        {
                            IGUIM.SpawnTextEfx("ĐỀN LÀNG", playersOnBoard[user.id].avatarView.imageAvatar.transform.position, false);
                            if (user.id == OGUIM.me.id)
                            {
                                IGUIM.SpawnTextEfx("ĐỀN LÀNG", Vector3.zero, false);
                            }
                        }
                    }
                    else if (data.specialty == 7)
                    {
                        if (user.chipChange > 0)
                        {
                            IGUIM.SpawnTextEfx("Ù khan", playersOnBoard[user.id].avatarView.imageAvatar.transform.position);
                            if (user.id == OGUIM.me.id)
                            {
                                IGUIM.SpawnTextEfx("Ù khan", Vector3.zero);
                            }
                        }
                        else if (user.chipChange < 0)
                        {
                            IGUIM.SpawnTextEfx("Thua", playersOnBoard[user.id].avatarView.imageAvatar.transform.position, false);
                        }
                    }
                }
                playersOnBoard[user.id].userData.isPlayer = true;
                playersOnBoard[user.id].userData.isReady = false;
                playersOnBoard[user.id].userData.isHaPhom = false;

                if (user.chipChange != 0)
                {
                    var str = Ultility.CoinToString(playersOnBoard[user.id].userData.chipChange) + " " + OGUIM.currentMoney.name;
                    IGUIM.SpawnTextEfx(str, playersOnBoard[user.id].avatarView.imageAvatar.transform.position + Vector3.down * 0.75f, user.chipChange > 0);
                }
            }
        }
        IGUIM.SetAllButtons(false);
        IGUIM.SetButtonsActive(new string[] { "ready_btn", "start_btn", "invite_btn" },
                                            new bool[] { !OGUIM.me.owner, OGUIM.me.owner,
                                                data.users.Count < IGUIM.instance.maxPlayerInGame && OGUIM.currentMoney.type == MoneyType.Gold });

        if (!OGUIM.me.owner && !OGUIM.me.isReady && GameBase.isOldVersion)
        {
            DOVirtual.DelayedCall(2, () =>
                { OGUIM.Toast.Show("Sẵn sàng để chơi ván mới", UIToast.ToastType.Notification, 2); });
        }

    }

    public override void Instance_OnSubmitTurn(TurnData data)
    {
        UIManager.PlaySound("flip");
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        float time = 0.3f;
        float distanceCard = 0.3f;

        PlayerView player = null;
        if (playersOnBoard.ContainsKey(data.userId))
        {
            player = playersOnBoard[data.userId];
            player.userData.playedCards = data.playedCards;
            player.SetRemainCard(0);
        }
        var tempCards = data.cards.OrderBy(x => x.card % 13 == 1 ? 14 : x.card).ToList();
        for (var i = 0; i < tempCards.Count; i++)
        {
            CardData cardData = tempCards[i];
            string cardStr = cardData.ToString();
            Card card = null;
            //if (GameUIManager.me.id == data.userId)
            //{
            if (cardsOnHand.ContainsKey(cardStr))
            {
                card = cardsOnHand[cardStr];
                cardsOnHand.Remove(cardStr);
                ReorderCardOnHandNotChangeSortType();
            }
            //}
            else if (playersOnBoard.ContainsKey(data.userId))
            {
                var go = IGUIM.CreateCard(cardPrefab, cardData, playersOnBoard[data.userId].Position);
                go.transform.localScale = cardOnHandScale * Vector3.one;
                card = go.GetComponent<Card>();
            }
            if (card != null && player != null)
            {
                //card.SetSortingOrder(cardsOnBoard.Count, boardLayerName);
                card.canTouch = false;
                AddCardToBoard(card);

                if (tempCards.Count == 1)
                {
                    float x = (player.userData.playedCards.Count - 1) * distanceCard;
                    card.SetSortingOrder(player.userData.playedCards.Count);

                    var pos = new Vector3(x, 0, 0) + player.PHOM_submitCardsTransform.position;
                    var tween = card.DoAnimate(time, 0, pos, 0, cardOnBoardScale).OnStart(() => card.SetFlip(true, 20));
                }
                else
                {
                    float x = i * distanceCard;
                    card.SetSortingOrder(i);

                    var pos = new Vector3(x, 0, 0) + player.PHOM_submitCardsTransform.position;
                    card.DoAnimate(time, 0, pos, 0, cardOnBoardScale).OnStart(() => card.SetFlip(true, 20));
                }
            }
        }
    }
    private void ChangeCardsOnBoard()
    {
        float time = 0.5f;
        float distanceCard = 0.3f;
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        foreach (var key in playersOnBoard.Keys)
        {
            var player = playersOnBoard[key];
            int i = 0;
            if (player.userData.playedCards == null || !player.userData.playedCards.Any())
                continue;
            foreach (var cardData in player.userData.playedCards)
            {
                var cardKey = cardData.ToString();
                Card card;
                if (cardsOnBoard.ContainsKey(cardKey))
                {
                    card = cardsOnBoard[cardKey];
                }
                else
                {
                    var go = IGUIM.CreateCard(cardPrefab, cardData, player.PHOM_submitCardsTransform.position);
                    card = go.GetComponent<Card>();
                    card.SetFlip(true, 20);
                    AddCardToBoard(card);
                }
                if (card != null)
                {
                    float x = i * distanceCard;
                    card.SetSortingOrder(i);

                    var pos = new Vector3(x, 0, 0) + player.PHOM_submitCardsTransform.position;
                    var tween = card.DoAnimate(time, 0, pos, 0, cardOnBoardScale);
                }
                i++;
            }
        }
    }

    SortingType sortingType = SortingType.ByCard;
    public List<CardData> SortSuite(int sortType)
    {
        foreach (var key in cardsOnHand.Keys)
        {
            cardsOnHand[key].cardData.pair = 4;
            //cardsOnHand[key].cardData.suite = 0;
            //cardsOnHand[key].cardData.index = 0;
        }
        var sortedCards = cardsOnHand.Select(x => x.Value.cardData).ToList();

        #region 1. Tính cạ và cây lẻ
        CardLogic.callPair(sortedCards);
        sortedCards = CardLogic.sort(sortedCards);

        if (sortType == (int)SortingType.ByFace)
        {
            CardLogic.sortFace(sortedCards, 2, 0);
            sortedCards = CardLogic.sort(sortedCards);
            CardLogic.sortNumber(sortedCards, 2, 11);
            sortedCards = CardLogic.sort(sortedCards);
        }
        else
        {
            CardLogic.sortNumber(sortedCards, 2, 0);
            sortedCards = CardLogic.sort(sortedCards);
            CardLogic.sortFace(sortedCards, 2, 11);
            sortedCards = CardLogic.sort(sortedCards);
        }
        #endregion

        #region 3. Xóa cạ và cây lẻ ngoài phỏm
        foreach (var e in sortedCards)
        {
            if (e.pair != 1)
                e.pair = 4;
        }
        #endregion

        #region 4. Tính lại cạ sau khi có phỏm
        foreach (var c1 in sortedCards)
        {
            if (c1.pair == 4)
            {
                foreach (var c2 in sortedCards)
                {
                    if (c2.pair == 4 || c2.pair == 2)
                    {
                        // cùng số khác chất thành cạ
                        // cùng số + nằm trong khoảng 2 thành cạ
                        if ((c1.card == c2.card && c1.face != c2.face) ||
                            (c1.card != c2.card && c1.face == c2.face && Mathf.Abs(c1.card - c2.card) <= 2))
                        {
                            c1.pair = 2;
                            c2.pair = 2;
                        }
                    }
                }
            }
        }

        sortedCards = CardLogic.sort(sortedCards);
        #endregion

        #region 5. Tính cạ với phỏm
        foreach (var c1 in sortedCards)
        {
            if (c1.pair == 4)
            {
                foreach (var c2 in sortedCards)
                {
                    if (c2.pair == 1)
                    {
                        // cùng số khác chất thành cạ
                        // cùng số + nằm trong khoảng 2 thành cạ
                        if ((c1.card == c2.card && c1.face != c2.face) ||
                            (c1.card != c2.card && c1.face == c2.face && Mathf.Abs(c1.card - c2.card) <= 2))
                        {
                            c1.pair = 3;
                        }
                    }
                }
            }
        }

        sortedCards = CardLogic.sort(sortedCards);
        return sortedCards;

        #endregion
    }
    public override void ReorderCardOnHand()
    {
        Debug.Log("ReorderCardOnHand");
        sortingType = (SortingType)(1 - (int)sortingType);
        var sortedCards = SortSuite((int)sortingType);

        float time = 0.3f;
        int i = 0;
        Vector2 sizeOfCards = CalculateSizeOfCards(sortedCards.Count, distanceCards, Vector3.down, cardOnHandScale);
        foreach (var card in sortedCards)
        {
            var id = card.ToString();
            if (cardsOnHand.ContainsKey(id))
            {
                float x = i * distanceCards - (sizeOfCards.x * 0.5f - 0.5f);
                float y = IGUIM.instance.cardOnHandTransform.position.y;
                cardsOnHand[id].gameObject.transform.localScale = cardOnHandScale * Vector3.one;
                cardsOnHand[id].canTouch = true;
                cardsOnHand[id].SetSelected(false);
                cardsOnHand[id].SetFlip(true);
                cardsOnHand[id].SetCoord(i, 0);
                cardsOnHand[id].SetSortingOrder(i + 200);
                cardsOnHand[id].gameObject.transform.DOMove(new Vector3(x, y, 0), time);
            }
            i++;
        }
    }
    public void ReorderCardOnHandNotChangeSortType()
    {
        sortingType = (SortingType)(1 - (int)sortingType);
        ReorderCardOnHand();
    }
    public void SuggestSuite()
    {
        // Waiting after user eaten card/take card 0.1s + Anim 0.3s
        DOVirtual.DelayedCall(0.2f, () =>
        {

            var hasHighlight = cardsOnHand.Any(x => x.Value.isHighlight);
            var sortByFace = SortSuite((int)SortingType.ByFace);
            var cardCountByFace = sortByFace.Count(x => x.suite != 0);

            var sortByCard = SortSuite((int)SortingType.ByCard);
            var cardCountByCard = sortByCard.Count(x => x.suite != 0);

            if (cardCountByFace > cardCountByCard)
            {
                sortingType = SortingType.ByFace;
            }
            else
            {
                sortingType = SortingType.ByCard;
            }
            // Anim 0.3s
            ReorderCardOnHand();

            // Waiting when reordercard animation 0.3s
            DOVirtual.DelayedCall(0.75f, () =>
            {
                UILogView.Log("-----CHECK PHOM-----");
                foreach (var key in cardsOnHand.Keys)
                {
                    if (cardsOnHand[key].cardData.pair == 1)
                    {
                        UILogView.Log(cardsOnHand[key].cardData.suite + "  " + cardsOnHand[key].cardData);
                        cardsOnHand[key].SetSelected(true);
                    }
                    else
                    {
                        cardsOnHand[key].SetSelected(false);
                    }

                }
                UILogView.Log("--------------------");
            });
        });
    }
    public string AddSuiteOnGame(List<CardData> oldSuite, List<CardData> attachs = null)
    {
        string key = "";
        foreach (var c in oldSuite)
        {
            if (suitesOnGame.ContainsKey(c.ToString()))
            {
                key = c.ToString();
                break;
            }
        }
        if (suitesOnGame.ContainsKey(key))
        {
            var suite = suitesOnGame[key];
            if (attachs != null)
            {
                foreach (var c in attachs)
                {
                    if (!suite.Any(cc => cc.IndexNumber == c.IndexNumber))
                        suite.Add(c);
                }
            }
            suitesOnGame[key] = suite;
        }
        else
        {
            key = oldSuite.FirstOrDefault().ToString();
            if (attachs != null)
                suitesOnGame.Add(key, oldSuite.Concat(attachs).ToList());
            else
                suitesOnGame.Add(key, oldSuite);
        }

        return key;
    }
    public static string GetRankByOrder(int order)
    {
        if (order == -1)
            return "Bét";
        else if (order == 0)
            return "Nhất";
        else if (order == 1)
            return "Nhì";
        else if (order == 2)
            return "Ba";

        return "";
    }


    private void FakeGetRoomInfo()
    {
        string s = "{ \"cards\" : [], \"keyTurn\" : true, \"lastTurnUser\" : 547747, \"currTurnUser\" : 474540, \"room\" : { \"id\" : 1539, \"name\" : null, \"type\" : null, \"chipType\" : 0, \"bet\" : 5000, \"maxUsers\" : 0, \"max_player\" : 4, \"curr_num_of_player\" : 4, \"locked\" : false, \"started\" : true, \"quickplay\" : false, \"funds\" : 0 }, \"users\" : [ { \"id\" : 3487, \"username\" : \"thinhpt\", \"order\" : 0, \"seatOrder\" : 0, \"owner\" : false, \"sessionId\" : 1871020155, \"isPlayer\" : false, \"remainCardCount\" : 0, \"chipChange\" : 0, \"displayName\" : \"thinhpt\", \"email\" : null, \"mobile\" : \"\", \"address\" : null, \"avatar\" : \"1\", \"faceBookId\" : null, \"gender\" : 1, \"koin\" : 14100100, \"gold\" : 148370, \"exp\" : 3, \"expTarget\" : 0, \"level\" : 0, \"allLevel\" : 5, \"win\" : 0, \"loss\" : 0, \"draw\" : 0, \"providerCode\" : \"Zendios\", \"online\" : false, \"isReady\" : false, \"extra\" : {}, \"isFirstLoginToday\" : false, \"expired\" : false, \"kicked\" : false, \"link_fb\" : false, \"link_acc\" : true, \"isFirstPurchase\" : false, \"properties\" : { \"suites\" : null }, \"playedCards\" : [], \"acquiredCards\" : [] }, { \"id\" : 474540, \"username\" : \"0971416133\", \"order\" : 1, \"seatOrder\" : 1, \"owner\" : true, \"sessionId\" : 772039698, \"isPlayer\" : true, \"remainCardCount\" : 10, \"chipChange\" : 0, \"displayName\" : \"nguyễn ngọc liên\", \"email\" : \"\", \"mobile\" : null, \"address\" : \"\", \"avatar\" : \"1\", \"faceBookId\" : null, \"gender\" : 0, \"koin\" : 2022987, \"gold\" : 38, \"exp\" : 54, \"expTarget\" : 0, \"level\" : 0, \"allLevel\" : 6, \"win\" : 0, \"loss\" : 0, \"draw\" : 0, \"providerCode\" : \"B6EqI\", \"online\" : false, \"isReady\" : true, \"extra\" : {}, \"isFirstLoginToday\" : false, \"expired\" : false, \"kicked\" : false, \"link_fb\" : true, \"link_acc\" : true, \"isFirstPurchase\" : true, \"properties\" : { \"suites\" : [] }, \"playedCards\" : [ { \"card\" : 13, \"face\" : 2 }, { \"card\" : 9, \"face\" : 4 }, { \"card\" : 7, \"face\" : 4 } ], \"acquiredCards\" : [ { \"card\" : 10, \"face\" : 4 } ] }, { \"id\" : 436231, \"username\" : \"giangkhong\", \"order\" : 2, \"seatOrder\" : 2, \"owner\" : false, \"sessionId\" : 885141840, \"isPlayer\" : true, \"remainCardCount\" : 9, \"chipChange\" : 0, \"displayName\" : \"Giang Không\", \"email\" : \"\", \"mobile\" : null, \"address\" : \"\", \"avatar\" : \"1\", \"faceBookId\" : null, \"gender\" : 0, \"koin\" : 92750, \"gold\" : 0, \"exp\" : 60, \"expTarget\" : 0, \"level\" : 0, \"allLevel\" : 7, \"win\" : 0, \"loss\" : 0, \"draw\" : 0, \"providerCode\" : \"nLTj5\", \"online\" : false, \"isReady\" : true, \"extra\" : {}, \"isFirstLoginToday\" : false, \"expired\" : false, \"kicked\" : false, \"link_fb\" : true, \"link_acc\" : true, \"isFirstPurchase\" : false, \"properties\" : { \"suites\" : [] }, \"playedCards\" : [ { \"card\" : 10, \"face\" : 2 }, { \"card\" : 8, \"face\" : 3 }, { \"card\" : 7, \"face\" : 3 } ], \"acquiredCards\" : [ { \"card\" : 11, \"face\" : 2 } ] }, { \"id\" : 547747, \"username\" : \"fb_1850017798560366\", \"order\" : 3, \"seatOrder\" : 3, \"owner\" : false, \"sessionId\" : 224537008, \"isPlayer\" : true, \"remainCardCount\" : 6, \"chipChange\" : 0, \"displayName\" : \"CT Quanghop\", \"email\" : \"igame@gmail.com\", \"mobile\" : \"989956548\", \"address\" : \"Unknown\", \"avatar\" : \"1\", \"faceBookId\" : \"1850017798560366\", \"gender\" : 1, \"koin\" : 141147, \"gold\" : 41, \"exp\" : 9, \"expTarget\" : 0, \"level\" : 0, \"allLevel\" : 5, \"win\" : 0, \"loss\" : 0, \"draw\" : 0, \"providerCode\" : \"B6EqI\", \"online\" : false, \"isReady\" : true, \"extra\" : {}, \"isFirstLoginToday\" : false, \"expired\" : false, \"kicked\" : false, \"link_fb\" : true, \"link_acc\" : false, \"isFirstPurchase\" : true, \"properties\" : { \"suites\" : [ [ { \"card\" : 12, \"face\" : 4 }, { \"card\" : 12, \"face\" : 3 }, { \"card\" : 12, \"face\" : 1 } ] ] }, \"playedCards\" : [ { \"card\" : 5, \"face\" : 2 }, { \"card\" : 9, \"face\" : 3 }, { \"card\" : 5, \"face\" : 3 }, { \"card\" : 13, \"face\" : 4 } ], \"acquiredCards\" : [] } ], \"isNewTurn\" : false, \"turnStartTime\" : 1478066764666 }";
        var data = JsonUtility.FromJson<GetRoomInfoData>(s);
        if (data.room != null)
        {
            interval = OGUIM.currentRoom.intervalPlay;
            IGUIM.SetRoomData(data.room);
        }
        else
        {
            UILogView.Log("Instance_OnGetRoomInfo: Room is null!");
        }
        if (data.users != null && data.users.Any())
        {
            IGUIM.SetUsers(data.users);
        }
        else
        {
            UILogView.Log("Instance_OnGetRoomInfo: Users is null or empty!");
        }

        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (data.room.started)
        {
            StartMatch(data.cards, true);
            foreach (var user in data.users)
            {
                if (playersOnBoard.ContainsKey(user.id))
                {
                    playersOnBoard[user.id].SetRemainCard(user.remainCardCount);
                    playersOnBoard[user.id].SetReady(user.isReady);


                    if (OGUIM.currentLobby.id == (int)LobbyId.MAUBINH)
                    {
                        playersOnBoard[user.id].SetTurn(user.isPlayer, interval, 90);
                    }

                    if (user.lastPlayedCards != null && user.lastPlayedCards.Any())
                        Instance_OnSubmitTurn(new TurnData
                        {
                            userId = user.id,
                            cards = user.lastPlayedCards,
                            remainCardCount = user.remainCardCount
                        });

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
                        var acquiredCards = new List<CardData>(user.acquiredCards);
                        if (playersOnBoard[user.id].userData.acquiredCards != null)
                            playersOnBoard[user.id].userData.acquiredCards = new List<CardData>();
                        if (this is PHOM_GameManager)
                        {
                            foreach (var cardData in acquiredCards)
                            {
                                var turnData = new PHOM_TurnData();
                                turnData.card = cardData;
                                turnData.userId = user.id;
                                (this as PHOM_GameManager)._wc_OnUserTakeCard(turnData);
                            }
                        }
                    }
                }
            }
            if (data.currTurnUser > 0 && OGUIM.currentLobby.id != (int)LobbyId.MAUBINH)
                Instance_OnSetTurn(new TurnData { userId = data.currTurnUser, newTurn = data.isNewTurn });

            IGUIM.SetButtonsActive(new string[] { "ready_btn", "start_btn" }, new bool[] { false, false });
        }
    }
    private void FakeSuiteSubmit()
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        int i = 1;
        foreach (var p in playersOnBoard.Values)
        {
            p.userData = new UserData { id = i };
            i++;
        }

        var suites = new List<List<CardData>>();
        var suite1 = new List<CardData> { new CardData(7, 3), new CardData(8, 3), new CardData(9, 3) };
        var suite2 = new List<CardData> { new CardData(4, 1), new CardData(4, 4), new CardData(4, 3) };
        suites.Add(suite1);
        suites.Add(suite2);

        var turn = new PHOM_TurnData();
        turn.type = 211;
        turn.userId = OGUIM.me.id;
        turn.suites = suites;
        _wc_OnPHOMUserSubmitSuite(turn);
    }
    int cardTest = 4;
    private void FakeAttach()
    {
        string s = "{ \"cards\" : [ { \"card\" : " + cardTest + ", \"face\" : 2 }], \"suite\" : { \"cards\" : [ { \"card\" : 4, \"face\" : 1 }, { \"card\" : 4, \"face\" : 4 }, { \"card\" : 4, \"face\" : 3 } ], \"userId\" : 3 }, \"type\" : 210, \"userId\" : " + 2 + " }";
        cardTest = cardTest % 12 + 1;

        var data = JsonUtility.FromJson<PHOM_TurnData>(s);
        _wc_OnAttachCardToSuite(data);
    }
    private void FakeStart()
    {
        //Debug.Log("Test");
        //var index = Random.Range(1, 4);
        //GameUIController.CreateCoinRainEfx(index);
        //DOVirtual.DelayedCall(0.7f, () => GameUIController.CreateCoinRainEfx(index));
        //DOVirtual.DelayedCall(1.4f, () => GameUIController.CreateCoinRainEfx(index));
        //var cards = new List<CardData> { new CardData(4,1), new CardData(4, 4) , new CardData(4,3 ) , new CardData(6, 2),
        //                                    new CardData(8,3), new CardData(9, 3) , new CardData(7, 3) , new CardData(13, 1),
        //                                    new CardData(13,2)};
        var cards = new List<CardData> { new CardData(10,1), new CardData(10, 2) , new CardData(10,3 ) , new CardData(3, 2),
                                            new CardData(3,3), new CardData(3, 4) , new CardData(11, 2) , new CardData(1, 4),
                                            new CardData(9,2), new CardData(1,3)};
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
        DOVirtual.DelayedCall(1, () => (this as PHOM_GameManager).SuggestSuite());
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
    public override void Update()
    {
        base.Update();
        //if (Input.GetButtonDown("Jump"))
        //{
        //    FakeAttach();
        //}
        if (Input.GetButtonDown("Jump"))
        {
            FakeStart();

            //DOVirtual.DelayedCall(2, () => FakeSuiteSubmit());

            //FakeGetRoomInfo();
        }
    }
}
