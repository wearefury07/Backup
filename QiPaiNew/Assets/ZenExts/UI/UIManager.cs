using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Text;

[DisallowMultipleComponent]
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //The container that has all the ui elements ("UI Container")
    private static Transform uiContainer;

    #region Sound Music DebugMode Particle
    public string defaultSoundName;
    public Toggle soundToggle;

    public AudioSource musicAudioSource;
    public string defaultMusicName;
    public Toggle musicToggle;
    public Slider musicVolume;

    public Toggle debugModeToggle;
    public Toggle particleToggle;
    public UIAnimation uiLogView;
    #endregion

    public static Vector2 startAnchoredPosition2D = Vector2.zero;
    public static float pixelsPerUnit = 1.0f;

    public delegate void ScreenSizeChangedDelegate();
    public event ScreenSizeChangedDelegate OnSizeChanged;

    void Awake()
    {
        InitDoTween();
        UpdateUiContainer();
        StartCoroutine("GetScreenSize");
		StartCoroutine ("GetOrientation");
        instance = this;
    }

    void Start()
    {
        DebugModeCheck();
        SoundCheck();
        MusicCheck();
        ParticleCheck();
        GetScreenSize();
        PlayMusic(true);
        currentGameTimeScale = Time.timeScale;
        pixelsPerUnit = uiContainer.GetComponent<RectTransform>().sizeDelta.y / 2f / Camera.main.orthographicSize;
    }

    public void OnClickPolicy()
    {
        if (GameBase.underReview)
            Application.OpenURL("https://368vip.club/policy.html");
        else
            Application.OpenURL("https://368vip.club/taikhoan/policy.html?id=123123123123");
    }

    public void OnClickSupport()
    {
        if (!string.IsNullOrEmpty(GameBase.fbFanpage))
        {
            OpenURL(GameBase.fbFanpage);
        }
        else
        {
            OGUIM.MessengerBox.Show("Thông Báo", "Chưa có FANPAGE");
        }

        
        //string tokenStr = string.Format("{0}|{1}|{2} ", System.DateTime.Now.ToString("yyyyMMddHHmmss"), OGUIM.me.username, PlayerPrefs.GetString("password", ""));
        //string tokenStr = string.Format("{0}|{1}|{2} ", System.DateTime.Now.ToString("yyyyMMddHHmmss"), "admin", "admin");
        //string tokenStr = "20181106163938|admin|admin";
        //string encrypted = RC4.AES_encrypt(tokenStr);
        //Debug.Log("encrypt: "+encrypted);
        //Debug.Log("URL : " + WWW.EscapeURL(encrypted));
        //encrypted = RC4.AES_decrypt(encrypted);
        //Debug.Log("decrypt: " +encrypted);
		//Application.OpenURL(HttpRequest.url_getPaypaypage+"?token="+encrypted);
//        StartCoroutine(HttpRequest.Instance.GetAPIPaypay(encrypted));
        //Debug.LogError("Toke STR: " + tokenStr);
        //tokenStr = RC4.RC4_Ver(HttpRequest.keyyRRCC4, tokenStr);
        //StartCoroutine(HttpRequest.Instance.GetAPIPaypay(WWW.EscapeURL(tokenStr));
        //Debug.Log("Encrypt: " + tokenStr);
        //tokenStr = RC4.RC4_Ver(HttpRequest.keyyRRCC4, tokenStr);

    }

    public void OnClickAgency()
    {
        OGUIM.instance.popupTopUp.Show(2);
    }

    public void OnClickNews()
    {
        OGUIM.MessengerBox.Show("Thông Báo", "Chưa có chức năng này");
    }

    public void OnClickEvents()
    {
        OGUIM.MessengerBox.Show("Thông Báo", "Chưa có trang sự kiện");
    }

    public void OnClickLostPswd()
    {
        Application.OpenURL("http://sso.win360.club/login");
    }

    public void OnClickVerifyPhoneNumber()
    {
        Application.OpenURL("http://sso.win360.club/login");
    }

    public void OnClickGiftReward()
    {
        Application.OpenURL("http://sso.win360.club/login");
    }

    public void OnClickPayment()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (GameBase.underReview)
        {
            OGUIM.instance.popupIAP.Show();
            return;
        }
