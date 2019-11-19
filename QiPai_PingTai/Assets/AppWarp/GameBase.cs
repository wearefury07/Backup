using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBase
{
    public static Money moneyGold = new Money { type = MoneyType.Gold, name = "Gold", image = Resources.Load<Sprite>("chip_yellow") };

    public static Money moneyKoin = new Money { type = MoneyType.Koin, name = "Koin", image = Resources.Load<Sprite>("chip_gray") };

    public static Dictionary<LobbyId, string> scenes = new Dictionary<LobbyId, string>()
    {
        {LobbyId.TLMNDL, "_Scene_Card_Card" },
        {LobbyId.TLMNDL_SOLO, "_Scene_Card_Card" },
        {LobbyId.SAM, "_Scene_Card_Card" },
        {LobbyId.SAM_SOLO, "_Scene_Card_Card" },
        {LobbyId.PHOM, "_Scene_Card_Card" },
        {LobbyId.PHOM_SOLO, "_Scene_Card_Card" },
        {LobbyId.MAUBINH, "_Scene_Card_Card" },
        {LobbyId.BACAY, "_Scene_Card_Card" },
        {LobbyId.BACAY_GA, "_Scene_Card_Card" },
        {LobbyId.LIENG, "_Scene_Card_Card" },
        {LobbyId.BAUCUA, "_Scene_Casino_Casino" },
        {LobbyId.XOCDIA, "_Scene_Casino_Casino" },
        {LobbyId.BAUCUA_OLD, "_Scene_Casino_Casino_old" },
        {LobbyId.XOCDIA_OLD, "_Scene_Casino_Casino_old" },

        {LobbyId.SLOT, "_Scene_DapHu" },
        {LobbyId.XENG_HOAQUA, "_Scene_Xeng" }
    };

    //How to go Slot = GETROOM -> JOINROOM - >

    #region SETTING SERVER CONFIG

	#if UNITY_WEBGL
	public static int osType = 4;
	public static string platform = "PC";
	#elif UNITY_STANDALONE
	public static int osType = 4;
	public static string platform = "WindowsPC";
	#elif UNITY_ANDROID
	public static int osType = 2; // 1: iOS, 2: Android  3: WindowsPhone 4: PC
	public static string platform = "Android";
	#elif UNITY_IOS
	public static int osType = 1; // 1: iOS, 2: Android  3: WindowsPhone 4: PC
	public static string platform = "iPhone OS";
	#endif

    public static string model = SystemInfo.deviceModel;
	#if UNITY_WEBGL
	public static string device_uuid = System.Guid.NewGuid().ToString();
	#else
    public static string device_uuid = SystemInfo.deviceUniqueIdentifier;
	#endif
    public static string osVersion = "Unkown";
    public static string refCode = "";

    // Global game default variable
    public static string hotline = "";
	public static string providerCode = "9BnwIN";
    public static string clientVersion = Application.version;

    //Global version config variable
    public static string downloadURL = "";
    public static string latest_version = "";

    //InitializeAPI
    public static string api_key = "pokede9469e42fb6f06vip";
    public static string api_secret = "pokvip0f3c1f207cda50454093be2c2cce40b";
	public static bool isOldVersion = true;
	public static bool isLiteVersion = false;
    internal static VersionConfigData versionConfigData;
    internal static ConfigData versionConfigDataV2;
    internal static string website = "";
    internal static string fbFanpage = "";
    internal static string emailSupport = "";
    internal static bool needUpdateToPlay;
    internal static string appPackageName = Application.identifier;
    internal static bool underReview = true;
    internal static bool newVersionAvaiable;

    internal static Version currentVersion = StringToVersion(clientVersion);
    internal static string apiToken;
	internal static string gameName = "6688";
	internal static string preGameName = "6688";
	internal static string sufGameName = ".Club";


    //REMENBER CHANGE GA-GoogleAnalytics on MAIN
    #endregion

	#region SAVE DATA CONFIG
	public static ConsecutiveDaysLogin consecutive_days_login;
	public static DaysLogin days_login;
	public static int likeReward;
	public static int rateReward;
	#endregion

    public static Version StringToVersion(string versionString)
    {
        var version = new Version(1, 0, 0, 0);
        var versionSplit = versionString.Split('.');
        if (versionSplit.Count() == 4)
            version = new Version(int.Parse(versionSplit[0]), int.Parse(versionSplit[1]), int.Parse(versionSplit[2]), int.Parse(versionSplit[3]));
        else if (versionSplit.Count() == 3)
            version = new Version(int.Parse(versionSplit[0]), int.Parse(versionSplit[1]), int.Parse(versionSplit[2]), 0);
        return version;
    }

    public static int ServerVersionCode = 1;
}

public class Money
{
    public MoneyType type { get; set; }
    public string name { get; set; }
    public Sprite image { get; set; }
}

public enum MoneyType
{
    Gold = 1,
    Koin = 0,
}

public enum ChatType
{
    Text = 1,
    Emotion = 2,
}

public enum ChatMode
{
    Room = 0,
    World = 1,
}
