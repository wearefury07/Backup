using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;
using System.Runtime.InteropServices;

public class OGUIM : MonoBehaviour
{
    public LobbyId currentLobbyId;
    public string currentGameScene = "";
    public static bool isListen = true;
    public static bool isLoadingScence = false;
    public static bool isTheFirst = true;

    public GameObject popupCanvas;
    public List<GameObject> underReviewList;
    public List<GameObject> webglList;
    public List<GameObject> liteList;

    public GameObject backBtn;
    public Text lobbyName;
    public Text roomBetValue;

    #region GAME
    public static Lobby currentLobby { get; set; }
    public static Lobby nextLobby { get; set; }
    public static Room currentRoom { get; set; }
    #endregion

    public AvatarView meView;
    public LobbyView lobbyViewInRooms;
    //public LobbyView lobbyViewInLeader;
    public GameObject lobbyModeView;

    public Toggle autoLoginToggle;
    public Toggle toggleGold;
    public Toggle toggleKoin;
    public Toggle toggleServer;

    public UIToast popupToast;
    public static UIToast Toast;

    public UIMessageBox popupMessenger;
    public static UIMessageBox MessengerBox;

    public UIAnimation homeCanvas;
    public UIAnimation LoginCanvas;
    public UIAnimation RegisterCanvas;
    public UIAnimation girl;

    public UIAnimation mainMenuCanvas;
    public UIAnimation mainLobiesCanvas;
    public UIAnimation mainRoomsCanvas;
    public UIAnimation mainLeaderCanvas;


    public UIAnimation miniGameGroup;

    public UIPopupTabs popupAchieMissiDaily;
    public UIPopupTabs popupTopUp;
    public UIAnimation popupIAP;
    public UIPopupTabs popupCashOut;
    public TopUpTransfer popupTransfer;
    public PopupUserInfo popupUserInfo;
    public PopupSendMes popupSendMes;
    public PopUpPlayersInRoom popUpPlayersInRoom;
    public PopupVerifyPhone popupVerifyPhone;
    public ChatView popupChatView;

    public UIPopupTabs popupHistory;

    public PopupAllMes popupAllMes;

    public UIAnimation inGameCanvas;

    public Toggle inviteToggleInMesBox;
    public Toggle inviteToggleInSettings;

    public Text gameLabel;
    public Text hotlineLabel;
    public GameObject hotlineButton;
    public GameObject hotlineButtonOnSettings;
    public GameObject facebooKButtonOnSettings;
    public GameObject websiteButtonOnSettings;
    public GameObject giftButton;
    public Text textVersion;
    public Text textNeedUpdate;
    public UIAnimation needUpdatePanel;
    private bool autoLoginGame = false;
    public static LoginType loginType { get; set; }
    public static UserData me { get; set; }
    public static int isVerified { get; set; }
    public static Money currentMoney { get; set; }

    public static bool autoLeaveRoom { get; set; }

    public static Dictionary<string, UIAnimation> listNavigation { get; set; }
    public static Dictionary<string, UIAnimation> listPopup { get; set; }
    public static Dictionary<string, UIAnimation> listMenu { get; set; }
    public static Dictionary<string, UIAnimation> listToast { get; set; }

    public static List<RootChatData> listChatRoomData { get; set; }
    public static List<RootChatData> listChatWorldData { get; set; }

    public static Action actionOnFacebookLoginDone { get; set; }
    #region SLOT - XENG
    public static List<RoomSlot> listRoomSlot = new List<RoomSlot>();
    public static List<RoomSlot> listRoomXeng = new List<RoomSlot>();
    #endregion

    public static OGUIM instance { get; set; }

    private void Awake()
    {


        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        instance = this;

        Toast = popupToast;
        MessengerBox = popupMessenger;
        listNavigation = new Dictionary<string, UIAnimation>();
        listPopup = new Dictionary<string, UIAnimation>();
        listMenu = new Dictionary<string, UIAnimation>();
        listToast = new Dictionary<string, UIAnimation>();

        me = new UserData();

        currentMoney = GameBase.moneyGold;

        AddHttpRequestListener();
        AddWarpRequestListener();
        gameLabel.text = "<color=#FFC800>" + GameBase.preGameName + "</color> " + GameBase.sufGameName;

#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif
    }

    public void Start()
    {
        if (OldWarpChannel.Channel != null)
            OldWarpChannel.Channel.SwichServerToTestCheck(toggleServer);

        CheckDebugMode();
        ShowInviteCheck();
#if UNITY_WEBGL && !UNITY_EDITOR
		GetProviderCode ();
#endif
        FixedCanvasOrder(popupCanvas);
        if (toggleGold != null && toggleKoin != null)
        {
            toggleGold.GetComponent<UIToggle>().UpdateTextContent(GameBase.moneyGold.name);
            toggleKoin.GetComponent<UIToggle>().UpdateTextContent(GameBase.moneyKoin.name);
        }
        Toast.ShowLoading("");
        StartCoroutine(HttpRequest.Instance.GetAPIToken());

        listChatRoomData = new List<RootChatData>();
        listChatWorldData = new List<RootChatData>();
        if (string.IsNullOrEmpty(currentGameScene))
        {
            SetGameScene("");
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && popupMessenger.popupContent.state == UIAnimation.State.IsHide) //The listener for the 'Back' button event
        {
            GoBack();
        }
    }

    #region HttpRequest
    private void AddHttpRequestListener()
    {
        HttpRequest.OnGetAPITokenDone += Http_OnGetAPITokenDone;
        HttpRequest.OnRegisterDone += Http_OnRegisterDone;
        HttpRequest.OnGetVersionConfigureDone += Http_OnGetVersionConfigureDone;
        HttpRequest.OnForgotPasswordDone += Http_OnForgotPasswordDone;
        HttpRequest.OnCheckLocationDone += Http_OnCheckLocationDone;
    }

    private void RemoveHttpRequestListener()
    {
        HttpRequest.OnGetAPITokenDone -= Http_OnGetAPITokenDone;
        HttpRequest.OnRegisterDone -= Http_OnRegisterDone;
        HttpRequest.OnGetVersionConfigureDone -= Http_OnGetVersionConfigureDone;
        HttpRequest.OnForgotPasswordDone -= Http_OnForgotPasswordDone;
        HttpRequest.OnCheckLocationDone -= Http_OnCheckLocationDone;
    }
    #endregion

    #region On HttpRequest Done
    private void Http_OnGetAPITokenDone(WarpResponseResultCode status, string apiToken)
    {
        if (status != WarpResponseResultCode.SUCCESS)
        {
            Toast.ShowNotification("Kết nối gặp sự cố, vui lòng kiểm tra internet");
            GotoHome();
        }

        //StartCoroutine(HttpRequest.Instance.GetVersionConfigureV2());
        // For Downgrade
        StartCoroutine(HttpRequest.Instance.GetVersionConfigure());
    }

    private void Http_OnGetVersionConfigureDone(WarpResponseResultCode status)
    {
#if UNITY_WEBGL
		GameBase.underReview = false;
		GameBase.needUpdateToPlay = false;
#endif

        textVersion.text = GameBase.gameName.ToUpper() + " - " + GameBase.clientVersion + " - " + GameBase.providerCode;

        if (!string.IsNullOrEmpty(GameBase.hotline))
        {
            hotlineLabel.text = GameBase.hotline;
            hotlineButton.SetActive(false);
            if (hotlineButtonOnSettings != null)
                hotlineButtonOnSettings.SetActive(false);
        }

        if (!string.IsNullOrEmpty(GameBase.website))
        {
            if (websiteButtonOnSettings != null)
                websiteButtonOnSettings.SetActive(true);
        }

        if (!string.IsNullOrEmpty(GameBase.fbFanpage))
        {
            if (facebooKButtonOnSettings != null)
                facebooKButtonOnSettings.SetActive(true);
        }

        if (GameBase.needUpdateToPlay)
        {
            textNeedUpdate.text = "<size=42>THÔNG BÁO</size>" + "\n\n" + "Đã có phiên bản mới của " + GameBase.gameName + "!." + "\n" + "Vui lòng cập nhật phiên bản mới nhất để tiếp tục chơi!";
            instance.girl.Show();
            needUpdatePanel.Show();
        }
        else
        {
            if (GameBase.newVersionAvaiable)
            {
                MessengerBox.Show("Thông báo", "Đã có phiên bản mới của " + GameBase.gameName + "!." + "\n" + "Vui lòng cập nhật phiên bản mới nhất để trải nghiệp tốt nhất!",
                    "Cập nhật", () =>
                    {
                        UpdateApp();
                    },
                    "Lúc khác", null);
                GotoHome();
            }
            else if (instance.mainMenuCanvas.state != UIAnimation.State.IsShow)
                AutoLoginCheck();
            // Dont autologincheck if already
        }

        Toast.Hide();

#if !UNITY_WEBGL
        StartCoroutine(HttpRequest.Instance.CheckLocation());
#endif
    }