#endif
        OGUIM.MessengerBox.Show("Thông Báo", "Mọi giao dịch nạp tiền của Win360 đều được thực hiện trên cổng thanh toán win360.club");
        //OGUIM.instance.popupTopUp.Show(0);
    }

    public void OnClickRewarded()
    {
        OGUIM.MessengerBox.Show("Thông Báo", "Mọi giao dịch đổi thưởng của Win360 đều được thực hiện trên cổng thanh toán win360.club");

    }

    #region Methods for Game Management - TogglePause, ApplicationQuit, DebugLog

    public static bool gamePaused = false;                              //Check if the game is paused or not
    public static float currentGameTimeScale = 1;                       //We presume 1, but we check every time the player presses the pause button
    public static float transitionTimeForTimeScaleChange = 0.25f;       //This is the transition time, in seconds, when we pause or unpause the game (looks nicer instead of instant stopping the game)

    public static void Call(object number)
    {
        try
        {
            //OGUIM.Toast.ShowNotification("Call to " + data.tel);
            if (number != null && !string.IsNullOrEmpty(number.ToString()))
            {
                Application.OpenURL("tel://" + number.ToString());
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public static void OpenURL(string url)
    {
        if (url != null && !string.IsNullOrEmpty(url))
        {
            if (url.Contains("http"))
                Application.OpenURL(url);
            else
                Application.OpenURL("http://" + url);

            Debug.Log(" Url : " + url);
        }
    }

    public static void TogglePause()
    {
        if (gamePaused)
        {
            //DOTween.To(x => Time.timeScale = x, 0f, currentGameTimeScale, transitionTimeForTimeScaleChange).Play(); //DISABLED in 2.4.1
            Time.timeScale = currentGameTimeScale;
            gamePaused = false;
        }
        else
        {
            currentGameTimeScale = Time.timeScale;
            //DOTween.To(x => Time.timeScale = x, currentGameTimeScale, 0f, transitionTimeForTimeScaleChange).Play(); //DISABLED in 2.4.1
            Time.timeScale = 0f;
            gamePaused = true;
        }
    }

    public static void ApplicationQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion

    #region Methods for UI - UpdateUiContainer, UpdateSettings, InitDoTween, UpdateUIScreenRect, CreateBlackScreen
    private void InitDoTween()
    {
//#if (UNITY_EDITOR)
//        DOTween.Init(null, false, null);
//#else
        DOTween.Init();
//#endif
    }

    public static Transform GetUiContainer
    {
        get
        {
            if (uiContainer == null)
            {
                UpdateUiContainer();
            }
            return uiContainer;
        }
    }

    private static void UpdateUiContainer()
    {
        if (uiContainer == null)
            uiContainer = FindObjectOfType<UIManager>().transform.parent;
    }
    #endregion

    #region Methods for Sound Music - SoundCheck, MusicCheck, DebugMode, ToggleSound, ToggleMusic, ToggleDebugMode

    #region Sound
	// deal_13 : chia bai
	// flip : danh bai
	// knock : co nguoi vao phong
	// close : co nguoi thoat phong
	// pass : bo luot
	// ready : san sang
	// winchip : duoc tien
	// white_win : thang trang + thang to
    public static void PlaySound(string soundName)
    {
        if (string.IsNullOrEmpty(soundName) || instance.soundToggle.isOn == false || soundName.Equals(instance.defaultSoundName))
            return;
        AudioClip clip = Resources.Load(soundName) as AudioClip;
        if (clip == null)
        {
            Debug.Log("[UIAnimation] There is no sound file with the name [" + soundName + "] in any of the Resources folders.\n Check that the spelling of the fileName (without the extension) is correct or if the file exists in under a Resources folder");
            return;
        }
        PlayClipAt(clip);
    }

    public static AudioSource PlayClipAt(AudioClip clip, Vector3 pos = new Vector3())
    {
        var tempGO = new GameObject("TempAudio - " + clip.name);
        tempGO.transform.position = pos;
        var aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.Play();
        Destroy(tempGO, clip.length);
        return aSource;
    }


    public void ToggleSound()
    {
        if (soundToggle != null)
        {
            PlayerPrefs.SetInt("soundState", soundToggle.isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static void SoundCheck()
    {
        if (instance.soundToggle != null)
        {
            instance.soundToggle.isOn = PlayerPrefs.GetInt("soundState", 1) == 1 ? true : false;
            instance.UpdateMusicState();
        }
    }
    #endregion

    #region DebugMode
    public void ToggleDebug()
    {
        if (instance.debugModeToggle != null && instance.uiLogView != null)
        {
            PlayerPrefs.SetInt("debugState", debugModeToggle.isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static void DebugModeCheck()
    {
        if (instance.debugModeToggle != null && instance.uiLogView != null)
        {
            instance.debugModeToggle.isOn = PlayerPrefs.GetInt("debugState", 0) == 1 ? true : false;
            if (instance.debugModeToggle.isOn)
                instance.uiLogView.Show();
            else
                instance.uiLogView.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Music
    public void ToggleMusic()
    {
        if (musicToggle != null)
        {
            musicVolume.interactable = musicToggle.isOn;
            PlayerPrefs.SetInt("musicState", musicToggle.isOn ? 1 : 0);
            PlayerPrefs.Save();
            instance.UpdateMusicState();
        }
    }

    public static void MusicCheck()
    {
        instance.musicToggle.isOn = PlayerPrefs.GetInt("musicState", 1) == 1 ? true : false;
        instance.musicVolume.interactable = instance.musicToggle.isOn;

        instance.musicVolume.value = PlayerPrefs.GetFloat("musicVolume", 0.7f);

        instance.UpdateMusicState();
    }

    public void PlayMusic(bool isDefault)
    {
        if (musicAudioSource.isPlaying)
        {
            MusicFadeOut(isDefault);
            return;
        }


        musicAudioSource.loop = true;
        LoadMusic(defaultMusicName); //we check if the menuMusic filename exists in a Resources folder; if it does we create a new gameObject with an AudioSource attached and we return the reference to it

        if (musicAudioSource != null)
        {
            musicAudioSource.volume = musicVolume.value; //we set the volume to the value set in the inspector
            musicAudioSource.mute = musicToggle;  //we check if the music is on or off
            musicAudioSource.Play(); //we start the music (even if the volume is 0)
            StartCoroutine(CheckMusicState()); //we activate a listerer for the music on/off toggle; it will check the music state every 0.7 seconds (more efficeint than in the Update method)
        }
    }

    IEnumerator CheckMusicState()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.7f);
            UpdateMusicState();
        }
    }

    public void MusicFadeOut(bool isDefault)
    {
        StopCoroutine(CheckMusicState());
        StartCoroutine(DoMusicFadeOut(isDefault));
    }

    IEnumerator DoMusicFadeOut(bool isDefault = true)
    {
        while (musicAudioSource.volume * 10 > 0 && musicAudioSource.isPlaying)
        {
            musicAudioSource.volume -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        StopCoroutine(DoMusicFadeOut());
        musicAudioSource.Stop();
        PlayMusic(isDefault);
    }

    public void UpdateMusicState()
    {
        if (musicAudioSource == null)
            return;

        if (musicToggle.isOn == musicAudioSource.mute)
        {
            musicAudioSource.mute = !musicToggle.isOn;
        }
    }

    public void UpdateMusicVolume()
    {
        musicAudioSource.volume = musicVolume.value;
        PlayerPrefs.SetFloat("musicVolume", musicVolume.value);
        PlayerPrefs.Save();
    }

    private void LoadMusic(string soundName)
    {
        if (string.IsNullOrEmpty(soundName) == false)
        {
            AudioClip clip = Resources.Load(soundName) as AudioClip;
            if (clip == null)
            {
                Debug.Log("[UIManagager] There is no file with the name [" + soundName + "] in any of the Resources folders.");
            }
            else
            {
                musicAudioSource.mute = !musicToggle;  //we check if the music is on or off
                musicAudioSource.clip = clip; // define the clip
            }
        }
    }
    #endregion

    #region Effect Particle
    public void ToggleParticle()
    {
        if (instance.particleToggle != null)
        {
            PlayerPrefs.SetInt("particleState", particleToggle.isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static void ParticleCheck()
    {
        if (instance.particleToggle != null)
        {
            instance.particleToggle.isOn = PlayerPrefs.GetInt("particleState", 1) == 1 ? true : false;
        }
    }
    #endregion
    #endregion

    #region IEnumerators - GetScreenSize, GetOrientation

    public static bool firstPass = true;
    public static Camera uiCamera = null;
    public static UIScreenRect uiScreenRect;

    [System.Serializable]
    public class UIScreenRect
    {
        public Vector2 size = Vector2.zero;
        public Vector2 position = Vector2.zero;
    }

    IEnumerator GetScreenSize()
    {
        int infiniteLoopBreak = 0;

        while (firstPass)
        {
            yield return new WaitForEndOfFrame();
            UpdateUIScreenRect();

            if (firstPass)  //this check is needed since in the first frame of the application the uiScreenRect is (0,0); only from the second frame can we get the screen size values
            {
                firstPass = false;
            }

            infiniteLoopBreak++;
            if (infiniteLoopBreak > 1000)
                break;
        }

        GetUICamera.enabled = true;
    }

    public static void UpdateUIScreenRect()
    {
        uiScreenRect = new UIScreenRect();
        UpdateUiContainer();
        uiScreenRect.size = GetUiContainer.GetComponent<RectTransform>().rect.size;
        uiScreenRect.position = GetUiContainer.GetComponent<RectTransform>().rect.position;
    }

    public static Camera GetUICamera
    {
        get
        {
            if (uiCamera == null)
            {
                uiCamera = GetUiContainer.transform.parent.GetComponentInChildren<Camera>();
            }
            return uiCamera;
        }
    }

    public static Orientation currentOrientation = Orientation.Unknown;

    IEnumerator GetOrientation()
    {
        if (currentOrientation != Orientation.Unknown)
            Debug.Log("DeviceOrientation_" + currentOrientation);

        int infiniteLoopBreak = 0;

        while (currentOrientation == Orientation.Unknown)
        {
            CheckDeviceOrientation();

            if (currentOrientation != Orientation.Unknown)
                break;

            yield return null;

            infiniteLoopBreak++;
            if (infiniteLoopBreak > 1000)
                break;
        }
    }

    public static void CheckDeviceOrientation()
    {
#if UNITY_EDITOR
        //PORTRAIT
        if (Screen.width < Screen.height)
        {
            if (currentOrientation != Orientation.Portrait)
            {
                ChangeOrientation(Orientation.Portrait);
            }
        }
        //LANDSCAPE
        else
        {
            if (currentOrientation != Orientation.Landscape)
            {
                ChangeOrientation(Orientation.Landscape);
            }
        }
#else
            if (Screen.orientation == ScreenOrientation.Landscape ||
               Screen.orientation == ScreenOrientation.LandscapeLeft ||
               Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                if (currentOrientation != Orientation.Landscape) //Orientation changed to LANDSCAPE
                {
                    ChangeOrientation(Orientation.Landscape);
                }
            }
            else if (Screen.orientation == ScreenOrientation.Portrait ||
                     Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            {
                if (currentOrientation != Orientation.Portrait) //Orientation changed to PORTRAIT
                {
                    ChangeOrientation(Orientation.Portrait);
                }
            }
            else //FALLBACK option if we are in AutoRotate or if we are in Unknown
            {
                ChangeOrientation(Orientation.Landscape);
            }
#endif
    }

    public static void ChangeOrientation(Orientation newOrientation)
    {
        currentOrientation = newOrientation;
        //Debug.Log("DeviceOrientation_" + currentOrientation);
    }

    public enum Orientation { Landscape, Portrait, Unknown }

    private void FixedUpdate()
    {
        if (uiScreenRect != null)
        {
            if (uiScreenRect.size != GetUiContainer.GetComponent<RectTransform>().rect.size || uiScreenRect.position != GetUiContainer.GetComponent<RectTransform>().rect.position)
            {
                UpdateUIScreenRect();
                if (OnSizeChanged != null)
                    OnSizeChanged();
            }
        }
    }
    #endregion
}
