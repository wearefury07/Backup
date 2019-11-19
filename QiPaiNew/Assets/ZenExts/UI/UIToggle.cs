using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
[DisallowMultipleComponent]
public class UIToggle : MonoBehaviour
{
    [Range(0f, 1f)]
    [Tooltip("AlphaIsOn = 0 is SetActive(false) && AlphaIsOn = 1 is SetActive(true)")]
    public float alphaIsOn = 0.5f;
    public List<GameObject> elementsAlphaIsOn;

    [Range(0f, 1f)]
    [Tooltip("AlphaIsOff = 0 is SetActive(false) && AlphaIsOff = 1 is SetActive(true)")]
    public float alphaIsOff = 0.5f;
    public List<GameObject> elementsAlphaIsOff;

    [Serializable]
    public class myFloatEvent : UnityEvent { }

    [SerializeField]
    [Tooltip("If Toggle Is On do eventOnChanged")]
    public myFloatEvent eventOnChanged;

    public Toggle toggle;
    public Text textContent;
    public Image imageContent;

    public Color colorIsOn = new Color32(255, 255, 255, 255);
    public Color colorIsOff = new Color32(255, 255, 255, 150);

    void Awake()
    {
        toggle = gameObject.GetComponent<Toggle>();
        ChangeEffect(true);
        toggle.onValueChanged.AddListener(ChangeEffect);
        if (eventOnChanged != null)
            toggle.onValueChanged.AddListener(ActionOnOnChanged);

        //updateContentView();
    }

    private void Start()
    {
        if (toggle == null)
            toggle = gameObject.GetComponent<Toggle>();
    }

    private void ActionOnOnChanged(bool v)
    {
        if (eventOnChanged != null && toggle.isOn)
            eventOnChanged.Invoke();
    }

    public void ChangeEffect(bool arg0)
    {
        if (elementsAlphaIsOff != null && toggle != null)
        {
            foreach (var i in elementsAlphaIsOff)
            {
                if (i != null)
                {
                    if (alphaIsOff == 0)
                    {
                        i.SetActive(toggle.isOn);
                    }
                    else if (alphaIsOff == 1)
                    {
                        i.SetActive(!toggle.isOn);
                    }
                    else
                    {
                        var cg = i.GetComponent<CanvasGroup>();
                        if (cg == null)
                            cg = i.AddComponent<CanvasGroup>();

                        if (toggle.isOn)
                            cg.alpha = 1f;
                        else
                            cg.alpha = alphaIsOff;
                    }
                }
            }
        }

        if (elementsAlphaIsOn != null && toggle != null)
        {
            foreach (var i in elementsAlphaIsOn)
            {
                if (alphaIsOn == 0)
                {
                    i.SetActive(!toggle.isOn);
                }
                else if (alphaIsOff == 1)
                {
                    i.SetActive(toggle.isOn);
                }
                else
                {
                    var cg = i.GetComponent<CanvasGroup>();
                    if (cg == null)
                        cg = i.AddComponent<CanvasGroup>();

                    if (toggle.isOn)
                        cg.alpha = 1f;
                    else
                        cg.alpha = alphaIsOn;
                }
            }
        }
        UpdateTextColor();

    }

    public void UpdateTextContent(object obj)
    {
        if (textContent == null)
            textContent = transform.GetComponentInChildren<Text>();
        if (textContent != null && obj != null)
            textContent.text = obj.ToString();
    }

    public void UpdateImageContent(Sprite obj)
    {
        if (imageContent == null)
            imageContent = transform.GetComponentInChildren<Image>();
        if (imageContent != null)
            imageContent.sprite = obj;
    }

    public void UpdateTextColor()
    {
        if (colorIsOn.a != 0)
            UpdateTextColor(colorIsOn, colorIsOff);
    }

    public void UpdateTextColor(Color isOnColor, Color isOffColor)
    {
        if (textContent == null)
            textContent = transform.GetComponentInChildren<Text>();
        if (textContent != null)
        {
            if (toggle.isOn)
                textContent.color = isOnColor;
            else
                textContent.color = isOffColor;
        }
    }

    public void updateContentView()
    {
        if (toggle.isOn)
        {
            OGUIM.instance.ToggleChangeMoneyType((int)MoneyType.Gold);
            imageContent.sprite = GameBase.moneyGold.image;
            textContent.text = GameBase.moneyGold.name;
        }
        else
        {
            OGUIM.instance.ToggleChangeMoneyType((int)MoneyType.Koin);
            imageContent.sprite = GameBase.moneyKoin.image;
            textContent.text = GameBase.moneyKoin.name;
        }
    }
}