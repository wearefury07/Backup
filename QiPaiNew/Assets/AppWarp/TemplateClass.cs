using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class loginRequest
{
    public int loginType;

    public string username;
    public string password;

    public string fbId;
    public string fbToken;

    public string client_version;
    public string platform;
    public string os_version;
    public string model;
    public string device_uuid;
    public string provider_code;
    public string refcode;
    public string apiKey;
}

public class roomRequest
{
    public int type;
    public int sessionId;
    public int roomId;
    public string message;
    public string pack;
    public int bet;
    public int pot;
}

public class SlotRequest
{
    public int type;
    public int deal;
    public List<int> line;
}

[Serializable]
public class MAUBINHRequest
{
    public int type;
    public List<List<CardData>> suites;
}

public class gameRequest
{
    public int gameId;
}

public class lobbyRequest
{
    public int lobbyId;
}

public class userRequest
{
    public int userId;
}

public class joinRoomRequest
{
    public int bet;
    public int type;
}

public class getTopRequest
{
    public int zone_id;
}

public class avaRequest
{
    public string avatar;
}

public class messageRequest
{
    public int msgId;
}

public class claimRequest
{
    public int type;
    public string value;
}

public class idRequest
{
    public int id;
}

public class stringIdRequest
{
    public string id;
}

public class changepassRequest
{
    public string oldPassword;
    public string newPassword;
}

public class updateinfoRequest
{
    public string name;
    public string trueName;
    public string email = "chưa cập nhật";
    public string address = "chưa cập nhật";
    public int gender;
    public int mobile;
    public int passport;
}

public class friendRequest
{
    public int friendId;
}

public class getMessageRequest
{
    public int status;
    public int page;
}

public class sendMessageRequest
{
    public int userId;
    public string title;
    public string message;
}

public class deleteMessRequest
{
    public string msgIds;
}

public class topupRequest
{
    public string serial;
    public string pin;
    public string provider;
}

/// <summary>
/// 线上支付
/// </summary>
public class PayRequest
{
    public string userId;//用户id
    public string account;//账号
    public string channel;//支付类型
    public int amount;//金额
    public string bankCode;//银行编码
}
/// <summary>
/// 线下支付
/// </summary>
public class OffLinePayRequest
{
    public string userId;//用户id
    public string account;//账号
    public string channel;//支付类型
    public int amount;//金额
    public string bankCode;//银行编码
    public string phone;//手机号
    public string payUsername; //付款人姓名

}
/// <summary>
/// 查询支付记录
/// </summary>
public class PayloadHistoryRequest
{
    public string userId;//用户id
}

public class configRequest
{
    public int type;
}

public class transferRequest
{
    public int userId;
    public int amount;
    public string message;
}

public class cashoutRequest
{
    public int id;
    public int quantity;
}

public class feedbackRequest
{
    public int userId;
    public string clientVersion;
    public string platform;
    public string model;
    public int type;
    public string content;
}

public class taixiuRequest
{
    public int type;
    public int chipType;
    public int bet;
    public int pot;
}

public class giftCodeRequest
{
    public string code;
}


public class iapRequest
{
    public string item;
    public string transId;
    public string transDate;
    public string packageName;
    public string productId;
    public string token;
    public string provider;
    public string receipt;
}

public class verifyRequestPhone
{
	public int type;
	public string mobile;
    public string pin;
}