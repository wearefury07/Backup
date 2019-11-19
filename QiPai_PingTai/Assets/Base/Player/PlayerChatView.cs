using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerChatView : MonoBehaviour
{
    public bool inListView = false;
    public Text displayName;
    public Text time;
    public Text message;
    public Image emotion;
    public Color myColor;
    public Color ortherColor;
    public int delayTime;

    public int maxCharacterLength = 128;

    private RectTransform rectTransform;
    private RectTransform rectTransformParent;
    private DateTime dateTime;

    void Awake()
    {
        rectTransform = message.GetComponent<RectTransform>().parent.GetComponent<RectTransform>();
        if (inListView)
        {
            rectTransformParent = message.GetComponent<RectTransform>().parent.parent.GetComponent<RectTransform>();
        }
        else
        {
            gameObject.SetActive(true);
            if (emotion != null)
                emotion.gameObject.SetActive(true);
        }
    }
    private void Start()
    {
        gameObject.SetActive(true);
        if (emotion != null)
            emotion.gameObject.SetActive(true);

        if (!inListView)
        {
            gameObject.SetActive(false);
            if (emotion != null)
                emotion.gameObject.SetActive(false);
        }
    }

    public bool FillData(RootChatData data)
    {
        try
        {
            if (data == null || string.IsNullOrEmpty(data.message))
                return false;

            if (data.message.Length > maxCharacterLength)
                message.text = data.message.Substring(0, maxCharacterLength) + "...";
            else
                message.text = data.message;

            if (inListView)
            {
                if (data.userId == OGUIM.me.id)
                {
                    displayName.color = myColor;
                }
                else
                {
                    displayName.color = ortherColor;
                }
                displayName.text = data.userName;
                time.text = "vài giây trước";

                dateTime = DateTime.Now;

                Debug.Log(message.preferredWidth + " " + message.preferredHeight);

                var background = message.transform.parent.GetComponent<RectTransform>();
                var chatView = GetComponent<RectTransform>();
                var verticalLayoutGroup = background.GetComponent<VerticalLayoutGroup>();
                var mesHeight = message.preferredHeight + verticalLayoutGroup.padding.top + verticalLayoutGroup.padding.bottom + Math.Abs(rectTransform.offsetMax.y);
                chatView.sizeDelta = new Vector2(rectTransformParent.sizeDelta.x, mesHeight);
            }


            if (!inListView)
            {
                while (!gameObject.activeSelf)
                    gameObject.SetActive(true);

                DOVirtual.DelayedCall(delayTime, () =>
                {
                    gameObject.SetActive(false);
                });
            }
        }
        catch (System.Exception ex)
        {
            UILogView.Log("PlayerChatView FillData: " + "\n" + ex.Message + "\n" + ex.StackTrace, true);
            return false;
        }
        return true;
    }

    public void UpdateTime()
    {
        if (time != null && inListView)
            time.text = DateTimeConverter.ToRelativeTime(dateTime);
    }
}
