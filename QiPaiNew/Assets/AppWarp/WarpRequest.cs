using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using MsgPack.Serialization;

public class WarpRequest
{
    // ["AUTH"] =  1,
    public static void Login(LoginType loginType, Action onLoginFail = null)
    {
        var buildLogin = BuildLogin(loginType);
        if (buildLogin != null)
        {
			WarpClient.wc.Send(WarpRequestTypeCode.AUTH, BuildLogin(loginType), () => OGUIM.Toast.Show("Đang tiến hành đăng nhập...", UIToast.ToastType.Warning));
        }
        else if (WarpClient.currentState == WarpConnectionState.RECOVERING)
        {
            //OGUIM.UnLoadGameScene(true);
        }
    }

    // ["SIGNOUT"] = 4,
    public static void Logout()
    {
        WarpClient.wc.Send(WarpRequestTypeCode.SIGNOUT, null, null);
    }

    // ["GET_USER_INFO"] = 5,
    public static void GetUserInfo(int _userId)
    {
        userRequest _params = new userRequest();
        _params.userId = _userId;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.GET_USER_INFO, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Lấy thông tin người chơi thất bại...", UIToast.ToastType.Warning));
    }

    // ["UPDATE_AVATAR"] = 6,
    public static void UpdateAvatar(string _avatar)
    {
        avaRequest _params = new avaRequest();
        _params.avatar = _avatar;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_AVATAR, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Cập nhật hình đại diện thất bại...", UIToast.ToastType.Warning));
    }

    // ["CLAIM_REWARD"] = 7,
    public static void ClaimReward(int _type, string _value)
    {
        claimRequest _params = new claimRequest();
        _params.type = _type;
        _params.value = _value;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.CLAIM_REWARD, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Nhận thưởng thất bại...", UIToast.ToastType.Warning));
    }

