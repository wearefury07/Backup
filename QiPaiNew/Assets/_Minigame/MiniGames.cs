using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MiniGames : MonoBehaviour
{
    public Text timeText;
    public Image timeImage;
	public static void SpawnTextEfx(string str, Vector3 pos, bool active = true)
    {
        var te = instance.textEfxPrefab.Spawn();
        te.gameObject.transform.SetParent(instance.transform, false);
        te.gameObject.transform.position = pos;
        te.gameObject.transform.SetZ(0, Space.Self);
        te.gameObject.transform.localScale = Vector3.one * (pos == Vector3.zero ? 1 : 0.6f);

		te.SetData(str.ToUpper(), active ? Color.white : Color.black, 0, 0);
    }

    public TextEffext textEfxPrefab;
    public Effect_Jackpot jackpotEfx;

    public UIRadialLayout uiRadialLayout;
    public CanvasGroup uiRadialLayoutCanvasGround;
    public bool menuIsShow = false;
    public bool menuIsShowing = false;

    public static readonly string miniGameTweenId = "MiniGameTweenId";
    public MiniSpin miniSpin;
    public MiniPoker miniPoker;
    public MiniTaiXiu miniTaiXiu;
    //public MiniCaoThap miniCaoThap;

    public LobbyId currentGame;

    public static MiniGames instance;

    long DOTweenId;
    void Awake()
    {
        instance = this;
        uiRadialLayout.gameObject.SetActive(false);
        AddWarpRequestListener();

        textEfxPrefab.CreatePool(10);
        DOTweenId = System.DateTime.Now.Ticks;
        SetTime(0);
    }

    private void Start()
    {
		// DONT SUB TAIXIU FROM THE START CUZ IS DDOS SERVER
//        if (OGUIM.isTheFirst && !miniTaiXiu.isSub)
//        {
//            miniTaiXiu.isAutoSub = true;
//            BuildWarpHelper.MINI_Sub(LobbyId.MINI_TAIXIU, 0, 1, () =>
//            {
//                UILogView.Log("MINI_Sub  MINI_TAIXIU is time out");
//                currentGame = LobbyId.NONE;
//                miniTaiXiu.isAutoSub = false;
//            });
//            currentGame = LobbyId.MINI_TAIXIU;
//        }
    }

    public void ShowMenuGame(bool show)
    {
        //if (!menuIsShowing)
        //{
            if (show && !menuIsShow)
                StartCoroutine(ShowAsync());
            else
                StartCoroutine(HideAsync());
        //}
    }

    public void SetTime(int time)
    {
        if (timeText == null || timeImage == null)
        {
            Debug.LogError("txtCoolDown == null || imgCoolDown == null");
            return;
        }

        if (time <= 0)
        {
            timeText.gameObject.SetActive(false);
            timeImage.gameObject.SetActive(false);
        }
        else
        {
            timeText.gameObject.SetActive(true);
            timeImage.gameObject.SetActive(true);
            DOTween.Kill(DOTweenId);
            DOVirtual.Float(time, 0, time, (x) =>
                {
                    timeImage.fillAmount = x / time;
                    timeText.text = x.ToString("00");
                    if (x <= 0)
                        timeText.gameObject.SetActive(false);
                }).SetEase(Ease.Linear).SetId(DOTweenId);
        }
    }

    IEnumerator ShowAsync()
    {
        uiRadialLayout.XMultiplier = uiRadialLayout.transform.position.x < 0 ? -1 : 1;
        uiRadialLayout.gameObject.SetActive(true);
        while (uiRadialLayoutCanvasGround.alpha < 1)
        {
            yield return new WaitForEndOfFrame();
            uiRadialLayoutCanvasGround.alpha += 0.1f;
            menuIsShowing = true;
        }
        menuIsShowing = false;
        menuIsShow = true;
    }

    IEnumerator HideAsync()
    {
        while (uiRadialLayoutCanvasGround.alpha > 0)
        {
            yield return new WaitForEndOfFrame();
            uiRadialLayoutCanvasGround.alpha -= 0.1f;
            menuIsShowing = true;
        }
        menuIsShowing = false;
        menuIsShow = false;
        uiRadialLayout.gameObject.SetActive(false);
    }


    void AddWarpRequestListener()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnMinigameResponse += _wc_OnMinigameResponse;
            WarpClient.wc.OnMiniRoomChanged += _wc_OnMiniRoomChanged;
            Debug.Log("MINIGAME AddWarpRequestListener");
        }
        currentGame = LobbyId.NONE;
    }

    void RemoveWarpRequestListener()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnMinigameResponse -= _wc_OnMinigameResponse;
            WarpClient.wc.OnMiniRoomChanged -= _wc_OnMiniRoomChanged;
            Debug.Log("MINIGAME RemoveWarpRequestListener");
        }
    }

    private void _wc_OnMiniRoomChanged(Mini_RootData data, WarpNotifyTypeCode notifyType)
    {
        if (currentGame != LobbyId.NONE)
        {
            if (miniSpin.isShow)
                miniSpin.SetJackpot(data.jackpot);
            else if (miniPoker.isShow)
                miniPoker.SetJackpot(data.jackpot);
            else if (miniTaiXiu.isShow)
                miniTaiXiu.OnTaiXiuNotifyDone(data);
            //else if (miniCaoThap.isShow && notifyType == WarpNotifyTypeCode.NOTIFY_MINI_ROOM_CHANGED)
            //    miniCaoThap.SetJackpot(data.jackpot);

            //if (miniCaoThap.isShow && notifyType == WarpNotifyTypeCode.NOTIFY_CAOTHAP_END_MATCH)
            //{
            //    miniCaoThap.EndMatch(data);
            //}
        }
        else
        {
            miniTaiXiu.OnTaiXiuNotifyDone(data);
        }
    }

    private void _wc_OnMinigameResponse(Mini_RootData data, int status)
    {
        if (data == null || (WarpResponseResultCode)status != WarpResponseResultCode.SUCCESS)
        {
            if ((WarpResponseResultCode)status == WarpResponseResultCode.INVALID_CHIP_MIN_BET)
            {
                OGUIM.Toast.Show("Cần tối thiểu x để tham gia (x là số tiền và đơn vị)", UIToast.ToastType.Warning, 3f);
                //OGUIM.MessengerBox.Show("Oops...!", "Số \"" + GameBase.moneyGold.name + "\" không đủ." + "\n"
                //+ "Vui lòng nạp thêm \"" + GameBase.moneyGold.name + "\" để tiêp tục trải nghiệm trò chơi!",
                //"Nạp " + GameBase.moneyGold.name,
                //    () => OGUIM.instance.popupTopUp.Show(-1),
                //"Lần sau", null);
            }
            else if (status == (int)WarpResponseResultCode.SET_BET_IN_READY_MODE && data.type == (int)MINIGAME.ADD_BET)
            {
                if (status != 0 && miniTaiXiu.isShow)
                    OGUIM.Toast.ShowNotification("Ván mới chưa bắt đầu. Vui lòng đợi giây lát!");
            }
        }
        else
        {
            if (data.type == (int)MINIGAME.SUBSCRIBE_ROOM)
            {
                if (currentGame == LobbyId.MINI_SPIN)
                {
                    miniSpin.Show();
                    miniSpin.SetJackpot(data.jackpot);
                    //OGUIM.Toast.ShowNotification("Popup Show... MINIGAME_ID.MINI_SPIN");
                }
                else if (currentGame == LobbyId.MINI_POKER)
                {
                    miniPoker.Show();
                    miniPoker.SetJackpot(data.jackpot);
                    //OGUIM.Toast.ShowNotification("Popup Show... MINIGAME_ID.MINI_POKER");
                }
                else if (currentGame == LobbyId.MINI_TAIXIU)
                {
					if (!miniTaiXiu.isAutoSub)
						miniTaiXiu.Show ();
					else
						currentGame = LobbyId.NONE;
					
                    miniTaiXiu.OnSubDone(data);
                    //OGUIM.Toast.ShowNotification("Popup Show... MINIGAME_ID.MINI_TAIXIU");
                }
                //else if (currentGame == LobbyId.MINI_CAOTHAP)
                //{
                //    miniCaoThap.Show();
                //    miniCaoThap.SetJackpot(data.jackpot);
                //    //OGUIM.Toast.ShowNotification("Popup Show... MINIGAME_ID.MINI_CAOTHAP");
                //}
            }
            else if (data.type == (int)MINIGAME.UNSUBSCRIBE_ROOM)
            {
                if (currentGame == LobbyId.MINI_SPIN)
                {
                    if (miniSpin.isShow)
                        miniSpin.Hide();
                }
                else if (currentGame == LobbyId.MINI_POKER)
                {
                    if (miniPoker.isShow)
                        miniPoker.Hide();
                }
                else if (currentGame == LobbyId.MINI_TAIXIU)
                {
                    if (miniTaiXiu.isShow)
                        miniTaiXiu.Hide();
                }
                //else if (currentGame == LobbyId.MINI_CAOTHAP)
                //{
                //    if (miniCaoThap.isShow)
                //        miniCaoThap.Hide();
                //}
                currentGame = LobbyId.NONE;
                ShowMenuGame(true);
            }
            else if (data.type == (int)MINIGAME.START_MATCH)
            {
                if (currentGame == LobbyId.MINI_SPIN)
                {
                    if (miniSpin.isShow && data != null)
                    {
                        miniSpin.StartMatch(data);
                    }
                }
                else if (currentGame == LobbyId.MINI_POKER)
                {
                    if (miniPoker.isShow && data != null)
                        miniPoker.StartMatch(data);
                }
                //else if (currentGame == LobbyId.MINI_CAOTHAP)
                //{
                //    if (miniCaoThap.isShow && data != null)
                //        miniCaoThap.StartMatch(data);
                //}
            }
            else if (data.type == (int)MINIGAME.ADD_BET)
            {
                miniTaiXiu.OnAddBetDone(data);
            }
            else if (data.type == (int)MINIGAME.CLEAR_BET)
            {
                miniTaiXiu.OnClearBetDone();
            }
        }
    }

    public void Reset()
    {
        if (miniSpin.isShow)
            miniSpin.Hide();
        else if (miniPoker.isShow)
            miniPoker.Hide();
        else if (miniTaiXiu.isShow)
            miniTaiXiu.Hide();
        //else if (miniCaoThap.isShow)
        //    miniCaoThap.Hide();

        currentGame = LobbyId.NONE;
        ShowMenuGame(true);
    }

    public void ShowGame(int gameType)
    {
        if (currentGame != LobbyId.NONE && currentGame != LobbyId.MINI_TAIXIU && gameType == 0)
        {
            OGUIM.Toast.Show("Bạn đang tham gia minigame...", UIToast.ToastType.Warning);
            return;
        }

        switch (gameType)
        {
            case 0:
                BuildWarpHelper.MINI_Sub(LobbyId.MINI_POKER, 100, 1, () =>
                {
                    UILogView.Log("MINI_Sub  MINI_POKER is time out");
                    currentGame = LobbyId.NONE;
                });
                currentGame = LobbyId.MINI_POKER;
                break;
            case 1:
                BuildWarpHelper.MINI_Sub(LobbyId.MINI_SPIN, 0, 1, () =>
                {
                    UILogView.Log("MINI_Sub  MINI_SPIN is time out");
                    currentGame = LobbyId.NONE;
                });
                currentGame = LobbyId.MINI_SPIN;
                break;
            case 2:
                miniTaiXiu.isAutoSub = false;
                BuildWarpHelper.MINI_Sub(LobbyId.MINI_TAIXIU, 0, 1, () =>
                {
                    UILogView.Log("MINI_Sub  MINI_TAIXIU is time out");
                    currentGame = LobbyId.NONE;
                });
                currentGame = LobbyId.MINI_TAIXIU;
                break;
            case 3:
                BuildWarpHelper.MINI_Sub(LobbyId.MINI_CAOTHAP, 1000, 1, () =>
                {
                    UILogView.Log("MINI_Sub  MINI_CAOTHAP is time out");
                    currentGame = LobbyId.NONE;
                });
                currentGame = LobbyId.MINI_CAOTHAP;
                break;
        }
        ShowMenuGame(false);
    }
}