    private void Http_OnForgotPasswordDone(WarpResponseResultCode status, string data)
    {
        if (string.IsNullOrEmpty(data))
            data = "Hệ thống đang được nâng cấp, vui lòng quay lại sau";
        MessengerBox.Show("Quên mật khẩu", data);

        Debug.Log("HTTPRequest_OnGetVersionConfigureDone: " + status);
    }

    private void Http_OnCheckLocationDone(WarpResponseResultCode status, bool result)
    {
#if UNITY_ANDROID || UNITY_IOS
        if (GameBase.underReview)
        {
            int index = 0;
            foreach (var i in underReviewList)
            {
                if (index != 2)
                    i.SetActive(false);
                index++;
            }
        }
#endif

        if (GameBase.isLiteVersion)
        {
            foreach (var i in liteList)
                i.SetActive(false);
        }

    }
    #endregion

    #region WEB_GL
    public void HiddenWebGLPart()
    {
#if UNITY_WEBGL
		foreach (var i in webglList) 
			i.SetActive (false);
#endif
    }
    #endregion

    public void ShowCashChange()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (GameBase.underReview)
        {
            OGUIM.instance.popupIAP.Show();
            return;
        }
#endif
        OGUIM.instance.popupCashOut.Show(0);
    }

    #region GIFT
    public void SetGiftButton()
    {
#if UNITY_WEBGL
		return;
#endif
        //if ((me.isLikeReward || string.IsNullOrEmpty(me.faceBookId)) && me.isRateReward)
        //	giftButton.gameObject.SetActive (false);
        //else
        //	giftButton.gameObject.SetActive (true);

    }
    #endregion

    private void AddWarpRequestListener()
    {
        #region Login Logout
        WarpClient.wc.OnLoginDone += Wc_OnLoginDone;
        WarpClient.wc.OnLogoutDone += Wc_OnLogoutDone;
        #endregion

        #region AddListener SubGame -> JoinLobby -> SubLobby -> (SLOT = GetRooms) -> JoinRoom -> Goto Scene
        WarpClient.wc.OnSubGameDone += OnSubGameDone;
        WarpClient.wc.OnJoinLobbyDone += OnJoinLobbyDone;
        WarpClient.wc.OnSubLobbyDone += OnSubLobbyDone;
        WarpClient.wc.OnJoinRoomDone += OnJoinRoomDone;
        WarpClient.wc.OnAddFriend += OnAddFriendMessage;
        WarpClient.wc.OnGetRoomsDone += OnGetRoomsDone;
        WarpClient.wc.OnActionsInLobbyDone += OnActionsInLobbyDone;
        #endregion

        #region AddListener LeaveRoom -> UnSubLobby -> LeaveLobby -> UnSubGame -> Goto
        WarpClient.wc.OnLeaveRoom += OnLeaveRoom;
        WarpClient.wc.OnUnSubLobbyDone += OnUnSubLobbyDone;
        WarpClient.wc.OnLeaveLobbyDone += OnLeaveLobbyDone;
        WarpClient.wc.OnUnSubGameDone += OnUnsubGameDone;
        #endregion

        #region Chat
        WarpClient.wc.OnChat += Wc_OnChat;
        WarpClient.wc.OnWorldChat += Wc_OnWorldChat;
        #endregion

        #region UserInfo UserStat UpdatedMoney GeneralMessageNotification CashOut OnGetMessages
        WarpClient.wc.OnGetUserInfoDone += Wc_OnGetUserInfoDone;
        WarpClient.wc.OnGetUserStatDone += Wc_OnGetUserStatDone;
        WarpClient.wc.OnUpdatedMoneyDone += Wc_OnUpdatedMoneyDone;
        WarpClient.wc.OnGeneralMessageNotificationDone += Wc_OnGeneralMessageNotificationDone;
        WarpClient.wc.OnGetSystemMessagesDone += Wc_OnGetSystemMessagesDone;
        WarpClient.wc.OnGetMessagesDone += Wc_OnGetMessagesDone;
        #endregion

        #region IAP
        WarpClient.wc.OnIAPTopupDone += Wc_OnIAPTopupDone;
        #endregion

        #region PromoConfig
        WarpClient.wc.OnGetPromotionConfigureDone += Wc_OnGetPromoConfigDone;
        #endregion

        #region Gift
        WarpClient.wc.OnGetConfigDone += Wc_OnGetConfigDone;
        #endregion
    }

    #region ShowInvite Check
    public void ToggleShowInvite()
    {
        if (inviteToggleInMesBox != null && inviteToggleInSettings != null)
        {
            if (MessengerBox.popupContent.gameObject.activeSelf)
            {
                PlayerPrefs.SetInt("showInvite", inviteToggleInMesBox.isOn ? 1 : 0);
            }
            else
            {
                PlayerPrefs.SetInt("showInvite", inviteToggleInSettings.isOn ? 1 : 0);
            }
            PlayerPrefs.Save();

            ShowInviteCheck();
        }
    }

    public static void ShowInviteCheck()
    {
        if (instance.inviteToggleInMesBox != null)
        {
            instance.inviteToggleInMesBox.isOn = PlayerPrefs.GetInt("showInvite", 1) == 1 ? true : false;
            instance.inviteToggleInSettings.isOn = PlayerPrefs.GetInt("showInvite", 1) == 1 ? true : false;
        }
    }
    #endregion

    #region AutoLogin QuickLogin Login Register Facebook Logout MoneyChange Event

    #region AutoLogin
    private void AutoLoginCheck()
    {
#if UNITY_WEBGL
        // PlayerPrefs.SetInt("autoLogin", 1);
#endif
        int autoLoginState = PlayerPrefs.GetInt("autoLogin", 1);
        if (autoLoginState == 1)
        {
            autoLoginToggle.isOn = true;
#if UNITY_WEBGL
           //  PlayerPrefs.SetInt("loginType", 1);
#endif
            loginType = (LoginType)PlayerPrefs.GetInt("loginType", 0);
            if (loginType == LoginType.USER_PASS)
            {
#if UNITY_WEBGL

                // PlayerPrefs.SetString("username", RsaToJava.userName);
               //  PlayerPrefs.SetString("password", RsaToJava.passWord);
#endif
               // PlayerPrefs.SetString("username","hanson");
               // PlayerPrefs.SetString("password","qweasdzxc");
                me.username = PlayerPrefs.GetString("username", "");
                me.password = PlayerPrefs.GetString("password", "");
                //Debug.LogError("INFO : name: " + me.username + ", pass: " + me.password);

                if (!string.IsNullOrEmpty(me.username) && !string.IsNullOrEmpty(me.password))
                {
                    Toast.ShowLoading("");
                    autoLoginGame = true;

                    WeiHuWarpClient.wc.ConnectedToServer(false, () => WarpRequest.SendKeepAlive());
                    WarpClient.wc.ConnectedToServer(false, () => WarpRequest.Login(LoginType.USER_PASS));
                }
                else
                    GotoHome();
            }
            else if (loginType == LoginType.FACEBOOK)
            {
                me.faceBookId = PlayerPrefs.GetString("faceBookId", "");
                me.facebookAccessToken = PlayerPrefs.GetString("facebookAccessToken", "");
                if (!string.IsNullOrEmpty(me.faceBookId) && !string.IsNullOrEmpty(me.facebookAccessToken))
                {
                    Toast.ShowLoading("");
                    autoLoginGame = true;
                    WarpClient.wc.ConnectedToServer(false, () => WarpRequest.Login(LoginType.FACEBOOK));
                }
                else
                    GotoHome();
            }
            else
            {
                GotoHome();
            }
        }
        else
        {
            autoLoginToggle.isOn = false;
            GotoHome();
        }
    }

    public void ToggleAutoLogin()
    {
        int autoLogin = -1;

        if (autoLoginToggle.isOn)
        {
            autoLogin = 1;
        }
        else
        {
            autoLogin = 0;
            PlayerPrefs.SetString("username", "");
            PlayerPrefs.SetString("password", "");
            PlayerPrefs.SetString("faceBookId", "");
            PlayerPrefs.SetString("facebookAccessToken", "");
        }

        PlayerPrefs.SetInt("autoLogin", autoLogin); //We set the new value in the PlayerPrefs
        PlayerPrefs.Save(); //We save the value
    }
    #endregion

    #region Facebook
    public void LoginWithFB()
    {
        Toast.ShowLoading("Đang tiến hành đăng nhập facebook");
        LoginWithFB(() =>
            {
                if (autoLoginToggle.isOn)
                {
                    PlayerPrefs.SetString("faceBookId", me.faceBookId);
                    PlayerPrefs.SetString("facebookAccessToken", me.facebookAccessToken);
                }
                else
                {
                    PlayerPrefs.SetString("faceBookId", "");
                    PlayerPrefs.SetString("facebookAccessToken", "");
                }
                PlayerPrefs.Save();
                Toast.ShowLoading("");
                WarpClient.wc.ConnectedToServer(false, () => WarpRequest.Login(LoginType.FACEBOOK));
            });
    }

    public void LoginWithFB(Action onFacebookLoginDone)
    {
        actionOnFacebookLoginDone = onFacebookLoginDone;
        InitFB();
    }

    private void InitFB()
    {
        if (!Facebook.Unity.FB.IsInitialized)
        {
            UILogView.Log("Initialize the Facebook SDK");
            Facebook.Unity.FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            UILogView.Log("Already initialized, signal an app activation App Event");
            Facebook.Unity.FB.ActivateApp();
            LoginFB();
        }
    }

    private void InitCallback()
    {
        if (Facebook.Unity.FB.IsInitialized)
        {
            UILogView.Log("Signal an app activation App Event");
            // Signal an app activation App Event
            Facebook.Unity.FB.ActivateApp();
            // Continue with Facebook SDK
            // ...

            LoginFB();
        }
        else
        {
            UILogView.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void LoginFB()
    {
        Toast.ShowLoading("");
        var perms = new List<string>() { "public_profile" }; //, "email", "user_friends"
        Facebook.Unity.FB.LogInWithReadPermissions(perms, LoginCallback);
    }

    private void LoginCallback(Facebook.Unity.ILoginResult result)
    {
        if (Facebook.Unity.FB.IsLoggedIn)
        {
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

            // Print current access token's granted permissions

            string permLog = "";
            foreach (string perm in aToken.Permissions)
            {
                permLog += perm + ",";
            }
            UILogView.Log("LoginCallback: " + permLog);
            UILogView.Log("LoginCallback: " + aToken.UserId);

            me.faceBookId = aToken.UserId;
            me.facebookAccessToken = aToken.TokenString;

            if (actionOnFacebookLoginDone != null)
                actionOnFacebookLoginDone();
        }
        else
        {
            Toast.Hide();
            if (result == null)
                UILogView.Log("Facebook LoginCallback: " + result.Error, true);
            if (!string.IsNullOrEmpty(result.Error))
                UILogView.Log("Facebook LoginCallback: " + result.Error, true);
            else if (result.Cancelled)
                UILogView.Log("Facebook LoginCallback: User cancelled login", true);
        }
    }
    #endregion

    #region ChangeMoneyType
    public void ChangeMoneyType(int type)
    {
        if (type == (int)MoneyType.Gold)
        {
            toggleGold.isOn = true;
            toggleKoin.isOn = false;
        }
        else
        {
            if (currentLobby != null && currentLobby.onlygold)
            {
                Toast.ShowNotification(currentLobby.desc + " chỉ hỗ trợ chơi " + GameBase.moneyGold.name);
            }
            else
            {
                toggleGold.isOn = false;
                toggleKoin.isOn = true;
            }
        }
    }

    public void ToggleChangeMoneyType(int type)
    {
        if (type == (int)MoneyType.Gold)
        {
            currentMoney = GameBase.moneyGold;
        }
        else
        {
            currentMoney = GameBase.moneyKoin;
        }

        meView.moneyView.FillData(me);

        PlayerPrefs.SetInt("currentMoney", (int)(currentMoney.type));
        PlayerPrefs.Save();
    }
    #endregion

    private void OnAddFriendMessage(AddFriend data)
    {
        if (data.currUser.isFriend)
        {
            OGUIM.instance.popupMessenger.Show("Thông báo", data.currUser.displayName + " Đã nhận lời mời kết bạn của bạn", "Đồng ý");
        }
        else
        {

        OGUIM.instance.popupMessenger.Show("Thông báo", data.currUser.displayName + " Đã gửi lời mời kết bạn", "Đồng ý",
            () =>
            {
                WarpRequest.AddFriends(data.currUser.id);
            },
            "Hủy bỏ", () => { });
        }
    }

    public void QuickLogin()
    {
        me = new UserData();
        Toast.ShowLoading("");
        WarpClient.wc.ConnectedToServer(false, () => WarpRequest.Login(LoginType.QUICK_LOGIN));
    }

    public void Login(string _username, string _password)
    {
        me = new UserData { username = _username, password = _password };
        if (autoLoginToggle.isOn)
        {
            PlayerPrefs.SetString("username", me.username);
            PlayerPrefs.SetString("password", me.password);
        }
        else
        {
            PlayerPrefs.SetString("username", "");
            PlayerPrefs.SetString("password", "");
        }
        PlayerPrefs.Save();
        Toast.ShowLoading("");

        WeiHuWarpClient.wc.ConnectedToServer(false, () => WarpRequest.SendKeepAlive());
        WarpClient.wc.ConnectedToServer(false, () => WarpRequest.Login(LoginType.USER_PASS));
    }

    public void Regiter(string _username, string _password, string _phoneNumber)
    {
        StartCoroutine(HttpRequest.Instance.Register(_username, _password, _phoneNumber));
    }
    #endregion


#region OnLoginDone OnGetUserStatDone OnLogoutDone OnRegisterDone
    private void Wc_OnLoginDone(WarpResponseResultCode resultCode, RootUserData data, string desc)
    {
        //loadingFade.destroyLoading();
        string annoucetext = "";
        if (resultCode == WarpResponseResultCode.SUCCESS)
        {
            //Debug.LogErrorFormat("DATA : username: {0}, password: {1}", data.desc.username, data.desc.password);
            me = data.desc;
            isVerified = me.verified;

            var lastMoney = PlayerPrefs.GetInt("currentMoney", 1);

            meView.FillData(data.desc);

            ChangeMoneyType(lastMoney);
            SetGiftButton();
        }
        else if (resultCode == WarpResponseResultCode.AUTH_ERROR)
        {
            if (!string.IsNullOrEmpty(desc))
                UILogView.Log(resultCode + "\n" + desc, true);

            //annoucetext = "Tài khoản hoặc mật khẩu không chính xác, vui lòng thử lại (" + resultCode + ")";
            annoucetext = "Tài khoản/mật khẩu không chính xác";
        }
        else if (resultCode == WarpResponseResultCode.NOT_INTERNET_CONNECTION)
        {
            //annoucetext = "Kết nối thất bại, Vui lòng thử lại";
            annoucetext = "Kết nối thất bại. Vui lòng thử lại hoặc kiểm tra kết nối mạng";
        }
        else if (resultCode == WarpResponseResultCode.USER_IS_LOCK)
        {
            //annoucetext = "Tài khoản của bạn đang bị khóa. Vui lòng liên hệ số hotline để biết thêm chi tiết";
            annoucetext = "Tài khoản đang bị khóa. Liên hệ CSKH để được giải đáp";
        }
        else if (resultCode == WarpResponseResultCode.USER_LOGGED)
        {
            //annoucetext = "Tài khoản đang được đăng nhập ở thiết bị khác. Vui lòng liên hệ số hotline để biết thêm chi tiết";
            annoucetext = "Rất tiếc, tài khoản đang đăng nhập thiết bị khác";
        }
        else
        {
            annoucetext = "Kết nối thất bại, vui lòng thử lại (" + resultCode + ")";
        }

        if (resultCode == WarpResponseResultCode.SUCCESS)
        {
            PlayerPrefs.SetInt("autoLogin", instance.autoLoginToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("loginType", (int)loginType);
            PlayerPrefs.Save();

            CheckDebugMode(me.username);

            if (WarpClient.wc.sessionId == 0 || homeCanvas.state == UIAnimation.State.IsShow)
            {
                WarpClient.wc.sessionId = data.sessionid;
                Toast.ShowLoading("");
                //WarpRequest.GetUserStat(me.id);
                WarpRequest.GetPromotionConfigure();
                //giftButton.gameObject.SetActive (false);
                WarpRequest.GetConfig(ConfigType.LIKE_RATE_REWARD);

                 
                LoadLastLobby();

                GoToLobies();
            }
            else
            {
                WarpClient.wc.sessionId = data.sessionid;
            }
        }
        else
        {
            //Debug.LogError("Sua dang nhap that bai tai day");
            OGUIM.Toast.Show(annoucetext, UIToast.ToastType.Warning, 2.5f);
            //        MessengerBox.Show("Đăng nhập thất bại", annoucetext,
            //            "Đồng ý", () =>
            //            {
            //                if (listNavigation.Count == 0)
            //                    GotoHome();
            //            },
            //            "Thử lại", () =>
            //{
            //	Toast.ShowLoading ("");
            //                WarpClient.wc.ConnectedToServer(false, () => WarpRequest.Login(loginType));
            //            });
            if (autoLoginGame)
            {
                UnLoadGameScene(true);
                autoLoginGame = false;
            }

        }
    }

    private void Wc_OnGetUserStatDone(WarpResponseResultCode status, RootStat data)
    {
        if (status == WarpResponseResultCode.SUCCESS && data != null && data.data != null)
        {
            if (isListen)
            {
                //Check favorite Lobby
                var temp = new List<Lobby>();
                foreach (var i in data.data)
                {
                    i.play = i.loss + i.win + i.draw;
                    temp.Add(new Lobby { id = i.zoneId, play = i.play });
                }

                if (LobbyViewListView.listFavoriteData != null)
                    LobbyViewListView.listFavoriteData = temp;

                LoadLastLobby();

                GoToLobies();
            }
        }
        Toast.Hide();
    }

    private void Wc_OnLogoutDone(WarpResponseResultCode status)
    {
        if (OldWarpChannel.Channel != null)
        {
            OldWarpChannel.Channel.StopAllCoroutines();
            OldWarpChannel.Channel.socket_close();
        }

        isTheFirst = true;
        PopupAllMes.listAdmin = new List<Message>();
        PopupAllMes.listPromo = new List<Message>();
        PopupAllMes.listUser = new List<Message>();
        PopupAllMes.listClaim = new List<Message>();

        GotoHome();
    }

    private void Http_OnRegisterDone(WarpResponseResultCode status, UserData data)
    {
        Toast.Hide();
        if (status == WarpResponseResultCode.SUCCESS)
        {
            me = data;
            Toast.ShowLoading("");
            WarpClient.wc.ConnectedToServer(false, () => WarpRequest.Login(LoginType.USER_PASS));
        }
        else if (status == WarpResponseResultCode.REG_USER_EXIST)
        {
            MessengerBox.Show("Hmm...!", "Không thành công. Tài khoản này đã tồn tại");
        }
        else if (status == WarpResponseResultCode.INVALID_USERNAME)
        {
            MessengerBox.Show("Hmm...!", "Không thành công. Tài khoản không được chứa ký tự đặc biệt");
        }
        else if (status == WarpResponseResultCode.INVALID_MAX_USER_PER_DEVICE)
        {
            MessengerBox.Show("Hmm...!", "Không thành công. Thiết bị đã đăng kí quá nhiều tài khoản");
        }
        else
        {
            MessengerBox.Show("Hmm...!", "Đăng ký thất bại. Liên hệ CSKH để được hỗ trợ");
        }
    }
#endregion

#region OnChat
    private void Wc_OnWorldChat(RootWorldChat data)
    {
        if (OGUIM.currentRoom != null && OGUIM.currentRoom.room != null && OGUIM.currentRoom.room.id != 0)
            return;
        var rootChatData = new RootChatData { userName = data.user.displayName, userId = data.user.id, message = data.message };
        listChatWorldData.Add(rootChatData);
        popupChatView.FillData(rootChatData);
    }

    private void Wc_OnChat(RootChatData rootChatData)
    {
        if (rootChatData != null)
        {
            listChatRoomData.Add(rootChatData);
            popupChatView.FillData(rootChatData);
            if (IGUIM.instance != null)
            {
                var players = IGUIM.GetPlayersOnBoard();
                if (players.ContainsKey(rootChatData.userId))
                    players[rootChatData.userId].chatView.FillData(rootChatData);
            }
            if (IGUIM_Casino.instance != null)
            {
                var players = IGUIM_Casino.GetPlayersOnBoard();
                if (players.ContainsKey(rootChatData.userId))
                    players[rootChatData.userId].chatView.FillData(rootChatData);
            }
        }
    }
#endregion

#region OnGetUserInfoDone OnGetAllMessDone
    private void Wc_OnGetUserInfoDone(WarpResponseResultCode status, RootUserInfo data)
    {
        if (status == WarpResponseResultCode.SUCCESS && data != null && data.user.id == me.id)
        {
            instance.meView.FillData(data.user);
        }
    }

    public void Wc_OnGetMessagesDone(WarpResponseResultCode status, List<Message> data)
    {
        // Filter Claimable Xu for Lite Version
        if (GameBase.isLiteVersion)
        {
            var _data = new List<Message>();
            foreach (Message item in data)
            {
                string compare = item.content.ToLower();
                if (item.type != (int)MesType.CLAIMABLE || (!compare.Contains("xu")))
                {
                    _data.Add(item);
                }
            }
            data = _data;
        }
        if (popupAllMes != null)
            popupAllMes.Wc_OnGetMessagesDone(status, data);
    }

    public void Wc_OnGetSystemMessagesDone(WarpResponseResultCode status, List<Message> data)
    {
        if (popupAllMes != null)
            popupAllMes.Wc_OnGetSystemMessagesDone(status, data);
    }
#endregion

#region Goto Play Game
    public void SubLobby(Lobby data)
    {
        //SubGame -> JoinLobby -> SubLobby
        if (currentLobby == null || currentLobby.id == (int)LobbyId.NONE)
        {
            Toast.ShowLoading("");
            //Toast.ShowLoading("Đang tiến hành vào " + "\"" + data.desc + " " + data.subname + "\"...");
            nextLobby = null;
            currentLobby = data;
            WarpRequest.SubGame(data.gameId);
            SaveLastLobby();
        }
        else if (currentLobby.id != data.id)
        {
            UnSubLobby(data);
        }
        else if (currentLobby.id == data.id)
        {
            GoToRooms();
        }
    }

    public void UnSubLobby(Lobby _nextLobby)
    {
        //UnSubLobby -> LeaveLobby -> UnSubGame
        nextLobby = _nextLobby;
        //Toast.ShowLoading("Đang tiến hành rời " + "\"" + currentLobby.desc + " " + currentLobby.subname + "\"...");
        Toast.ShowLoading("");
        WarpRequest.LeaveLobby(currentLobby.id);
    }

    public void LoadLastLobby()
    {
        int lastLobbyId = PlayerPrefs.GetInt("lastLobbyId", 0);
        if (lastLobbyId != 0 && LobbyViewListView.listFavoriteData != null)
        {
            var lastLobby = LobbyViewListView.listFavoriteData.FirstOrDefault(x => x.id == lastLobbyId);
            if (lastLobby != null)
                lastLobby.play = 999999999;
        }
    }

    public void SaveLastLobby()
    {
        if (currentLobby != null)
            PlayerPrefs.SetInt("lastLobbyId", OGUIM.currentLobby.id);
        PlayerPrefs.Save();
    }
#endregion

#region OnSubGameDone -> OnJoinLobbyDone -> OnSubLobbyDone -> (SLOT = OnGetRoomsDone) -> OnJoinRoomDone
    private void OnSubGameDone(WarpResponseResultCode status)
    {
        if (status == WarpResponseResultCode.SUCCESS && currentLobby != null)
        {
            Toast.ShowLoading("");
            WarpRequest.JoinLobby(currentLobby.id);
        }
    }

    private void OnJoinLobbyDone(WarpResponseResultCode status)
    {
        if (status == WarpResponseResultCode.SUCCESS && string.IsNullOrEmpty(currentGameScene))
        {
            currentLobbyId = (LobbyId)currentLobby.id;
            if (currentLobby.playmode == (int)PlayMode.QUICK)
            {
                Toast.ShowLoading("");
                WarpRequest.SubLobby(currentLobby.id);
            }
            else
            {
                if (mainRoomsCanvas.state == UIAnimation.State.IsHide)
                    GoToRooms();
                else
                {
                    Toast.ShowLoading("");
                    WarpRequest.SubLobby(currentLobby.id);
                }
            }
        }
    }

    private void OnSubLobbyDone(WarpResponseResultCode status, RootRoom data)
    {
        if (status == WarpResponseResultCode.SUCCESS && currentLobby != null)
        {
            Debug.Log("OnSubLobbyDone: on OGUIM: " + (LobbyId)currentLobby.id + " " + currentLobby.lobbymode);
            if (currentLobby.playmode == (int)PlayMode.QUICK)
            {
                LobbyView.QuickJoinRoom(data);
            }
        }
    }

    private void OnGetRoomsDone(WarpResponseResultCode status, List<RoomSlot> data)
    {
        if (status == WarpResponseResultCode.SUCCESS && currentLobbyId == LobbyId.NONE && string.IsNullOrEmpty(instance.currentGameScene))
        {
            if (currentLobby.playmode == (int)PlayMode.QUICK)
            {
                if (currentLobby.id == (int)LobbyId.SLOT)
                {
                    var checkRoom = data.OrderBy(x => x.bet).FirstOrDefault();
                    if (checkRoom != null)
                    {
                        Toast.ShowLoading("");
                        WarpRequest.JoinRoom(currentLobby.onlygold ? GameBase.moneyGold.type : currentMoney.type, checkRoom.bet);
                    }
                }
            }
            else
            {
                Toast.Show(currentLobby.desc + " " + currentLobby.playmode, UIToast.ToastType.Warning, 3f);
            }
        }
        else if (status != WarpResponseResultCode.SUCCESS)
        {
            Toast.Show("Có lỗi xảy ra. Vui lòng thử lại", UIToast.ToastType.Warning, 3f);
        }
    }

    private void OnJoinRoomDone(WarpResponseResultCode status, Room data)
    {
        if (isListen)
        {
            if (status == WarpResponseResultCode.SUCCESS && string.IsNullOrEmpty(instance.currentGameScene))
            {
                popupChatView.chatMode = ChatMode.Room;
                popupChatView.chatTitle.text = "CHAT BÀN";
                popupChatView.FillData(new List<RootChatData>());
                currentRoom = data;
                autoLeaveRoom = false;

                // Hide invite popup when join room success
                OGUIM.MessengerBox.Hide();
                GoToIngame();


            }
            else if (status == WarpResponseResultCode.INVALID_CHIP)
            {
                // When invalid chip --> dont have room and dont have chipType too
                //var moneyName = data.room.chipType == 1 ? GameBase.moneyGold.name : GameBase.moneyKoin.name;
                Toast.Show("Rất tiếc, cần thêm " + OGUIM.currentMoney.name + " để trải nghiệm", UIToast.ToastType.Warning);
            }
            else
            {
                Toast.Show("Vào phòng chơi thất bại - Mã lỗi: #" + (int)status, UIToast.ToastType.Warning);
                BackToLobies();
            }
        }
    }
#endregion

#region OnLeaveRoom -> OnLeaveLobbyDone -> OnUnSubLobbyDone -> OnUnsubGameDone
    private void OnLeaveRoom(Room room, int status)
    {
        if (room != null && room.userId == me.id)
        {
            if (status == (int)WarpResponseResultCode.ROOM_STARTED)
            {
                if (!autoLeaveRoom)
                {
                    Toast.ShowNotification("Đăng ký rời phòng khi kết thúc ván!");
                }
                else
                {
                    Toast.ShowNotification("Hủy đăng ký rời phòng!");
                }
                autoLeaveRoom = !autoLeaveRoom;
            }
            else if (status == (int)WarpResponseResultCode.SUCCESS || status == (int)WarpResponseResultCode.BAD_REQUEST)
            {
                Debug.Log(status);
                instance.lobbyName.text = "LOADING...";
                instance.roomBetValue.text = "Please wait...";
                if (isListen)
                {
                    popupChatView.chatMode = ChatMode.World;
                    popupChatView.chatTitle.text = "CHAT CHUNG";
                    popupChatView.FillData(listChatRoomData);
                    UnLoadGameScene();
                    Toast.Hide();
                }
            }
        }
    }

    private void OnLeaveLobbyDone(WarpResponseResultCode status)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            if (currentLobby.id != (int)LobbyId.NONE)
            {
                Toast.ShowLoading("");
                WarpRequest.UnSubLobby(currentLobby.id);
            }
        }
        else
        {
            BackToLobies();
        }
    }

    private void OnUnSubLobbyDone(WarpResponseResultCode status)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            var gameId = currentLobby.gameId;
            currentLobby = null;
            currentLobbyId = LobbyId.NONE;
            Toast.ShowLoading("");
            WarpRequest.UnSubGame(gameId);
        }
        else
        {
            BackToLobies();
        }
    }

    private void OnUnsubGameDone(WarpResponseResultCode status)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            if (nextLobby == null)
            {
                Toast.Hide();
                BackToLobies();
            }
            else
            {
                SubLobby(nextLobby);
            }
        }
        else
        {
            BackToLobies();
        }
    }

    private void OnActionsInLobbyDone(SlotJacpot data)
    {
        if (data != null)
        {
            if (!listRoomSlot.Any(x => x.id == data.roomId))
                listRoomSlot.Add(new RoomSlot { id = data.roomId, funds = data.jackpot });

            var checkJackpot = listRoomSlot.FirstOrDefault(x => x.id == data.roomId);
            if (checkJackpot != null)
                checkJackpot.funds = data.jackpot;

            if (LobbyViewListView.listView != null && listRoomSlot.Any())
            {
                var checkLobbyView = LobbyViewListView.listView.FirstOrDefault(x => x.lobbyData.id == (int)LobbyId.SLOT);
                if (checkLobbyView != null)
                    checkLobbyView.lobbyData.tables = listRoomSlot.OrderByDescending(x => x.funds).FirstOrDefault().funds;
            }
        }
    }
