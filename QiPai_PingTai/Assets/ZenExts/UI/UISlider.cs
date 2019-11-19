using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
[DisallowMultipleComponent]
public class UISlider : MonoBehaviour
{
    public string nameValue = "";
    public int scaleValue = 1;
    public Text sliderValue;
    public Slider slider;

    [SerializeField]
    public myFloatEvent eventOnMaxValue;

    [Serializable]
    public class myFloatEvent : UnityEvent { }

    public void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
        slider.value = 0;
        if (scaleValue == 0)
            scaleValue = 1;
        if (string.IsNullOrEmpty(nameValue))
            nameValue = "";
    }

    public void UpdateSliderValue()
    {
        if (slider != null && sliderValue != null)
        {
            sliderValue.text = (slider.value * scaleValue + " " + nameValue).Trim();
        }
    }


    public Text statusLeft, statusRight;
    public void Status(string left = "", string right = "")
    {
        if (!string.IsNullOrEmpty(left))
            statusLeft.text = left;
        if (!string.IsNullOrEmpty(right))
            statusRight.text = right;
    }

    public void Loading(string status, float percent)
    {
        if (!string.IsNullOrEmpty(status))
            statusLeft.text = status;

        slider.value = percent;
        statusRight.text = percent.ToString("F0") + "%";

        if (slider.value == slider.maxValue && eventOnMaxValue != null)
        {
            eventOnMaxValue.Invoke();
            eventOnMaxValue = null;
        }
    }

    public void SetValue(float value)
    {
        slider.value = value;
        UpdateSliderValue();
    }

    public float GetValue()
    {
        return slider.value;
    }
}
