using UnityEngine;
using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using MsgPack;

public class WarpClient : MonoBehaviour
{
    public static WarpClient wc;
    public static TimeOutHelper timeOutHelper = new TimeOutHelper();
    public static WarpConnectionState currentState = WarpConnectionState.DISCONNECTED;

    public int sessionId;

    public string PayloadHisTemp;
    public string PayloadUrlTemp;
    public string OffLinePayloadTemp;
    public string WeiHuTemp;
    void Awake()
    {
        wc = this;
    }

    public void ConnectedToServer(bool isReConnect, Action actionOnConnected)
    {
        OGUIM.Toast.ShowLoading("Đang kết nối máy chủ...");
        WarpClient.currentState = WarpConnectionState.CONNECTING;
        OldWarpChannel.Channel.socket_connect(actionOnConnected);
    }

    #region Socket

    private byte[] currentData;
    private int readedCount;
    private BinaryReader reader;

    public void ResponseData(byte[] data)
    {
        if (data != null && data.Length > 0)
        {
            if (currentData == null)
                currentData = data;
            else
                currentData = currentData.Concat(data).ToArray();

        }
        if (currentData == null || currentData.Length <= 0)
            return;
        if (reader == null)
        {
            try
            {
                var temp = new byte[currentData.Length];
                Array.Copy(currentData, temp, currentData.Length);
                reader = new BinaryReader(new MemoryStream(temp));
                StartCoroutine(ResponseData(reader));
            }
            catch (Exception ex)
            {
                UILogView.Log("ResponseData(byte[] data) throw: " + ex.Message, true);
            }
            finally
            {

                if (reader != null)
#if !NETFX_CORE
                    reader.Close();
#else
                    reader.Dispose();
#endif
                reader = null;
            }
        }
    }

    IEnumerator ResponseData(BinaryReader reader)
    {
        var calltype = reader.ReadByte();
        if (calltype == (int)WarpMessageTypeCode.RESPONSE)
        {
            byte requestType = reader.ReadByte();
            byte resultCode = reader.ReadByte();
            byte reserved = reader.ReadByte();
            byte payLoadType = reader.ReadByte();

            byte[] payLoadSizeBytes = reader.ReadBytes(4).Reverse().ToArray();
            int payLoadSize = BitConverter.ToInt32(payLoadSizeBytes, 0);

            int readableSize = (int)reader.BaseStream.Length;
            if (payLoadSize + 9 <= readableSize)
            {
                byte[] payLoadBytes = reader.ReadBytes(payLoadSize);
				OnResponse((WarpRequestTypeCode)requestType, resultCode, payLoadBytes);
				// Reset last send time when receive data successfull
				OldWarpChannel.Channel.tcpLastSendTime = Time.time;
                readedCount += payLoadSize + 9;
                if (readableSize - readedCount > 9)
                    ResponseData(reader);
            }
            else
            {
            }
        }
        else if (calltype == (int)WarpMessageTypeCode.UPDATE)
        {
            byte notifyType = reader.ReadByte();
            byte reserved = reader.ReadByte();
            byte payLoadType = reader.ReadByte();

            byte[] payLoadSizeBytes = reader.ReadBytes(4).Reverse().ToArray();
            int payLoadSize = BitConverter.ToInt32(payLoadSizeBytes, 0);

            int readableSize = (int)reader.BaseStream.Length;
            if (payLoadSize + 8 <= readableSize)
            {
                byte[] payLoadBytes = reader.ReadBytes(payLoadSize);
                OnNotify((WarpNotifyTypeCode)notifyType, payLoadBytes);
				// Reset last send time when receive data successfull
				OldWarpChannel.Channel.tcpLastSendTime = Time.time;
                readedCount += payLoadSize + 8;
                if (readableSize - readedCount > 8)
                    ResponseData(reader);
            }
            else
            {
            }
        }
        else
        {
            currentData = null;
            readedCount = 0;
        }

        if (readedCount > 0 && currentData != null && currentData.Length > 0)
        {
            currentData = currentData.Skip(readedCount).ToArray();
            readedCount = 0;
        }
        yield return 0;
    }
    #endregion

    #region ActionOnNotify
    #region OUTGAME
    public delegate void UpdatedMoneyDelegate(UpdatedMoneyData data);
    public event UpdatedMoneyDelegate OnUpdatedMoneyDone;

    public delegate void ActionsInLobbyDelegate(SlotJacpot data);
    public event ActionsInLobbyDelegate OnActionsInLobbyDone;

    public delegate void GeneralMessageNotificationDelegate(GeneralMessage data);
    public event GeneralMessageNotificationDelegate OnGeneralMessageNotificationDone;

    //public delegate void TaiXiuNotifyDelegate(TaiXiuData data);
    //public event TaiXiuNotifyDelegate OnTaiXiuNotifyDone;

    public delegate void NewHeadLineDelegate(Message data);
    public event NewHeadLineDelegate OnNewHeadLine;

    public delegate void InviteDelegate(RootConfig data);
    public event InviteDelegate OnInvite;

    public delegate void AddFriendDelegate(AddFriend data);
    public event AddFriendDelegate OnAddFriend;

    #endregion

    #region CHAT
    public delegate void ChatDelegate(RootChatData data);
    public event ChatDelegate OnChat;

    public delegate void WorldChatDelegate(RootWorldChat data);
    public event WorldChatDelegate OnWorldChat;

    public delegate void HeadlineDelegate(RootChatData data);
    public event HeadlineDelegate OnHeadline;

    #endregion

    #region INGAME
    public delegate void KickUserDelegate(int userId);
    public event KickUserDelegate OnKickUser;

    public delegate void AutoKickDelegate(Room data);
    public event AutoKickDelegate OnAutoKick;

    public delegate void JoinRoomDelegate(Room data);
    public event JoinRoomDelegate OnJoinRoom;

    public delegate void LeaveRoomDelegate(Room room, int resultCode);
    public event LeaveRoomDelegate OnLeaveRoom;

    public delegate void GetRoomInfoDelegate(int requestType, int resultCode, byte[] payLoad);
    public event GetRoomInfoDelegate OnGetRoomInfo;

    public delegate void StartMatchDelegate(byte[] payLoadBytes);
    public event StartMatchDelegate OnStartMatch;

    public delegate void EndMatchDelegate(byte[] payLoadBytes);
    public event EndMatchDelegate OnEndMatch;

    public delegate void SetTurnDelegate(TurnData data);
    public event SetTurnDelegate OnSetTurn;

    public delegate void PassTurnDelegate(TurnData data);
    public event PassTurnDelegate OnPassTurn;

    public delegate void SubmitTurnDelegate(TurnData data);
    public event SubmitTurnDelegate OnSubmitTurn;

    public delegate void WhiteWinDelegate(TurnData data);
    public event WhiteWinDelegate OnWhiteWin;

    public delegate void SetReadyDelegate(TurnData data);
    public event SetReadyDelegate OnReady;

    public delegate void SetUnreadyDelegate(TurnData data);
    public event SetUnreadyDelegate OnUnready;

    public delegate void PassOwnerDelegate(TurnData data);
    public event PassOwnerDelegate OnPassOwner;

    public delegate void ChipChangedDelegate(List<UserData> data);
    public event ChipChangedDelegate OnChipChanged;
    #endregion

    #region TLMN
    public delegate void Dut3BichDelegate(int userId);
    public event Dut3BichDelegate OnDut3Bich;

    public event Thoi3BichDelegate OnThoi3Bich;
    public delegate void Thoi3BichDelegate(int loserId, int winnerId);

    public event SubmitTurnFailedDelegate OnSubmitFailed;
    public delegate void SubmitTurnFailedDelegate();
    #endregion

    #region SAM
    public delegate void RequestTurnDelegate(TurnData turn);
    public event RequestTurnDelegate OnRequestTurn;

    public delegate void CancelTurnDelegate(TurnData turn);
    public event CancelTurnDelegate OnCancelTurn;

    public delegate void RedAlertDelegate(TurnData turn);
    public event RedAlertDelegate OnRedAlert;
    #endregion

    #region XOC DIA
    public delegate void NotifyUserSetBetDelegate(CasinoTurnData casinoTurn);
    public event NotifyUserSetBetDelegate OnSetBet;

    public delegate void NotifyUserClearBetDelegate(CasinoTurnData casinoTurn);
    public event NotifyUserClearBetDelegate OnClearBet;

    public delegate void RoomStateChangedDelegate(byte[] payloads, WarpContentTypeCode payloadType);
    public event RoomStateChangedDelegate OnRoomStateChanged;

    public delegate void GetPlayerDelegate(List<UserData> users);
    public event GetPlayerDelegate OnGetPlayers;

    public delegate void UserTakeOwnerDelegate(CasinoData users);
    public event UserTakeOwnerDelegate OnUserTakeOwnerDone;

    public delegate void UserPassOwnerDelegate(CasinoData users);
    public event UserPassOwnerDelegate OnUserPassOwnerDone;

    public delegate void OwnerSellPotDelegate(CasinoData users);
    public event OwnerSellPotDelegate OnOwnerSellPotDone;

    public delegate void UserBuyPotDelegate(CasinoData users);
    public event UserBuyPotDelegate OnUserBuyPotDone;

    public delegate void UserSitDownDelegate(CasinoData users);
    public event UserSitDownDelegate OnUserSitDownDone;

    public delegate void OwnerTakeAllDelegate(CasinoData users);
    public event OwnerTakeAllDelegate OnOwnerTakeAllDone;

    public delegate void ReturnBetDelegate(CasinoData users);
    public event UserBuyPotDelegate OnReturnBetDone;

    public delegate void UserStandUpDelegate(CasinoData users);
    public event UserStandUpDelegate OnUserStandUpDone;
    #endregion

    #region SLOT
    public delegate void SlotStartMatchDelegate(JSONObject data);
    public event SlotStartMatchDelegate OnSlotStart;

    public delegate void JackpotHistoryDelegate(List<HistoryBase> data, LobbyId id);
    public event JackpotHistoryDelegate OnJackpotHistory;

    public delegate void UserHistoryDelegate(List<HistoryBase> data, LobbyId id);
    public event UserHistoryDelegate OnUserHistory;

    public delegate void SlotStartFailed();
    public event SlotStartFailed OnSlotStartFailed;
    #endregion

	#region XENG
	public delegate void XengStartMatchDelegate(XengResponse data);
	public event XengStartMatchDelegate OnXengStart;

	public delegate void XengReceiveMoneyDelegate(XengResponse data);
	public event XengReceiveMoneyDelegate OnXengReceiveMoney;

	public delegate void XengBetTaiXiuDelegate(XengResponse data);
	public event XengBetTaiXiuDelegate OnXengBetTaiXiu;

