using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Linq;

public class SAM_GameManager : CardGameManager
{
    public override void AddListener()
    {
        base.AddListener();

        WarpClient.wc.OnRequestTurn += Instance_OnRequestTurn;
        WarpClient.wc.OnCancelTurn += Instance_OnCancelTurn;
        WarpClient.wc.OnRedAlert += Instance_OnRedAlert;
    }

    public override void RemoveListener()
    {
        base.RemoveListener();

        WarpClient.wc.OnRequestTurn -= Instance_OnRequestTurn;
        WarpClient.wc.OnCancelTurn -= Instance_OnCancelTurn;
        WarpClient.wc.OnRedAlert -= Instance_OnRedAlert;
    }
    public override void Instance_OnSizeChanged()
    {
        DOVirtual.DelayedCall(1, () =>
        {
            base.Instance_OnSizeChanged();
            ReorderCardOnHand();
        });
    }
    public override void Instance_OnStartMatch(byte[] payLoadBytes)
    {
        base.Instance_OnStartMatch(payLoadBytes);

        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        IGUIM.SetButtonsActive(new string[] { "SAM_bao_btn", "SAM_huy_btn", "pass_btn", "submit_btn" },
                                            new bool[] { true, true, false, false });
        foreach(var key in playersOnBoard.Keys)
        {
            playersOnBoard[key].SetTurn(true, interval);
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
        IGUIM.UpdateRoomState((data.users == null || !data.users.Any()) ? CardRoomState.STOP : CardRoomState.WAIT, 5);

        foreach (var user in data.users)
        {
            if (user.extra != null && user.extra.cards != null)
                ShowCardsEachPlayer(user.id, user.extra.cards);
            if (playersOnBoard.ContainsKey(user.id))
            {
                if (user.isPlayer)
                {
                    if (data.specialty == 0 && data.reason == 0 && user.chipChange != 0)
                        IGUIM.SpawnTextEfx(user.chipChange > 0 ? "Thắng" : "Thua", 
                            playersOnBoard[user.id].avatarView.imageAvatar.transform.position,
                            user.chipChange > 0);

                    if (data.specialty == 2 && user.chipChange != 0)
                    {
                        IGUIM.SpawnTextEfx(user.chipChange > 0 ? "Thắng" : "phạt sâm", 
                            playersOnBoard[user.id].avatarView.imageAvatar.transform.position,
                            user.chipChange > 0);

                        if (user.id == OGUIM.me.id && user.chipChange > 0)
                        {
                            IGUIM.SpawnTextEfx("phạt sâm", Vector3.zero);
                            IGUIM.CreateCoinRainEfx();
                            //DOVirtual.DelayedCall(0.8f, () => GameUIController.CreateCoinRainEfx());
                            //DOVirtual.DelayedCall(1.4f, () => GameUIController.CreateCoinRainEfx());
                        }
                    }
                    else if (data.specialty == 3 && user.chipChange != 0)
                    {
                        IGUIM.SpawnTextEfx(user.chipChange > 0 ? "Thắng sâm" : "thua", 
                            playersOnBoard[user.id].avatarView.imageAvatar.transform.position,
                            user.chipChange > 0);
                        if (user.id == OGUIM.me.id && user.chipChange > 0)
                        {
                            IGUIM.SpawnTextEfx("Thắng sâm", Vector3.zero);
                            IGUIM.CreateCoinRainEfx();
                            //DOVirtual.DelayedCall(0.8f, () => GameUIController.CreateCoinRainEfx());
                            //DOVirtual.DelayedCall(1.4f, () => GameUIController.CreateCoinRainEfx());
                        }
                    }
                    else if (data.specialty == 4 && user.chipChange != 0)
                    {
                        IGUIM.SpawnTextEfx(user.chipChange > 0 ? "Thắng" : "Phạt báo", 
                            playersOnBoard[user.id].avatarView.imageAvatar.transform.position,
                            user.chipChange > 0);
                        if (user.id == OGUIM.me.id && user.chipChange < 0)
                        {
                            IGUIM.SpawnTextEfx("Phạt báo", playersOnBoard[user.id].avatarView.imageAvatar.transform.position, false);
                        }
                    }
                    else if (data.specialty == 5 && user.chipChange != 0)
                    {
                        IGUIM.SpawnTextEfx(user.chipChange > 0 ? "Thắng" : "Thối 2", 
                            playersOnBoard[user.id].avatarView.imageAvatar.transform.position,
                            user.chipChange > 0);
                    }
                }
                playersOnBoard[user.id].userData.isPlayer = true;
                playersOnBoard[user.id].userData.owner = user.owner;
				if (GameBase.isOldVersion)
					playersOnBoard[user.id].SetReady(user.owner);
                playersOnBoard[user.id].SetTurn(false);
                playersOnBoard[user.id].FillData(user);

                if (user.chipChange != 0)
                {
                    var str = Ultility.CoinToString(playersOnBoard[user.id].userData.chipChange) + " " + OGUIM.currentMoney.name;
                    IGUIM.SpawnTextEfx(str, playersOnBoard[user.id].avatarView.imageAvatar.transform.position + Vector3.down * 0.75f, user.chipChange > 0);
                }
            }
        }
        IGUIM.SetButtonsActive(new string[6] { "submit_btn", "pass_btn", "unready_btn", "ready_btn", "start_btn", "invite_btn" },
                                            new bool[6] { false, false, false, !OGUIM.me.owner, OGUIM.me.owner,
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
                IGUIM.SpawnTextEfx("chặt tứ quý", Vector3.zero);
            }
        }
        lastSubmitCards = data.cards;
    }

    private void Instance_OnRedAlert(TurnData data)
    {
        Debug.Log("RedAlert turn - userId: " + data.userId);
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (playersOnBoard.ContainsKey(data.userId))
        {
            IGUIM.SpawnTextEfx("Báo 1", playersOnBoard[data.userId].avatarView.imageAvatar.transform.position);
        }
    }

    private void Instance_OnCancelTurn(TurnData data)
	{
		UIManager.PlaySound ("pass");
        Debug.Log("cancel turn - userId: " + data.userId);
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (playersOnBoard.ContainsKey(data.userId))
        {
            playersOnBoard[data.userId].SetStatus("Hủy báo");
        }
        if(data.userId == OGUIM.me.id)
        {
            IGUIM.SetButtonsActive(new string[] { "SAM_bao_btn", "SAM_huy_btn" },
                                                new bool[] { false, false });
        }
    }

    private void Instance_OnRequestTurn(TurnData data)
	{
		UIManager.PlaySound ("pass");
        Debug.Log("request turn - userId: " + data.userId);
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (playersOnBoard.ContainsKey(data.userId))
        {
            //playersOnBoard[data.userId].SetStatus("Báo sâm");
            IGUIM.SpawnTextEfx("Báo sâm", playersOnBoard[data.userId].avatarView.imageAvatar.transform.position);
        }
        IGUIM.SetButtonsActive(new string[] { "SAM_bao_btn", "SAM_huy_btn" },
                                            new bool[] { false, false});
    }
}