#endregion

#region OnActionsInLobbyDone OnUpdatedMoneyDone OnCashOutDone
    private void Wc_OnUpdatedMoneyDone(UpdatedMoneyData data)
    {
        if (data != null)
        {

            string chipName = "";
            if (data.chipType == (int)GameBase.moneyGold.type)
            {
                me.gold = data.total;
                chipName = GameBase.moneyGold.name;
            }
            else
            {
                me.koin = data.total;
                chipName = GameBase.moneyKoin.name;
            }

            string textContent = "";
            string changeMoney = LongConverter.ToFull(data.change);

            if (data.change > 0)
            {
                if (data.type == (int)UpdateMoneyTypeCode.VIDEO_ADS)
                    textContent = "Xem video thành công, nhận thưởng " + changeMoney + " " + chipName;
                else if (data.type == (int)UpdateMoneyTypeCode.INVITE_FB)
                    textContent = "Mời bạn thành công, nhận thưởng " + changeMoney + chipName;
                else if (data.type == (int)UpdateMoneyTypeCode.SMS)
                    textContent = "Tài khoản vừa nạp thành công " + changeMoney + " " + chipName + " từ giao dịch tin nhắn SMS.";
                else if (data.type == (int)UpdateMoneyTypeCode.CARD)
                    textContent = "Tài khoản vừa nạp thành công " + changeMoney + " " + chipName + " từ giao dịch thẻ cào.";
                else if (data.type == (int)UpdateMoneyTypeCode.IAP)
                    textContent = "Tài khoản vừa nạp thành công " + changeMoney + " " + chipName + " từ giao dịch IAP.";
                else if (data.type == (int)UpdateMoneyTypeCode.ADMIN)
                    textContent = "Tài khoản vừa được nhận " + changeMoney + " " + chipName + " từ hệ thống.";
                else if (data.type == (int)UpdateMoneyTypeCode.TRANSFER_GOLD)
                    textContent = "Tài khoản vừa được nhận " + changeMoney + " " + chipName + " từ giao dịch chuyển 368vipEdited.";
                else if (data.type == (int)UpdateMoneyTypeCode.IGAME_CHARGING)
                    textContent = "Tài khoản vừa được nạp thành công " + changeMoney + " " + chipName + " từ cổng thanh toán.";
                else if (data.type == (int)UpdateMoneyTypeCode.BY_GIFT_CODE)
                    textContent = "Nhận thưởng thành công " + changeMoney + " " + chipName + " từ Giftcode!";
                else if (data.type == (int)UpdateMoneyTypeCode.BY_LIKE_RATE)
                    textContent = "Nhận thưởng thành công " + changeMoney + " " + chipName + ".";
                else
                    textContent = "Tài khoản vừa nạp thành công " + changeMoney + " " + chipName;
            }
            else if (data.change < 0)
                textContent = "Tài khoản vừa bị trừ " + changeMoney + " " + chipName + ". Liên hệ hotline để biết thêm chi tiết.";

            Toast.ShowNotification(textContent);
            MessengerBox.Show("Thông báo", textContent);


            if (meView != null)
                meView.moneyView.FillData((MoneyType)data.chipType, data.total);
            if (IGUIM.instance != null && IGUIM.instance.currentUser != null)
                IGUIM.instance.currentUser.avatarView.moneyView.FillData((MoneyType)data.chipType, data.total);
            else if (IGUIM_Casino.instance != null && IGUIM_Casino.instance.currentUser != null)
                IGUIM_Casino.instance.currentUser.avatarView.moneyView.FillData((MoneyType)data.chipType, data.total);
            else if (data.chipType == (int)MoneyType.Gold && UISlot.instance != null && UISlot.instance.moneyLabel != null)
                UISlot.instance.moneyLabel.FillData(data.total);
        }
    }

    private void Wc_OnGeneralMessageNotificationDone(GeneralMessage data)
    {
        if (data != null)
        {
            string title = "Thông báo";
            string message = "Thông báo từ hệ thống!";
            if (!string.IsNullOrEmpty(data.title))
                title = data.title;
            if (!string.IsNullOrEmpty(data.message))
                message = data.message;
            MessengerBox.Show(title, message);
        }
    }
