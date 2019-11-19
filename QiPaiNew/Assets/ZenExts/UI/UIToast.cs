using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIToast : MonoBehaviour
{
    public float paddingLeft;
    public float paddingRight;
    public float maximumWidth;
    public Text icon;
    public Text message;
    public GameObject deactvie;
	public GameObject content;

    public float timeAutoHide = 5.0f;
    public ToastType toastType;
    public string defaultMessage = "";
    public UIAnimation anim;

    private int startCount;
    //private int randomSelection = 0;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = message.GetComponent<RectTransform>().parent.GetComponent<RectTransform>();
        anim = gameObject.GetComponent<UIAnimation>();
    }

    private string[] notificationMessages = new string[]
    {
        "Responsive UI",
        "Custom Animation Presets",
        "No Code Needed",
        "UIManager calculates all the resolution and aspect ratio changes and adjusts the animations accordingly. Because of this all the animations look and feel the same in both Landscape and Portrait Modes.",
        "Save your animations and load them in other projects. (simple .xml files)",
    };

    public void RandomMes()
    {
        var ran = Random.Range(0, notificationMessages.Length);
        ShowNotification(notificationMessages[ran]);
    }

    public void ShowNotification(string mes)
    {
        ShowToast(mes, ToastType.Notification, 1f);
    }

    public void ShowLoading(string mes, float _timeAutoHide = 0f)
    {
        if (_timeAutoHide == 0)
            _timeAutoHide = OldWarpChannel.Channel.recieveTimeOut;
        ShowToast(mes, ToastType.Loading, _timeAutoHide);
    }

    public void Show(string mes, ToastType _toastType = ToastType.Loading, float _timeAutoHide = 1f)
    {
        ShowToast(mes, _toastType, _timeAutoHide);
    }

    public void Hide()
    {
        UIManager.instance.StopCoroutine(HideNotification());
        if (!string.IsNullOrEmpty(message.text.Trim()) && anim.state != UIAnimation.State.IsHide)
        {
            anim.Hide();
        }
        deactvie.SetActive(false);
        timeAutoHide = 0;
    }


    private void ShowToast(string mes, ToastType toast = ToastType.Loading, float _timeAutoHide = 1f)
    {
         timeAutoHide = _timeAutoHide;

        if (rectTransform == null)
            rectTransform = message.GetComponent<RectTransform>().parent.GetComponent<RectTransform>();
        if (anim == null)
            anim = gameObject.GetComponent<UIAnimation>();

        icon.text = ToastIcon(toast);
        if (toast == ToastType.Notification)
        {
            //icon.color = new Color32(180, 255, 255, 255);
            deactvie.SetActive(false);
        }
        else if (toast == ToastType.Warning)
        {
            //icon.color = new Color32(255, 180, 255, 255);
            deactvie.SetActive(false);
        }
        else if (toast == ToastType.Error)
        {
            //icon.color = new Color32(220, 0, 0, 255);
            deactvie.SetActive(false);
        }
        else
        {
            //icon.color = new Color32(255, 255, 255, 255);
            deactvie.SetActive(true);
        }

        if (!string.IsNullOrEmpty(mes.Trim()))
        {
            message.text = mes ?? "";
            var preferredWidth = message.preferredWidth;
            if (maximumWidth != 0 && preferredWidth + paddingLeft + paddingRight > maximumWidth)
                preferredWidth = maximumWidth;
            rectTransform.sizeDelta = new Vector2(preferredWidth + paddingLeft + paddingRight, rectTransform.sizeDelta.y);

			startCount = 0;
			if (anim.state != UIAnimation.State.IsShow)
				anim.Show();
		}
		else
		{
			content.gameObject.SetActive (false);
		}

		UIManager.instance.StartCoroutine(HideNotification());

		if (!gameObject.activeSelf)
			gameObject.SetActive(true);
    }

    private string ToastIcon(ToastType toast)
    {
        var icon = "";
        switch (toast)
        {
            case ToastType.Loading:
                icon = "";
                break;
            case ToastType.Notification:
                icon = "";
                break;
            case ToastType.Warning:
                icon = "";
                break;
            case ToastType.Error:
                icon = "";
                break;
        }
        return icon;
    }

    public void BackButtonEvent()
    {
		if (toastType != ToastType.Loading)
        {
            if (UIManager.instance != null)
                UIManager.instance.StopCoroutine(HideNotification());
            deactvie.SetActive(false);
			if (anim.state != UIAnimation.State.IsHide)
				anim.Hide ();
			else if (gameObject.activeSelf)
				gameObject.SetActive (false);
        }
    }

    public IEnumerator HideNotification()
    {
        //Debug.LogWarning("startCount : " + startCount + "/" + timeAutoHide);
        startCount = 0;
		while (startCount < timeAutoHide || anim.state == UIAnimation.State.IsAnimation)
        {
            startCount++;
            //Debug.LogWarning(startCount + "/" + timeAutoHide);
            yield return new WaitForSeconds(1);
        }
        deactvie.SetActive(false);
		if (anim.state != UIAnimation.State.IsHide) {
			anim.Hide ();
		} else if (gameObject.activeSelf)
				gameObject.SetActive (false);
    }

    public enum ToastType
    {
        Loading,
        Notification,
        Warning,
        Error,
    }
}
