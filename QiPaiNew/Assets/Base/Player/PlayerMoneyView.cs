using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoneyView : MonoBehaviour
{
    public MoneyType type;
    public NumberAddEffect moneyNumber;
    public Image moneyImage;
    private UserData userData;
    public static PlayerMoneyView View;
    public Sprite[] iconCurrency;

    private void Awake()
    {
        View = this;
    }


    public void FillData(UserData _userData)
    {
        if (_userData == null || moneyNumber == null || moneyImage == null)
        {
            Debug.Log("_userData = NULL or moneyNumber = NULL or moneyImage = NULL");
            return;
        }

        userData = _userData;
        
        if (OGUIM.currentMoney.type == MoneyType.Gold)
        {
			moneyNumber.FillData(userData.gold);
			if (_userData.id == OGUIM.me.id) 
				OGUIM.me.gold = userData.gold;
            //moneyImage.sprite = iconCurrency[0];
        }
        else
        {
			moneyNumber.FillData(userData.koin);
			if (_userData.id == OGUIM.me.id) 
				OGUIM.me.koin = userData.koin;
            //moneyImage.sprite = iconCurrency[1];
        }
        moneyImage.sprite = OGUIM.currentMoney.image;

       

    }

    public void FillData(MoneyType type, long value)
    {
        if (type == MoneyType.Gold)
        {
            moneyNumber.FillData(value);
            moneyImage.sprite = GameBase.moneyGold.image;
            //moneyImage.sprite = iconCurrency[0];
        }
        else
        {
            moneyNumber.FillData(value);
            moneyImage.sprite = GameBase.moneyKoin.image;
            //moneyImage.sprite = iconCurrency[1];
        }
    }

    public void UpdateBet(long value)
    {
        if (OGUIM.currentMoney.type == MoneyType.Gold)
        {
            moneyNumber.FillData(value);
            //moneyImage.sprite = iconCurrency[0];
        }
        else
        {
            moneyNumber.FillData(value);
            //moneyImage.sprite = iconCurrency[0];
        }

        moneyImage.sprite = OGUIM.currentMoney.image;

        gameObject.SetActive(value > 0);
    }

    public void ShowTopUp()
	{
#if UNITY_ANDROID || UNITY_IOS
		if (GameBase.underReview)
		{
			OGUIM.instance.popupIAP.Show();
			return;
		}
#endif
        //OGUIM.instance.popupTopUp.Show(0);
        //OGUIM.MessengerBox.Show("Thông Báo", "Mọi giao dịch nạp tiền của Win360 đều được thực hiện trên cổng thanh toán win360.club");
    }
}