	#endregion
    #region MAU BINH
    public delegate void UserSubmitSuiteDelegate(TurnData data);
    public event UserSubmitSuiteDelegate OnUserSubmitSuite;

    public delegate void UserCancelSubmitSuiteDelegate(TurnData data);
    public event UserCancelSubmitSuiteDelegate OnUserCancelSubmitSuite;

    public delegate void CompareChiDelegate(MAUBINH_Chi data);
    public event CompareChiDelegate OnCompareChi;
    #endregion

    #region PHOM
    public delegate void UserGetCardDelegate(PHOM_TurnData turn);
    public event UserGetCardDelegate OnUserGetCard;

    public delegate void UserTakeCardDelegate(PHOM_TurnData turn);
    public event UserTakeCardDelegate OnUserTakeCard;

    public delegate void AttachCardToSuiteDelegate(PHOM_TurnData turn);
    public event AttachCardToSuiteDelegate OnAttachCardToSuite;

    public delegate void PHOM_UserSubmitSuiteDelegate(PHOM_TurnData turn);
    public event PHOM_UserSubmitSuiteDelegate OnPHOMUserSubmitSuite;

    public delegate void CardsChangedDelegate(TurnData turn);
    public event CardsChangedDelegate OnCardsChanged;

    public delegate void SetSubmitSuiteDelegate(TurnData turn);
    public event SetSubmitSuiteDelegate OnSetSubmitSuite;

    public delegate void SetKeyTurnDelegate(TurnData turn);
    public event SetKeyTurnDelegate OnSetKeyTurn;

    public delegate void CancelKeyTurnDelegate(TurnData turn);
    public event CancelKeyTurnDelegate OnCancelKeyTurn;

    public delegate void UKDelegate(TurnData turn);
    public event UKDelegate OnUK;

    public delegate void ShowHandDelegate(TurnData turn);
    public event ShowHandDelegate OnShowHand;
    #endregion

    #region BACAY
    public delegate void NotifySetBetDelegate(CasinoTurnData turn);
    public event NotifySetBetDelegate OnNotifySetBet;

    public delegate void NotifyOnChickenBetDelegate(CasinoTurnData turn);
    public event NotifyOnChickenBetDelegate OnNotifyChickenBet;
    #endregion

    #region LIENG
    public delegate void UserRaiseDelegate(TurnData turn);
    public event UserRaiseDelegate OnUserRaise;//215

    public delegate void UserCallDelegate(TurnData turn);
    public event UserCallDelegate OnUserCall;//216

    public delegate void UserFoldDelegate(TurnData turn);
    public event UserFoldDelegate OnUserFold;//217

    public delegate void CalculatePotDelegate(TurnData turn);
    public event CalculatePotDelegate OnCalculatePot;
    #endregion

    #region MINIGAME
    public delegate void MinigameResponseDelegate(Mini_RootData data, int status);
    public event MinigameResponseDelegate OnMinigameResponse;

    public delegate void MiniRoomChangedDelegate(Mini_RootData data, WarpNotifyTypeCode notifyType);
    public event MiniRoomChangedDelegate OnMiniRoomChanged;
    #endregion
    #endregion

