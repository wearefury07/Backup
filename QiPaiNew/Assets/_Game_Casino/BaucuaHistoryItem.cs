using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BaucuaHistoryItem : MonoBehaviour
{
    public Image[] images;
    // Use this for initialization
    void Start()
    {

    }

    public void SetData(bool isFade, Sprite[] sprites)
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].sprite = sprites[i];

            var color = images[i].color;
            color.a = isFade ? 0.5f : 1;
            images[i].color = color;
        }
    }
}
