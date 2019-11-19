using System;
using UnityEngine;
using UnityEngine.UI;

public class CashOutItemView : MonoBehaviour
{
    public Text itemName;
    public Image itemImage;
    public Text itemCost;
    public Image itemCostImage;
    public Image hotImage;

    public CashoutProduct data;

    public bool FillData(CashoutProduct _data)
    {
        try
        {
            data = _data;

            itemName.text = data.name;
            if (data.type == (int)CashOutType.CARD && ImageSheet.Instance.resourcesDics.ContainsKey("iap-card-" + data.image))
            {
                itemImage.sprite = ImageSheet.Instance.resourcesDics["iap-card-" + data.image];
            }
            else
            {
                if (!data.image.ToLower().Contains(".png") && !data.image.ToLower().Contains(".jpg"))
                    data.image = data.image + ".png";
                string url = CashOutListView.cashOutUrlImage + data.image;
                ImageHelper.GetFromUrl(url, itemImage, null);
            }

            itemCost.text = LongConverter.ToK(data.gold);
            itemCostImage.sprite = GameBase.moneyGold.image;
            if (hotImage != null)
            {
                if (data.hot == 1)
                    hotImage.gameObject.SetActive(true);
                else
                    hotImage.gameObject.SetActive(false);
            }
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("CashOutCardItemView: FillData: " + ex.Message + "\n" + ex.StackTrace);
        }
        return false;
    }

    public void CashOut()
    {
        CashOutListView.curentCashOut = data;

		if (OGUIM.isVerified == 0 && !GameBase.isOldVersion)
        {
            OGUIM.MessengerBox.Show("Yêu cầu xác thực số điện thoại", "Vui long xác thực SỐ ĐIỆN THOẠI trước",
                "Đồng ý", () =>
                {
                    OGUIM.instance.popupVerifyPhone.Show();
                },
                "Lần sau", null);
        }
        else
        {
            OGUIM.MessengerBox.Show("Xác nhận đổi thưởng", "Yêu cầu đổi thưởng "
                + "\n" + "<color=#FFC800FF>" + CashOutListView.curentCashOut.name + " trị giá " + LongConverter.ToFull(CashOutListView.curentCashOut.money) + "VNĐ" + "</color>"
                + "\n" + " bằng "
                + "\n" + "<color=#FFC800FF>" + LongConverter.ToFull(CashOutListView.curentCashOut.gold) + " " + GameBase.moneyGold.name + "</color>",
                "Xác nhận", () =>
                {
                    OGUIM.Toast.ShowLoading("Đang gửi yêu cầu đổi " + CashOutListView.curentCashOut.name + "...");
                    WarpRequest.CashOut(data.id);
                },
                "Lần sau", null);
        }
    }
}
