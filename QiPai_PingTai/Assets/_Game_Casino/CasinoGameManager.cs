using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections.Generic;


public class CasinoGameManager : GameManager
{
    // Use this for initialization
    public override void LoadScene()
    {
        base.LoadScene();
        if (OGUIM.currentRoom.room != null)
        {
            IGUIM_Casino.SetRoomData(OGUIM.currentRoom.room);
        }
    }
    public override void OnUnloadScene()
    {
        //GameUIManager.currentRoom = null;
        RemoveListener();
    }

    public override void AddListener()
    {
        WarpClient.wc.OnGetRoomInfo += _wc_OnGetRoomInfo;
        WarpClient.wc.OnStartMatch += _wc_OnStartMatch;
        WarpClient.wc.OnEndMatch += _wc_OnEndMatch;
        WarpClient.wc.OnSetBet += _wc_OnSetBet;
        WarpClient.wc.OnClearBet += _wc_OnClearBet;
        WarpClient.wc.OnRoomStateChanged += _wc_OnRoomStateChanged;
        WarpClient.wc.OnLeaveRoom += _wc_OnLeaveRoom;
        WarpClient.wc.OnJoinRoom += Wc_OnJoinRoom; ;
        WarpClient.wc.OnGetPlayers += _wc_OnGetPlayers;
        WarpClient.wc.OnReconnected += _wc_OnReconnected;

        WarpClient.wc.OnUserTakeOwnerDone += Wc_OnUserTakeOwnerDone;
        WarpClient.wc.OnUserPassOwnerDone += Wc_OnUserPassOwnerDone;
        WarpClient.wc.OnUserStandUpDone += Wc_OnUserStandUpDone;
        WarpClient.wc.OnUserSitDownDone += Wc_OnUserSitDownDone;
        WarpClient.wc.OnOwnerSellPotDone += Wc_OnOwnerSellPotDone;
        WarpClient.wc.OnUserBuyPotDone += Wc_OnUserBuyPotDone;
        WarpClient.wc.OnReturnBetDone += Wc_OnReturnBetDone;
        WarpClient.wc.OnOwnerTakeAllDone += Wc_OnOwnerTakeAllDone;
    }

    public override void RemoveListener()
    {
        WarpClient.wc.OnGetRoomInfo -= _wc_OnGetRoomInfo;
        WarpClient.wc.OnStartMatch -= _wc_OnStartMatch;
        WarpClient.wc.OnEndMatch -= _wc_OnEndMatch;
        WarpClient.wc.OnSetBet -= _wc_OnSetBet;
        WarpClient.wc.OnClearBet -= _wc_OnClearBet;
        WarpClient.wc.OnRoomStateChanged -= _wc_OnRoomStateChanged;
        WarpClient.wc.OnLeaveRoom -= _wc_OnLeaveRoom;
        WarpClient.wc.OnJoinRoom -= Wc_OnJoinRoom; ;
        WarpClient.wc.OnGetPlayers -= _wc_OnGetPlayers;
        WarpClient.wc.OnReconnected -= _wc_OnReconnected;
        WarpClient.wc.OnHeadline -= _wc_OnHeadline;

        WarpClient.wc.OnUserTakeOwnerDone -= Wc_OnUserTakeOwnerDone;
        WarpClient.wc.OnUserPassOwnerDone -= Wc_OnUserPassOwnerDone;
        WarpClient.wc.OnUserStandUpDone -= Wc_OnUserStandUpDone;
        WarpClient.wc.OnUserSitDownDone -= Wc_OnUserSitDownDone;
        WarpClient.wc.OnOwnerSellPotDone -= Wc_OnOwnerSellPotDone;
        WarpClient.wc.OnUserBuyPotDone -= Wc_OnUserBuyPotDone; ;
        WarpClient.wc.OnReturnBetDone -= Wc_OnReturnBetDone;
        WarpClient.wc.OnOwnerTakeAllDone -= Wc_OnOwnerTakeAllDone;
    }

