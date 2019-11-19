using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Linq;

public class TLMN_GameManager : CardGameManager
{
    public override void AddListener()
    {
        base.AddListener();

        WarpClient.wc.OnDut3Bich += Instance_OnDut3Bich;
        WarpClient.wc.OnThoi3Bich += Instance_OnThoi3Bich;
    }
    public override void RemoveListener()
    {
        base.RemoveListener();
        WarpClient.wc.OnDut3Bich -= Instance_OnDut3Bich;
        WarpClient.wc.OnThoi3Bich -= Instance_OnThoi3Bich;
    }

    public override void Instance_OnSizeChanged()
    {
        DOVirtual.DelayedCall(1, () =>
        {
            base.Instance_OnSizeChanged();
            ReorderCardOnHand();
        });
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
            if (playersOnBoard.ContainsKey(user.id))
            {
                if (data.specialty == 0 && data.reason == 0 && user.chipChange != 0 && user.isPlayer)
                {
                    IGUIM.SpawnTextEfx(user.chipChange > 0 ? "THẮNG" : "THUA", 
                        playersOnBoard[user.id].avatarView.imageAvatar.transform.position,
                        user.chipChange > 0);
                }
                if(data.reason == 2 && user.chipChange < 0 && user.isPlayer)
                    IGUIM.SpawnTextEfx("THUA", playersOnBoard[user.id].avatarView.imageAvatar.transform.position, false);

                playersOnBoard[user.id].userData.isPlayer = true;
                playersOnBoard[user.id].userData.owner = user.owner;
				playersOnBoard[user.id].SetTurn(false);
				if (GameBase.isOldVersion)
					playersOnBoard[user.id].SetReady(user.owner);
                playersOnBoard[user.id].FillData(user);

                if (user.chipChange != 0)
                {
                    var str = Ultility.CoinToString(playersOnBoard[user.id].userData.chipChange) + " " + OGUIM.currentMoney.name;
                    IGUIM.SpawnTextEfx(str, playersOnBoard[user.id].avatarView.imageAvatar.transform.position + Vector3.down * 0.75f, user.chipChange > 0);
                }
            }
            if (user.extra != null && user.extra.cards != null)
                ShowCardsEachPlayer(user.id, user.extra.cards);
        }
        IGUIM.SetButtonsActive(new string[] { "submit_btn", "pass_btn", "unready_btn", "ready_btn", "start_btn", "invite_btn" },
                                            new bool[] { false, false, false, !OGUIM.me.owner, OGUIM.me.owner,
                                                data.users.Count < IGUIM.instance.maxPlayerInGame && OGUIM.currentMoney.type == MoneyType.Gold });
		
		if (!OGUIM.me.owner && !OGUIM.me.isReady && GameBase.isOldVersion)
		{
			DOVirtual.DelayedCall(2, () =>
				{OGUIM.Toast.Show("Sẵn sàng để chơi ván mới", UIToast.ToastType.Notification, 2);});
		}
    }

    public override void Instance_OnSubmitTurn(TurnData data)
	{
		UIManager.PlaySound ("flip");
        base.Instance_OnSubmitTurn(data);

        var arrayCards = data.cards.ToArray();
        if (lastSubmitCards.Any())
        {
            if (CardLogic.IsTuQuy(arrayCards))
            {
                IGUIM.SpawnTextEfx("TỨ QUÝ", Vector3.zero);
            }
            else if (CardLogic.Is3DoiThong(arrayCards))
            {
                IGUIM.SpawnTextEfx("BA ĐÔI THÔNG", Vector3.zero);
            }
            else if (CardLogic.Is4DoiThong(arrayCards))
            {
                IGUIM.SpawnTextEfx("BỐN ĐÔI THÔNG", Vector3.zero);
            }
        }
        lastSubmitCards = data.cards;
    }

    public void Instance_OnThoi3Bich(int loserId, int winnerId)
    {
		UIManager.PlaySound ("white_win");
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        foreach (var key in playersOnBoard.Keys)
        {
            if (key == loserId)
                IGUIM.SpawnTextEfx("THỐI 3 BÍCH", playersOnBoard[key].avatarView.imageAvatar.transform.position, false);
            else if (key == winnerId)
                IGUIM.SpawnTextEfx("CỨU LÀNG", playersOnBoard[key].avatarView.imageAvatar.transform.position);
        }

        IGUIM.SetButtonsActive(new string[] { "submit_btn", "pass_btn", "unready_btn", "ready_btn", "start_btn" },
                                            new bool[] { false, false, false, !OGUIM.me.owner, OGUIM.me.owner });
    }

    public void Instance_OnDut3Bich(int userId)
	{
		UIManager.PlaySound ("white_win");
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (OGUIM.me.id == userId)
        {
            IGUIM.SpawnTextEfx("ĐÚT 3 BÍCH", Vector3.zero, false);
        }

        foreach (var key in playersOnBoard.Keys)
        {
            if(playersOnBoard[key].userData.isPlayer)
                IGUIM.SpawnTextEfx(key == userId ? "ĐÚT 3 BÍCH" : "THUA", playersOnBoard[key].avatarView.imageAvatar.transform.position, false);
        }

        IGUIM.SetButtonsActive(new string[] { "submit_btn", "pass_btn", "unready_btn", "ready_btn", "start_btn" },
                                            new bool[] { false, false, false, !OGUIM.me.owner, OGUIM.me.owner });
    }
}
