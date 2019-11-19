using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Effect_Jackpot : MonoBehaviour {
    public Image typeImage;
    public Sprite[] jackpotSprites;
    public void Active(int typeJackpot = 1)
    {
        typeImage.sprite = typeJackpot == 1 ? jackpotSprites[0] : jackpotSprites[1];
        gameObject.SetActive(true);
    }
    public void OnAnimationDone()
    {
        gameObject.SetActive(false);
    }
}