    // ["GET_CONFIG"] = 8,
    public static void GetConfig(ConfigType _configType)
    {
        configRequest _params = new configRequest();
        _params.type = (int)_configType;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.GET_CONFIG, BuildParamsRequest(strData), () => 
		{
			if (_configType == ConfigType.LIKE_RATE_REWARD)
			{
				OGUIM.me.isLikeReward = true;
				OGUIM.me.isRateReward = true;
				OGUIM.instance.SetGiftButton();
			}
			else
			{
				OGUIM.Toast.Show("Lấy dữ liệu thiết lập thất bại...", UIToast.ToastType.Warning);
			}
		});

				
    }

    // ["JOIN_LOBBY"] = 10,
    public static void JoinLobby(int _lobbyId)
    {
        lobbyRequest _params = new lobbyRequest();
        _params.lobbyId = _lobbyId;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.JOIN_LOBBY, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Vào game thất bại...", UIToast.ToastType.Warning));
    }

    // ["SUBSCRIBE_LOBBY"] = 11,
    public static void SubLobby(int _lobbyId)
    {
        lobbyRequest _params = new lobbyRequest();
        _params.lobbyId = _lobbyId;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.SUBSCRIBE_LOBBY, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Vào game thất bại...", UIToast.ToastType.Warning));
    }

    // ["UNSUBSCRIBE_LOBBY"] = 12,
    public static void UnSubLobby(int _lobbyId)
    {
        lobbyRequest _params = new lobbyRequest();
        _params.lobbyId = _lobbyId;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.UNSUBSCRIBE_LOBBY, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Thoát game thất bại...", UIToast.ToastType.Warning));
    }

    // ["LEAVE_LOBBY"] = 13,
    public static void LeaveLobby(int _lobbyId)
    {
        lobbyRequest _params = new lobbyRequest();
        _params.lobbyId = _lobbyId;
        var strData = JsonUtility.ToJson(_params);
        Debug.Log(strData);
        WarpClient.wc.Send(WarpRequestTypeCode.LEAVE_LOBBY, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Thoát game thất bại...", UIToast.ToastType.Warning));
    }

    // ["LIST_LOBBIES"] = 14,
    public static void ListLobbies()
    {
        gameRequest _params = new gameRequest();
        _params.gameId = -1;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.LIST_LOBBIES, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Lấy thông tin game thất bại...", UIToast.ToastType.Warning));
    }

    //["JOIN_ROOM"] = 22,
    public static void JoinRoom(MoneyType _type, int _bet)
    {
        joinRoomRequest _params = new joinRoomRequest();
        _params.bet = _bet;
        _params.type = (int)_type;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.JOIN_ROOM, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Vào phòng thất bại...", UIToast.ToastType.Warning));
    }

    // ["GET_ROOMS"] = 27,	
    public static void GetRooms(int _lobbyId)
    {
        lobbyRequest _params = new lobbyRequest();
        _params.lobbyId = _lobbyId;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.GET_ROOMS, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Lấy dữ liệu phòng thất bại...", UIToast.ToastType.Warning));
    }

    //["SEND_KEEP_ALIVE"] = 30,
    public static void SendKeepAlive()
    {
        WarpClient.wc.Send(WarpRequestTypeCode.SEND_KEEP_ALIVE, null, null);
        WeiHuWarpClient.wc.Send(WarpRequestTypeCode.SEND_WEIHU, null, null);
    }



    // ["GET_USER_ACHIEVEMENT"] = 40,
    public static void GetUserAchievement()
    {
        WarpClient.wc.Send(WarpRequestTypeCode.GET_USER_ACHIEVEMENT, BuildNonParamsRequest(), () => OGUIM.Toast.Show("Lấy thành tựu thất bại...", UIToast.ToastType.Warning));
    }

    // ["ADD_FRIENDS"] = 41,
    public static void AddFriends(int _friendId)
    {
        friendRequest _params = new friendRequest();
        _params.friendId = _friendId;
        var strData = JsonUtility.ToJson(_params);
        Debug.Log(strData);
        WarpClient.wc.Send(WarpRequestTypeCode.ADD_FRIENDS, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Kết bạn thất bại...", UIToast.ToastType.Warning));
    }

    // 	["GET_FRIENDS"] = 42,
    public static void GetFriends()
    {
        WarpClient.wc.Send(WarpRequestTypeCode.GET_FRIENDS, BuildNonParamsRequest(), () => OGUIM.Toast.Show("Lấy danh sách bạn bè thất bại...", UIToast.ToastType.Warning));
    }

    //	["SEND_MESSAGE"] = 43,
    public static void SendMessage(int _userId, string _title, string _message)
    {
        sendMessageRequest _params = new sendMessageRequest();
        _params.userId = _userId;
        _params.title = _title;
        _params.message = _message;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.SEND_MESSAGE, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Gửi tin nhắn thất bại...", UIToast.ToastType.Warning));
    }

    //  ["GET_MESSAGES"] = 44,
    public static void GetMessages()
    {
        getMessageRequest _params = new getMessageRequest();
        _params.status = -1;
        _params.page = 0;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.GET_MESSAGES, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Lấy danh sách tin nhắn thất bại...", UIToast.ToastType.Warning));
    }

    //  ["CHANGE_PASS"] = 46,
    public static void ChangePassword(string _oldPassword, string _newPassword)
    {
        changepassRequest _params = new changepassRequest();
        _params.oldPassword = _oldPassword;
        _params.newPassword = _newPassword;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.CHANGE_PASS, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Thay đổi mật khẩu thất bại...", UIToast.ToastType.Warning));
    }

    //修改个人信息
    //  ["UPDATE_INFO"] = 47,
    public static void UpdateUserInfo(string _name, string _mobile = "", int _gender = 0, string _email = "", string _address = "", int _passport = 0)
    {
        updateinfoRequest _params = new updateinfoRequest();
        _params.name = _name;
        if (!string.IsNullOrEmpty(_email))
            _params.email = _email;
        if (!string.IsNullOrEmpty(_address))
            _params.address = _address;
        _params.gender = _gender;
        int mobile = 0;
        if (int.TryParse(_mobile, out mobile))
            _params.mobile = mobile;
        else
            _params.mobile = mobile;

        _params.passport = _passport;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.UPDATE_INFO, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Cập nhật thông tin cá nhân thất bại...", UIToast.ToastType.Warning));
    }

    // ["READ_MESSAGE"] = 49,
    public static void ReadMessage(int _msgId)
    {
        messageRequest _params = new messageRequest();
        _params.msgId = _msgId;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.READ_MESSAGE, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Đọc tin nhắn thất bại...", UIToast.ToastType.Warning));
    }

    // ["DELETE_MESSAGES"] = 50,
    public static void DeleteMessages(List<string> _messagesId)
    {
        deleteMessRequest _params = new deleteMessRequest();
        for (int i = 0; i < _messagesId.Count; i++)
        {
            if (i < _messagesId.Count - 1)
                _params.msgIds = _params.msgIds + _messagesId[i] + ",";
            else if (i == _messagesId.Count - 1)
                _params.msgIds = _params.msgIds + _messagesId[i];
        }
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.DELETE_MESSAGES, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Xóa tin nhắn thất bại...", UIToast.ToastType.Warning));
    }

	// ["INVITE_TO_PLAY"] = 51,
	public static void InviteToPlay()
	{
		WarpClient.wc.Send(WarpRequestTypeCode.INVITE_TO_PLAY, BuildNonParamsRequest(), () => OGUIM.Toast.Show("Mời người chơi thất bại...", UIToast.ToastType.Warning));
	}

    // ["GET_SYSTEM_MESSAGE"] = 53,
    public static void GetSystemMessage(SystemMessageType _systemMessageType)
    {
        configRequest _params = new configRequest();
        _params.type = (int)_systemMessageType;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.GET_SYSTEM_MESSAGE, BuildParamsRequest(strData));
    }

    // ["GET_SMS_CONFIG"] = 54,
    public static void GetSMSConfig()
    {
        WarpClient.wc.Send(WarpRequestTypeCode.GET_SMS_CONFIG, BuildNonParamsRequest(), () => OGUIM.Toast.Show("Lấy cú pháp sms thất bại...", UIToast.ToastType.Warning));
    }

    // ["CARD_TOPUP"] = 55,
    public static void CardTopup(string _serial, string _pin, string _provider)
    {
        topupRequest _params = new topupRequest();
        _params.serial = _serial;
        _params.pin = _pin;
        _params.provider = _provider;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.CARD_TOPUP, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Gửi thông tin thẻ cào thất bại...", UIToast.ToastType.Warning));
    }

    /// <summary>
    /// 线上支付
    /// </summary>
    /// <param name="_userId">id</param>
    /// <param name="_account">支付账号</param>
    /// <param name="_channel">支付类型</param>
    /// <param name="_amount">金额</param>
    /// <param name="_bankCode">银行编码</param>
    public static void QRCodePay(string _userId, string _account, string _channel , int _amount ,string _bankCode)
    {
        PayRequest _params = new PayRequest();
        _params.userId = _userId ;
        _params.account = _account;
        _params.channel = _channel;
        _params.amount = _amount;
        _params.bankCode = _bankCode;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.PAY_LOAD, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Phản hồi thất bại...", UIToast.ToastType.Warning));
    }
    /// <summary>
    /// 线下支付
    /// </summary>
    /// <param name="_userId">id</param>
    /// <param name="_account">账号</param>
    /// <param name="_channel">类型</param>
    /// <param name="_amount">金额</param>>
    /// <param name="_phone">手机号</param>
    /// <param name="_payUsername">姓名</param>
    public static void OffLinePay(string _userId, string _account, string _channel, int _amount, string _phone,string _payUsername)
    {
        OffLinePayRequest _params = new OffLinePayRequest();
        _params.userId = _userId;
        _params.account = _account;
        _params.channel = _channel;
        _params.amount = _amount;
        _params.phone = _phone;
        _params.payUsername = _payUsername;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.PAY_OFFLINE, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Phản hồi thất bại...", UIToast.ToastType.Warning));
    }
    /// <summary>
    /// 查询充值历史
    /// </summary>
    /// <param name="_userId">id</param>
    public static void PayloadHistoryView(string _userId)
    {
        PayloadHistoryRequest _params = new PayloadHistoryRequest();
        _params.userId = _userId;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.PAY_LOAD_HISTROAY, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Truy vấn thất bại...", UIToast.ToastType.Warning));
    }

    public static void GetPayloadDetail(int _questId)
    {
        var _params = new idRequest();
        _params.id = _questId;
        var strData = JsonUtility.ToJson(_params);
        Debug.Log(strData);
        WarpClient.wc.Send(WarpRequestTypeCode.GET_CASHOUT_DETAIL, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Lấy thông tin đổi thưởng thất bại...", UIToast.ToastType.Warning));
    }


    // ["GET_KOIN_EXCHANGE"] = 56,
    public static void GetKoinExchange()
    {
        WarpClient.wc.Send(WarpRequestTypeCode.GET_KOIN_EXCHANGE, BuildNonParamsRequest(), () => OGUIM.Toast.Show("Lấy tỉ giá nạp tiền thất bại...", UIToast.ToastType.Warning));
    }

    // ["CHANGE_OWNER"] = 48,
    public static void ChangeOwner(int _userId)
    {
        userRequest _params = new userRequest();
        _params.userId = _userId;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.CHANGE_OWNER, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Thay đổi chủ phòng thất bại...", UIToast.ToastType.Warning));
    }

    // ["TOP_LEVEL_BY_GAME"] = 62,	
    public static void GetTopLevelByGame(LobbyId _id)
    {
        getTopRequest _params = new getTopRequest();
        _params.zone_id = (int)_id;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.TOP_LEVEL_BY_GAME, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Lấy bảng xếp hạng thất bại...", UIToast.ToastType.Warning));
    }

    // ["GET_USER_STAT"] = 65,
    public static void GetUserStat(int _userId)
    {
        userRequest _params = new userRequest();
        _params.userId = _userId;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.GET_USER_STAT, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Lấy chiến tích người chơi thất bại...", UIToast.ToastType.Warning));
    }

    // ["GET_DAILY_MISSION"] = 66,
    public static void GetDailyMission()
    {
        WarpClient.wc.Send(WarpRequestTypeCode.GET_DAILY_MISSION, BuildNonParamsRequest(), () => OGUIM.Toast.Show("Lấy thông tin đăng nhập hàng ngày thất bại...", UIToast.ToastType.Warning));
    }

    // ["GET_DAILY_BONUS"] = 67,
    public static void ClaimDailyQuestBonus(int _questId)
    {
        idRequest _params = new idRequest();
        _params.id = _questId;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.GET_DAILY_BONUS, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Nhận thưởng nhiệm vụ thất bại...", UIToast.ToastType.Warning));
    }

    // ["GET_ACHIEVEMENT_BONUS"] = 68,
    public static void ClaimAchievementBonus(int _achieveId)
    {
        idRequest _params = new idRequest();
        _params.id = _achieveId;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.GET_ACHIEVEMENT_BONUS, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Nhận thưởng thành tựu thất bại...", UIToast.ToastType.Warning));
    }

    // ["GET_USER_MESSAGE_COUNT"] = 69,
    public static void GetUserMessageCount()
    {
        WarpClient.wc.Send(WarpRequestTypeCode.GET_USER_MESSAGE_COUNT, BuildNonParamsRequest(), () => OGUIM.Toast.Show("Lấy thông số tin nhắn thất bại...", UIToast.ToastType.Warning));
    }

    // ["CURRENT_DAILY_LOGIN"] = 70,
    public static void GetCurrentDailyLogin()
    {
        WarpClient.wc.Send(WarpRequestTypeCode.CURRENT_DAILY_LOGIN, BuildNonParamsRequest(), () => OGUIM.Toast.Show("Lấy thông tin đăng nhập hàng ngày thất bại...", UIToast.ToastType.Warning));
    }

    // ["PROMOTION_CONFIGURE"] = 71,
    public static void GetPromotionConfigure()
    {
        WarpClient.wc.Send(WarpRequestTypeCode.PROMOTION_CONFIGURE, null, () => OGUIM.Toast.Show("Lấy thông tin khuyến mại thất bại...", UIToast.ToastType.Warning));
    }

    //  ["FEEDBACK"] = 72,
    public static void Feedback(FeedbackType _feedBackType, string _content)
    {
        feedbackRequest _params = new feedbackRequest();
        _params.userId = OGUIM.me.id;
        _params.clientVersion = GameBase.clientVersion;
        _params.platform = GameBase.platform;
        _params.model = GameBase.model;
        _params.type = (int)_feedBackType;
        _params.content = _content;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.FEEDBACK, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Phản hồi thất bại...", UIToast.ToastType.Warning));
    }

	//  ["LINK_FB"] = 76,
	public static void LinkFB(string _fbId, string _fbToken)
	{
		loginRequest _params = new loginRequest ();
		_params.fbId = _fbId;
		_params.fbToken = _fbToken;
		var strData = JsonUtility.ToJson(_params);
		WarpClient.wc.Send(WarpRequestTypeCode.LINK_FB, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Kết nối tài khoản với FB thất bại...", UIToast.ToastType.Warning));
	}

	//  ["LINK_ACC"] = 77,
	public static void LinkAcc(string _username, string _password)
	{
		loginRequest _params = new loginRequest ();
		_params.username = _username;
		_params.password = _password;
		var strData = JsonUtility.ToJson(_params);
		WarpClient.wc.Send(WarpRequestTypeCode.LINK_ACC, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Kết nối tài khoản với FB thất bại...", UIToast.ToastType.Warning));
	}
		
    // IAP_TOPUP = 80,
    public static void IapTopup(string _item, string _transId, string _transDate, string _packageName, string _productId, string _token, string _provider, string _receipt)
    {
        iapRequest _params = new iapRequest();
        _params.item = _item;
        _params.transId = _transId;
        _params.transDate = _transDate;
        _params.packageName = _packageName;
        _params.productId = _productId;
        _params.token = _token;
        _params.provider = _provider;
        _params.receipt = _receipt;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.IAP_TOPUP, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Gửi thông tin IAP thất bại...", UIToast.ToastType.Warning));
    }

    // 	["DELETE_FRIEND"] = 83,
    public static void DeleteFriend(int _friendId)
    {
        friendRequest _params = new friendRequest();
        _params.friendId = _friendId;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.DELETE_FRIEND, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Xóa bạn thất bại...", UIToast.ToastType.Warning));
    }

    // ["GET_TOP_ALLLEVEL"] = 84,
    public static void GetTopLevel()
    {
        WarpClient.wc.Send(WarpRequestTypeCode.GET_TOP_ALLLEVEL, BuildNonParamsRequest(), () => OGUIM.Toast.Show("Lấy danh sách cao thủ thất bại...", UIToast.ToastType.Warning));
    }

    // ["GOLD_TO_KOIN"] = 86,
    public static void ConvertGoldToCoin(int _userId, int _amount, string _message)
    {
        transferRequest _params = new transferRequest();
        _params.userId = _userId;
        _params.amount = _amount;
        _params.message = _message;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.GOLD_TO_KOIN, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Chuyển đổi thất bại...", UIToast.ToastType.Warning));
    }

    // ["CASH_OUT"] = 87,
    public static void CashOut(int _id)
    {
        cashoutRequest _params = new cashoutRequest();
        _params.id = _id;
        _params.quantity = 1;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.CASHOUT, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Đổi thưởng thất bại...", UIToast.ToastType.Warning));
    }


    // ["TOP_GOLD"] = 89,
    public static void GetTopGold(int _lobbyId)
    {
        idRequest _params = new idRequest();
        _params.id = _lobbyId;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.TOP_GOLD, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Lấy danh sách đại gia thất bại...", UIToast.ToastType.Warning));
    }

    // ["CASHOUT_HISTORY"] = 91,
    public static void GetCashoutHistory()
    {
        WarpClient.wc.Send(WarpRequestTypeCode.CASHOUT_HISTORY, BuildNonParamsRequest(), () => OGUIM.Toast.Show("Lấy lịch sử đổi thưởng thất bại...", UIToast.ToastType.Warning));
    }

    // ["REWARD_LIST"] = 92,
    public static void GetCashOutList()
    {
        WarpClient.wc.Send(WarpRequestTypeCode.REWARD_LIST, BuildNonParamsRequest(), () => OGUIM.Toast.Show("Lấy danh sách phần thưởng thất bại...", UIToast.ToastType.Warning));
    }

    //  ["LIST_GAMES"] = 93;
    public static void GetListGame()
    {
        WarpClient.wc.Send(WarpRequestTypeCode.LIST_GAMES, null, () => OGUIM.Toast.Show("Lấy danh sách game thất bại...", UIToast.ToastType.Warning));
    }

    // ["SUBSCRIBE_GAME"] = 94;
    public static void SubGame(int _gameId)
    {
        gameRequest _params = new gameRequest();
        _params.gameId = _gameId;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.SUBSCRIBE_GAME, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Vào game thất bại...", UIToast.ToastType.Warning));
    }

    // ["UNSUBSCRIBE_GAME"] = 95;
    public static void UnSubGame(int _gameId)
    {
        gameRequest _params = new gameRequest();
        _params.gameId = _gameId;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.UNSUBSCRIBE_GAME, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Thoát game thất bại...", UIToast.ToastType.Warning));
    }

    // ["TAI_XIU"] = 95;
    public static void TaiXiuRequest(taixiuRequest _params)
    {
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.TAI_XIU, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Gửi yêu cầu thất bại...", UIToast.ToastType.Warning));
    }

    // ["TRANSFER_GOLD"] = 101,
    public static void TransferGold(int _userId, int _amount, string _message)
    {
        transferRequest _params = new transferRequest();
        _params.userId = _userId;
        _params.amount = _amount;
        _params.message = _message;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.TRANSFER_GOLD, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Chuyển tiền thất bại...", UIToast.ToastType.Warning));
    }

    // ["GIFT_CODE"] = 103,
    public static void EnterGiftCode(string _code)
    {
        giftCodeRequest _params = new giftCodeRequest();
        _params.code = _code;
        var strData = JsonUtility.ToJson(_params);
        WarpClient.wc.Send(WarpRequestTypeCode.GIFT_CODE, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Nhập giftcode thất bại...", UIToast.ToastType.Warning));
    }

    // ["GET_CARD_PROVIDER_CONFIG"] = 104,
    public static void GetCardProviderConfig()
    {
        WarpClient.wc.Send(WarpRequestTypeCode.GET_CARD_PROVIDER_CONFIG, BuildNonParamsRequest(), () => OGUIM.Toast.Show("Lấy danh sách nhà cung cấp thất bại...", UIToast.ToastType.Warning));
    }


	// ["USER_VERIFY_REQUEST"] = 105,
	// _type 0 : get otp 
	// _type 1 : verity otp
	public static void UserVerifyPhone(int _type, string _mobile, string _pin)
	{
		var _params = new verifyRequestPhone ();
		_params.type = _type;
		_params.mobile = _mobile;
		_params.pin = _pin;
		var strData = JsonUtility.ToJson(_params);
		WarpClient.wc.Send(WarpRequestTypeCode.USER_VERIFY_REQUEST, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Xác thực thất bại...", UIToast.ToastType.Warning));
	}

    // ["GET_CASHOUT_DETAIL"] = 106,
    public static void GetCashOutDetail(int _questId)
    {
        var _params = new idRequest();
        _params.id = _questId;
        var strData = JsonUtility.ToJson(_params);
		Debug.Log (strData);
        WarpClient.wc.Send(WarpRequestTypeCode.GET_CASHOUT_DETAIL, BuildParamsRequest(strData), () => OGUIM.Toast.Show("Lấy thông tin đổi thưởng thất bại...", UIToast.ToastType.Warning));
    }

	// ["GET_LOBBY_INFO_CONFIG"] = 107,
	public static void GetLobbyInfoConfig()
	{
		Debug.Log ("GET_LOBBY_INFO_CONFIG");
		//WarpClient.wc.Send(WarpRequestTypeCode.GET_LOBBY_INFO_CONFIG, BuildNonParamsRequest(), () => OGUIM.Toast.Show("Lấy trạng thái các phòng thất bại...", UIToast.ToastType.Warning));
		WarpClient.wc.Send(WarpRequestTypeCode.GET_LOBBY_INFO_CONFIG, BuildNonParamsRequest(), () => 
			WarpClient.wc.FakeGetLobbyInfo()
		);

	}
		
	//GET_RATE_REWARD = 108,
	public static void GetRateReward()
	{
		Debug.Log ("GET_RATE_REWARD");
		//WarpClient.wc.Send(WarpRequestTypeCode.GET_LOBBY_INFO_CONFIG, BuildNonParamsRequest(), () => OGUIM.Toast.Show("Lấy trạng thái các phòng thất bại...", UIToast.ToastType.Warning));
		WarpClient.wc.Send(WarpRequestTypeCode.GET_RATE_REWARD, BuildNonParamsRequest());

	}

	//GET_LIKE_REWARD = 109,
	public static void GetLikeReward()
	{
		Debug.Log ("GET_LIKE_REWARD");
		//WarpClient.wc.Send(WarpRequestTypeCode.GET_LOBBY_INFO_CONFIG, BuildNonParamsRequest(), () => OGUIM.Toast.Show("Lấy trạng thái các phòng thất bại...", UIToast.ToastType.Warning));
		WarpClient.wc.Send(WarpRequestTypeCode.GET_LIKE_REWARD, BuildNonParamsRequest());

	}


    //拼接登陆参数
    public static MemoryStream BuildLogin(LoginType loginType)
    {
        OGUIM.loginType = loginType;

        var job = new UserLogin();
        job.loginType = (int)OGUIM.loginType;

        if (OGUIM.loginType == LoginType.USER_PASS)
        {
            if (string.IsNullOrEmpty(OGUIM.me.username) || string.IsNullOrEmpty(OGUIM.me.password))
                TryGetLastLogin(loginType);
            job.username = OGUIM.me.username;
            job.password = OGUIM.me.password;
            if (string.IsNullOrEmpty(OGUIM.me.username) || string.IsNullOrEmpty(OGUIM.me.password))
                return null;
        }
        else if (OGUIM.loginType == LoginType.FACEBOOK)
        {
            if (string.IsNullOrEmpty(OGUIM.me.faceBookId) || string.IsNullOrEmpty(OGUIM.me.facebookAccessToken))
                TryGetLastLogin(loginType);

            job.fbId = OGUIM.me.faceBookId;
            job.fbToken = OGUIM.me.facebookAccessToken;

            if (string.IsNullOrEmpty(OGUIM.me.faceBookId) || string.IsNullOrEmpty(OGUIM.me.facebookAccessToken))
                return null;
        }

        job.apiKey = GameBase.api_key;

        if (!string.IsNullOrEmpty(GameBase.clientVersion))
            job.client_version = GameBase.clientVersion;
        if (!string.IsNullOrEmpty(GameBase.platform))
            job.platform = GameBase.platform;
        if (!string.IsNullOrEmpty(GameBase.osVersion))
            job.os_version = GameBase.osVersion;
        if (!string.IsNullOrEmpty(GameBase.model))
            job.model = GameBase.model;
        if (!string.IsNullOrEmpty(GameBase.device_uuid))
            job.device_uuid = GameBase.device_uuid;
        if (!string.IsNullOrEmpty(GameBase.providerCode))
            job.provider_code = GameBase.providerCode;
        if (!string.IsNullOrEmpty(GameBase.refCode))
            job.refcode = GameBase.refCode;

#if DEBUG
        //job.device_uuid = "Yogame" + DateTime.Now.Ticks;
#endif

        var ms = ZenMessagePack.SerializeObject<UserBase>(job);
        return ms;
    }

    public static void TryGetLastLogin(LoginType loginType)
    {
        if (loginType == LoginType.USER_PASS)
        {
            if (string.IsNullOrEmpty(OGUIM.me.username))
                OGUIM.me.username = PlayerPrefs.GetString("username", "");
            if (string.IsNullOrEmpty(OGUIM.me.password))
                OGUIM.me.password = PlayerPrefs.GetString("password", "");
        }
        else if (loginType == LoginType.FACEBOOK)
        {
            if (string.IsNullOrEmpty(OGUIM.me.faceBookId))
                OGUIM.me.faceBookId = PlayerPrefs.GetString("faceBookId", "");
            if (string.IsNullOrEmpty(OGUIM.me.facebookAccessToken))
                OGUIM.me.facebookAccessToken = PlayerPrefs.GetString("facebookAccessToken", "");
        }
    }

    private static MemoryStream BuildParamsRequest(string message)
    {
        var warpMessage = new MemoryStream();
        // 1. Create serializer instance.
        var serializer = MessagePackSerializer.Get<string>();
        // 2. Serialize object to the specified stream.
        serializer.Pack(warpMessage, message);
        return warpMessage;
    }

    private static MemoryStream BuildNonParamsRequest()
    {
        string message = "{}";
        var warpMessage = new MemoryStream();
        // 1. Create serializer instance.
        var serializer = MessagePackSerializer.Get<string>();
        // 2. Serialize object to the specified stream.
        serializer.Pack(warpMessage, message);
        return warpMessage;
    }

}
