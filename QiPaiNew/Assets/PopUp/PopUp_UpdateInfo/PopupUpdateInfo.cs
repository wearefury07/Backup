using UnityEngine;
using UnityEngine.UI;

public class PopupUpdateInfo : MonoBehaviour
{
    public InputField inputFieldDisplayName;
    public InputField inputFieldTrueName;
    public InputField inputFieldStatus;
    public InputField inputFieldPhoneNum;
    public InputField inputFieldEmail;
    public InputField inputFieldBirthday;

    public bool showUpdateStatus;
    public UIAnimation anim;
    public UserData userData;

    //修改信息界面
    public GameObject InfoObj;
    private void OnEnable()
    {
        if (WarpClient.wc != null)
            WarpClient.wc.OnUpdateUserInfoDone += Wc_OnUpdateUserInfoDone;
    }

    private void OnDisable()
    {
        if (WarpClient.wc != null)
            WarpClient.wc.OnUpdateUserInfoDone -= Wc_OnUpdateUserInfoDone;
    }

    private void Wc_OnUpdateUserInfoDone(WarpResponseResultCode status)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            inputFieldDisplayName.text = "";
            inputFieldStatus.text = "";
            anim.Hide();
            OGUIM.Toast.ShowNotification("Cập nhật thông tin thành công...");
        }
    }

    public void Show(UserData _user)
    {   
        userData = _user;
        inputFieldDisplayName.text = userData.displayName;
        inputFieldStatus.gameObject.SetActive(showUpdateStatus);
        inputFieldStatus.text = userData.status;
        inputFieldPhoneNum.text=userData.mobile;
        inputFieldEmail.text = userData.Email;
        ShowInfo();
        anim.Show();
    }

    /// <summary>
    /// 显示修改信息界面
    /// </summary>
    public void ShowInfo()
    {
        InfoObj.SetActive(true);
    }
    /// <summary>
    ///显示修改密码界面
    /// </summary>
    public void ShowUpdataPassword()
    {

    }

    public void UpdateInfo()
    {
        if (SubmitFormExtend.ValidateString(inputFieldDisplayName, "Tên hiển thị", false, 6, 24) && SubmitFormExtend.ValidateString(inputFieldStatus, "Cảm xúc", false, 6, 40))
        {
            OGUIM.Toast.ShowLoading("Đang cập nhật thông tin...");
            WarpRequest.UpdateUserInfo(inputFieldDisplayName.text, userData.mobile);
        }
    }
}