#endregion

#region OnIAPTopupDone
    private void Wc_OnIAPTopupDone(WarpResponseResultCode status)
    {
        if (status != WarpResponseResultCode.SUCCESS)
            OGUIM.MessengerBox.Show("Thông báo", "Nạp thẻ thất bại. Liên hệ CSKH để được hỗ trợ");
    }
#endregion

#region OnGetPromoConfigDone
    private void Wc_OnGetPromoConfigDone(WarpResponseResultCode status, PromotionConfig _data)
    {
        DailyLogin data = _data.data;
        if (status == WarpResponseResultCode.SUCCESS)
        {
            GameBase.consecutive_days_login = data.consecutive_days_login;
            GameBase.days_login = data.days_login;
        }
    }
#endregion

#region OnGetConfigDone
    private void Wc_OnGetConfigDone(WarpResponseResultCode status, RootConfig _data)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            GameBase.likeReward = _data.data.likeReward;
            GameBase.rateReward = _data.data.rateReward;
            SetGiftButton();
        }

    }
#endregion
#region Main Event Static
    public static void GoBack()
    {
        if (listNavigation != null && listNavigation.Any())
        {
            try
            {
                //Check current toast stage
                if (Toast.toastType == UIToast.ToastType.Loading && Toast.anim.state != UIAnimation.State.IsHide)
                {
                    UILogView.Log("GoBack: Please wait all toast is hide!!!!!!!!!!");
                    return;
                }
                else if (MessengerBox.popupContent.state == UIAnimation.State.IsAnimation)
                {
                    UILogView.Log("GoBack: Please wait all MessengerBox is hide!!!!!!!!!!");
                    return;
                }

                //Check all popup 
                //is IsShow -> IsHide
                //is IsAnimation -> Return
                var checkPopUp = listPopup.FirstOrDefault(x => x.Value.state != UIAnimation.State.IsHide).Value;
                if (checkPopUp != null)
                {
                    if (checkPopUp.state == UIAnimation.State.IsAnimation)
                    {
                        UILogView.Log("GoBack: Please wait all popup is hide!!!!!!!!!!");
                    }
                    else
                    {
                        checkPopUp.Hide();
                    }
                    return;
                }

                else if (listNavigation.Any(x => x.Value.state == UIAnimation.State.IsAnimation))
                {
                    UILogView.Log("GoBack: Please wait all animations done!!!!!!!!!!");
                    return;
                }
                else if (listMenu.Any(x => x.Value.state == UIAnimation.State.IsAnimation))
                {
                    UILogView.Log("GoBack: Please wait all animations done!!!!!!!!!!");
                    return;
                }

                if (MiniGames.instance != null && MiniGames.instance.miniPoker != null && MiniGames.instance.miniPoker.isShow)
                {
                    MiniGames.instance.miniPoker.Close_Click();
                    return;
                }
                else if (MiniGames.instance != null && MiniGames.instance.miniSpin != null && MiniGames.instance.miniSpin.isShow)
                {
                    MiniGames.instance.miniSpin.Close_Click();
                    return;
                }
                else if (MiniGames.instance != null && MiniGames.instance.miniTaiXiu != null && MiniGames.instance.miniTaiXiu.isShow)
                {
                    MiniGames.instance.miniTaiXiu.Close_Click();
                    return;
                }
                //else if (MiniGames.instance != null && MiniGames.instance.miniCaoThap != null && MiniGames.instance.miniCaoThap.isShow)
                //{
                //    MiniGames.instance.miniCaoThap.Close_Click();
                //    return;
                //}

                //else if (instance.mainLeaderCanvas.state == UIAnimation.State.IsShow)
                //{
                //    BackFromLeaderToLast();
                //    return;
                //}

                //Check all navigate
                else if (instance.inGameCanvas.state == UIAnimation.State.IsShow)
                {
                    if (currentLobby.id == (int)LobbyId.SLOT || instance.currentLobbyId == LobbyId.SLOT)
                    {
                        if (UISlot.instance != null && UISlot.instance.isSpining)
                        {
                            autoLeaveRoom = true;
                            Toast.ShowNotification("Vui lòng chờ lượt quay kết thúc");
                            return;
                        }
                    }
                    if (currentLobby.id == (int)LobbyId.XENG_HOAQUA || instance.currentLobbyId == LobbyId.XENG_HOAQUA)
                    {
                        //if (UIXeng.instance != null)
                        //{
                        //	if (UIXeng.instance.isSpining)
                        //	{
                        //		autoLeaveRoom = true;
                        //		Toast.ShowNotification("Vui lòng chờ lượt quay kết thúc");
                        //		return;
                        //	}
                        //	else if (Convert.ToInt32(UIXeng.instance.txtWinChips.text) != 0)
                        //	{
                        //		Toast.ShowNotification("Hãy nhận tiền trước khi thoát phòng");	
                        //		return;
                        //	}
                        //}
                    }
                    Toast.ShowLoading("");
                    //Debug.LogError("ouut room");
                    isLoadingScence = false;
                    SetGameScene("");
                    BuildWarpHelper.LeaveRoom(null);
                    return;
                }

                else if (instance.mainMenuCanvas.state == UIAnimation.State.IsAnimation)
                {
                    return;
                }

                else if (instance.mainRoomsCanvas.state != UIAnimation.State.IsHide)
                {
                    instance.UnSubLobby(null);

                    return;
                }

                else if (instance.mainLobiesCanvas.state != UIAnimation.State.IsHide)
                {
                    //LogoutShow();
                    return;
                }

                else if (listNavigation.Count > 1)
                {
                    var current = listNavigation.LastOrDefault();
                    listNavigation.Remove(current.Key);
                    current.Value.Hide();


                    var previous = listNavigation.LastOrDefault();
                    listNavigation.Remove(previous.Key);
                    previous.Value.Show();

                }
                else
                {
                    ApplicationQuitShow();
                }
            }
            catch (Exception ex)
            {
                UILogView.Log("GoBack: " + ex.Message);
            }
        }
        else
        {
            UILogView.Log("GoBack: UIManager.lastShow null or UIManager.isShow null");
            GotoHome();
        }
    }

    public static void LogoutShow()
    {
        MessengerBox.Show("Đăng xuất!?", "Bạn có muốn đăng xuất không?",
            "Không", null,
            "Đăng xuất", () =>
            {
                Toast.ShowLoading("Đang tiến hành đang xuất...");
                WarpRequest.Logout();
            });
    }

    public static void GotoHome()
    {
        instance.mainMenuCanvas.Hide();
        var current = listNavigation.LastOrDefault();
        if (current.Value != null && current.Value != instance.homeCanvas)
            current.Value.Hide();

        // hide all minigame
        if (MiniGames.instance)
        {
            MiniGames.instance.Reset();
        }
        if (instance.homeCanvas.state != UIAnimation.State.IsShow)
            instance.homeCanvas.Show();
        if (instance.girl.state != UIAnimation.State.IsShow)
            instance.girl.Show();

        instance.miniGameGroup.Hide();

        listNavigation = new Dictionary<string, UIAnimation>();
        listNavigation.Add(instance.homeCanvas.name, instance.homeCanvas);

        if (!instance.autoLoginToggle.isOn)
        {
            if (loginType == LoginType.USER_PASS)
            {
                PlayerPrefs.SetString("username", "");
                PlayerPrefs.SetString("password", "");
            }
            else if (loginType == LoginType.FACEBOOK)
            {
                PlayerPrefs.SetString("faceBookId", "");
                PlayerPrefs.SetString("facebookAccessToken", "");
            }

            PlayerPrefs.SetInt("loginType", 0);
            PlayerPrefs.SetInt("currentMoney", 0);
            PlayerPrefs.Save();
        }

        instance.currentLobbyId = LobbyId.NONE;
        currentLobby = null;
        currentRoom = null;

        OldWarpChannel.Channel.socket_close();
        WarpClient.wc.sessionId = 0;

        Toast.Hide();
    }

    public static void GoToLobies()
    {
        if (listNavigation.Any())
            listNavigation.LastOrDefault().Value.Hide();

        if (instance.girl.state != UIAnimation.State.IsHide)
            instance.girl.Hide();

        instance.miniGameGroup.Show();
        instance.mainMenuCanvas.Show();
        instance.mainLobiesCanvas.Show();

#if UNITY_ANDROID || UNITY_IOS
        if (GameBase.underReview)
            return;
#endif
        DOVirtual.DelayedCall(0.5f, () =>
        {
            instance.popupAllMes.GetAllMes();
        });
    }

    public static void GoToIngame()
    {
        Toast.ShowLoading("");
        instance.mainMenuCanvas.Hide();
        listNavigation.LastOrDefault().Value.Hide();
        instance.inGameCanvas.Show();
        LoadGameScene();
    }

    public static void GoToRooms()
    {
        Toast.ShowLoading("");
        if (currentLobby.lobbymode != LobbyMode.CLASSIC)
            instance.lobbyModeView.SetActive(true);
        else
            instance.lobbyModeView.SetActive(false);

        // Hidden in lite mode
        if (GameBase.isLiteVersion)
            instance.lobbyModeView.SetActive(false);

        instance.mainLobiesCanvas.Hide();
        instance.mainRoomsCanvas.Show();
    }

    public static void BackToLobies()
    {
        var current = listNavigation.LastOrDefault();
        current.Value.Hide();
        listNavigation.Remove(current.Key);
        instance.mainLobiesCanvas.Show();
    }

    public static void BackFromLeaveRoomToLast()
    {
        instance.popupChatView.chatMode = ChatMode.World;
        instance.popupChatView.chatTitle.text = "CHAT CHUNG";
        instance.popupChatView.FillData(listChatWorldData);

        var current = listNavigation.LastOrDefault();
        current.Value.Hide();
        listNavigation.Remove(current.Key);
        current = listNavigation.LastOrDefault();
        if (current.Value == instance.mainLobiesCanvas)
            currentLobby = null;
        listNavigation.LastOrDefault().Value.Show();
        instance.mainMenuCanvas.Show();
    }

    public void GoToLeader()
    {
        instance.mainMenuCanvas.Hide();
        listNavigation.LastOrDefault().Value.Hide();
        instance.mainLeaderCanvas.Show();
    }

    public void SupportCall()
    {
        UIManager.Call(GameBase.hotline);
    }

    public void SupportFacebookPage()
    {
        UIManager.OpenURL(GameBase.fbFanpage);
    }

    public void SupportWebsite()
    {
        UIManager.OpenURL(GameBase.website);
    }


    public static void BackFromLeaderToLast()
    {
        var current = listNavigation.LastOrDefault();
        current.Value.Hide();
        listNavigation.Remove(current.Key);
        listNavigation.LastOrDefault().Value.Show();
        instance.mainMenuCanvas.Show();
    }

    public static void ApplicationQuitShow()
    {
        MessengerBox.Show("Thoát khỏi trò chơi!?", "Bạn có chắc chắn muốn thoát khỏi trò chơi hay không?",
            "Không", null,
            "Thoát", () =>
            {
                UIManager.ApplicationQuit();
            });
    }

    public void UpdateApp()
    {
        Application.OpenURL(GameBase.downloadURL);
    }
