using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum MAUBINH_State
{
    SORTING,
    SORTED,
    COMPARE
}
public class MAUBINH_GameManager : CardGameManager
{
    private const string tweenSortCardId = "sortingCard";

    public MAUBINH_State state = MAUBINH_State.SORTED;

    Card selectedCard;
    bool isStarting;
    public Text valueCardTxt;
    public override void AddListener()
    {
        base.AddListener();

        WarpClient.wc.OnUserSubmitSuite += _wc_OnUserSubmitSuite;
        WarpClient.wc.OnUserCancelSubmitSuite += _wc_OnUserCancelSubmitSuite;
        WarpClient.wc.OnCompareChi += _wc_OnCompareChi;
    }

    public override void RemoveListener()
    {
        base.RemoveListener();

        WarpClient.wc.OnUserSubmitSuite -= _wc_OnUserSubmitSuite;
        WarpClient.wc.OnUserCancelSubmitSuite -= _wc_OnUserCancelSubmitSuite;
        WarpClient.wc.OnCompareChi -= _wc_OnCompareChi;
    }

    public override void Update()
    {
        //if (Input.GetButtonDown("Jump"))
        //{

        //    //    //GameUIController.CreateCoinRainEfx();
        //    //    //DOVirtual.DelayedCall(0.7f, () => GameUIController.CreateCoinRainEfx());
        //    //    //DOVirtual.DelayedCall(1.4f, () => GameUIController.CreateCoinRainEfx());
        //    var cards = new List<CardData> { new CardData(1,1), new CardData(1, 4) , new CardData(6, 4) , new CardData(6, 3),
        //                                             new CardData(13,4), new CardData(12, 1) , new CardData(12, 2) , new CardData(9, 1),
        //                                             new CardData(9,2), new CardData(2, 4) , new CardData(5, 3) , new CardData(10, 4), new CardData(4, 3),};
        //    //    //var cards = new List<CardData> { new CardData(1, 1), new CardData(2, 1), new CardData(3, 1), new CardData(4, 1), new CardData(5, 1) };
        //    //    //GameUIManager.Toast.Show(CardLogic.IsSanh(cards.ToArray()) + "");
        //    //StartMatch(cards);
        //    //    //ShowCardsEachPlayer(1, cards);
        //    //    //ShowCardsEachPlayer(2, cards);
        //    //    //ShowCardsEachPlayer(3, cards);
        //    //    //Instance_OnStartMatch(new TurnData
        //    //    //{
        //    //    userId = 0
        //    //});
        //    StartMatch(cards);
        //    //foreach (var p in GameUIController.Instance.players)
        //    //{
        //    //    p.SetChipChange(-1000000, GameUIManager.me, 1);
        //    //    p.SetStatus("aaaaaaaaaaaaaaaaa");
        //    //}
        //    //currentUserUI.SetChipChange(100000, GameUIManager.me, 1);
        //    //currentUserUI.SetEffect("efx_thang", true);

        //    //DOVirtual.DelayedCall(1, () =>
        //    //{
        //    //    SubmitChi(3, new List<CardData> { new CardData(1, 1), new CardData(1, 2), new CardData(1, 3) }, 1, 1, true, false);
        //    //    SubmitChi(3, new List<CardData> { new CardData(4, 1), new CardData(4, 2), new CardData(4, 3) }, 2, 2, true, false);
        //    //    SubmitChi(3, new List<CardData> { new CardData(7, 1), new CardData(7, 2), new CardData(7, 3) }, 3, 3, true, false);
        //    //});
        //    //DOVirtual.DelayedCall(2, () =>
        //    //{
        //    //    SubmitChi(2, new List<CardData> { new CardData(2, 1), new CardData(2, 2), new CardData(2, 3), new CardData(2, 4), new CardData(12, 1) }, 1, 1, true, false);
        //    //    SubmitChi(2, new List<CardData> { new CardData(5, 1), new CardData(5, 2), new CardData(5, 3), new CardData(5, 4), new CardData(12, 2) }, 2, 2, true, false);
        //    //    SubmitChi(2, new List<CardData> { new CardData(8, 1), new CardData(8, 2), new CardData(8, 3), new CardData(8, 4), new CardData(12, 3) }, 3, 3, true, false);
        //    //});
        //    //DOVirtual.DelayedCall(3, () =>
        //    //{
        //    //    SubmitChi(1, new List<CardData> { new CardData(3, 1), new CardData(3, 2), new CardData(3, 3), new CardData(3, 4), new CardData(13, 1) }, 1, 3, true, false);
        //    //    SubmitChi(1, new List<CardData> { new CardData(6, 1), new CardData(6, 2), new CardData(6, 3), new CardData(6, 4), new CardData(13, 2) }, 2, 3, true, false);
        //    //    SubmitChi(1, new List<CardData> { new CardData(9, 1), new CardData(9, 2), new CardData(9, 3), new CardData(9, 4), new CardData(13, 3) }, 3, 3, true, false);
        //    //});
        //}
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(point, Vector2.zero, 1000);
            if (hits.Any())
            {
                var colliders = hits.Where(x => x.collider != null).Select(x => x.collider).OrderBy(c => c.gameObject.transform.position.x).ToArray();
                var col = colliders.LastOrDefault();
                if (col.gameObject.tag == "card")
                {
                    var cardObj = col.gameObject.GetComponent<Card>();
                    if (!cardObj.canTouch)
                        return;
                    if (selectedCard != null && state == MAUBINH_State.SORTING)
                    {
						SwapCard(selectedCard, cardObj);
                        selectedCard = null;
                    }
                    else
                    {
                        if(selectedCard != null && selectedCard.canTouch)
                            selectedCard.SetHighLight(false);
                        selectedCard = col.gameObject.GetComponent<Card>();
                        selectedCard.SetHighLight(true);
                    }
                }
            }
        }
        if(isStarting && interval > 0)
        {
            interval -= Time.deltaTime;
            if (interval <= 0)
                interval = 0;
        }
    }

    public override void StartMatch(List<CardData> cards, bool skipAnimate = false)
	{
		if (selectedCard != null) {
			selectedCard.SetHighLight (false);
			selectedCard = null;
		}
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
            card.SetCoord(i, 0);
            card.SetSortingOrder(i);
            cardsOnHand.Add(cardData.ToString(), card);
            if (!skipAnimate)
            {
                foreach (var key in playersOnBoard.Keys)
                {
                    var player = playersOnBoard[key];
                    if (key != OGUIM.me.id)
                    {
                        int remainCard = i + 1;
                        var go1 = IGUIM.CreateCard(cardPrefab, cardData, Vector3.zero);
                        go1.transform.DOKill();
                            go1.transform.DOMove(player.cardView.transform.position, time).SetDelay(delay * i);
                        go1.transform.DORotate(Vector3.forward * 360, time, RotateMode.FastBeyond360).SetDelay(delay * i);
                        go1.transform.DOScale(Vector3.one * 0.3f, time).SetDelay(delay * i).OnComplete(() =>
                        {
                            go1.Recycle();
                        });
                    }
                    else
                    {
                        float x = i * distanceCards - (sizeOfCards.x * 0.5f - 0.5f);
                        float y = currentPos.y;
                        go.transform.DOKill();
                        go.transform.DOMove(new Vector3(x, y, 0), time).SetDelay(delay * i).OnComplete(() =>
                        {
                            card.SetFlip(true);
                        });
                        var tween = go.transform.DOScale(cardOnHandScale, time).SetDelay(delay * i);
                        if (i == cards.Count - 1)
                            tween.OnComplete(() =>
                            {
                                SortCards(true);
                            }).SetId(tweenSortCardId);
                        go.transform.DORotate(Vector3.forward * 360, time, RotateMode.FastBeyond360).SetDelay(delay * i);
                    }
                }
            }
            else
            {
                go.transform.position = currentPos;
                go.transform.localScale = cardOnHandScale * Vector3.zero;
                card.SetFlip(true);
                card.canTouch = true;
                DOVirtual.DelayedCall(1,()=> SortCards(false)).SetId(tweenSortCardId);
            }
            i++;
        }

        if (playersOnBoard.ContainsKey(OGUIM.me.id))
        {
            if (cards.Any())
                IGUIM.SetButtonsActive(new string[] { "MAUBINH_xepxong_btn", "MAUBINH_huyxep_btn", "MAUBINH_swapchi_btn" },
                                                    new bool[] { true, false, true });
            else
                OGUIM.Toast.Show("Vui lòng đợi hết ván", UIToast.ToastType.Notification, 2);
        }
    }
    public override void Instance_OnStartMatch(byte[] payLoadBytes)
    {
        base.Instance_OnStartMatch(payLoadBytes);
        interval = 90;
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        foreach (var key in playersOnBoard.Keys)
        {
            if (playersOnBoard[key].userData.isPlayer && playersOnBoard[key].userData.isReady)
            {
                playersOnBoard[key].SetTurn(true, interval, 90);
                if(key == OGUIM.me.id)
                {
                    IGUIM.SetButtonsActive(new string[] { "MAUBINH_xepxong_btn", "MAUBINH_huyxep_btn", "MAUBINH_swapchi_btn" },
                                                        new bool[] { true, false, true });
                }
            }
        }
        isStarting = true;

//#if UNITY_EDITOR
//        OGUIM.me.properties = new Properties
//        {
//            sau_doi = true
//        };
//#endif
        if (OGUIM.me.properties != null)
        {
            var maubinhType = "";
            if (OGUIM.me.properties.sanh_rong)
                maubinhType = "SẢNH RỒNG";
            if (OGUIM.me.properties.ba_thung)
                maubinhType = "BA THÙNG";
            if (OGUIM.me.properties.muoi2do_1den)
                maubinhType = "12 ĐỎ 1 ĐEN";
            if (OGUIM.me.properties.muoiba_laden)
                maubinhType = "13 ĐEN";
            if (OGUIM.me.properties.ba_sanh)
                maubinhType = "BA SẢNH";
            if (OGUIM.me.properties.muoiba_lado)
                maubinhType = "13 ĐỎ";
            if (OGUIM.me.properties.sau_doi)
                maubinhType = "SÁU ĐÔI";
            if (OGUIM.me.properties.muoi2den_1do)
                maubinhType = "12 ĐEN 1 ĐỎ";

            if (!string.IsNullOrEmpty(maubinhType))
            {
                IGUIM.SetButtonsActive(new string[] { "MAUBINH_xepxong_btn", "MAUBINH_huyxep_btn", "MAUBINH_swapchi_btn" },
                                                           new bool[] { false, false, false });
                DOVirtual.DelayedCall(3.5f, (TweenCallback)(() =>
                {
                    _wc_OnUserSubmitSuite(new TurnData { userId = OGUIM.me.id });
                    IGUIM.SpawnTextEfx(maubinhType, IGUIM.instance.currentUser.avatarView.imageAvatar.transform.position);

                    IGUIM.SpawnTextEfx("MẬU BINH", Vector3.zero);
                    IGUIM.CreateCoinRainEfx();
                    DOVirtual.DelayedCall(0.6f, (TweenCallback)(() =>
                    {
                        //GameUIController.CreateCoinRainEfx();
                        var msg = "Bài của bạn được mậu binh " + maubinhType + ". Bạn sẽ thắng các người chơi khác mà không cần xếp bài. Vui lòng chờ người chơi khác xếp bài xong!";
                        OGUIM.Toast.ShowNotification(msg);
                    }));
                    //DOVirtual.DelayedCall(1.4f, () => GameUIController.CreateCoinRainEfx());
                }));
            }
        }
    }
    public override void Instance_OnEndMatch(byte[] payLoadBytes)
    {
        base.Instance_OnEndMatch(payLoadBytes);
        _wc_OnUserSubmitSuite(new TurnData { userId = OGUIM.me.id });
        IGUIM.SetChiInfo("");
        isStarting = false;

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
            if (playersOnBoard.ContainsKey(user.id))
            {
                var player = playersOnBoard[user.id];
                if (data.specialty == 0 && user.chipChange != 0 && user.isPlayer)
                {
                    if (user.chipChange > 0)
                    {
                        IGUIM.SpawnTextEfx(user.properties.sap_lang ? "Sập làng" : "Thắng", 
                            player.avatarView.imageAvatar.transform.position);
                        if (user.id == OGUIM.me.id && user.properties.sap_lang)
                        {
                            IGUIM.SpawnTextEfx("SẬP LÀNG", Vector3.zero);
                            IGUIM.CreateCoinRainEfx();
                            //DOVirtual.DelayedCall(0.8f, () => GameUIController.CreateCoinRainEfx());
                            //DOVirtual.DelayedCall(1.4f, () => GameUIController.CreateCoinRainEfx());
                        }
                    }
                    else
                    {
                        IGUIM.SpawnTextEfx(user.properties.sap_ham ? "Sập hầm" : "Thua", 
                            playersOnBoard[user.id].avatarView.imageAvatar.transform.position, false);
                    }
                }
                
                player.userData.isPlayer = true;
                player.userData.owner = user.owner;
				if (GameBase.isOldVersion)
				{
					player.userData.isReady = user.owner;
					player.SetReady (user.owner);
				}
                player.SetTurn(false);
                player.FillData(user);

                if (user.chipChange != 0)
                {
                    var str = Ultility.CoinToString(playersOnBoard[user.id].userData.chipChange) + " " + OGUIM.currentMoney.name;
                    IGUIM.SpawnTextEfx(str, playersOnBoard[user.id].avatarView.imageAvatar.transform.position + Vector3.down * 0.75f, user.chipChange > 0);
                }
            }
        }

        IGUIM.SetButtonsActive(new string[7] { "MAUBINH_xepxong_btn", "MAUBINH_huyxep_btn", "MAUBINH_swapchi_btn", "unready_btn", "ready_btn", "start_btn", "invite_btn" },
                                            new bool[7] { false, false, false, false, !OGUIM.me.owner, OGUIM.me.owner,
                                                data.users.Count < IGUIM.instance.maxPlayerInGame && OGUIM.currentMoney.type == MoneyType.Gold});

		if (!OGUIM.me.owner && !OGUIM.me.isReady && GameBase.isOldVersion)
		{
			DOVirtual.DelayedCall(2, () =>
				{OGUIM.Toast.Show("Sẵn sàng để chơi ván mới", UIToast.ToastType.Notification, 2);});
		}
		
    }

    private void _wc_OnCompareChi(MAUBINH_Chi data)
    {
		UIManager.PlaySound ("flip");
        IGUIM.SetButtonsActive(new string[] { "MAUBINH_xepxong_btn", "MAUBINH_huyxep_btn", "MAUBINH_swapchi_btn" },
                                            new bool[] { false, false, false });
        if (data == null || data.users == null || !data.users.Any())
            UILogView.Log("_wc_OnCompareChi: data is null or empty");
        ReorderCardOnHand();
        foreach (var user in data.users)
        {
            SubmitChi(data.index, user.cards, user.userId, user.numOfChiTaken, user.maubinh, user.binhlung);
        }
    }

    private void _wc_OnUserCancelSubmitSuite(TurnData data)
    {

        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (playersOnBoard.ContainsKey(data.userId))
        {
            playersOnBoard[data.userId].SetTurn(true, interval, 90);
            playersOnBoard[data.userId].SetStatus("Xếp lại");
            if (data.userId == OGUIM.me.id)
            {
                IGUIM.SetButtonsActive(new string[] { "MAUBINH_xepxong_btn", "MAUBINH_huyxep_btn", "MAUBINH_swapchi_btn" },
                                                    new bool[] { true, false, true });
                SortCards();
            }
        }
    }

    private void _wc_OnUserSubmitSuite(TurnData data)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (playersOnBoard.ContainsKey(data.userId))
        {
            playersOnBoard[data.userId].SetTurn(false, interval, 90);
            playersOnBoard[data.userId].SetStatus("Xếp xong");
            IGUIM.SpawnTextEfx("Xếp xong", playersOnBoard[data.userId].avatarView.imageAvatar.transform.position);

            if (data.userId == OGUIM.me.id)
            {
                if (OGUIM.me.properties == null || !OGUIM.me.properties.isMaubinh)
                {
                    IGUIM.SetButtonsActive(new string[] { "MAUBINH_xepxong_btn", "MAUBINH_huyxep_btn", "MAUBINH_swapchi_btn" },
                                                    new bool[] { false, true, false });
                }
                ReorderCardOnHand();
            }
        }
    }
    public List<List<CardData>> GetSuites()
    {
        var suites = new List<List<CardData>>();
        var tempCards = cardsOnHand.Values.OrderBy(x => x.coordinate.x).Select(x => new CardData(x.cardData.card > 13 ? (x.cardData.card % 13) : x.cardData.card, x.cardData.face)).ToList();

        var chi1 = tempCards.Take(5).ToList();
        var chi2 = tempCards.Skip(5).Take(5).ToList();
        var chi3 = tempCards.Skip(10).Take(3).ToList();

        suites.Add(chi1);
        suites.Add(chi2);
        suites.Add(chi3);

        return suites;
}
    public void SortCards(bool animate = true)
    {
        var kill = DOTween.Kill(tweenSortCardId);
        state = MAUBINH_State.SORTING;
        float time = 0.3f;
        int i = 0;
		var tempCard = cardsOnHand.OrderBy (x => x.Value.coordinate.x).Select(x=>x.Value);
		foreach (var card in tempCard)
		{
			var id = card.cardData.ToString();
            cardsOnHand[id].gameObject.transform.DOKill();
            cardsOnHand[id].transform.SetRotation(0);
            cardsOnHand[id].SetFlip(true);
            if (animate)
                cardsOnHand[id].gameObject.transform.DOMove(IGUIM.instance.cardGameUI.MAUBINHCardsTransform[i].position, time)
                    .SetId(tweenSortCardId).OnComplete(() => cardsOnHand[id].canTouch = true);
            else
            {
                cardsOnHand[id].canTouch = true;
                cardsOnHand[id].gameObject.transform.position = IGUIM.instance.cardGameUI.MAUBINHCardsTransform[i].position;
            }
            cardsOnHand[id].SetSortingOrder(i);
            i++;
        }
        if(animate)
            DOVirtual.DelayedCall(1 + time, () => SortCards(false)).SetId(tweenSortCardId);
        if (cardsOnHand.Any())
        {
            ShowChiInfo();
        }
    }
    public override void ReorderCardOnHand()
	{
		if (selectedCard != null) {
			selectedCard.SetHighLight (false);
			selectedCard = null;
		}
        IGUIM.SetChiInfo("");
        state = MAUBINH_State.SORTED;
        float time = 0.3f;
        int i = 0;
        Vector2 sizeOfCards = CalculateSizeOfCards(cardsOnHand.Count, distanceCards, Vector3.down, cardOnHandScale);

        var tempCards = cardsOnHand.Values.OrderBy(x => x.coordinate.x).Select(x=>x.cardData).ToList();

        foreach (var card in tempCards)
        {
            var id = card.ToString();
            if (cardsOnHand.ContainsKey(id))
            {
                float x = i * distanceCards - (sizeOfCards.x * 0.5f - cardOnHandScale * 0.5f);
                if (cardsOnHand.Count >= 13)
                {
                    if (i < 5)
                        x -= cardOnBoardScale;
                    else if (i > 9)
                        x += cardOnBoardScale;
                }
                else if (cardsOnHand.Count >= 5)
                {
                    if (i < 5)
                        x -= cardOnBoardScale * 0.5f;
                    else
                        x += cardOnBoardScale * 0.5f;
                }
                float y = IGUIM.instance.cardOnHandTransform.position.y;
                cardsOnHand[id].gameObject.transform.localScale = cardOnHandScale * Vector3.zero;
                cardsOnHand[id].canTouch = true;
                cardsOnHand[id].SetFlip(true);
                cardsOnHand[id].SetSortingOrder(i);
                cardsOnHand[id].gameObject.transform.DOKill();
                cardsOnHand[id].DoAnimate(time, 0, new Vector3(x, y, 0), 0, 1);
                cardsOnHand[id].SetCoord(i, 0);
            }
            i++;
        }
    }
    public void SwapChi()
    {
        var tempCards = cardsOnHand.Values.OrderBy(x => x.coordinate.x).ToList();
        for (int i = 0; i < 5; i++)
        {
            var first = tempCards.ElementAtOrDefault(i);
            var second = tempCards.ElementAtOrDefault(i + 5);
            SwapCard(first, second);
        }

        ShowChiInfo();
    }
    void SwapCard(Card first, Card second)
    {
        if (first == null || second == null)
            return;
        if (first.cardData.ToNumber() == second.cardData.ToNumber())
            return;
        UILogView.Log("before: " + first.coordinate + "  " + second.coordinate);
        first.SetHighLight(false);
        second.SetHighLight(false);

        var tempCoord = first.coordinate;
        var firstPos = IGUIM.instance.cardGameUI.MAUBINHCardsTransform[(int)first.coordinate.x].position;
        var secondPos = IGUIM.instance.cardGameUI.MAUBINHCardsTransform[(int)second.coordinate.x].position;

        first.transform.DOKill();
        first.transform.SetPosition(firstPos);
        first.transform.SetRotation(0);
        first.SetFlip(true);
        first.coordinate = second.coordinate;
        first.transform.DOMove(secondPos, 0.3f).SetId(tweenSortCardId);
        first.SetSortingOrder((int)first.coordinate.x);

        second.transform.DOKill();
        second.transform.SetPosition(secondPos);
        second.transform.SetRotation(0);
        second.SetFlip(true);
        second.coordinate = tempCoord;
        second.transform.DOMove(firstPos, 0.3f).SetId(tweenSortCardId);
        second.SetSortingOrder((int)second.coordinate.x);
        ShowChiInfo();
        UILogView.Log("after: " + first.coordinate + "  " + second.coordinate);
    }

    void ShowChiInfo()
    {

        if (state == MAUBINH_State.SORTING)
        {
            var tempCards = cardsOnHand.Values.OrderBy(x => x.coordinate.x).Select(x => new CardData(x.cardData.card > 13 ? (x.cardData.card % 13) : x.cardData.card, x.cardData.face)).ToList();
            var chi1 = new MAUBINHTurn(tempCards.Take(5).ToList());
            var chi2 = new MAUBINHTurn(tempCards.Skip(5).Take(5).ToList());
            var chi3 = new MAUBINHTurn(tempCards.Skip(10).Take(5).ToList());
            if(chi1.Value >= chi2.Value && chi2.Value >= chi3.Value)
                IGUIM.SetChiInfo(chi1.FullName, chi2.FullName, chi3.FullName);
            else
                IGUIM.SetChiInfo("", "Binh lủng", "");
        }
    }
    void SubmitChi(int index, List<CardData> cards, int userId, int numOfChiTaken, bool maubinh, bool binhlung)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        float time = 0.5f;
        float distanceCard = 0.4f;
        Vector2 sizeOfCards = CalculateSizeOfCards(cards.Count, distanceCard, Vector3.down, cardOnBoardScale);

        var tempCards = cards.OrderBy(c => c.card % 13 == 1 ? 14 : c.card).ToList();
        float x = 0;
        float y = 0;
        var order = 0;
        var cardPos = IGUIM.instance.cardOnHandTransform.position;
        var currentPlayer = IGUIM.instance.currentUser;

        var content = "";
        var win = numOfChiTaken > 0;
        if (maubinh)
            content = "Mậu binh";
        else if (binhlung)
            content = "Binh lủng";
        else
        {
            var maubinhTurn = new MAUBINHTurn(cards);
            content = maubinhTurn.FullName;
        }
        if(numOfChiTaken != 0)
            content += " " + Ultility.CoinToString(numOfChiTaken) + "chi";


        if (playersOnBoard.ContainsKey(userId))
        {
            IGUIM.SpawnTextEfx(content, playersOnBoard[userId].avatarView.imageAvatar.transform.position, win);
            playersOnBoard[userId].SetRemainCard(0);
            playersOnBoard[userId].SetTurn(false);
        }
        for (var i = 0; i < tempCards.Count; i++)
        {
            CardData cardData = tempCards[i];
            Card card = null;
            if (OGUIM.me.id == userId)
            {
                string cardStr = cardData.ToString();
                if (cardsOnHand.ContainsKey(cardStr))
                {
                    card = cardsOnHand[cardStr];
                    cardsOnHand.Remove(cardStr);

                    order = cardsOnBoard.Count + 100;

                    x = i * distanceCard - (sizeOfCards.x * 0.5f - 0.5f);
                    y = cardPos.y + (index - 1) * cardOnBoardScale;
                }
                ReorderCardOnHand();
            }
            else if (playersOnBoard.ContainsKey(userId))
            {
                var go = IGUIM.CreateCard(cardPrefab, cardData, playersOnBoard[userId].Position);
                go.transform.localScale = cardOnHandScale * Vector3.zero;
                card = go.GetComponent<Card>();
                x = playersOnBoard[userId].showCardDirection.x * ((i + (5 - tempCards.Count) / 2) * distanceCard + 0.3f) + playersOnBoard[userId].cardView.transform.position.x;
                y = playersOnBoard[userId].showCardDirection.y * (index - 1) * cardOnBoardScale + playersOnBoard[userId].cardView.transform.position.y - 1.1f;
                order = (i % 5) * (int)playersOnBoard[userId].showCardDirection.x - index * 5 * (int)playersOnBoard[userId].showCardDirection.y + 150;
            }
            if (card != null)
            {
                card.SetSortingOrder(order);
                card.SetFlip(true, 20);
                card.canTouch = false;


                var pos = new Vector3(x, y, 0);
#if !UNITY_WEBGL
                var rotate = Random.Range(-10, 10);
#else
                var rotate = 0;
#endif
                var tween = card.DoAnimate(time, 0, pos, rotate, cardOnBoardScale);

                if (maubinh)
                {
                    var xx = x;
                    var yy = y - 0.3f;
                    var oo = order;
                    TweenCallback action = () =>
                    {
                        var valueTxt = Instantiate(valueCardTxt) as Text;
                        valueTxt.GetComponent<Canvas>().sortingOrder = oo + 2;
                        valueTxt.gameObject.transform.SetParent(IGUIM.instance.transform, false);
                        valueTxt.gameObject.transform.position = new Vector3(xx, yy, 0);
                        valueTxt.text = "MẬU BINH";
                    };
                    if (i == tempCards.Count / 2)
                        tween.OnComplete(action);
                }

                AddCardToBoard(card);
                if ((i == tempCards.Count - 1 && playersOnBoard[userId].showCardDirection.x == 1) || (i == 0 && playersOnBoard[userId].showCardDirection.x == -1))
                    card.SetHeader(Ultility.CoinToString(numOfChiTaken), numOfChiTaken >= 0);
            }
        }
    }
}
