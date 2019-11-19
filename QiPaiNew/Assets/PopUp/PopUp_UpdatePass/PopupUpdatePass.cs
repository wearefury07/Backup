using UnityEngine;
using UnityEngine.UI;

public class PopupUpdatePass : MonoBehaviour
{
    public InputField inputFieldOldPass;
    public InputField inputFieldNewPass;
    public InputField inputFieldReNewPass;
    public UIAnimation anim;

    private void OnEnable()
    {
        if (WarpClient.wc != null)
            WarpClient.wc.OnChangePasswordDone += Wc_OnChangePasswordDone; ;
    }

    private void OnDisable()
    {
        if (WarpClient.wc != null)
            WarpClient.wc.OnChangePasswordDone -= Wc_OnChangePasswordDone;
    }

    private void Wc_OnChangePasswordDone(WarpResponseResultCode status)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            inputFieldOldPass.text = "";
            inputFieldNewPass.text = "";
            inputFieldReNewPass.text = "";
            anim.Hide();
            OGUIM.Toast.ShowNotification("Thay đổi mật khẩu thành công...");
        };
        
    }

    public void Show()
    {
        inputFieldOldPass.text = "";
        inputFieldNewPass.text = "";
        inputFieldReNewPass.text = "";
        anim.Show();
    }

    public void UpdatePass()
    {
        if (SubmitFormExtend.ValidatePassWord(inputFieldOldPass, "Mật khẩu cũ", false) 
            && SubmitFormExtend.ValidatePassWord(inputFieldNewPass, "Mật khẩu mới", false)
            && SubmitFormExtend.ValidateRePassWord(inputFieldNewPass, inputFieldReNewPass, "Nhập lại mật khẩu mới", false))
        {
            OGUIM.Toast.ShowLoading("");
            WarpRequest.ChangePassword(inputFieldOldPass.text, inputFieldReNewPass.text);
        }
    }
}
