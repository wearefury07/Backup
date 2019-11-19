using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public enum ToastType
{
    BLACK = 0,
    RED = 1,
    YELLOW = 2,
    GREEN = 3
}
public class SystemToast : MonoBehaviour {

    public Text txtContent;
    public Image imgBackground;
    public float maxWidth = 300;
    public float maxHeight = 100;
    RectTransform rect;
    Image image;
    // Use this for initialization
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    public void SetType(ToastType toastType)
    {
        switch(toastType)
        {
            case ToastType.RED:
                image.color = Color.red;
                imgBackground.color = Color.red;
                break;
            case ToastType.GREEN:
                image.color = Color.green;
                imgBackground.color = Color.green;
                break;
            case ToastType.YELLOW:
                image.color = Color.yellow;
                imgBackground.color = Color.yellow;
                break;
            default:
                //imgBackground.color = Color.black;
                break;
        }
    }
    public void SetData(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            txtContent.text = "";
            txtContent.text = str;
            rect.sizeDelta = new Vector2(Mathf.Clamp(txtContent.preferredWidth, 200, maxWidth) + 50, Mathf.Min(txtContent.preferredHeight + 15, maxHeight));
        }
    }
    public float GetHeight()
    {
        return rect.sizeDelta.y;
    }
    public void OnAnimationDone()
    {
        gameObject.Recycle();
    }
}