#endregion

#region Load Unload Scene
    public static string GetActiveScene()
    {
        var activeScene = SceneManager.GetActiveScene();
        return activeScene.name;
    }

    //IMPORTANT - IMPORTANT - IMPORTANT
    public static void FixedCanvasOrder(GameObject go, float delay = 0.0f)
    {
        instance.StartCoroutine(FixedCanvasOrderAsync(go, delay));
    }

    private static IEnumerator FixedCanvasOrderAsync(GameObject go, float delay = 0.0f)
    {
        while (go.activeSelf)
        {
            if (delay == 0)
                yield return new WaitForEndOfFrame();
            else
                yield return new WaitForSeconds(delay);
            go.SetActive(false);
        }
        while (!go.activeSelf)
        {
            if (delay == 0)
                yield return new WaitForEndOfFrame();
            else
                yield return new WaitForSeconds(delay);
            go.SetActive(true);
        }
    }

    public static void LoadGameScene()
    {
        Debug.Log("Check Loadscene : " + isLoadingScence + " | name : " + instance.currentGameScene);
        if (isLoadingScence == false && string.IsNullOrEmpty(instance.currentGameScene))
        {
            isLoadingScence = true;
            var checkScene = GameBase.scenes.FirstOrDefault(x => x.Key == (LobbyId)currentLobby.id);
            if (!string.IsNullOrEmpty(checkScene.Value))
            {
                SetGameScene(checkScene.Value);
                Debug.Log("current scene : " + checkScene.Value + "| " + instance.currentGameScene);
#if UNITY_WEBGL
                //SceneManager.LoadScene(checkScene.Value, LoadSceneMode.Additive);
                //var scene = SceneManager.GetSceneByName(checkScene.Value);
                //if (scene != null)
                //    SceneManager.SetActiveScene(scene);
                //FixedCanvasOrder(instance.popupCanvas);
                //Toast.Hide();
                //isLoadingScence = false;

                //            Debug.Log("Check Loadscene : " + isLoadingScence + " | name : " + instance.currentGameScene);
                 instance.StartCoroutine(LoadGameScene(checkScene.Value));
#else
                instance.StartCoroutine(LoadGameScene(checkScene.Value));
#endif
            }
            else
            {
                Toast.ShowNotification("LoadGameScene not found " + currentLobby.desc);
            }
        }
        else
        {
            UILogView.Log("Loading Scene, please wait...!");
        }
    }

    public static IEnumerator LoadGameScene(string name)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            yield return null;
            var scaledPerc = 0.5f * async.progress / 0.9f;
            Debug.Log("LoadGameScene " + currentLobby.desc + " : " + (100f * scaledPerc).ToString("F0") + "%");
        }

        async.allowSceneActivation = true;
        float perc = 0.5f;
        while (!async.isDone)
        {
            yield return null;
            perc = Mathf.Lerp(perc, 1f, 0.05f);
            Debug.Log("LoadGameScene " + currentLobby.desc + " : " + (100f * perc).ToString("F0") + "%");
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
        //SceneManager.MoveGameObjectToScene(instance.popupCanvas, SceneManager.GetSceneByName(name));
        FixedCanvasOrder(instance.popupCanvas);
        Toast.Hide();
        isLoadingScence = false;
    }

    public static void UnLoadGameScene(bool goHome = false)
    {
        //UIManager.instance.StartCoroutine(OGUIM.Toast.HideNotification());
        if (currentLobby != null)
        {
            var checkScene = GameBase.scenes.FirstOrDefault(x => x.Key == (LobbyId)currentLobby.id);
            if (!string.IsNullOrEmpty(checkScene.Value))
            {
                Scene targetScene = SceneManager.GetSceneByName(checkScene.Value);
                if (targetScene.isLoaded)
                {
                    //SceneManager.MoveGameObjectToScene(instance.popupCanvas, SceneManager.GetSceneByName("1_Scene_Main"));
                    FixedCanvasOrder(instance.popupCanvas);
                    if (targetScene == SceneManager.GetActiveScene())
                    {
                        Toast.ShowLoading("");
                        UILogView.Log("Unload current scene will destroy your GameObjects !");
#if UNITY_WEBGL
                        //SceneManager.UnloadScene(checkScene.Value);
                        //instance.currentGameScene = "";
                        //ActionOnUnLoadScene(goHome);
                        //Toast.Hide();
                        instance.StartCoroutine(UnLoadGameScene(checkScene.Value, goHome));
#else
                        instance.StartCoroutine(UnLoadGameScene(checkScene.Value, goHome));
#endif
                    }
                    else
                    {
                        Toast.ShowLoading("");
#if UNITY_WEBGL
                        //SceneManager.UnloadScene(checkScene.Value);
                        //instance.currentGameScene = "";
                        //ActionOnUnLoadScene(goHome);
                        //Toast.Hide();
                        instance.StartCoroutine(UnLoadGameScene(checkScene.Value, goHome));
#else
                        instance.StartCoroutine(UnLoadGameScene(checkScene.Value, goHome));
#endif
                    }
                }
                else
                {
                    UILogView.Log(checkScene.Value + " does not loaded !", true);
                    ActionOnUnLoadScene(goHome);
                }
            }
            else
            {
                UILogView.Log("UnLoadGameScene not found " + currentLobby.desc, true);
                ActionOnUnLoadScene(goHome);
            }
        }
        else
        {
            ActionOnUnLoadScene(goHome);
        }
    }

    public static IEnumerator UnLoadGameScene(string name, bool goHome)
    {
        AsyncOperation async = SceneManager.UnloadSceneAsync(name);
        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            yield return null;
            var scaledPerc = 0.5f * async.progress / 0.9f;
            Debug.Log("UnLoadGameScene: " + (100f * scaledPerc).ToString("F0") + "%");
        }

        float perc = 0.5f;
        while (!async.isDone)
        {
            yield return null;
            perc = Mathf.Lerp(perc, 1f, 0.05f);
            Debug.Log("UnLoadGameScene: " + (100f * perc).ToString("F0") + "%");
        }

        SetGameScene("");
        ActionOnUnLoadScene(goHome);
        Toast.Hide();
    }

    static void SetGameScene(string sceneName)
    {
        //Debug.LogError("Scene name: " + sceneName);
        instance.currentGameScene = sceneName;
        RectTransform rectTrans = instance.backBtn.GetComponent<RectTransform>();
        if (!string.IsNullOrEmpty(sceneName))
        {
            rectTrans.anchorMin = new Vector2(0, 1);
            rectTrans.anchorMax = new Vector2(0, 1);
            rectTrans.anchoredPosition = new Vector2(5, -(rectTrans.rect.height + 5));
        }
        else
        {
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.zero;
            rectTrans.anchoredPosition = new Vector2(5, 5);
        }
        //Debug.LogError(" change: " + instance.backBtn.GetComponent<RectTransform>().rect.position);
    }

    public static void ActionOnUnLoadScene(bool goHome = false)
    {
        if (WarpClient.wc != null && WarpClient.wc.sessionId != 0)
        {
            foreach (var i in listPopup.Keys)
            {
                if (listPopup[i].state != UIAnimation.State.IsHide)
                    listPopup[i].Hide();
            }
        }

        if (goHome)
            GotoHome();
        else
            BackFromLeaveRoomToLast();
    }
