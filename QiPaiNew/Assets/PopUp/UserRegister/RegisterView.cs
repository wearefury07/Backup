using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RegisterView : MonoBehaviour
{
    public RegisterType type;
    public InputField userName;
    public InputField password;
    public InputField rePassword;
    public InputField phoneNumber;

    public bool requestPhoneNumber;

    public Action actionOnDone;

    [SerializeField]
    public myFloatEvent eventOnCompleted;

    [Serializable]
    public class myFloatEvent : UnityEvent { }

    public UIAnimation anim;

    void Awake()
    {
        if (phoneNumber != null)
        {
            if (requestPhoneNumber)
                phoneNumber.gameObject.SetActive(true);
            else
                phoneNumber.gameObject.SetActive(false);
        }
    }

    public void Update()
    {
        if (gameObject.activeSelf)
        {
            if (userName.isFocused && (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Return)))
            {
                password.Select();
                password.ActivateInputField();
            }
            else if (password.isFocused && (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Return)))
            {
                rePassword.Select();
                rePassword.ActivateInputField();
            }
            else if (rePassword.isFocused && (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Return)))
            {
                if (phoneNumber != null && phoneNumber.gameObject.activeSelf)
                {
                    phoneNumber.Select();
                    phoneNumber.ActivateInputField();
                }
                else
                {
                    Register();
                }
            }
            else if (phoneNumber != null && phoneNumber.gameObject.activeSelf && phoneNumber.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                Register();
            }
        }
    }

    public void OnEnable()
    {
        if (WarpClient.wc != null)
            WarpClient.wc.OnLinkAccDone += Wc_OnLinkAccDone;
    }

    public void OnDisable()
    {
        if (WarpClient.wc != null)
            WarpClient.wc.OnLinkAccDone -= Wc_OnLinkAccDone;
    }

    public void Wc_OnLinkAccDone(WarpResponseResultCode status)
    {
        string message = "";
        if (status == WarpResponseResultCode.SUCCESS)
        {
            OGUIM.me.link_acc = true;

            if (eventOnCompleted != null)
                eventOnCompleted.Invoke();
            if (actionOnDone != null)
                actionOnDone.Invoke();

            anim.Hide(() =>
            {
                OGUIM.MessengerBox.Show("Thông báo", "Liên kết tài khoản thành công." + "\n" + "Phần thưởng đã được gửi tới hòm thư của bạn.");
            });
        }
        else
        {
            if (status == WarpResponseResultCode.REG_USER_EXIST)
                message = "Liên kết tài khoản thất bại. Tài khoản đã tồn tại.";
            else if (status == WarpResponseResultCode.REG_PASS_LEN_WRONG)
                message = "Liên kết tài khoản thất bại. Độ dài mật khẩu không hợp lệ.";
            else if (status == WarpResponseResultCode.REG_USER_EQ_PASS)
                message = "Liên kết tài khoản thất bại. Tài khoản và mật khẩu giống nhau.";
            else if (status == WarpResponseResultCode.INVALID_REG_USERNAME)
                message = "Liên kết tài khoản thất bại. Tài khoản không hợp lệ.";
            else if (status == WarpResponseResultCode.INVALID_USERNAME)
                message = "Liên kết tài khoản thất bại. Tài khoản không hợp lệ.";
            OGUIM.Toast.Show(message);
        }
    }

    public void Register()
    {
        if (SubmitFormExtend.ValidateRegister(userName, password, rePassword, requestPhoneNumber ? phoneNumber : null, false))
        {
            OGUIM.Toast.ShowLoading("Đang tiến hành đăng nhập...");
            OGUIM.instance.Regiter(userName.text, password.text, requestPhoneNumber ? phoneNumber.text : "");
        }
    }

    public void Show(Action _actionOnDone)
    {
        actionOnDone = _actionOnDone;
        anim.Show();
    }

    public void LinkAcc()
    {
        if (SubmitFormExtend.ValidateRegister(userName, password, rePassword, null, false))
        {
            OGUIM.Toast.ShowLoading("");
            WarpRequest.LinkAcc(userName.text, password.text);
        }
    }
}

public enum RegisterType
{
    REGISTER = 0,
    LINKACC = 1,
}
