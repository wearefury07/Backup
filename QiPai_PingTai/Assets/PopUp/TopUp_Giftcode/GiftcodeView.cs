using UnityEngine;
using UnityEngine.UI;

public class GiftcodeView : MonoBehaviour
{
    public InputField giftCode;
    public int wrongNumberMax = 3;
    private int wrongNumber;

    private void OnEnable()
    {
        WarpClient.wc.OnGiftCodeDone += Wc_OnGiftCodeDone;
    }
    
    private void OnDisable()
    {
        WarpClient.wc.OnGiftCodeDone -= Wc_OnGiftCodeDone;
    }

    private void Wc_OnGiftCodeDone(WarpResponseResultCode status)
    {
        if (status != WarpResponseResultCode.SUCCESS)
		{            
			var text = "";
			if (status == WarpResponseResultCode.GIFT_CODE_NOT_EXITS)
				text = "Xin lỗi hệ thống không nhận diện được giftcode";
			else if (status == WarpResponseResultCode.GIFT_CODE_USED)
				text = "Giftcode đã được sử dụng";
			else
				text = "Đã có lỗi xảy ra. Mã lỗi (" + status + ")";

			OGUIM.Toast.Show(text, UIToast.ToastType.Warning);

        }
        else
        {
            //OGUIM.Toast.ShowNotification("Xác nhận mã khuyến mại thành công!");
        }
        giftCode.enabled = true;
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return) && giftCode.isFocused && !string.IsNullOrEmpty(giftCode.text))
            {
                Submit();
            }
        }
    }

    public void Submit()
    {
        if (wrongNumber < wrongNumberMax)
        {

            if (SubmitFormExtend.ValidateString(giftCode, "Mã quà tặng", false))
            {
                OGUIM.Toast.ShowLoading("Đang tiến hành xác nhận...");
                giftCode.enabled = false;
				WarpRequest.EnterGiftCode(giftCode.text);
				giftCode.text = "";
            }
        }
        else
        {
            OGUIM.Toast.ShowLoading("Bạn đã nhập mã quà tặng sai quá " + wrongNumberMax + " lần");
        }
    }
}
