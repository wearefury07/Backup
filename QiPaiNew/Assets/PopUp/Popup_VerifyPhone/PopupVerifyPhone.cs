using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopupVerifyPhone : MonoBehaviour
{
    public UIToggleGroup uiToggleGroup;

    public Text noteStep1;
    public InputField mobileInputField;
    public Button getTokenButton;

    public Text noteStep2;
    public InputField tokenInputField;
    public Button verifyTokenButton;

    public PopupUserInfo popupUserInfo;

    public string stringNote1 = "<color=#FFFFFF>Miễn phí xác thực số điện thoại:</color>"
                + "\n\n" + "√ Tặng 500 368vipEdited khi xác thực thành công"
                + "\n" + "√ Khôi phục mật khẩu khi bạn quên"
                + "\n" + "√ Kích hoạt tính năng đổi thưởng"
                + "\n" + "(mỗi tài khoản xác thực 01 số điện thoại duy nhất)";

    public UIAnimation anim;

    public void Show()
    {
        mobileInputField.text = "";
        tokenInputField.text = "";
        anim.Show(() => FillData());
    }

    private void OnEnable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnUserVerityMobileDone += Wc_OnUserVerityMobileDone;
        }
    }

    private void OnDisable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnUserVerityMobileDone -= Wc_OnUserVerityMobileDone;
        }
    }

    private void Wc_OnUserVerityMobileDone(WarpResponseResultCode status, BaseData data)
    {
        if (status == WarpResponseResultCode.SUCCESS && data != null)
        {
            //data.type == 0 đã gửi phone chờ token OTP
            //data.type == 1 đã gửi token và verity mobile done
            if (data.type == 0)
                OnGetTokenDone();
            else
                OnVerifyTokenDone();
        }
        else if (status == WarpResponseResultCode.USER_HAS_VERIFIED)
        {
            OnVerifyTokenDone();
        }
        else if (status == WarpResponseResultCode.MOBILE_VERIFIED_OTHER)
        {
            OGUIM.Toast.ShowNotification("Số điện thoại này đã được xác thực trên một tài khoản khác!");
        }
        else
        {
            OGUIM.Toast.ShowNotification("Xác thực số điện thoại thất bại!" + "\n" + "Mã lỗi: " + status.ToString());
        }
    }

    public void FillData()
    {
        uiToggleGroup.IsOn(0);

        string stringNote1 = "<color=#FFFFFF>Miễn phí xác thực số điện thoại và nhận được:</color>"
           + "\n\n" + "√ Tặng 500 " + GameBase.moneyGold.name + " khi xác thực thành công"
           + "\n" + "√ Khôi phục mật khẩu khi bạn quên"
           + "\n" + "√ Kích hoạt tính năng đổi thưởng"
           + "\n" + "(mỗi tài khoản xác thực 01 số điện thoại duy nhất)";
        noteStep1.text = stringNote1;
        string stringNote2 = "<color=#FFFFFF>Nhập mã xác thực (OTP) từ tin nhắn để xác thực và nhận được:</color>"
           + "\n\n" + "√ Tặng 500 " + GameBase.moneyGold.name + " khi xác thực thành công"
           + "\n" + "√ Khôi phục mật khẩu khi bạn quên"
           + "\n" + "√ Kích hoạt tính năng đổi thưởng"
           + "\n" + "(mỗi tài khoản xác thực 01 số điện thoại duy nhất)";
        noteStep2.text = stringNote2;
    }

    public void GetToken()
    {
        if (SubmitFormExtend.ValidatePhoneNumber(mobileInputField, "Số điện thoại", false))
        {
            OGUIM.Toast.ShowLoading("");
            WarpRequest.UserVerifyPhone(0, mobileInputField.text.Trim(), "");
        }
    }

    public void VerifyTokenOTP()
    {
        if (SubmitFormExtend.ValidateString(tokenInputField, "Mã OTP", false, 6, 12))
        {
            OGUIM.Toast.ShowLoading("");
            WarpRequest.UserVerifyPhone(1, "", tokenInputField.text.Trim());
        }
    }

    public void OnGetTokenDone()
    {
        uiToggleGroup.IsOn(1);
        OGUIM.Toast.Hide();
    }

    public void OnVerifyTokenDone()
    {
        anim.Hide(() =>
        {
            OGUIM.me.verified = 1;
            OGUIM.me.mobile = mobileInputField.text;
            OGUIM.isVerified = OGUIM.me.verified;

            if (popupUserInfo != null && popupUserInfo.userData != null)
                popupUserInfo.FillData(OGUIM.me);

            OGUIM.Toast.ShowNotification("Xác thực số điện thoại thành công!");

            OGUIM.MessengerBox.Show("Xác thực số điện thoại thành công", "Chúc mừng bạn xác thực tài khoản thành công +500 " + GameBase.moneyGold.name);
        });
    }
}