#endregion

#region DEBUG MODE
    private List<string> listUserDebug = new List<string> { "andoprof", "thinhpt", "tuth", "huynt", "thangnx03", "thangnx04", "thangnx05", "thangnx06", "thangnx07", "thangnx08" };
    public Toggle debugModeToggle;
    public InputField serverInputField;
    public InputField portInputField;
    public GameObject frameRate;

    public void CheckDebugMode(string userName = null)
    {
        if (listUserDebug.Contains(userName))
        {
            debugModeToggle.gameObject.SetActive(true);
            serverInputField.gameObject.SetActive(true);
            portInputField.gameObject.SetActive(true);
            frameRate.SetActive(true);
            if (UILogView.uiLogView != null)
                UILogView.uiLogView.gameObject.SetActive(true);
        }
        else
        {
            debugModeToggle.gameObject.SetActive(false);
            serverInputField.gameObject.SetActive(false);
            portInputField.gameObject.SetActive(false);
            frameRate.SetActive(false);
            if (UILogView.uiLogView != null)
                UILogView.uiLogView.gameObject.SetActive(false);
        }
    }
#endregion

#if UNITY_WEBGL && !UNITY_EDITOR
	private void GetProviderCode()
	{
		string name = "";


		name = getParameterByName();

		if (!String.IsNullOrEmpty (name)) {
			string[] str = name.Split('&');
			for (var i = 0; i < str.Length; i++) {
				string[] pair = str[i].Split('=');
				if (pair[0] == "pc") {
					//Debug.Log (pair [1]);
					GameBase.providerCode = pair [1];
				}

				if (pair[0] == "ch") {
					//Debug.Log (pair [1]);
					GameBase.refCode = pair [1];
				}

				if (pair[0] == "lt" && !String.IsNullOrEmpty(pair [1])){
					//Debug.Log (pair [1]);
					PlayerPrefs.SetInt("loginType", int.Parse(pair [1]));
					PlayerPrefs.Save ();
				}

				if (pair[0] == "un") {
					//Debug.Log (pair [1]);
					int loginType = PlayerPrefs.GetInt ("loginType", 0);
					if (loginType == (int)LoginType.USER_PASS)
						PlayerPrefs.SetString("username", pair [1]);
					else if (loginType == (int)LoginType.FACEBOOK)
						PlayerPrefs.SetString("userFBId", pair [1]);

					PlayerPrefs.Save ();
				}

				if (pair[0] == "pw") {
					//Debug.Log (pair [1]);
					int loginType = PlayerPrefs.GetInt ("loginType", 0);
					if (loginType == (int)LoginType.USER_PASS)
						PlayerPrefs.SetString("password", pair [1]);
					else if (loginType == (int)LoginType.FACEBOOK)
						PlayerPrefs.SetString("fbAccessToken", pair [1]);

					PlayerPrefs.Save ();
				}

				if (pair [0] == "aufb" && pair [1] == "true")
				{
					LoginWithFB ();
					//PlayerPrefs.DeleteKey("userFBId");
					//PlayerPrefs.DeleteKey("fbAccessToken");
					// Avoid conflict when using lt/un/pw
				}
			} 
		}

	}


	[DllImport("__Internal")]
	private static extern string getParameterByName();
#endif
}
