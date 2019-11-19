using UnityEngine;
using UnityEngine.UI;

public class IAPView_Item : MonoBehaviour
{
    public Text Description;
    public Image image;
    public Text BuyButonContent;

    public void BuyIAP()
    {
        Debug.LogError("Buy IAP " + BuyButonContent.text);
    }
}
