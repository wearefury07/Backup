using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RoomView : MonoBehaviour
{
    public Image moneyImage;
    public Image background;
    public Text valueText;
    public bool canJoin;
    public int roomValue;
    public MoneyType moneyType;

    public void JoinRoom()
    {
        if (canJoin)
        {
			OGUIM.Toast.ShowLoading("");
            WarpRequest.JoinRoom(moneyType, roomValue);

        }
        else
        {
            OGUIM.Toast.Show("Vui lòng chọn mức cược phù hợp!", UIToast.ToastType.Warning, 3f);
            //OGUIM.MessengerBox.Show("Oops...!", "Số \"" + OGUIM.currentMoney.name + "\" không đủ." + "\n"
            //    + "Vui lòng nạp thêm \"" + OGUIM.currentMoney.name + "\" để tiêp tục trải nghiệm trò chơi!",
            //    "Nạp " + OGUIM.currentMoney.name,
            //        () => OGUIM.Toast.ShowNotification("Show popup TopUp...!"),
            //    "Lần sau", null);
        }
    }

    public void FillData(int _roomValue, bool _canJoin, MoneyType _moneyType)
    {
        try
        {
            roomValue = _roomValue;
            canJoin = _canJoin;
            moneyType = _moneyType;

            valueText.text = LongConverter.ToK(roomValue);
            if (moneyType == MoneyType.Gold)
                moneyImage.sprite = GameBase.moneyGold.image;
            else
                moneyImage.sprite = GameBase.moneyKoin.image;

            if (!canJoin)
            {
                valueText.color = new Color32(255, 255, 255, 128);
                moneyImage.color = new Color32(255, 255, 255, 128);
            }
            else
            {
                valueText.color = new Color32(255, 200, 0, 255);
                moneyImage.color = new Color32(255, 255, 255, 255);
            }
        }
        catch (System.Exception ex)
        {
            UILogView.Log("RoomView FillData: " + roomValue + "\n" + ex.Message + "\n" + ex.StackTrace, true);
        }
    }
}