    #region Notify
    private void OnNotify(WarpNotifyTypeCode notifyType, byte[] payLoadBytes)
    {
        UILogView.Log("----- OnNotify :" + notifyType.ToString());

        try
        {
            #region OUTGAME
            if (notifyType == WarpNotifyTypeCode.UPDATE_MONEY)
            {
                var dataObject = ZenMessagePack.DeserializeObject<UpdatedMoneyData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                OnUpdatedMoneyDone(dataObject);
            }
            else if (notifyType == WarpNotifyTypeCode.ACTIONS_IN_LOBBY)
            {
                var dataObject = ZenMessagePack.DeserializeObject<SlotJacpot>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                if (OnActionsInLobbyDone != null)
                    OnActionsInLobbyDone(dataObject);
            }
            else if (notifyType == WarpNotifyTypeCode.GENERAL_MESSAGE_NOTIFICATION)
            {
                var dataObject = ZenMessagePack.DeserializeObject<GeneralMessage>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                if (OnGeneralMessageNotificationDone != null)
                    OnGeneralMessageNotificationDone(dataObject);
            }
            else if (notifyType == WarpNotifyTypeCode.NOTIFY_HEADLINE_MESSAGE)
            {
                var dataObject = ZenMessagePack.DeserializeObject<Message>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                if (OnNewHeadLine != null)
                    OnNewHeadLine(dataObject);
               
            }
            else if (notifyType == WarpNotifyTypeCode.INVITE_TO_PLAY)
            {
                var dataObject = ZenMessagePack.DeserializeObject<RootConfig>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                if (OnInvite != null)
                    OnInvite(dataObject);
            }
            else if (notifyType == WarpNotifyTypeCode.NOTIFY_ADD_FRIEND)
            {
                var dataObject = ZenMessagePack.DeserializeObject<AddFriend>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                if (OnAddFriend != null)
                    OnAddFriend(dataObject);
            }

            #endregion

            #region INGAME
            else if (notifyType == WarpNotifyTypeCode.JOIN_ROOM)
            {
                if (OnJoinRoom != null)
                {
                    var rootRoomData = ZenMessagePack.DeserializeObject<Room>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnJoinRoom(rootRoomData);
                }
            }
            else if (notifyType == WarpNotifyTypeCode.USER_SUBSCRIBE_ROOM)
            {
                if (OnJoinRoom != null)
                {
                    var rootRoomData = ZenMessagePack.DeserializeObject<Room>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnJoinRoom(rootRoomData);
                }
            }
            else if (notifyType == WarpNotifyTypeCode.LEAVE_ROOM)
            {
                UILogView.Log("------------LEAVE_ROOM---------------", false);
                if (OnLeaveRoom != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<Room>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnLeaveRoom(turnData, (int)WarpResponseResultCode.SUCCESS);
                }
            }
            else if (notifyType == WarpNotifyTypeCode.USER_UNSUBSCRIBE_ROOM)
            {
                UILogView.Log("------------USER_UNSUBSCRIBE_ROOM---------------", false);
                if (OnLeaveRoom != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<Room>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnLeaveRoom(turnData, (int)WarpResponseResultCode.SUCCESS);
                }
            }
            else if (notifyType == WarpNotifyTypeCode.KICK_USER)
            {
                if (OnKickUser != null)
                {
                    var temp = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnKickUser(temp.userId);
                }
            }
            else if (notifyType == WarpNotifyTypeCode.NOTIFY_AUTO_KICK)
            {
                if (OnAutoKick != null)
                {
                    UILogView.Log("--------NOTIFY_AUTO_KICK-------");
                    var temp = ZenMessagePack.DeserializeObject<Room>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnAutoKick(temp);
                }
            }
            else if (notifyType == WarpNotifyTypeCode.SET_READY)
            {
                //Stop time out action
                StopTimeOut(WarpNotifyTypeCode.SET_READY);
                UILogView.Log("------------SET_READY---------------", false);
                if (OnReady != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnReady(turnData);
                }
            }
            else if (notifyType == WarpNotifyTypeCode.SET_UNREADY)
            {
                //Stop time out action
                StopTimeOut(WarpNotifyTypeCode.SET_UNREADY);
                UILogView.Log("------------SET_UNREADY---------------", false);
                if (OnUnready != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnUnready(turnData);
                }
            }
            else if (notifyType == WarpNotifyTypeCode.PASS_OWNER)
            {
                UILogView.Log("------------PASS_OWNER---------------", false);
                if (OnPassOwner != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnPassOwner(turnData);
                }
            }
            else if (notifyType == WarpNotifyTypeCode.START_MATCH)
            {
                //Stop time out action
                StopTimeOut(WarpNotifyTypeCode.START_MATCH);
                UILogView.Log("------------START_MATCH---------------", false);
                if (OnStartMatch != null)
                {
                    OnStartMatch(payLoadBytes);
                }
            }
            else if (notifyType == WarpNotifyTypeCode.CHIP_CHANGE)
            {
                UILogView.Log("------------CHIP_CHANGE---------------", false);
                if (OnChipChanged != null)
                {
                    var roomData = ZenMessagePack.DeserializeObject<Room>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    if (roomData.users != null && roomData.users.Any())
                        OnChipChanged(roomData.users);
                    //else
                    UILogView.Log("CHIP_CHANGE: users is null or empty");
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_TLMNDL.END_MATCH)
            {
                //Stop time out action
                StopTimeOut(GameNotifyTypesCode_TLMNDL.END_MATCH);
                UILogView.Log("------------END_MATCH---------------", false);
                if (OnEndMatch != null)
                {
                    OnEndMatch(payLoadBytes);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_TLMNDL.SET_TURN)
            {
                //Stop time out action
                StopTimeOut(GameNotifyTypesCode_TLMNDL.SET_TURN);
                UILogView.Log("------------SET_TURN---------------", false);
                if (OnSetTurn != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnSetTurn(turnData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_TLMNDL.PASS_TURN)
            {
                //Stop time out action
                StopTimeOut(GameNotifyTypesCode_TLMNDL.PASS_TURN);
                UILogView.Log("------------PASS_TURN---------------", false);
                if (OnPassTurn != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnPassTurn(turnData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_TLMNDL.SUBMIT_TURN)
            {
                //Stop time out action
                StopTimeOut(GameNotifyTypesCode_TLMNDL.SUBMIT_TURN);
                UILogView.Log("------------SUBMIT_TURN---------------", false);
                if (OnSubmitTurn != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnSubmitTurn(turnData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_TLMNDL.WHITE_WIN)
            {
                UILogView.Log("------------WHITE_WIN---------------", false);
                if (OnWhiteWin != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnWhiteWin(turnData);
                }
            }
            #endregion

            #region CHAT

            else if (notifyType == WarpNotifyTypeCode.CHAT_NOTIFICATION)
            {
                if (OnChat != null)
                {
                    var temp = ZenMessagePack.DeserializeObject<RootChatData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnChat(temp);
                }
            }
            else if (notifyType == WarpNotifyTypeCode.NEW_CHAT_NOTIFICATION)
            {
            }
            else if (notifyType == WarpNotifyTypeCode.WORLD_CHAT_NOTIFICATION)
            {
                if (OnWorldChat != null)
                {
                    var temp = ZenMessagePack.DeserializeObject<RootWorldChat>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnWorldChat(temp);
                }
            }
            else if (notifyType == WarpNotifyTypeCode.NOTIFY_HEADLINE_MESSAGE)
            {

                if (OnHeadline != null)
                {
                    var temp = ZenMessagePack.DeserializeObject<RootChatData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnHeadline(temp);
                }

               
            }
            else if (notifyType == WarpNotifyTypeCode.NOTIFY_NEW_MESSAGE)
            {
                UILogView.Log("------------ Notify : " + notifyType.ToString());
                    var temp = ZenMessagePack.DeserializeObject<AddFriend>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);

                if (OGUIM.instance.popupToast != null)
                    OGUIM.instance.popupToast.Show(temp.currUser.displayName + " đã gửi tin nhắn cho bạn",UIToast.ToastType.Notification,3f);
            }
            #endregion

            #region TLMN
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_TLMNDL.NOTIFY_ROOM_STATE_CHANGE)
            {
                UILogView.Log("------------NOTIFY_ROOM_STATE_CHANGE---------------", false);
                if (OnRoomStateChanged != null)
                {
                    OnRoomStateChanged(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_TLMNDL.DUT_3_BICH)
            {
                UILogView.Log("------------DUT_3_BICH---------------", false);
                if (OnDut3Bich != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnDut3Bich(turnData.userId);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_TLMNDL.THOI_3_BICH)
            {
                UILogView.Log("------------THOI_3_BICH---------------", false);
                if (OnThoi3Bich != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnThoi3Bich(turnData.loserId, turnData.winnerId);
                }
            }
            #endregion

            #region SAM

            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_SAM.REQUEST_TURN)
            {
                //Stop time out action
                StopTimeOut(GameNotifyTypesCode_SAM.REQUEST_TURN);
                UILogView.Log("------------REQUEST_TURN---------------", false);
                if (OnRequestTurn != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnRequestTurn(turnData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_SAM.CANCEL_TURN)
            {
                //Stop time out action
                StopTimeOut(GameNotifyTypesCode_SAM.CANCEL_TURN);
                UILogView.Log("------------CANCEL_TURN---------------", false);
                if (OnCancelTurn != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnCancelTurn(turnData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_SAM.RED_ALERT)
            {
                UILogView.Log("------------RED_ALERT---------------", false);
                if (OnRedAlert != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnRedAlert(turnData);
                }
            }
            #endregion

            #region MAU BINH
            if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_MAUBINH.USER_SUBMIT_SUITE)
            {
                if (OGUIM.currentLobby.id == (int)LobbyId.MAUBINH)
                {
                    StopTimeOut(GameRequestTypesCode_MAUBINH.SUBMIT_SUITE);
                    UILogView.Log("----------------USER_SUBMIT_SUITE----------------------", false);
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    if (OnUserSubmitSuite != null)
                        OnUserSubmitSuite(turnData);
                }
                else if (OGUIM.currentLobby.id == (int)LobbyId.PHOM || OGUIM.currentLobby.id == (int)LobbyId.PHOM_SOLO)
                {
                    UILogView.Log("------------PHOM_USER_SUBMIT_SUITE---------------", false);
                    StopTimeOut(GameRequestTypesCode_PHOM.SUBMIT_SUITE);
                    if (OnPHOMUserSubmitSuite != null)
                    {
                        var data = ZenMessagePack.DeserializeObject(payLoadBytes);
                        if (data != null)
                        {
                            try
                            {
                                var turnData = new PHOM_TurnData();
                                turnData.userId = (int)data["userId"].n;
                                turnData.suites = data["suites"].list.Select(x =>
                                {
                                    var suite = new List<CardData>();
                                    for (int i = 0; i < x.list.Count; i++)
                                    {
                                        suite.Add(JsonUtility.FromJson<CardData>(x[i].Print()));
                                    }
                                    return suite;
                                }).ToList();
                                OnPHOMUserSubmitSuite(turnData);
                            }
                            catch (Exception ex)
                            {
                                UILogView.Log("PHOM_USER_SUBMIT_SUITE data in parsing: " + ex.Message);
                            }
                        }
                        else
                            UILogView.Log("PHOM_USER_SUBMIT_SUITE data is null");
                    }
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_MAUBINH.NOTIFY_CANCEL_SUBMIT_SUITE)
            {
                StopTimeOut(GameRequestTypesCode_MAUBINH.CANCEL_SUBMIT_SUITE);
                UILogView.Log("----------------NOTIFY_CANCEL_SUBMIT_SUITE----------------------", false);
                var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                if (OnUserCancelSubmitSuite != null)
                    OnUserCancelSubmitSuite(turnData);
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_MAUBINH.NOTIFY_COMPARE_CHI)
            {
                UILogView.Log("----------------NOTIFY_COMPARE_CHI----------------------", false);
                var turnData = ZenMessagePack.DeserializeObject<MAUBINH_Chi>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                if (OnCompareChi != null)
                    OnCompareChi(turnData);
            }
            #endregion

            #region PHOM
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_PHOM.USER_GET_CARD)
            {
                UILogView.Log("------------USER_GET_CARD---------------", false);
                StopTimeOut(GameRequestTypesCode_PHOM.GET_CARD);
                if (OnUserGetCard != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<PHOM_TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnUserGetCard(turnData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_PHOM.USER_TAKE_CARD)
            {
                UILogView.Log("------------USER_TAKE_CARD---------------", false);
                StopTimeOut(GameRequestTypesCode_PHOM.TAKE_CARD);
                if (OnUserTakeCard != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<PHOM_TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnUserTakeCard(turnData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_PHOM.ATTACH_CARD_TO_SUITE)
            {
                UILogView.Log("------------ATTACH_CARD_TO_SUITE---------------", false);
                StopTimeOut(GameRequestTypesCode_PHOM.ATTACH_CARD_TO_SUITE);
                if (OnAttachCardToSuite != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<PHOM_TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnAttachCardToSuite(turnData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_PHOM.CARDS_CHANGE)
            {
                UILogView.Log("------------CARDS_CHANGE---------------", false);
                if (OnCardsChanged != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnCardsChanged(turnData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_PHOM.SET_SUBMIT_SUITE)
            {
                UILogView.Log("------------SET_SUBMIT_SUITE---------------", false);
                if (OnSetSubmitSuite != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnSetSubmitSuite(turnData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_PHOM.SET_KEY_TURN)
            {
                UILogView.Log("------------SET_KEY_TURN---------------", false);
                if (OnSetKeyTurn != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnSetKeyTurn(turnData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_PHOM.CANCEL_KEY_TURN)
            {
                UILogView.Log("------------CANCEL_KEY_TURN---------------", false);
                if (OnCancelKeyTurn != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnCancelKeyTurn(turnData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_PHOM.U_K)
            {
                UILogView.Log("------------U_K---------------", false);
                if (OnUK != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnUK(turnData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_PHOM.SHOW_HAND)
            {
                UILogView.Log("------------SHOW_HAND---------------", false);
                if (OnShowHand != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnShowHand(turnData);
                }
            }
            #endregion

            #region XOCDIA
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_XOCDIA.NOTIFY_USER_SET_BET)
            {
                UILogView.Log("------------NOTIFY_USER_SET_BET---------------", false);
                if (OnSetBet != null)
                {
                    StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_USER_SET_BET);
                    var turnData = ZenMessagePack.DeserializeObject<CasinoTurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnSetBet(turnData);
                }
                else if (OnNotifySetBet != null)
                {
                    var temp = ZenMessagePack.DeserializeObject<CasinoTurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    StopTimeOut(GameRequestTypesCode_BACAY.USER_SET_BET);
                    OnNotifySetBet(temp);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_XOCDIA.NOTIFY_USER_CLEAR_BET)
            {
                StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_USER_CLEAR_BET);
                UILogView.Log("------------NOTIFY_USER_CLEAR_BET---------------", false);
                if (OnClearBet != null)
                {
                    var turnData = ZenMessagePack.DeserializeObject<CasinoTurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnClearBet(turnData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_XOCDIA.ROOM_STATE_CHANGED)
            {
                UILogView.Log("------------ROOM_STATE_CHANGED---------------", false);
                if (OnRoomStateChanged != null)
                {
                    OnRoomStateChanged(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_XOCDIA.NOTIFY_USER_SIT_CHANGED)
            {
                StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_USER_SIT_CHANGED);
                UILogView.Log("------------NOTIFY_USER_SIT_CHANGED---------------", false);
                if (OnUserSitDownDone != null)
                {
                    var casinoData = ZenMessagePack.DeserializeObject<CasinoData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnUserSitDownDone(casinoData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_XOCDIA.NOTIFY_USER_TAKE_OWNER)
            {
                StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_USER_TAKE_OWNER);
                UILogView.Log("------------NOTIFY_USER_TAKE_OWNER---------------", false);
                if (OnUserTakeOwnerDone != null)
                {
                    var casinoData = ZenMessagePack.DeserializeObject<CasinoData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnUserTakeOwnerDone(casinoData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_XOCDIA.NOTIFY_USER_PASS_OWNER)
            {
                StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_USER_PASS_OWNER);
                UILogView.Log("------------NOTIFY_USER_PASS_OWNER---------------", false);
                if (OnUserPassOwnerDone != null)
                {
                    var casinoData = ZenMessagePack.DeserializeObject<CasinoData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnUserPassOwnerDone(casinoData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_XOCDIA.NOTIFY_OWNER_SELL_POT)
            {
                StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_OWNER_SELL_POT);
                UILogView.Log("------------ROOM_STATE_CHANGED---------------", false);
                if (OnOwnerSellPotDone != null)
                {
                    var casinoData = ZenMessagePack.DeserializeObject<CasinoData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnOwnerSellPotDone(casinoData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_XOCDIA.NOTIFY_USER_BUY_POT)
            {
                StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_USER_BUY_POT);
                UILogView.Log("------------NOTIFY_USER_BUY_POT---------------", false);
                if (OnUserBuyPotDone != null)
                {
                    var casinoData = ZenMessagePack.DeserializeObject<CasinoData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnUserBuyPotDone(casinoData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_XOCDIA.NOTIFY_OWNER_TAKE_ALL)
            {
                StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_OWNER_TAKE_ALL);
                UILogView.Log("------------NOTIFY_OWNER_TAKE_ALL---------------", false);
                if (OnOwnerTakeAllDone != null)
                {
                    var casinoData = ZenMessagePack.DeserializeObject<CasinoData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnOwnerTakeAllDone(casinoData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_XOCDIA.NOTIFY_RETURN_BET)
            {
                UILogView.Log("------------NOTIFY_RETURN_BET---------------", false);
                if (OnReturnBetDone != null)
                {
                    var casinoData = ZenMessagePack.DeserializeObject<CasinoData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnReturnBetDone(casinoData);
                }
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_XOCDIA.NOTIFY_USER_STAND_UP)
            {
                StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_USER_STAND_UP);
                UILogView.Log("------------NOTIFY_USER_STAND_UP---------------", false);
                if (OnUserStandUpDone != null)
                {
                    var casinoData = ZenMessagePack.DeserializeObject<CasinoData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnUserStandUpDone(casinoData);
                }
            }
            #endregion

            #region MINIGAME
            else if (notifyType == WarpNotifyTypeCode.NOTIFY_MINI_ROOM_CHANGED || notifyType == WarpNotifyTypeCode.TAI_XIU || notifyType == WarpNotifyTypeCode.NOTIFY_CAOTHAP_END_MATCH)
            {
                if (OnMiniRoomChanged != null)
                {
                    var data = ZenMessagePack.DeserializeObject<Mini_RootData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                    OnMiniRoomChanged(data, notifyType);
                }
            }
            #endregion

            #region BACAY
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_BACAY_GA.NOTIFY_ON_CHICKEN_BET)
            {
                UILogView.Log("----------------NOTIFY_ON_CHICKEN_BET----------------------", false);
                var temp = ZenMessagePack.DeserializeObject<CasinoTurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                if (OnNotifyChickenBet != null)
                    OnNotifyChickenBet(temp);
            }
            #endregion

            #region LIENG
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_LIENG.USER_RAISE)
            {
                UILogView.Log("----------------USER_RAISE----------------------", false);
                StopTimeOut(GameRequestTypesCode_LIENG.USER_RAISE);
                var temp = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                if (OnUserRaise != null)
                    OnUserRaise(temp);
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_LIENG.USER_CALL)
            {
                UILogView.Log("----------------USER_CALL----------------------", false);
                StopTimeOut(GameRequestTypesCode_LIENG.USER_CALL);
                var temp = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                if (OnUserCall != null)
                    OnUserCall(temp);
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_LIENG.USER_FOLD)
            {
                UILogView.Log("----------------USER_FOLD----------------------", false);
                StopTimeOut(GameRequestTypesCode_LIENG.USER_FOLD);
                var temp = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                if (OnUserFold != null)
                    OnUserFold(temp);
            }
            else if (notifyType == (WarpNotifyTypeCode)GameNotifyTypesCode_LIENG.CALCULATE_POT)
            {
                UILogView.Log("----------------CALCULATE_POT----------------------", false);
                var temp = ZenMessagePack.DeserializeObject<TurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
                if (OnCalculatePot != null)
                    OnCalculatePot(temp);
            }
            #endregion
        }
        catch (Exception ex)
        {
            UILogView.Log("WarpClient: OnNotify: " + ex.Message + " " + ex.StackTrace, true);
        }
    }
    #endregion

    #region ActionOnResponse

    #region USER
    //  1
    public delegate void LoginDelegate(WarpResponseResultCode status, RootUserData data, string desc);
    public event LoginDelegate OnLoginDone;

    public delegate void ReconnectedDelegate(int resultCode);
    public event ReconnectedDelegate OnReconnected;

    //  4
    public delegate void SignOutDelegate(WarpResponseResultCode status);
    public event SignOutDelegate OnLogoutDone;

    //  5
    public delegate void GetUserInfoDelegate(WarpResponseResultCode status, RootUserInfo data);
    public event GetUserInfoDelegate OnGetUserInfoDone;

    //  6
    public delegate void UpdateAvatarDelegate(WarpResponseResultCode status);
    public event UpdateAvatarDelegate OnUpdateAvatarDone;

    //  8
    public delegate void GetConfigDelegate(WarpResponseResultCode status, RootConfig data);
    public event GetConfigDelegate OnGetConfigDone;

    //  40
    public delegate void GetUserAchievementDelegate(WarpResponseResultCode status, List<AchieData> data);
    public event GetUserAchievementDelegate OnGetUserAchievementDone;

    //  46
    public delegate void ChangePasswordDelegate(WarpResponseResultCode status);
    public event ChangePasswordDelegate OnChangePasswordDone;

    //  47
    public delegate void UpdateUserInfoDelegate(WarpResponseResultCode status);
    public event UpdateUserInfoDelegate OnUpdateUserInfoDone;

    //  51
    public delegate void InviteToPlayDelegate(WarpResponseResultCode status);
    public event InviteToPlayDelegate OnInviteToPlayDone;

    //  53
    //public delegate void GetSystemMessageDelegate(WarpResponseResultCode status, List<SystemMessage> data);
    //public event GetSystemMessageDelegate OnGetSystemMessageDone;
    //return OnGetMessagesDone;

    //  65
    public delegate void GetUserStatDelegate(WarpResponseResultCode status, RootStat data);
    public event GetUserStatDelegate OnGetUserStatDone;

    //  66
    //return OnGetUserAchievementDone

    //  67
    public delegate void GetDailyBonusDelegate(WarpResponseResultCode status, Reward data = null);
    public event GetDailyBonusDelegate OnClaimRewardDone;

    //  70
    public delegate void GetCurrentDailyLoginDelegate(WarpResponseResultCode status, JSONObject data);
    public event GetCurrentDailyLoginDelegate OnGetCurrentDailyLoginDone;

    //  71
    public delegate void GetPromotionConfigureDelegate(WarpResponseResultCode status, PromotionConfig data);
    public event GetPromotionConfigureDelegate OnGetPromotionConfigureDone;

    //  72
    public delegate void FeedbackDelegate(WarpResponseResultCode status);
    public event FeedbackDelegate OnFeedbackDone;

    //  76
    public delegate void LinkFBDelegate(WarpResponseResultCode status);
    public event LinkFBDelegate OnLinkFBDone;

    //  77
    public delegate void LinkAccDelegate(WarpResponseResultCode status);
    public event LinkAccDelegate OnLinkAccDone;


    //  89, 84, 62
    public delegate void TopUserDelegate(WarpResponseResultCode status, RootTopUser data);
    public event TopUserDelegate OnTopUserDone;

    //  103
    public delegate void GiftCodeDelegate(WarpResponseResultCode status);
    public event GiftCodeDelegate OnGiftCodeDone;

    //  103
    public delegate void CardProviderConfigDelegate(WarpResponseResultCode status, List<CardProvider> data);
    public event CardProviderConfigDelegate OnCardProviderConfigDone;

	// 107
	public delegate void GetRateRewardDelegate(WarpResponseResultCode status);
	public event GetRateRewardDelegate OnGetRateRewardDone;

	// 108
	public delegate void GetLikeRewardDelegate(WarpResponseResultCode status);
	public event GetRateRewardDelegate OnGetLikeRewardDone;
	#endregion

    #region FRIEND
    //  41
    public delegate void AddFriendsDelegate(WarpResponseResultCode status);
    public event AddFriendsDelegate OnAddFriendsDone;

    //  42
    public delegate void GetFriendsDelegate(WarpResponseResultCode status, List<UserData> data);
    public event GetFriendsDelegate OnGetFriendsDone;

    //  83
    public delegate void DeleteFriendDelegate(WarpResponseResultCode status);
    public event DeleteFriendDelegate OnDeleteFriendDone;
    #endregion

    #region CHAT
    public delegate void SenChatDelegate(WarpResponseResultCode status);
    public event SenChatDelegate OnSenChatDone;
    #endregion

    #region MESSAGE
    //  43
    public delegate void SentMessageDelegate(WarpResponseResultCode status);
    public event SentMessageDelegate OnSentMessageDone;

    //  44
    public delegate void GetMessagesDelegate(WarpResponseResultCode status, List<Message> data);
    public event GetMessagesDelegate OnGetMessagesDone;

    //  49
    public delegate void ReadMessageDelegate(WarpResponseResultCode status, List<Message> data);
    public event ReadMessageDelegate OnReadMessageDone;

    //  50
    public delegate void DeleteMessageDelegate(WarpResponseResultCode status);
    public event DeleteMessageDelegate OnDeleteMessagesDone;

	//  53
	public delegate void GetSystemMessagesDelegate(WarpResponseResultCode status, List<Message> data);
	public event GetSystemMessagesDelegate OnGetSystemMessagesDone;

    //  69
    public delegate void UserMessageCountDelegate(WarpResponseResultCode status, UserMessageCount data);
    public event UserMessageCountDelegate OnUserMessageCountDone;
    #endregion

    #region GAME
    //  10
    public delegate void JoinLobbyDelegate(WarpResponseResultCode status);
    public event JoinLobbyDelegate OnJoinLobbyDone;

    //  11
    public delegate void SubscribeLobbyDelegate(WarpResponseResultCode status, RootRoom data);
    public event SubscribeLobbyDelegate OnSubLobbyDone;

    //  12
    public delegate void UnsubscribeLobbyDelegate(WarpResponseResultCode status);
    public event UnsubscribeLobbyDelegate OnUnSubLobbyDone;

    //  13
    public delegate void LeaveLobbyDelegate(WarpResponseResultCode status);
    public event LeaveLobbyDelegate OnLeaveLobbyDone;

    //  14
    public delegate void ListLobbiesDelegate(WarpResponseResultCode status, List<Lobby> data);
    public event ListLobbiesDelegate OnListLobbiesDone;

    //  22
    public delegate void JoinRoomDoneDelegate(WarpResponseResultCode status, Room data);
    public event JoinRoomDoneDelegate OnJoinRoomDone;

    //  27
    public delegate void GetRoomsDelegate(WarpResponseResultCode status, List<RoomSlot> data);
    public event GetRoomsDelegate OnGetRoomsDone;

    //  30
    public delegate void SendKeepAliveDelegate(WarpResponseResultCode status);
    public event SendKeepAliveDelegate OnSendKeepAliveDone;

    //  48
    public delegate void ChangeOwnerDelegate(WarpResponseResultCode status);
    public event ChangeOwnerDelegate OnChangeOwnerDone;

    // 93
    public delegate void ListGameDelegate(WarpResponseResultCode status, List<Game> data);
    public event ListGameDelegate OnListGameDone;

    // 94
    public delegate void SubGameDelegate(WarpResponseResultCode status);
    public event SubGameDelegate OnSubGameDone;

    // 95
    public delegate void UnSubGameDelegate(WarpResponseResultCode status);
    public event UnSubGameDelegate OnUnSubGameDone;

    // 96
    //public delegate void TaiXiuRequestDelegate(WarpResponseResultCode status, TaiXiuData data, byte[] payload);
    //public event TaiXiuRequestDelegate OnTaiXiuRequestDone;

	// 107
	public delegate void LobbyInfoDelegate(WarpResponseResultCode status, List<LobbyStatus> data);
	public event LobbyInfoDelegate OnGetLobbyInfo;
    #endregion

    #region PAYMENT - CASHOUT

    //CASHOUT FLOW
    public static void CashoutFlow()
    {
        #region Get reward config --> Get list reward --> Get cashout config
        //// Lấy thông tin giải thưởng
        //WarpRequest.GetConfig(ConfigType.REWARD_CONFIG);
        //// Nếu dữ liệu trả về có data.version --> gán data.link(link ảnh cashout), data.verson --> lưu tham số và đối chiếu phiên bản 
        //WarpClient.wc.OnGetConfigDone();

        //// Lấy danh sách giải thưởng
        //WarpRequest.GetListReward();
        //// List giải thưởng với type = 0 là card, type = 1 là vật phẩm --> Dựa vào link + tên ảnh để lấy ảnh cashout từ web về và vẽ
        //WarpClient.wc.OnRewardListDone();

        //// Lấy thông tin đổi thưởng
        //WarpRequest.GetConfig(ConfigType.CASHOUT_CHANNEL);
        //// Nếu dữ liệu trả về có data.userMin --> gán thành số tiền tối thiểu sau khi đổi thưởng  
        //WarpClient.wc.OnGetConfigDone();

        ////Cashout
        //int id; // Chỉ cần id vì hiện tại server chỉ cho đổi số lượng 1 với tất cả phần thưởng nên gán cứng luôn
        //WarpRequest.CashOut(id);
        //// Dựa vào result code, status để hiển thị thông báo
        //WarpClient.wc.OnCashOutDone();
        #endregion
    }

    // 54
    public delegate void GetSMSConfigDelegate(WarpResponseResultCode status, JSONObject data);
    public event GetSMSConfigDelegate OnGetSMSConfig;

    // 55
    public delegate void CardTopupDelegate(WarpResponseResultCode status);
    public event CardTopupDelegate OnCardTopupDone;

    // 56
    public delegate void GetKoinExchangeDelegate(WarpResponseResultCode status, List<ValueTopup> data);
    public event GetKoinExchangeDelegate OnGetKoinExchangeDone;

    // 80
    public delegate void IAPTopupDelegate(WarpResponseResultCode status);
    public event IAPTopupDelegate OnIAPTopupDone;

    // 83
    public delegate void ConvertGoldToCoinDelegate(WarpResponseResultCode status, UserData data);
    public event ConvertGoldToCoinDelegate OnConvertGoldToCoinDone;

    // 87
    public delegate void CashOutDelegate(WarpResponseResultCode status, CashoutData data);
    public event CashOutDelegate OnCashOutDone;

    // 91
    public delegate void CashOutHistoryDelegate(WarpResponseResultCode status, List<CashoutHistory> data);
    public event CashOutHistoryDelegate OnCashoutHistoryDone;

    //充值历史
    public delegate void PayloadHistoryDelegate(WarpResponseResultCode status, List<PayLoadHistory> data);
    public event PayloadHistoryDelegate OnPayloadHistoryDone;

    //线上充值url
    public delegate void PayloadUrlDelegate(WarpResponseResultCode status, string data);
    public event PayloadUrlDelegate OnPayloadUrlDone;

    //普通充值回调
    public delegate void OfflinePayloadDelegate(WarpResponseResultCode status, OffLinePayloadData data);
    public event OfflinePayloadDelegate OnOffLinePayloadDone;

    public delegate void WeiHuDelegate(WarpResponseResultCode status, string data);
    public event WeiHuDelegate OnWeiHu;


    // 92
    public delegate void RewardListDelegate(WarpResponseResultCode status, List<CashoutProduct> data);
    public event RewardListDelegate OnGetCashOutListDone;

    //101
    public delegate void TransferGoldDelegate(WarpResponseResultCode status, UserData data);
    public event TransferGoldDelegate OnTransferGoldDone;

    //105
    public delegate void UserVerityDelegate(WarpResponseResultCode status, BaseData data);
    public event UserVerityDelegate OnUserVerityMobileDone;

    //106
    public delegate void GetCashOutDetailDelegate(WarpResponseResultCode status, List<CardInfo> data);
    public event GetCashOutDetailDelegate OnGetCashOutDetailDone;

    public delegate void GetPayloadDetailDelegate(WarpResponseResultCode status, List<PayLoadHistory> data);
    public event GetPayloadDetailDelegate OnGetPayloadDetailDone;

    #endregion

    #endregion

    #region Response
    private void OnResponse(WarpRequestTypeCode requestType, int resultCode, byte[] payLoad)
    {

        try
        {
            if (requestType != WarpRequestTypeCode.SEND_KEEP_ALIVE)
            {
                UILogView.Log("Request: " + requestType + " resultCode: " + (WarpResponseResultCode)resultCode);
                //if (OGUIM.GA != null)
                //{
                //    if (OGUIM.currentLobby == null)
                //        OGUIM.GA.LogEvent(new EventHitBuilder().SetEventCategory("OUT GAME").SetEventAction(requestType.ToString() + ": " + ((WarpResponseResultCode)resultCode).ToString()));
                //    else
                //        OGUIM.GA.LogEvent(new EventHitBuilder().SetEventCategory("IN GAME: " + OGUIM.currentLobby.desc + " " + OGUIM.currentLobby.subname).SetEventAction(requestType.ToString() + ": " + ((WarpResponseResultCode)resultCode).ToString()));
                //}
            }
            else
            {
                OldWarpChannel.Channel.countPendingKeepAlives = 0;
                OldWarpChannel.Channel.countPendingRecovery = 0;

                if (currentState == WarpConnectionState.RECOVERING)
                {
#if !NETFX_CORE
                    reader.Close();
#else
                    reader.Dispose();
#endif
                    reader = null;
                    currentData = null;
                    currentState = WarpConnectionState.CONNECTED;

                    UILogView.Log("RECOVERING: Done!");
                    if (OldWarpChannel.Channel.actionOnConnected != null)
                        OldWarpChannel.Channel.actionOnConnected();
                    if (OnReconnected != null)
                        OnReconnected(resultCode);
                }
            }

            #region BASE
            if (resultCode == (int)WarpResponseResultCode.INVALID_SESSION)
            {
                //Debug.Log("Session Expired: INVALID_SESSION from sendKeepAlive");
                //OGUIM.UnLoadGameScene(true);
                //return;
            }
            #endregion

            #region USER
            // 1
            if (requestType == WarpRequestTypeCode.AUTH)
            {
                StopTimeOut(requestType);

                if (resultCode == (int)WarpResponseResultCode.AUTH_ERROR)
                {
                    var data = ZenMessagePack.DeserializeObject<RootUserDataError>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                    if (OnLoginDone != null)
                        OnLoginDone((WarpResponseResultCode)resultCode, null, data.desc);
                }
                else
                {
                    RootUserData dataObject = null;
                    dataObject = ZenMessagePack.DeserializeObject<RootUserData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                    if (OnLoginDone != null)
                        OnLoginDone((WarpResponseResultCode)resultCode, dataObject, null);
                }
            }
            else if (requestType == WarpRequestTypeCode.SIGNOUT)
            {
                StopTimeOut(requestType);
                if (OnLogoutDone != null)
                    OnLogoutDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.GET_USER_INFO)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootUserInfo>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnGetUserInfoDone != null)
                    OnGetUserInfoDone((WarpResponseResultCode)resultCode, dataObject);
            }
            else if (requestType == WarpRequestTypeCode.UPDATE_AVATAR)
            {
                StopTimeOut(requestType);
                if (OnUpdateAvatarDone != null)
                    OnUpdateAvatarDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.CLAIM_REWARD)
            {
                StopTimeOut(requestType);
                if (OnClaimRewardDone != null)
                    OnClaimRewardDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.GET_CONFIG)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootConfig>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnGetConfigDone != null)
                    OnGetConfigDone((WarpResponseResultCode)resultCode, dataObject);
            }
            else if (requestType == WarpRequestTypeCode.JOIN_LOBBY)
            {
                StopTimeOut(requestType);
                if (OnJoinLobbyDone != null)
                    OnJoinLobbyDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.SUBSCRIBE_LOBBY)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootRoom>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnSubLobbyDone != null)
                    OnSubLobbyDone((WarpResponseResultCode)resultCode, dataObject);

            }
            else if (requestType == WarpRequestTypeCode.UNSUBSCRIBE_LOBBY)
            {
                StopTimeOut(requestType);
                if (OnUnSubLobbyDone != null)
                    OnUnSubLobbyDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.LEAVE_LOBBY)
            {
                StopTimeOut(requestType);
                if (OnLeaveLobbyDone != null)
                    OnLeaveLobbyDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.LIST_LOBBIES)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootLobby>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnListLobbiesDone != null)
                    OnListLobbiesDone((WarpResponseResultCode)resultCode, dataObject.lobbies);
            }
            else if (requestType == WarpRequestTypeCode.JOIN_ROOM)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<Room>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnJoinRoomDone != null)
                    OnJoinRoomDone((WarpResponseResultCode)resultCode, dataObject);
            }
            else if (requestType == WarpRequestTypeCode.GET_ROOMS)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootRoomSlot>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnGetRoomsDone != null)
                    OnGetRoomsDone((WarpResponseResultCode)resultCode, dataObject.rooms);
            }
            else if (requestType == WarpRequestTypeCode.SEND_KEEP_ALIVE)
            {
				StopTimeOut(requestType);                
				if (resultCode == (int)WarpResponseResultCode.INVALID_SESSION) 
				{
					OldWarpChannel.Channel.onLostConnection(3);  
					return;
				}

                if (OnSendKeepAliveDone != null)
                    OnSendKeepAliveDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.GET_USER_ACHIEVEMENT)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootAchievement>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnGetUserAchievementDone != null)
                    OnGetUserAchievementDone((WarpResponseResultCode)resultCode, dataObject.data);
            }
            else if (requestType == WarpRequestTypeCode.ADD_FRIENDS)
            {
                StopTimeOut(requestType);
                if (OnAddFriendsDone != null)
                    OnAddFriendsDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.GET_FRIENDS)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootFriend>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnGetFriendsDone != null)
                    OnGetFriendsDone((WarpResponseResultCode)resultCode, dataObject.friends);
            }
            else if (requestType == WarpRequestTypeCode.SEND_MESSAGE)
            {
                StopTimeOut(requestType);
                if (OnSentMessageDone != null)
                    OnSentMessageDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.GET_MESSAGES)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootMessage>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnGetMessagesDone != null)
                    OnGetMessagesDone((WarpResponseResultCode)resultCode, dataObject.messages);
            }
            else if (requestType == WarpRequestTypeCode.CHANGE_PASS)
            {
                StopTimeOut(requestType);
                if (OnChangePasswordDone != null)
                    OnChangePasswordDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.UPDATE_INFO)
            {
                StopTimeOut(requestType);
                if (OnUpdateUserInfoDone != null)
                    OnUpdateUserInfoDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.INVITE_TO_PLAY)
            {
                StopTimeOut(requestType);
                if (OnInviteToPlayDone != null)
                    OnInviteToPlayDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.CHANGE_OWNER)
            {
                StopTimeOut(requestType);
                if (OnChangeOwnerDone != null)
                    OnChangeOwnerDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.READ_MESSAGE)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootMessage>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnReadMessageDone != null)
                    OnReadMessageDone((WarpResponseResultCode)resultCode, dataObject.messages);
            }
            else if (requestType == WarpRequestTypeCode.DELETE_MESSAGES)
            {
                StopTimeOut(requestType);
                if (OnDeleteMessagesDone != null)
                    OnDeleteMessagesDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.GET_SYSTEM_MESSAGE)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootMessage>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnGetSystemMessagesDone != null)
					OnGetSystemMessagesDone((WarpResponseResultCode)resultCode, dataObject.messages);
            }
            else if (requestType == WarpRequestTypeCode.GET_SMS_CONFIG)
            {
                StopTimeOut(requestType);

                var raw = MsgPack.Unpacking.UnpackObject(payLoad).Value.ToString();
                string payloadString = raw.ToString().ToLower().Replace("{ \"value\" :", "").Replace("} ] } }", "} ] }").Replace("value10", "sms_10").Replace("value20", "sms_20").Replace("value50", "sms_50"); ;
#if DEBUG
                Debug.Log(payloadString);
#endif
                var rawObject = JsonUtility.FromJson<RootSMSValue>(payloadString);

                var dataObject = ZenMessagePack.DeserializeObject(payLoad);
                if (OnGetSMSConfig != null)
                    OnGetSMSConfig((WarpResponseResultCode)resultCode, dataObject);
            }
            else if (requestType == WarpRequestTypeCode.CARD_TOPUP)
            {
                StopTimeOut(requestType);
                if (OnCardTopupDone != null)
                    OnCardTopupDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.GET_KOIN_EXCHANGE)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootTopup>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnGetKoinExchangeDone != null)
                    OnGetKoinExchangeDone((WarpResponseResultCode)resultCode, dataObject.value);
            }
            else if (requestType == WarpRequestTypeCode.TOP_LEVEL_BY_GAME)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootTopUser>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnTopUserDone != null)
                    OnTopUserDone((WarpResponseResultCode)resultCode, dataObject);
            }
            else if (requestType == WarpRequestTypeCode.CASHOUT_HISTORY)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootCashoutHistory>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnCashoutHistoryDone != null)
                    OnCashoutHistoryDone((WarpResponseResultCode)resultCode, dataObject.data);
            }
            else if (requestType == WarpRequestTypeCode.PAY_LOAD_HISTROAY)
            {
                //充值历史
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootPayloadHistory>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                var raw = Unpacking.UnpackObject(payLoad).Value.ToString();
                PayloadHisTemp = raw.ToString();

                if (OnPayloadHistoryDone != null)
                    OnPayloadHistoryDone((WarpResponseResultCode)resultCode, dataObject.rechargeRecordList);
            }

            else if (requestType == WarpRequestTypeCode.PAY_LOAD)
            {
                //充值链接
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<PayUrlModel>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                var raw = Unpacking.UnpackObject(payLoad).Value.ToString();
                PayloadUrlTemp = raw.ToString();

                if (OnPayloadUrlDone != null)
                    OnPayloadUrlDone((WarpResponseResultCode)resultCode,dataObject.resultUrl);
            }

            else if (requestType == WarpRequestTypeCode.PAY_OFFLINE)
            {
                //线下充值回调
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<OffLinePayloadDataRoot>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                var raw = Unpacking.UnpackObject(payLoad).Value.ToString();
                OffLinePayloadTemp = raw.ToString();

                if (OnOffLinePayloadDone != null)
                    OnOffLinePayloadDone((WarpResponseResultCode)resultCode, dataObject.patamMap);
            }

            else if (requestType == WarpRequestTypeCode.GET_CASHOUT_DETAIL)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<CashoutHistoryData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnGetCashOutDetailDone != null)
                    OnGetCashOutDetailDone((WarpResponseResultCode)resultCode, dataObject.data);
            }
            else if (requestType == WarpRequestTypeCode.PAY_LOAD_HISTROAY)
            {
                //充值记录
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootPayloadHistory>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnGetPayloadDetailDone != null)
                    OnGetPayloadDetailDone((WarpResponseResultCode)resultCode, dataObject.rechargeRecordList);
            }
            else if (requestType == WarpRequestTypeCode.GET_USER_STAT)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootStat>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnGetUserStatDone != null)
                    OnGetUserStatDone((WarpResponseResultCode)resultCode, dataObject);
            }
            else if (requestType == WarpRequestTypeCode.GET_DAILY_MISSION)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootAchievement>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnGetUserAchievementDone != null)
                    OnGetUserAchievementDone((WarpResponseResultCode)resultCode, dataObject.data);
            }
            else if (requestType == WarpRequestTypeCode.GET_DAILY_BONUS)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<Reward>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnClaimRewardDone != null)
                    OnClaimRewardDone((WarpResponseResultCode)resultCode, dataObject);
            }
            else if (requestType == WarpRequestTypeCode.GET_ACHIEVEMENT_BONUS)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<Reward>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnClaimRewardDone != null)
                    OnClaimRewardDone((WarpResponseResultCode)resultCode, dataObject);
            }
            else if (requestType == WarpRequestTypeCode.GET_USER_MESSAGE_COUNT)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<UserMessageCount>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnUserMessageCountDone != null)
                    OnUserMessageCountDone((WarpResponseResultCode)resultCode, dataObject);
            }
            else if (requestType == WarpRequestTypeCode.CURRENT_DAILY_LOGIN)
            {
                StopTimeOut(requestType);
                // Du lieu nay dung JSON
				JSONObject dataObject = ZenMessagePack.DeserializeObject(payLoad);
                if (OnGetCurrentDailyLoginDone != null)
                    OnGetCurrentDailyLoginDone((WarpResponseResultCode)resultCode, dataObject);
            }
            else if (requestType == WarpRequestTypeCode.PROMOTION_CONFIGURE)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<PromotionConfig>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnGetPromotionConfigureDone != null)
                    OnGetPromotionConfigureDone((WarpResponseResultCode)resultCode, dataObject);
            }
            else if (requestType == WarpRequestTypeCode.FEEDBACK)
            {
                StopTimeOut(requestType);
                if (OnFeedbackDone != null)
                    OnFeedbackDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.LINK_FB)
            {
                StopTimeOut(requestType);
                if (OnLinkFBDone != null)
                    OnLinkFBDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.LINK_ACC)
            {
                StopTimeOut(requestType);
                if (OnLinkAccDone != null)
                    OnLinkAccDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.IAP_TOPUP)
            {
                StopTimeOut(requestType);
                if (OnIAPTopupDone != null)
                    OnIAPTopupDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.DELETE_FRIEND)
            {
                StopTimeOut(requestType);
                if (OnDeleteFriendDone != null)
                    OnDeleteFriendDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.GET_TOP_ALLLEVEL)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootTopUser>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnTopUserDone != null)
                    OnTopUserDone((WarpResponseResultCode)resultCode, dataObject);
            }
            else if (requestType == WarpRequestTypeCode.GOLD_TO_KOIN)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<UserData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnConvertGoldToCoinDone != null)
                    OnConvertGoldToCoinDone((WarpResponseResultCode)resultCode, dataObject);
            }
            else if (requestType == WarpRequestTypeCode.CASHOUT)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<CashoutData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnCashOutDone != null)
                    OnCashOutDone((WarpResponseResultCode)resultCode, dataObject);
            }
            else if (requestType == WarpRequestTypeCode.TOP_GOLD)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootTopUser>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnTopUserDone != null)
                    OnTopUserDone((WarpResponseResultCode)resultCode, dataObject);
            }
            else if (requestType == WarpRequestTypeCode.REWARD_LIST)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootCashout>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnGetCashOutListDone != null)
                    OnGetCashOutListDone((WarpResponseResultCode)resultCode, dataObject.data);
            }
            else if (requestType == WarpRequestTypeCode.LIST_GAMES)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootGame>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnListGameDone != null)
                    OnListGameDone((WarpResponseResultCode)resultCode, dataObject.games);
            }
            else if (requestType == WarpRequestTypeCode.SUBSCRIBE_GAME)
            {
                StopTimeOut(requestType);
                if (OnSubGameDone != null)
                    OnSubGameDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.UNSUBSCRIBE_GAME)
            {
                StopTimeOut(requestType);
                if (OnUnSubGameDone != null)
                    OnUnSubGameDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.TRANSFER_GOLD)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<UserData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnTransferGoldDone != null)
                    OnTransferGoldDone((WarpResponseResultCode)resultCode, dataObject);
            }
            else if (requestType == WarpRequestTypeCode.GIFT_CODE)
            {
                StopTimeOut(requestType);
                if (OnGiftCodeDone != null)
                    OnGiftCodeDone((WarpResponseResultCode)resultCode);
            }
            else if (requestType == WarpRequestTypeCode.GET_CARD_PROVIDER_CONFIG)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootCardProvider>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnCardProviderConfigDone != null)
                    OnCardProviderConfigDone((WarpResponseResultCode)resultCode, dataObject.data);
            }
            else if (requestType == WarpRequestTypeCode.LIST_GAMES)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<RootGame>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnListGameDone != null)
                    OnListGameDone((WarpResponseResultCode)resultCode, dataObject.games);

            }
            else if (requestType == WarpRequestTypeCode.USER_VERIFY_REQUEST)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<BaseData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                if (OnUserVerityMobileDone != null)
                    OnUserVerityMobileDone((WarpResponseResultCode)resultCode, dataObject);
            }
			else if (requestType == WarpRequestTypeCode.GET_RATE_REWARD)
			{
				StopTimeOut(requestType);
				if (OnGetRateRewardDone != null)
					OnGetRateRewardDone((WarpResponseResultCode)resultCode);
			}
			else if (requestType == WarpRequestTypeCode.GET_LIKE_REWARD)
			{
				StopTimeOut(requestType);
				if (OnGetLikeRewardDone != null)
					OnGetLikeRewardDone((WarpResponseResultCode)resultCode);
			}
            #endregion

            #region CHAT
            else if (requestType == WarpRequestTypeCode.GLOBAL_CHAT || requestType == WarpRequestTypeCode.CHAT_LOBBY || requestType == WarpRequestTypeCode.CHAT)
            {
                StopTimeOut(WarpNotifyTypeCode.CHAT_NOTIFICATION);
                OnSenChatDone((WarpResponseResultCode)resultCode);
            }
            #endregion

            #region MINIGAME
            else if (requestType == (WarpRequestTypeCode)MINIGAME_ID.MINI_POKER || 
                requestType == (WarpRequestTypeCode)MINIGAME_ID.MINI_SPIN || 
                requestType == (WarpRequestTypeCode)MINIGAME_ID.MINI_TAIXIU ||
                requestType == (WarpRequestTypeCode)MINIGAME_ID.MINI_CAOTHAP)
            {
                StopTimeOut(requestType);
                if (resultCode == (int)WarpResponseResultCode.SUCCESS)
                {
                    var dataObject = ZenMessagePack.DeserializeObject<Mini_RootData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);

                    StopTimeOut((MINIGAME)dataObject.type);

                    if (OnMinigameResponse != null)
                        OnMinigameResponse(dataObject, resultCode);

                    if (dataObject != null && dataObject.history != null)
                    {
                        if (dataObject.type == (int)GameRequestTypesCode_SLOT.GET_JACKPOT_HISTORY && OnJackpotHistory != null)
                        {
                            StopTimeOut(GameRequestTypesCode_SLOT.GET_JACKPOT_HISTORY);
                            OnJackpotHistory(dataObject.history, (LobbyId)requestType);
                        }
                        else if (dataObject.type == (int)GameRequestTypesCode_SLOT.GET_USER_HISTORY && OnUserHistory != null)
                        {
                            StopTimeOut(GameRequestTypesCode_SLOT.GET_USER_HISTORY);
                            OnUserHistory(dataObject.history, (LobbyId)requestType);
                        }
                    }
                }
                else if (resultCode == (int)WarpResponseResultCode.INVALID_CHIP_MIN_BET)
                {
                    var dataObject = ZenMessagePack.DeserializeObject<BaseData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                    StopTimeOut((MINIGAME)dataObject.type);
                    if (OnMinigameResponse != null)
                        OnMinigameResponse(null, resultCode);
                }
                else if (resultCode == (int)WarpResponseResultCode.BAD_REQUEST)
                {
                    var dataObject = new Mini_RootData { type = (int)MINIGAME.UNSUBSCRIBE_ROOM };
                    StopTimeOut((MINIGAME)dataObject.type);
                    if (OnMinigameResponse != null)
                        OnMinigameResponse(dataObject, resultCode);
                }
                else if (resultCode == (int)WarpResponseResultCode.SET_BET_IN_READY_MODE)
                {
                    var dataObject = ZenMessagePack.DeserializeObject<Mini_RootData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                    StopTimeOut((MINIGAME)dataObject.type);
                    if (OnMinigameResponse != null)
                        OnMinigameResponse(dataObject, resultCode);
                }
                else
                {
                    UILogView.Log("MINIGAME requestType: " + (MINIGAME_ID)requestType + "    resultCode: " + resultCode);
                }
            }
            #endregion

            else if (requestType == WarpRequestTypeCode.GET_ROOM_INFO)
            {
                //Stop time out action
                StopTimeOut(requestType);
                if (OnGetRoomInfo != null)
                {
                    OnGetRoomInfo((int)requestType, resultCode, payLoad);
                }
            }
            else if (requestType == WarpRequestTypeCode.LEAVE_ROOM)
            {
                //Stop time out action
                StopTimeOut(requestType);
                if (OnLeaveRoom != null)
                {
                    OnLeaveRoom(new Room { userId = OGUIM.me.id, room = null }, resultCode);
                }
            }
			else if (requestType == WarpRequestTypeCode.GET_LOBBY_INFO_CONFIG)
			{
				//Stop time out action
				StopTimeOut(requestType);
				var dataObject = ZenMessagePack.DeserializeObject<LobiesStatus>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
				if (OnGetLobbyInfo != null)
				{
					OnGetLobbyInfo((WarpResponseResultCode)resultCode, dataObject.list);
				}
			}

            #region UPDATE_PEERS
            if (requestType == WarpRequestTypeCode.UPDATE_PEERS)
            {
                if (resultCode == (int)WarpResponseResultCode.SUCCESS)
                {
                    if (OGUIM.currentLobby.id == (int)LobbyId.SLOT)
                    {
                        JSONObject dataObject = ZenMessagePack.DeserializeObject(payLoad);
                        if (dataObject == null)
                        {
                            UILogView.Log("OnResponse: slot data is null");
                        }
                        if ((int)dataObject["type"].n == (int)GameRequestTypesCode_SLOT.START_MATCH)
						{
                            StopTimeOut(GameRequestTypesCode_SLOT.START_MATCH);
                            UILogView.Log("------------GameRequestTypesCode_SLOT.START_MATCH---------------", false);
                            if (OnSlotStart != null)
                            {
                                OnSlotStart(dataObject);
                            }
                        }
                        else if ((int)dataObject["type"].n == (int)GameRequestTypesCode_SLOT.GET_JACKPOT_HISTORY)
                        {
                            StopTimeOut(GameRequestTypesCode_SLOT.GET_JACKPOT_HISTORY);
                            UILogView.Log("------------GameRequestTypesCode_SLOT.GET_JACKPOT_HISTORY---------------", false);
                            var slotData = ZenMessagePack.DeserializeObject<SLOT_ResultData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                            if (OnJackpotHistory != null)
                                OnJackpotHistory(slotData.slotHistory, LobbyId.SLOT);
                        }
                        else if ((int)dataObject["type"].n == (int)GameRequestTypesCode_SLOT.GET_USER_HISTORY)
                        {
                            StopTimeOut(GameRequestTypesCode_SLOT.GET_USER_HISTORY);
                            UILogView.Log("------------GameRequestTypesCode_SLOT.GET_USER_HISTORY---------------", false);
                            var slotData = ZenMessagePack.DeserializeObject<SLOT_ResultData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                            if (OnUserHistory != null)
                                OnUserHistory(slotData.slotHistory, LobbyId.SLOT);
                        }
                    }
					else if (OGUIM.currentLobby.id == (int)LobbyId.XENG_HOAQUA)
					{
						XengResponse dataObject = ZenMessagePack.DeserializeObject<XengResponse>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
						if (dataObject == null)
						{
							UILogView.Log("OnResponse: slot data is null");
						}
						else
							Debug.Log(dataObject);
						if (dataObject.type == (int)GameRequestTypesCode_XENG.START_MATCH)
						{
							StopTimeOut(GameRequestTypesCode_XENG.START_MATCH);
							UILogView.Log("------------GameRequestTypesCode_XENG.START_MATCH---------------", false);
							if (OnXengStart != null)
							{
								OnXengStart(dataObject);
							}
						}
						else if (dataObject.type == (int)GameRequestTypesCode_XENG.RECEIVE_MONEY)
						{
							StopTimeOut(GameRequestTypesCode_XENG.RECEIVE_MONEY);
							UILogView.Log("------------GameRequestTypesCode_XENG.RECEIVE_MONEY---------------", false);
							if (OnXengReceiveMoney != null)
							{
								OnXengReceiveMoney(dataObject);
							}
						}
						else if (dataObject.type == (int)GameRequestTypesCode_XENG.BET_TAI_XIU)
						{
							StopTimeOut(GameRequestTypesCode_XENG.BET_TAI_XIU);
							UILogView.Log("------------GameRequestTypesCode_XENG.BET_TAI_XIU---------------", false);
							if (OnXengBetTaiXiu != null)
							{
								OnXengBetTaiXiu(dataObject);
							}
						}
                        else if (dataObject.type == (int)GameRequestTypesCode_SLOT.GET_USER_HISTORY)
                        {
                            StopTimeOut(GameRequestTypesCode_SLOT.GET_USER_HISTORY);
                            UILogView.Log("------------GameRequestTypesCode_XENG.GET_USER_HISTORY---------------", false);
                            var slotData = ZenMessagePack.DeserializeObject<SLOT_ResultData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                            if (OnUserHistory != null)
                                OnUserHistory(slotData.slotHistory, LobbyId.XENG_HOAQUA);
                        }
                    }
                    else
                    {
                        var dataObject = ZenMessagePack.DeserializeObject<UpdatePeerData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                        if (dataObject.type == (int)WarpRequestTypeCode.GET_PLAYERS_IN_ROOM)
                        {
                            //Stop time out action
                            StopTimeOut(WarpRequestTypeCode.GET_PLAYERS_IN_ROOM);
                            UILogView.Log("------------GET_PLAYERS_IN_ROOM---------------", false);
                            if (OnGetPlayers != null)
                            {
                                OnGetPlayers(dataObject.users);
                            }
                        }
                    }
                }
                else if (resultCode == (int)WarpResponseResultCode.MAX_POT)
                {
                    StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_USER_SET_BET);
                    var dataObject = ZenMessagePack.DeserializeObject<BaseData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                    if (dataObject.type == (int)GameRequestTypesCode_XOCDIA.USER_SET_BET)
                    {
                        OGUIM.Toast.ShowNotification("Bạn không thể đặt quá 3 cửa");
                    }
                }
                else if (resultCode == (int)WarpResponseResultCode.BAD_REQUEST)
                {
                    UILogView.Log("------------BAD_REQUEST---------------", false);
                    var dataObject = ZenMessagePack.DeserializeObject<BaseData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);

                    if (dataObject.type == (int)WarpRequestTypeCode.SET_UNREADY)
                    {
                        StopTimeOut(WarpRequestTypeCode.SET_UNREADY);
                        OGUIM.Toast.ShowNotification("Đang trong ván chơi, không thể bỏ sẵn sàng");
                    }
                    else if (dataObject.type == (int)GameRequestTypesCode_TLMNDL.SUBMIT_TURN)
                    {
                        StopTimeOut(GameNotifyTypesCode_TLMNDL.SUBMIT_TURN);
                        OGUIM.Toast.ShowNotification("Đánh bài không hợp lệ");
                        if (OnSubmitFailed != null)
                        {
                            OnSubmitFailed();
                        }
                    }
                    else if (dataObject.type == (int)GameRequestTypesCode_TLMNDL.PASS_TURN)
                    {
                        StopTimeOut(GameNotifyTypesCode_TLMNDL.PASS_TURN);
                        OGUIM.Toast.ShowNotification("Không thể bỏ lượt");
                    }
                    else if (dataObject.type == (int)WarpRequestTypeCode.START_MATCH)
                    {
                        StopTimeOut(WarpNotifyTypeCode.START_MATCH);
                        OGUIM.Toast.ShowNotification(dataObject.desc.Contains("enough") ? "Chưa đủ người chơi" : "Người chơi chưa sẵn sàng");
                    }
                    else if (dataObject.type == (int)GameWarpResponseResultCode_PHOM.CANNOT_TAKE_CARD || dataObject.type == (int)GameWarpResponseResultCode_PHOM.TAKE_CARD_FIRST)
                    {
                        StopTimeOut(GameRequestTypesCode_PHOM.TAKE_CARD);
                        OGUIM.Toast.ShowNotification("Không thể ăn được");
                    }
                    else if (dataObject.type == (int)GameRequestTypesCode_PHOM.GET_CARD)
                    {
                        StopTimeOut(GameRequestTypesCode_PHOM.GET_CARD);
                        OGUIM.Toast.ShowNotification("Không thể bốc được");
                    }
                    else if (dataObject.type == (int)GameRequestTypesCode_PHOM.ATTACH_CARD_TO_SUITE)
                    {
                        StopTimeOut(GameRequestTypesCode_PHOM.ATTACH_CARD_TO_SUITE);
                        OGUIM.Toast.ShowNotification("Không thể gửi được");
                    }
                    else if (dataObject.type == (int)GameRequestTypesCode_PHOM.SUBMIT_SUITE)
                    {
                        StopTimeOut(GameRequestTypesCode_PHOM.SUBMIT_SUITE);
                        OGUIM.Toast.ShowNotification("Phỏm ko hợp lệ");
                    }
                    else if (dataObject.type == (int)GameRequestTypesCode_XOCDIA.USER_STAND_UP)
                    {
                        StopTimeOut(GameRequestTypesCode_XOCDIA.USER_STAND_UP);
                        StopTimeOut(GameRequestTypesCode_BAUCUA.USER_STAND_UP);
                        OGUIM.Toast.ShowNotification("Bạn chưa thể đứng lên trong khi " + (OGUIM.me.owner ? "làm cái" : "chơi"));
                    }
					else if (dataObject.type == (int)GameRequestTypesCode_XOCDIA.USER_BUY_POT)
					{
						OGUIM.Toast.ShowNotification("Vui lòng đợi hết thời gian bán cửa");
					}
                    else
                    {
                        UILogView.Log("BAD_REQUEST: (" + dataObject.type + ")" + dataObject.desc);
                        OGUIM.Toast.ShowNotification("Có lỗi xảy ra, vui lòng thử lại (" + dataObject.type + ")");
                        //OGUIM.UnLoadGameScene(true);
                    }
                }
                else if (resultCode == (int)WarpResponseResultCode.ROOM_STARTED)
                {
                    StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_USER_SET_BET);
                    UILogView.Log("------------ROOM_STARTED---------------", false);
                    var dataObject = ZenMessagePack.DeserializeObject<BaseData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                    if (dataObject.type == (int)GameRequestTypesCode_XOCDIA.USER_SET_BET)
                    {
                        StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_USER_SET_BET);
                        OGUIM.Toast.ShowNotification("Hiện tại chưa thể đặt cửa");
                    }
                }
                else if (resultCode == (int)WarpResponseResultCode.INVALID_CHIP_MAX_BET_PER_POT)
                {
                    StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_USER_SET_BET);
                    UILogView.Log("------------INVALID_CHIP_MAX_BET_PER_POT---------------", false);
                    var dataObject = ZenMessagePack.DeserializeObject<BaseData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                    if (dataObject.type == (int)GameRequestTypesCode_XOCDIA.USER_SET_BET)
                    {
                        StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_USER_SET_BET);
                        OGUIM.Toast.ShowNotification("Mỗi cửa chỉ được đặt 20 lần mức cược");
                    }
                }
                else if (resultCode == (int)WarpResponseResultCode.INVALID_CHIP_MAX_BET)
                {
                    StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_USER_SET_BET);
                    UILogView.Log("------------INVALID_CHIP_MAX_BET---------------", false);
                    var dataObject = ZenMessagePack.DeserializeObject<BaseData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                    if (dataObject.type == (int)GameRequestTypesCode_XOCDIA.USER_SET_BET)
                    {
                        StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_USER_SET_BET);
                        OGUIM.Toast.ShowNotification("Không đủ " + OGUIM.currentMoney.name);
                    }
                }
                else if (resultCode == (int)WarpResponseResultCode.INVALID_CHIP_MIN_BET || resultCode == (int)WarpResponseResultCode.INVALID_CHIP)
                {
                    UILogView.Log("------------INVALID_CHIP_MIN_BET | INVALID_CHIP---------------", false);
                    var dataObject = ZenMessagePack.DeserializeObject<BaseData>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                    if (dataObject.type == (int)GameRequestTypesCode_XOCDIA.USER_SET_BET)
                    {
                        StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_USER_SET_BET);

                        OGUIM.Toast.ShowNotification("Tiền cược không hợp lệ!!!");
                    }
                    if (dataObject.type == (int)GameRequestTypesCode_XOCDIA.USER_TAKE_OWNER)
                    {
                        StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_USER_TAKE_OWNER);

                        OGUIM.Toast.ShowNotification("Bạn không đủ tiền để làm cái!!!");
                    }
                    if (dataObject.type == (int)GameRequestTypesCode_XOCDIA.OWNER_SELL_POT)
                    {
                        StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_OWNER_SELL_POT);

                        OGUIM.Toast.ShowNotification("Không thể bán quá 3 cửa!!!");
                    }
                    if (dataObject.type == (int)GameRequestTypesCode_XOCDIA.USER_BUY_POT)
                    {
                        StopTimeOut(GameNotifyTypesCode_XOCDIA.NOTIFY_USER_BUY_POT);

                        OGUIM.Toast.ShowNotification("Hiện tại chưa được mua cửa!!!");
                    }
                    if(dataObject.type == (int)GameRequestTypesCode_XOCDIA.USER_SIT_DOWN)
                    {
                        OGUIM.Toast.ShowNotification("Bạn không đủ tiền để ngồi vào bàn");
                    }

                    if (dataObject.type == (int)GameRequestTypesCode_SLOT.START_MATCH)
                    {
                        StopTimeOut(GameRequestTypesCode_SLOT.START_MATCH);
                        OGUIM.Toast.ShowNotification("Không đủ tiền");
                        if (OnSlotStartFailed != null)
                            OnSlotStartFailed();
                    }
                }
            }
            #endregion
        }
        catch (Exception ex)
        {
            UILogView.Log("WarpClient: OnResponse: " + ex.Message + " " + ex.StackTrace, true);
        }
    }
    #endregion

    #region Prive Method
    void SendRequest(int calltype, int requestType, int requestId, long sessionId, int reserved, WarpContentTypeCode payloadType, MemoryStream stream)
    {
        try
        {
            if (OldWarpChannel.Channel != null && OldWarpChannel.Channel.client_socket != null && OldWarpChannel.Channel.client_socket.isConnected)
            {
                BinaryWriter writer = new BinaryWriter(new MemoryStream());
                var payLoadBytes = stream.ToArray();
                writer.Write((byte)calltype);
                writer.Write((byte)requestType);
                writer.Write(BitConverter.GetBytes((int)sessionId).Reverse().ToArray());
                writer.Write(BitConverter.GetBytes(requestId).Reverse().ToArray());
                writer.Write((byte)reserved);
                writer.Write((byte)payloadType);
                writer.Write(BitConverter.GetBytes(payLoadBytes.Length).Reverse().ToArray());
                writer.Write(payLoadBytes);

                byte[] bytes = (writer.BaseStream as MemoryStream).ToArray();
                OldWarpChannel.Channel.socket_send(bytes);
            }
            else
            {
                Debug.Log("Socket write failed with error:  " + currentState);
                if (sessionId != 0 && currentState != WarpConnectionState.RECOVERING)
                    currentState = WarpConnectionState.RECOVERING;
            }
        }
        catch (Exception exp)
        {
            UILogView.Log("Socket write failed with Exception:  " + exp.Message, true);
        }
    }

    void SendRequest(int requestTypeCode, MemoryStream stream)
    {
        var requestId = 0;
        var reserved = 0;
        if (stream != null)
            SendRequest((int)WarpMessageTypeCode.REQUEST, requestTypeCode, requestId, sessionId, reserved, WarpContentTypeCode.MESSAGE_PACK, stream);
        else
            SendRequest((int)WarpMessageTypeCode.REQUEST, requestTypeCode, requestId, sessionId, reserved, WarpContentTypeCode.MESSAGE_PACK, new ZenDictionary { str = "z" }.ToStream());
    }

    IEnumerator CreateTimeOut(float timeOutDuration, Action onTimeOut)
    {
        yield return new WaitForSeconds(timeOutDuration);
        onTimeOut();
    }

    void StopTimeOut(object requestType)
    {
        OGUIM.Toast.deactvie.SetActive(false);
        Coroutine handle;
        if (timeOutHelper.TryGetValue(requestType + "", out handle))
        {
            StopCoroutine(handle);
            timeOutHelper.Remove(requestType + "");
        }
    }
    #endregion

    #region Public Method
    public void Send(object requestTypeCode, MemoryStream data, Action actionTimeOut = null, float timeOutDuration = -1)
    {
        if (timeOutHelper.IsWorking(requestTypeCode.ToString()))
        {
            UILogView.Log(requestTypeCode.ToString() + " is requesting...");
        }
        else
        {
            if (actionTimeOut != null)
            {
                var type = requestTypeCode;
                var timeOutHandle = CreateTimeOut(timeOutDuration == -1 ? OldWarpChannel.Channel.recieveTimeOut : timeOutDuration, () =>
                {
                    StopTimeOut(type);
                    actionTimeOut();
                });
                timeOutHelper.Start(requestTypeCode.ToString(), StartCoroutine(timeOutHandle));
            }
            SendRequest((int)requestTypeCode, data);
        }
    }

    public void Send(object requestTypeCode, object notifyTypeCode, MemoryStream data, Action onTimeOutAction, float timeOutDuration = -1)
    {
        if (timeOutHelper.IsWorking(notifyTypeCode.ToString()))
        {
            UILogView.Log(notifyTypeCode.ToString() + " is requesting..", true);
        }
        else
        {
            if (onTimeOutAction != null)
            {
                var timeOutHandle = CreateTimeOut(timeOutDuration == -1 ? OldWarpChannel.Channel.recieveTimeOut : timeOutDuration, () =>
                {
                    StopTimeOut(notifyTypeCode);
                    onTimeOutAction();
                });
                timeOutHelper.Start(notifyTypeCode.ToString(), StartCoroutine(timeOutHandle));
            }
            SendRequest((int)requestTypeCode, data);
        }
    }
    #endregion

	#region Fake Method
	public void FakeGetLobbyInfo()
	{
		if (OnGetLobbyInfo != null)
		{
			OnGetLobbyInfo(WarpResponseResultCode.BAD_REQUEST, null);
		}
	}
	#endregion
}