using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class UIMessageBox : MonoBehaviour
{
    public Text title;
    public Text messager;

    public UIAnimation popupOverlay;
    public UIAnimation popupContent;
    public ScrollRect scrollRect;

    //Zendios – a team spcializing in cross-platform mobile applications and games based on Vietnamese legendary with funny and perfect pixel design.
    //Zendios – a team spcializing in cross-platform mobile applications and games based on Vietnamese legendary with funny and perfect pixel design.
    //Zendios – a team spcializing in cross-platform mobile applications and games based on Vietnamese legendary with funny and perfect pixel design.


    [Serializable]
    public class myFloatEvent : UnityEvent { }

    public Button buttonClose;
    public myFloatEvent eventOnClose;

    public Button button_1;
    public myFloatEvent eventButton_1;

    public Button button_2;
    public myFloatEvent eventButton_2;

    public Toggle toggle;

    private static UIMessageBox MessageBox;

    private void Awake()
    {
        MessageBox = this;
    }

    public void Show(string _title, string _mes, string button1Content = null, TweenCallback button1Action = null, string button2Content = null, TweenCallback button2Action = null, TweenCallback actionOnClose = null, bool showToggle = false)
    {
        popupOverlay.gameObject.SetActive(true);
        if (title != null)
            title.text = _title.ToUpper();
        if (messager != null)
            messager.text = "\n" + _mes + "\n";

        if (messager.preferredHeight > 200)
            scrollRect.vertical = true;
        else
            scrollRect.vertical = false;

        scrollRect.verticalNormalizedPosition = 1f;

        if (buttonClose != null)
        {
            buttonClose.onClick.RemoveAllListeners();
            buttonClose.onClick.AddListener(() => Hide(actionOnClose));
        }

        button_1.onClick.RemoveAllListeners();
        if (button_1 != null)
        {
            if (string.IsNullOrEmpty(button1Content))
                button1Content = "Đồng ý";
            button_1.transform.GetComponentInChildren<Text>().text = button1Content;

            button_1.onClick.RemoveAllListeners();
            if (button1Action != null)
            {
                button_1.onClick.AddListener(() => Hide(() =>
                {
                    eventButton_1.Invoke(); button1Action();
                }));
            }
            else
            {
                button_1.onClick.AddListener(() => Hide(() =>
                {
                    eventButton_1.Invoke();
                }));
            }
        }

        if (button_2 != null)
        {
            if (!string.IsNullOrEmpty(button2Content))
            {
                button_2.gameObject.SetActive(true);
                button_2.transform.GetComponentInChildren<Text>().text = button2Content;
                button_2.onClick.RemoveAllListeners();
                if (button2Action != null)
                {
                    button_2.onClick.AddListener(() => Hide(() =>
                    {
                        eventButton_2.Invoke(); button2Action();
                    }));
                }
                else
                {
                    button_2.onClick.AddListener(() => Hide(() =>
                    {
                        eventButton_2.Invoke();
                    }));
                }
            }
            else
            {
                button_2.gameObject.SetActive(false);
            }
        }

        toggle.gameObject.SetActive(showToggle);

        transform.SetPosition(Vector2.zero, Space.Self);
        popupOverlay.Show();
        popupContent.Show();
    }

    public void Hide(TweenCallback actionOnHide = null)
    {
        popupOverlay.Hide();
        popupContent.Hide(actionOnHide);
    }
}
