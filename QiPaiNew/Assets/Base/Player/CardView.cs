using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public Text remainCard;
    private int remainCardCount;

    public void Awake()
    {
    }

    public void UpdateView(int remainCardCount)
    {
        gameObject.SetActive(remainCardCount > 0);
        if (remainCard == null)
            return;
		remainCard.text = remainCardCount == 0 ? "" : remainCardCount.ToString();
    }

    public void PlusCard()
    {
        remainCardCount++;
        gameObject.SetActive(remainCardCount > 0);
        if (remainCard == null)
            return;
        remainCard.text = remainCardCount == 0 ? "" : remainCardCount.ToString();
    }
}
