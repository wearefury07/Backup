using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class RootUserData
{
    public int sessionid;
    public UserData desc;
}
public class RootUserDataError
{
    public string desc;
}


[Serializable]
public class RootUserInfo
{
    public UserData user;
    public string mobile;
    public string faceBookId;
    public bool isFriend;
    public int level;
    public int allLevel;
    public int exp;
    public int allExp;
    public int target;
    public int allTarget;
}

[Serializable]
public class RootFriend
{
    public List<UserData> friends;
}

[Serializable]
public class UserData : UserBase
{
    public int id;
    public string displayName;
    //真实姓名
    public string trueName;
    public string status;
    public string faceBookId;
    public string facebookAccessToken;
    public string avatar;
    public string mobile;
    public string Email;
    public int verified;
    public string passport;
    public long koin;
    public long gold;
    public bool link_acc;
    public bool link_fb;

    public int exp;
    public int expPercent;
    public int level;
    public bool online;
    public int position;
    public int target;
    public List<Arch> archs;

    public long total_chips;
    public long chipChange;
    public int allLevel;
    public int order;
    public int seatOrder;
    public int gender;
    public bool owner;
    public bool isPlayer;
    public bool isReady;
    public bool isHaPhom;
	public bool isLikeReward;
	public bool isRateReward;

    public int remainCardCount;
    public Properties properties;
    public Extra extra;
    public List<CardData> playedCards;
    public List<CardData> acquiredCards;
    public List<CardData> lastPlayedCards;
    internal bool isFriend;

    public bool isStanding()
    {
        return seatOrder != -1;
    }

    public void CopyDataIfEmpty(UserData baseData)
    {
        if (baseData == null)
            return;
        if (!string.IsNullOrEmpty(baseData.displayName) && string.IsNullOrEmpty(displayName))
            displayName = baseData.displayName;
        if (baseData.faceBookId != null && faceBookId == null)
            faceBookId = baseData.faceBookId;
        if (!string.IsNullOrEmpty(baseData.avatar) && string.IsNullOrEmpty(avatar))
            avatar = baseData.avatar;
        if (baseData.mobile != null && mobile == null)
            mobile = baseData.mobile;

        
    }
    
}

public class UserLogin : UserBase
{
    public int loginType;
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

public class UserBase
{
    public string username;
    public string password;
}

public enum UseStatus
{
    Offline = -1,
    Online = 0,
    IdGame = 3
}

[Serializable]
public class Properties
{
    public int user_bet_prev;
    public string client_platform;
    public string client_version;
    public int user_bet;
    public int pot_bet_6;
    public int pot_bet_4;
    public int pot_bet_5;
    public int pot_bet_2;
    public int pot_bet_3;
    public int pot_bet_1;

    public bool binhlung;
    public bool muoiba_laden;
    public bool sap_lang;
    public bool ba_thung;
    public bool sanh_rong;
    public bool isSubmitSuiteMB;
    public bool muoi2den_1do;
    public bool muoiba_lado;
    public bool ba_sanh;
    public bool sau_doi;
    public bool muoi2do_1den;
    public bool sap_ham;

    public bool isMaubinh
    {
        get
        {
            return muoiba_laden || ba_thung || sanh_rong || muoi2den_1do || muoiba_lado
                || ba_sanh || sau_doi || muoi2do_1den;
        }
    }

    public bool punished_uhhhh;
    public int suite_count;

    public List<CardData> suites;
    public bool isShowCard;
    public List<CardData> showCards;

    public bool user_allin;
}

[Serializable]
public class Extra
{
    public int TuQuy;
    public int FourCouples;
    public int CardsTwo;
    public int ThreeCouples;
    public List<CardData> cards;
    public List<CardData> punishedCards;
}

[Serializable]
public class Stat
{
    public int zoneId;
    public string zoneDesc;
    public long play;
    public long playTime;
    public long win;
    public double winPercent;
    public long loss;
    public long draw;
    public long level;
    public List<Arch> archs;
}

[Serializable]
public class Arch
{
    public int actual;
    public int type;
    internal string image;
    internal string name;
}

[Serializable]
public class RootStat
{
    public List<Stat> data;
    public int userid;
    public long playTotal;
    public long playTimeTotal;
    public long winTotal;
    public double winPercentTotal;
}


[Serializable]
public class CasinoData
{
	public List<UserData> users;
	public int userId;
	public int username;
	public long value;
	public int pot;
    public List<CasinoPot> pots;
}
[Serializable]
public class CasinoPot
{
    public int pot;
}


[Serializable]
public class RootTopUser
{
    public UserData myPosition;
    public List<UserData> topUsers;
}

public class AddFriend
{
    public UserData currUser;
    public string message;
}