    private void _wc_OnReconnected(int resultCode)
    {
        if (resultCode == (int)WarpResponseResultCode.INVALID_SESSION)
        {
            UILogView.Log("CardGameManager  Instance_OnReconnected  resultCode:" + resultCode);
            //OGUIM.UnLoadGameScene(true);
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
    public void _wc_OnLeaveRoom(Room data, int resultCode)
    {
        if (resultCode == (int)WarpResponseResultCode.SUCCESS)
        {
            var playersOnBoard = IGUIM_Casino.GetPlayersOnBoard();
            if (data.userId == OGUIM.me.id)
            {
                Debug.Log("CardGameManager  Instance_OnLeaveRoom  success -> go to list room");
            }
            else
            {
                var name = playersOnBoard[data.userId].userData.displayName;
                if(playersOnBoard[data.userId].userData.owner || playersOnBoard[data.userId].userData.seatOrder == -2)
                    IGUIM_Casino.SetButtonActive("CASINO_lamcai_btn", true);
                IGUIM_Casino.instance.allUsers.Remove(playersOnBoard[data.userId].userData);

                playersOnBoard[data.userId].FillData(null);
                playersOnBoard.Remove(data.userId);
                OGUIM.Toast.ShowNotification(name + " đã thoát khỏi phòng");
            }
        }
    }


    private void Wc_OnJoinRoom(Room data)
    {
        if (data != null && data.users != null)
        {
            UILogView.Log("join room - Users count: " + data.users.Count, false);
            IGUIM_Casino.SetUsers(data.users);
        }
    }
    public void _wc_OnGetPlayers(System.Collections.Generic.List<UserData> users)
    {
        //XocdiaUIController.SetPlayersInRoom(users);
        Debug.LogError("_wc_OnGetPlayers: " + users.Count);
    }
    public void _wc_OnGetRoomInfo(int requestType, int resultCode, byte[] payLoad)
    {
        UILogView.Log(this.GetType().Name + " _wc_OnGetRoomInfo", false);
        
        var data = ZenMessagePack.DeserializeObject<CasinoTurnData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
        if (data == null)
        {
            UILogView.Log("Instance_OnGetRoomInfo: data is null!");
        }
        else
        {
            if (data.users != null && data.users.Any())
            {
                IGUIM_Casino.SetUsers(data.users);
            }
            else
            {
                UILogView.Log("Instance_OnGetRoomInfo: Users is null or empty!");
            }
            if (data.room != null)
            {
                IGUIM_Casino.SetRoomData(data.room);
                HandleRoomState(data.room);
            }
            else
            {
                UILogView.Log("Instance_OnGetRoomInfo: Room is null!");
            }
            if (data.history != null && data.history.Any())
            {
                IGUIM_Casino.SetHistory(data.history);
                Debug.Log("----set history");
            }

            IGUIM_Casino.instance.anim.Show();
        }
       
    }
    private void _wc_OnStartMatch(byte[] payLoadBytes)
    {
        Debug.LogError(this.GetType().Name + " _wc_OnStartMatch");
    }
    public void _wc_OnRoomStateChanged(byte[] payloads, WarpContentTypeCode payloadType)
    {
        var casinoTurn = ZenMessagePack.DeserializeObject<CasinoTurnData>(payloads, WarpContentTypeCode.MESSAGE_PACK);
        if(casinoTurn == null)
        {
            UILogView.Log(GetType().Name + " casinoTurn: room is null");
            return;
        }
        if (casinoTurn.room == null)
        {
            UILogView.Log (GetType().Name + " _wc_OnRoomStateChanged: room is null");
            return;
        }
        HandleRoomState(casinoTurn.room);
    }
    private void HandleRoomState(RoomInfo room)
    {
		// For using stateNew + timeCountDownNew in XOCDIA, BAUCUA new
		if (OGUIM.currentLobby.id == (int)LobbyId.XOCDIA || OGUIM.currentLobby.id == (int)LobbyId.BAUCUA) {
			room.state = room.stateNew;
			room.timeCountDown = room.timeCountDownNew;
		}

        var time = room.timeCountDown;
		if (OGUIM.currentLobby.id == (int)LobbyId.XOCDIA_OLD || OGUIM.currentLobby.id == (int)LobbyId.BAUCUA_OLD) {
			switch ((CASINOGameStateOld)room.state)
			{
			case CASINOGameStateOld.DEAL:
				if (IGUIM_Casino.instance != null)
					IGUIM_Casino.instance.ResetBoard();
				IGUIM_Casino.StartShake();
				time = 0;
				break;
			case CASINOGameStateOld.BET:
				OGUIM.Toast.ShowNotification("Nhà cái bắt đầu nhận cược");
				IGUIM_Casino.StopShake();
				time = room.timeCountDown != 0 ? room.timeCountDown : 20;
				break;
			case CASINOGameStateOld.WAIT:
				OGUIM.Toast.ShowNotification ("Nhà cái kết thúc nhận cược");
				time = room.timeCountDown != 0 ? room.timeCountDown : 0;
				if (room.lucky) {
					var pot = room.luckySlot.pot;
					var rate = room.luckySlot.rate;
					IGUIM_Casino.SetLucky (pot, rate);
				}
				break;
			case CASINOGameStateOld.END:
				if (room.lucky) {
					var pot = room.luckySlot.pot;
					var rate = room.luckySlot.rate;
					IGUIM_Casino.SetLucky (pot, rate, false);
				}
				IGUIM_Casino.Open();
				time = room.timeCountDown;
				IGUIM_Casino.instance.sellBuyPotsToggle.HideAll();
				break;
			case CASINOGameStateOld.CALCULATE:
				if (IGUIM_Casino.instance != null)
					DOVirtual.DelayedCall(2.5f, () => IGUIM_Casino.instance.ResetBoard());
				time = room.timeCountDown != 0 ? room.timeCountDown : 5;

				IGUIM_Casino.Close();
				IGUIM_Casino.instance.sellBuyPotsToggle.HideAll();
				break;
			}
		} else {
			switch ((CASINOGameState)room.state)
			{
			case CASINOGameState.STOP:
				DOVirtual.DelayedCall (1f, () => {
					OGUIM.Toast.ShowNotification ( OGUIM.currentLobby.desc + ": Vui lòng đợi đủ người chơi");
				});
				time = 0;
				break;
			case CASINOGameState.WAIT:
				DOVirtual.DelayedCall (1f, () => {
					OGUIM.Toast.ShowNotification(OGUIM.currentLobby.desc + ": Ván mới sắp bắt đầu");
				});
				time = room.timeCountDown;
				break;
			case CASINOGameState.CALCULATE:
				if (IGUIM_Casino.instance != null)
					DOVirtual.DelayedCall(2.5f, () => IGUIM_Casino.instance.ResetBoard());
				time = room.timeCountDown != 0 ? room.timeCountDown : 5;

				IGUIM_Casino.Close();
				IGUIM_Casino.instance.sellBuyPotsToggle.HideAll();
				break;
			case CASINOGameState.DEAL:
				if (IGUIM_Casino.instance != null)
					IGUIM_Casino.instance.ResetBoard();

				IGUIM_Casino.StartShake();
				time = 0;
				break;
			case CASINOGameState.BET:
				OGUIM.Toast.ShowNotification(OGUIM.currentLobby.desc + ": Nhà cái bắt đầu nhận cược");
				IGUIM_Casino.StopShake();
				time = room.timeCountDown != 0 ? room.timeCountDown : 20;
				break;
			case CASINOGameState.STOP_SET_BET:
				OGUIM.Toast.ShowNotification(OGUIM.currentLobby.desc + ": Chờ nhà cái cân cửa");
				time = room.timeCountDown != 0 ? room.timeCountDown : 5;
				break;
			case CASINOGameState.END:
				IGUIM_Casino.Open();
				time = room.timeCountDown;
				IGUIM_Casino.instance.sellBuyPotsToggle.HideAll();
				break;
			case CASINOGameState.BUY_POT:
				OGUIM.Toast.ShowNotification(OGUIM.currentLobby.desc + ": Chờ người chơi mua cửa");
				time = room.timeCountDown != 0 ? room.timeCountDown : 10;
				break;
			}
		}
        IGUIM_Casino.SetRoomData(room);
        IGUIM_Casino.SetUIWithEachOwnerRule();
        IGUIM_Casino.SetTime(time);
    }
    public void _wc_OnClearBet(CasinoTurnData data)
    {
        if(data != null)
        {
            if(data.user.id == OGUIM.me.id)
                OGUIM.Toast.ShowNotification("Hủy cược thành công");
            IGUIM_Casino.SetRoomBet(data.room.slots, data.user);

            var playerOnBoards = IGUIM_Casino.GetPlayersOnBoard();
            if (playerOnBoards.ContainsKey(data.user.id))
            {
                playerOnBoards[data.user.id].SetStatus("");
                IGUIM_Casino.ClearUserBet(data.user.id);
            }
        }
    }

    public void _wc_OnSetBet(CasinoTurnData casinoTurn)
    {
        //var coinList = Ultility.ConvertBet2Chip(casinoTurn.bet, OGUIM.currentRoom.room.chipType);
        var playerOnBoards = IGUIM_Casino.GetPlayersOnBoard();
        if (playerOnBoards.ContainsKey(casinoTurn.user.id))
        {
            playerOnBoards[casinoTurn.user.id].SetStatus(IGUIM_Casino.GetPotName(casinoTurn.pot) + " " + LongConverter.ToK(casinoTurn.bet));
        }

        if (IGUIM_Casino.instance != null)
        {
            IGUIM_Casino.SetRoomBet(casinoTurn.room.slots, casinoTurn.user);
            IGUIM_Casino.SpawnChipSprite(casinoTurn.user != null ? casinoTurn.user.id : -1, casinoTurn.pot);
            //if (casinoTurn.user != null && casinoTurn.user.id == OGUIM.me.id)
            //{
            //    XocdiaUIController.instance.AddChipsFromUser(casinoTurn.pot, coinList, coinList[0]);
            //}
            //else
            //{
            //    XocdiaUIController.instance.AddChipsFromUser(casinoTurn.pot, coinList, -1);
            //    XocdiaUIController.instance.UpdateRoomBet(casinoTurn.room.slots);
            //}
        }
        else
            UILogView.Log("_wc_OnSetBet: Xocdia UI Controller has been destroyed");
    }


    public void _wc_OnHeadline(RootChatData data)
    {
    }
    
    public virtual void _wc_OnEndMatch(byte[] payLoadBytes)
    {
        UILogView.Log(this.GetType().Name + " _wc_OnEndMatch", false);
        IGUIM_Casino.instance.sellBuyPotsToggle.HideAll();

        if (OGUIM.autoLeaveRoom && IGUIM_Casino.instance != null)
        {
            OGUIM.Toast.ShowLoading("");
            DOVirtual.DelayedCall(3f, () => IGUIM_Casino.instance.BackBtn_Click());
        }
    }



    private void Wc_OnUserSitDownDone(CasinoData userData)
    {
        if (userData != null && userData.users != null && userData.users.Any())
        {
            IGUIM_Casino.SetUsers(userData.users);
        }
        IGUIM_Casino.SetUIWithEachOwnerRule();

        if (OGUIM.instance != null && OGUIM.instance.popUpPlayersInRoom != null)
        {
            OGUIM.instance.popUpPlayersInRoom.FillData();
        }
    }

    private void Wc_OnUserStandUpDone(CasinoData userData)
    {
        var playerOnBoards = IGUIM_Casino.GetPlayersOnBoard();

        if (playerOnBoards[userData.userId].userData.owner || playerOnBoards[userData.userId].userData.seatOrder == -2)
            IGUIM_Casino.SetButtonActive("CASINO_lamcai_btn", true);

        if (playerOnBoards.ContainsKey(userData.userId))
        {
            playerOnBoards[userData.userId].userData.seatOrder = -1;
            playerOnBoards[userData.userId].FillData(null);
            playerOnBoards.Remove(userData.userId);
        }
        if(OGUIM.me.id == userData.userId)
        {
            OGUIM.me.seatOrder = -1;
            OGUIM.me.owner = false;
        }
        IGUIM_Casino.SetUIWithEachOwnerRule();

        if (OGUIM.instance != null && OGUIM.instance.popUpPlayersInRoom != null)
        {
            OGUIM.instance.popUpPlayersInRoom.FillData();
        }
    }

    private void Wc_OnUserPassOwnerDone(CasinoData userData)
    {
        var playerOnBoards = IGUIM_Casino.GetPlayersOnBoard();
        var name = playerOnBoards[userData.userId].userData.displayName;
        OGUIM.Toast.ShowNotification(name + " từ bỏ cái");
        if (userData.users != null)
            IGUIM_Casino.SetUsers(userData.users);

        IGUIM_Casino.SetUIWithEachOwnerRule();

        if (OGUIM.instance != null && OGUIM.instance.popUpPlayersInRoom != null)
        {
            OGUIM.instance.popUpPlayersInRoom.FillData();
        }
    }

    private void Wc_OnUserTakeOwnerDone(CasinoData userData)
    {
        var playerOnBoards = IGUIM_Casino.GetPlayersOnBoard();
        if (playerOnBoards.ContainsKey(userData.userId))
        {
            OGUIM.Toast.ShowNotification(playerOnBoards[userData.userId].userData.displayName + " đã trở thành cái");
        }
        if (userData != null && userData.users != null && userData.users.Any())
        {
            IGUIM_Casino.SetUsers(userData.users);
            if (OGUIM.me.id == userData.userId)
            {
                IGUIM_Casino.SetUIWithEachOwnerRule();
            }
        }
    }


    private void Wc_OnOwnerSellPotDone(CasinoData data)
    {
        if (OGUIM.me.seatOrder == -2)
        {
            IGUIM_Casino.instance.sellBuyPotsToggle.Hide(data.pot - 1);
            IGUIM_Casino.SetButtonActive("CASINO_cancua_btn", false);
        }
        else
            IGUIM_Casino.instance.sellBuyPotsToggle.Show(true, data.pot);
    }

    private void Wc_OnReturnBetDone(CasinoData data)
    {
        var toastes = new List<string>();
        if (data.pots != null)
        {
            for (int i = 0; i < data.pots.Count; i++)
            {
                toastes.Add("Nhà cái hoàn trả cửa " + IGUIM_Casino.GetPotName(data.pots[i].pot));
                IGUIM_Casino.ClearPotBet(data.pots[i].pot);
            }
        }

        for(int i = 0; i < toastes.Count; i++)
        {
            var pos = (i - 0.5f * toastes.Count + 0.5f) * Vector3.down * 0.75f + Vector3.up * 0.01f;
            IGUIM_Casino.SpawnTextEfx(toastes[i], pos);
        }
    }

    private void Wc_OnUserBuyPotDone(CasinoData data)
    {
        IGUIM_Casino.UserBuyPot(data.pot, data.userId);
    }

    private void Wc_OnOwnerTakeAllDone(CasinoData users)
    {
        IGUIM_Casino.SetButtonActive("CASINO_cancua_btn", false);
        OGUIM.Toast.ShowNotification("Nhà cái cân cửa");
    }
    
}

public enum CASINOGameState
{
    STOP = -1, // Đợi (chưa đủ người chơi)
    WAIT = 0, // Xóc
    DEAL = 1, // Cược
    BET = 2, // Chờ
    STOP_SET_BET = 3,
    BUY_POT = 4,
    END = 5,
    CALCULATE = 6
}

public enum CASINOGameStateOld
{
	DEAL = 0, // Xóc
	BET = 1, // Cược
	WAIT = 2, // Chờ
	END = 3,
	CALCULATE = 4
}