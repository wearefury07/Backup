using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class MessageToast : MonoBehaviour {
    public Image toastBg;
    public Text toastContent;
    public float maxWidth = 200;
    public float maxHeight = 100;

    RectTransform rect;
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        SetAlpha(0);
    }

    void SetAlpha(float alpha)
    {
        var colorText = toastContent.color;
        var colorBg = toastBg.color;
        colorText.a = alpha;
        colorBg.a = alpha;
        toastContent.color = colorText;
        toastBg.color = colorBg;
    }
    public void SetData(string str)
    {
        if (string.IsNullOrEmpty(str))
            return;
        SetAlpha(1);
        toastContent.text = "";
        toastContent.text = str;
        rect.sizeDelta = new Vector2(Mathf.Min(toastContent.preferredWidth, maxWidth) + 50, maxHeight);

        DOTween.Kill(this);
        DOVirtual.Float(1, 0, 1, (x) => {
            SetAlpha(x);
        }).SetDelay(2).SetId(this);
    }
}
