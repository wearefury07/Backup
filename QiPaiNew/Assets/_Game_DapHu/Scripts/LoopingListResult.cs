using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class LoopingListResult : MonoBehaviour {
    public Image[] resultImages;
    
    // Use this for initialization
    void Start ()
    {
    }

    public void SetData(params Sprite[] sprites)
    {
        for (int i = 0; i < sprites.Length; i++)
            resultImages[i].sprite = sprites[i];
    }

    public void SetData(int roomId, params int[] sprites)
    {
        for (int i = 0; i < sprites.Length; i++)
            resultImages[i].sprite = ImageSheet.Instance.resourcesDics[string.Format("slot_{0}_{1}", roomId, sprites[i])];
    }
}
