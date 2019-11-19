using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChipButton : MonoBehaviour
{
    public Toggle toggle;
    public Image image;
    public Text text;

    public void SetImage(Sprite sprite)
    {
        //image.sprite = sprite;
    }

    public void SetText(string value)
    {
        text.text = value;
    }
}
