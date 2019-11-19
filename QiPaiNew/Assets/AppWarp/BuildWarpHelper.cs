using System;
using System.Collections.Generic;
using UnityEngine;

public static class BuildWarpHelper
{
    public static void LeaveRoom(Action onTimeOut)
    {
        var roomId = 0;
        if (OGUIM.currentRoom == null || OGUIM.currentRoom.room == null)
        {
            UILogView.Log("BuildWarpHelper  LeaveRoom: room is null");
			OGUIM.BackToLobies();
        }
        else
            roomId = OGUIM.currentRoom.room.id;
        var dic = new roomRequest
        {
            roomId = roomId
        };
        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(dic);

        WarpClient.wc.Send(WarpRequestTypeCode.LEAVE_ROOM, jsonStr, onTimeOut, 3.5f);
    }
    public static void GetRoomInfo(Room rootRoomData, Action onTimeOut)
    {
        var dic = new roomRequest
        {
            sessionId = WarpClient.wc.sessionId,
            type = (int)WarpRequestTypeCode.GET_ROOM_INFO,
            roomId = rootRoomData.room.id
        };
        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(dic);

        WarpClient.wc.Send(WarpRequestTypeCode.GET_ROOM_INFO, jsonStr, onTimeOut);
    }
    public static void StartRequest(Action onTimeOut)
    {
        var dic = new roomRequest
        {
            type = (int)WarpRequestTypeCode.START_MATCH,
        };
        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(dic);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, WarpNotifyTypeCode.START_MATCH, jsonStr, onTimeOut, 2);
    }
    public static void ReadyRequest(bool ready, Action onTimeOut)
    {
        var dic = new roomRequest
        {
            type = ready ? (int)WarpRequestTypeCode.SET_READY : (int)WarpRequestTypeCode.SET_UNREADY,
        };
        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(dic);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, ready ? WarpNotifyTypeCode.SET_READY : WarpNotifyTypeCode.SET_UNREADY, jsonStr, onTimeOut, 5f);
    }

    #region CHAT
    public static void SendChat(int roomId, string message, int type, string pack, Action onTimeOut)
    {

        var dic = new roomRequest
        {
            roomId = roomId,
            message = message
        };
        if (type == 2)
        {
            dic.type = type;
            dic.pack = pack;
            var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(dic);
            WarpClient.wc.Send(WarpRequestTypeCode.EMOTION, WarpNotifyTypeCode.CHAT_NOTIFICATION, jsonStr, onTimeOut, 1);
        }
        else
        {
            var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(dic);
            WarpClient.wc.Send(WarpRequestTypeCode.CHAT, WarpNotifyTypeCode.CHAT_NOTIFICATION, jsonStr, onTimeOut, 1);
        }
    }

    public static void SendChat(string message, Action onTimeOut)
    {
        var dic = new roomRequest
        {
            message = message
        };

        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(dic);
        WarpClient.wc.Send(WarpRequestTypeCode.GLOBAL_CHAT, WarpNotifyTypeCode.CHAT_NOTIFICATION, jsonStr, onTimeOut, 1);
    }
    #endregion

    #region TLMN
    public static void TLMN_PassTurnRequest(Action onTimeOut)
    {
        var dic = new roomRequest
        {
            type = (int)GameRequestTypesCode_TLMNDL.PASS_TURN,
        };
        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(dic);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameRequestTypesCode_TLMNDL.PASS_TURN, jsonStr, onTimeOut, 5f);
    }

    public static void TLMN_SubmitTurnRequest(List<CardData> listCards, Action onTimeOut)
    {
        var jsonObject = new JSONObject(JSONObject.Type.OBJECT);
        var jsonArr = new JSONObject(JSONObject.Type.ARRAY);
        foreach (var s in listCards)
        {
            var jo = new JSONObject(JSONObject.Type.OBJECT);
            jo.AddField("card", s.card);
            jo.AddField("face", s.face);
            jsonArr.Add(jo);
        }
        jsonObject.AddField("cards", jsonArr);
        jsonObject.AddField("type", (int)GameRequestTypesCode_TLMNDL.SUBMIT_TURN);


        var jsonStr = ZenMessagePack.SerializeObject(jsonObject.Print());
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameRequestTypesCode_TLMNDL.SUBMIT_TURN, jsonStr, onTimeOut, 5f);
    }
    #endregion

    #region SAM
    public static void SAM_CancelOrRequestTurn(bool isRequest, Action onTimeOut)
    {
        var jsonObject = new roomRequest
        {
            type = isRequest ? (int)GameRequestTypesCode_SAM.REQUEST_TURN : (int)GameRequestTypesCode_SAM.CANCEL_TURN,
        };

        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(jsonObject);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, isRequest ? GameNotifyTypesCode_SAM.REQUEST_TURN : GameNotifyTypesCode_SAM.CANCEL_TURN, jsonStr, onTimeOut, 5f);
    }
    #endregion

    #region MAUBINH
    public static void MAUBINH_CancelSubmitSuite(Action onTimeOut)
    {
        var jsonObject = new roomRequest
        {
            type = (int)GameRequestTypesCode_MAUBINH.CANCEL_SUBMIT_SUITE,
        };

        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(jsonObject);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameRequestTypesCode_MAUBINH.CANCEL_SUBMIT_SUITE, jsonStr, onTimeOut, 5f);
    }
    public static void MAUBINH_SubmitSuite(List<List<CardData>> suites, Action onTimeOut)
    {
        var jsonObject = new JSONObject(JSONObject.Type.OBJECT);
        var jsonArr = new JSONObject(JSONObject.Type.ARRAY);
        foreach (var s in suites)
        {
            var arr = new JSONObject(JSONObject.Type.ARRAY);
            foreach (var ss in s)
            {
                var jo = new JSONObject(JSONObject.Type.OBJECT);
                jo.AddField("card", ss.card);
                jo.AddField("face", ss.face);
                arr.Add(jo);
            }
            jsonArr.Add(arr);
        }
        jsonObject.AddField("suites", jsonArr);
        jsonObject.AddField("type", (int)GameRequestTypesCode_MAUBINH.SUBMIT_SUITE);

        var jsonStr = ZenMessagePack.SerializeObject(jsonObject.Print());
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameRequestTypesCode_MAUBINH.SUBMIT_SUITE, jsonStr, onTimeOut, 5f);
    }
    #endregion

    #region PHOM

    public static void PHOM_GetOrTakeCard(int type, Action onTimeOut)
    {
        var jsonObject = new ZenDictionary();
        jsonObject.Add("type", type);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, (GameRequestTypesCode_PHOM)type, jsonObject.ToStream(), onTimeOut, 5f);
    }
    public static void PHOM_AttachCardToSuite(PHOM_TurnData data, Action onTimeOut)
    {
        var jsonObject = new PHOM_TurnData();
        jsonObject.type = (int)GameRequestTypesCode_PHOM.ATTACH_CARD_TO_SUITE;
        jsonObject.userId = data.userId;
        jsonObject.cards = data.cards;
        jsonObject.suite = data.suite;
        var jsonStr = ZenMessagePack.SerializeObject<PHOM_TurnData>(jsonObject);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameRequestTypesCode_PHOM.ATTACH_CARD_TO_SUITE, jsonStr, onTimeOut, 5f);
    }
    public static void PHOM_SubmitSuite(List<List<CardData>> suites, Action onTimeOut)
    {
        var jsonObject = new JSONObject(JSONObject.Type.OBJECT);
        var jsonArr = new JSONObject(JSONObject.Type.ARRAY);
        foreach (var s in suites)
        {
            var arr = new JSONObject(JSONObject.Type.ARRAY);
            foreach (var ss in s)
            {
                var jo = new JSONObject(JSONObject.Type.OBJECT);
                jo.AddField("card", ss.card);
                jo.AddField("face", ss.face);
                arr.Add(jo);
            }
            jsonArr.Add(arr);
        }
        jsonObject.AddField("suites", jsonArr);
        jsonObject.AddField("type", (int)GameRequestTypesCode_PHOM.SUBMIT_SUITE);

        UILogView.Log("-----------package sent: " + jsonObject.Print());

        var jsonStr = ZenMessagePack.SerializeObject(jsonObject.Print());
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameRequestTypesCode_PHOM.SUBMIT_SUITE, jsonStr, onTimeOut, 5f);
    }
    #endregion

    #region XOC DIA
    public static void CASINO_SetBet(int pot, int bet, Action onTimeOut)
    {
        var jsonObject = new roomRequest
        {
            type = (int)GameRequestTypesCode_XOCDIA.USER_SET_BET,
            bet = bet,
            pot = pot,
        };
        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(jsonObject);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameNotifyTypesCode_XOCDIA.NOTIFY_USER_SET_BET, jsonStr, onTimeOut, 5f);
    }

    public static void CASINO_ClearBet(Action onTimeOut)
    {
        var jsonObject = new roomRequest
        {
            type = (int)GameRequestTypesCode_XOCDIA.USER_CLEAR_BET,
        };
        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(jsonObject);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameNotifyTypesCode_XOCDIA.NOTIFY_USER_CLEAR_BET, jsonStr, onTimeOut, 5f);
    }

    public static void CASINO_GetPlayerInRoom(Action onTimeOut)
    {
        var jsonObject = new roomRequest
        {
            sessionId = WarpClient.wc.sessionId,
            type = (int)WarpRequestTypeCode.GET_PLAYERS_IN_ROOM,
        };
        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(jsonObject);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, WarpRequestTypeCode.GET_PLAYERS_IN_ROOM, jsonStr, onTimeOut);
    }

    public static void CASINO_GetOwner(Action onTimeOut)
    {
        var jsonObject = new roomRequest
        {
            type = (int)GameRequestTypesCode_XOCDIA.USER_TAKE_OWNER,
        };
        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(jsonObject);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameNotifyTypesCode_XOCDIA.NOTIFY_USER_TAKE_OWNER, jsonStr, onTimeOut, 5f);
    }

    public static void CASINO_PassOwner(Action onTimeOut)
    {
        var jsonObject = new roomRequest
        {
            type = (int)GameRequestTypesCode_XOCDIA.USER_PASS_OWNER,
        };
        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(jsonObject);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameNotifyTypesCode_XOCDIA.NOTIFY_USER_PASS_OWNER, jsonStr, onTimeOut, 5f);
    }

    public static void CASINO_HandleAll(Action onTimeOut)
    {
        var jsonObject = new roomRequest
        {
            type = (int)GameRequestTypesCode_XOCDIA.OWNER_TAKE_ALL,
        };
        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(jsonObject);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameNotifyTypesCode_XOCDIA.NOTIFY_OWNER_TAKE_ALL, jsonStr, onTimeOut, 5f);
    }

    public static void CASINO_SellPot(int pot, Action onTimeOut)
    {
        var jsonObject = new roomRequest
        {
            type = (int)GameRequestTypesCode_XOCDIA.OWNER_SELL_POT,
            pot = pot
        };
        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(jsonObject);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameNotifyTypesCode_XOCDIA.NOTIFY_OWNER_SELL_POT, jsonStr, onTimeOut, 5f);
    }

    public static void CASINO_BuyPot(int pot, Action onTimeOut)
    {
        var jsonObject = new roomRequest
        {
            type = (int)GameRequestTypesCode_XOCDIA.USER_BUY_POT,
            pot = pot
        };
        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(jsonObject);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameNotifyTypesCode_XOCDIA.NOTIFY_USER_BUY_POT, jsonStr, onTimeOut, 5f);
    }


    public static void CASINO_StandOrSit(bool stand, Action onTimeOut)
    {
        var jsonObject = new roomRequest
        {
            type = stand ? (int)GameRequestTypesCode_XOCDIA.USER_STAND_UP : (int)GameRequestTypesCode_XOCDIA.USER_SIT_DOWN,
        };
        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(jsonObject);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS,
            stand ? GameNotifyTypesCode_XOCDIA.NOTIFY_USER_STAND_UP : GameNotifyTypesCode_XOCDIA.NOTIFY_USER_SIT_CHANGED,
            jsonStr, onTimeOut, 5f);
    }
    #endregion

    #region SLOT
    public static void SLOT_GetSlotHistory(Action onTimeOut)
    {
        var jsonObject = new roomRequest
        {
            type = (int)GameRequestTypesCode_SLOT.GET_JACKPOT_HISTORY,
        };
        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(jsonObject);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameRequestTypesCode_SLOT.GET_JACKPOT_HISTORY, jsonStr, onTimeOut, 2);
    }

    public static void SLOT_GetUserHistory(Action onTimeOut)
    {
        var jsonObject = new roomRequest
        {
            type = (int)GameRequestTypesCode_SLOT.GET_USER_HISTORY,
        };
        var jsonStr = ZenMessagePack.SerializeObject<roomRequest>(jsonObject);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameRequestTypesCode_SLOT.GET_USER_HISTORY, jsonStr, onTimeOut, 2);
    }

    public static void SLOT_Spin(int deal, List<int> line, Action onTimeOut)
    {
        var jsonObject = new SlotRequest
        {
            type = (int)GameRequestTypesCode_SLOT.START_MATCH,
            deal = deal,
            line = line
        };
        var jsonStr = ZenMessagePack.SerializeObject<SlotRequest>(jsonObject);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameRequestTypesCode_SLOT.START_MATCH, jsonStr, onTimeOut, 3);
    }


    #endregion

	#region XENG
	public static void XENG_Spin(int[] bet, Action onTimeOut)
	{
		var jsonObject = new JSONObject(JSONObject.Type.OBJECT);
		var jsonArr = new JSONObject(JSONObject.Type.ARRAY);
		for (int i = 0; i < 8;i++)
		{
			var obj = new JSONObject(JSONObject.Type.OBJECT);
			obj.AddField ("pot", i + 1);
			obj.AddField("bet", bet[i]);
			jsonArr.Add (obj);
		}
		jsonObject.AddField("listBet", jsonArr);
		jsonObject.AddField("type", (int)GameRequestTypesCode_XENG.START_MATCH);

		var jsonStr = ZenMessagePack.SerializeObject(jsonObject.Print());
		WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameRequestTypesCode_XENG.START_MATCH, jsonStr, onTimeOut, 3);
	}

	public static void XENG_ReceiveMoney(Action onTimeOut)
	{
		var jsonObject = new JSONObject(JSONObject.Type.OBJECT);
		jsonObject.AddField("type", (int)GameRequestTypesCode_XENG.RECEIVE_MONEY);

		var jsonStr = ZenMessagePack.SerializeObject(jsonObject.Print());
		WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameRequestTypesCode_XENG.RECEIVE_MONEY, jsonStr, onTimeOut, 3);
	}

	public static void XENG_BetTaiXiu(int bet, bool isTai, Action onTimeOut)
	{
		var jsonObject = new JSONObject(JSONObject.Type.OBJECT);
		jsonObject.AddField("bet", bet);
		jsonObject.AddField("isTai", isTai);
		jsonObject.AddField("type", (int)GameRequestTypesCode_XENG.BET_TAI_XIU);

		var jsonStr = ZenMessagePack.SerializeObject(jsonObject.Print());
		WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameRequestTypesCode_XENG.BET_TAI_XIU, jsonStr, onTimeOut, 3);
	}
	#endregion

    #region MINIGAME
    // SUB ROOM -> STARTMATH -> DOI CUOC -> UNSUB ROOM ->
    public static void MINI_Sub(LobbyId id, int bet, int chipType, Action onTimeOut)
    {
        var dic = new ZenDictionary();
        dic.Add("bet", bet);
        dic.Add("chipType", chipType);
        dic.Add("type", (int)MINIGAME.SUBSCRIBE_ROOM);
        WarpClient.wc.Send(id, MINIGAME.SUBSCRIBE_ROOM, dic.ToStream(), onTimeOut, 2);
    }

    public static void MINI_UnSub(LobbyId id, Action onTimeOut)
    {
        var dic = new ZenDictionary();
        dic.Add("type", (int)MINIGAME.UNSUBSCRIBE_ROOM);
        WarpClient.wc.Send(id, MINIGAME.UNSUBSCRIBE_ROOM, dic.ToStream(), onTimeOut, 2);
    }

    public static void MINI_StartMatch(LobbyId id, Action onTimeOut, bool isNew = false)
    {
        var dic = new ZenDictionary();
        dic.Add("type", (int)MINIGAME.START_MATCH);
        if (isNew)
            dic.Add("isNew", true);
        WarpClient.wc.Send(id, MINIGAME.START_MATCH, dic.ToStream(), onTimeOut, 4);
    }

    public static void MINI_EndMatch(LobbyId id, Action onTimeOut, bool isNew = false)
    {
        var dic = new ZenDictionary();
        dic.Add("type", (int)MINIGAME.CAOTHAP_END_MATCH);
        WarpClient.wc.Send(id, (int)MINIGAME.CAOTHAP_END_MATCH, dic.ToStream(), onTimeOut, 4);
    }

    public static void MINI_ContinueMatch(LobbyId id, bool isHigh, Action onTimeOut)
    {
        var dic = new ZenDictionary();
        dic.Add("type", (int)MINIGAME.START_MATCH);
        dic.Add("isHigh", isHigh);
        WarpClient.wc.Send(id, MINIGAME.START_MATCH, dic.ToStream(), onTimeOut, 4);
    }

    public static void MINI_GetUserHis(int id, Action onTimeOut)
    {
        var dic = new ZenDictionary();
        dic.Add("type", (int)MINIGAME.GET_USER_HISTORY);
        WarpClient.wc.Send(id, MINIGAME.GET_USER_HISTORY, dic.ToStream(), onTimeOut);
    }

    public static void MINI_GetJackpotHis(int id, Action onTimeOut)
    {
        var dic = new ZenDictionary();
        dic.Add("type", (int)MINIGAME.GET_JACKPOT_HISTORY); 
        WarpClient.wc.Send(id, MINIGAME.GET_JACKPOT_HISTORY, dic.ToStream(), onTimeOut);
    }

    #endregion

    #region BACAY

    public static void BACAY_SubmitTurnRequest(List<CardData> listCards, Action onTimeOut)
    {
        var jsonObject = new PHOM_SuiteData();
        jsonObject.type = (int)GameRequestTypesCode_BACAY.SUBMIT_TURN;
        jsonObject.cards = listCards;
        var jsonStr = ZenMessagePack.SerializeObject<PHOM_SuiteData>(jsonObject);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameRequestTypesCode_BACAY.SUBMIT_TURN, jsonStr, onTimeOut, 5f);
    }
    public static void BACAY_SetBet(int bet, Action onTimeOut)
    {
        var jsonObject = new ZenDictionary();
        jsonObject.Add("type", (int)GameRequestTypesCode_BACAY.USER_SET_BET);
        jsonObject.Add("bet", bet);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameRequestTypesCode_XOCDIA.USER_SET_BET, jsonObject.ToStream(), onTimeOut, 5f);
    }
    #endregion

    #region LIENG
    public static void LIENG_UserRaise(float bet, Action onTimeOut)
    {
        var jsonObject = new ZenDictionary();
        jsonObject.Add("type", (int)GameRequestTypesCode_LIENG.USER_RAISE);
        jsonObject.Add("bet", bet);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameRequestTypesCode_LIENG.USER_RAISE, jsonObject.ToStream(), onTimeOut, 5f);
    }
    public static void LIENG_UserCall(Action onTimeOut)
    {
        var jsonObject = new ZenDictionary();
        jsonObject.Add("type", (int)GameRequestTypesCode_LIENG.USER_CALL);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameRequestTypesCode_LIENG.USER_CALL, jsonObject.ToStream(), onTimeOut, 5f);
    }
    public static void LIENG_UserFold(Action onTimeOut)
    {
        var jsonObject = new ZenDictionary();
        jsonObject.Add("type", (int)GameRequestTypesCode_LIENG.USER_FOLD);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_PEERS, GameRequestTypesCode_LIENG.USER_FOLD, jsonObject.ToStream(), onTimeOut, 5f);
    }
    #endregion
}
