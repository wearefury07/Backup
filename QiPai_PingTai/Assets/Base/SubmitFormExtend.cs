using UnityEngine;
using UnityEngine.UI;

public class SubmitFormExtend : MonoBehaviour
{
    #region ValidateCard
    public static bool ValidateCard(InputField seriInputField, InputField pinInputField, Text statusLabel)
    {
        if (seriInputField == null)
            Debug.LogError("seriInputField not found");
        if (pinInputField == null)
            Debug.LogError("pinInputField not found");
        if (statusLabel == null)
            Debug.LogError("statusLabel not found");

        if (!ValidateCard(seriInputField, statusLabel, "Seri thẻ", false))
            return false;
        if (!ValidateCard(pinInputField, statusLabel, "Mã thẻ", false))
            return false;
        return true;
    }

    public static bool ValidateCard(InputField inputField, Text statusLabel, string name, bool autoFocus)
    {
        string status = "";

        status = inputField.ValidateEmpty(name, autoFocus);
        if (!string.IsNullOrEmpty(status))
        {
            statusLabel.text = status;
            OGUIM.Toast.Show(status, UIToast.ToastType.Warning, 3f);
            return false;
        }

        status = inputField.ValidateLength(name, 6, 30, autoFocus);
        if (!string.IsNullOrEmpty(status))
        {
            statusLabel.text = status;
            OGUIM.Toast.Show(status, UIToast.ToastType.Warning, 3f);
            return false;
        }

        statusLabel.text = "";
        return true;
    }
    #endregion

    #region ValidateLogin
    public static bool ValidateLogin(InputField userNameInputField, InputField passwordInputField, bool autoFocus)
    {
        if (userNameInputField == null)
            Debug.LogError("userNameInputField not found");
        if (passwordInputField == null)
            Debug.LogError("passwordInputField not found");

        if (!ValidateUserName(userNameInputField, "Tên tài khoản", autoFocus))
            return false;
        if (!ValidatePassWord(passwordInputField, "Mật khẩu", autoFocus))
            return false;
        return true;
    }
    #endregion

    #region ValidateRegister
    public static bool ValidateRegister(InputField userNameInputField, InputField passwordInputField, InputField rePasswordInputField, InputField phoneNumber, bool autoFocus)
    {
        if (userNameInputField == null)
            Debug.LogError("userNameInputField not found");
        if (passwordInputField == null)
            Debug.LogError("passwordInputField not found");
        if (rePasswordInputField == null)
            Debug.LogError("rePasswordInputField not found");

        if (!ValidateUserName(userNameInputField, "Tên tài khoản", autoFocus))
            return false;
        if (!ValidatePassWord(passwordInputField, "Mật khẩu", autoFocus))
            return false;
        if (!ValidateRePassWord(passwordInputField, rePasswordInputField, "Xác nhận mật khẩu", autoFocus))
            return false;
        if (phoneNumber != null && !ValidatePhoneNumber(phoneNumber, "Số điện thoại", autoFocus))
            return false;
        return true;
    }

    public static bool ValidateUserName(InputField inputField, string name, bool autoFocus)
    {
        string status = "";

        status = inputField.ValidateEmpty(name, autoFocus);
        if (!string.IsNullOrEmpty(status))
        {
            OGUIM.Toast.Show(status, UIToast.ToastType.Warning, 3f);
            return false;
        }

        status = inputField.ValidateLength(name, 3, 18, autoFocus);
        if (!string.IsNullOrEmpty(status))
        {
            OGUIM.Toast.Show(status, UIToast.ToastType.Warning, 3f);
            return false;
        }

        status = inputField.ValidateUserName(name, autoFocus);
        if (!string.IsNullOrEmpty(status))
        {
            OGUIM.Toast.Show(status, UIToast.ToastType.Warning, 3f);
            return false;
        }
        return true;
    }

    public static bool ValidatePassWord(InputField inputField, string name, bool autoFocus)
    {
        string status = "";

        status = inputField.ValidateEmpty(name, autoFocus);
        if (!string.IsNullOrEmpty(status))
        {
            OGUIM.Toast.Show(status, UIToast.ToastType.Warning, 3f);
            return false;
        }

        status = inputField.ValidateLength(name, 6, 18, autoFocus);
        if (!string.IsNullOrEmpty(status))
        {
            OGUIM.Toast.Show(status, UIToast.ToastType.Warning, 3f);
            return false;
        }
        return true;
    }

    public static bool ValidateRePassWord(InputField passWord, InputField rePassWord, string name, bool autoFocus)
    {
        if (string.IsNullOrEmpty(rePassWord.text))
        {
            if (autoFocus)
                rePassWord.FocusInputField();
            OGUIM.Toast.Show(name + " không được để trống", UIToast.ToastType.Warning);
            return false;
        }

        if (rePassWord.text != passWord.text)
        {
            if (autoFocus)
                rePassWord.FocusInputField();
            OGUIM.Toast.Show(name + " không khớp", UIToast.ToastType.Warning);
            return false;
        }
        return true;
    }
    #endregion

    #region ValidateUpdateInfo
    public static bool ValidateUpdateInfo(InputField displayNameInputField, InputField phoneInputField, Text statusLabel)
    {
        if (displayNameInputField == null)
            Debug.LogError("displayNameInputField not found");
        if (phoneInputField == null)
            Debug.LogError("phoneInputField not found");
        if (statusLabel == null)
            Debug.LogError("statusLabel not found");

        if (!ValidateDisplayName(displayNameInputField, "Tên hiển thị", false))
            return false;
        if (!ValidatePhoneNumber(phoneInputField, "Số điện thoại", false))
            return false;
        return true;
    }

    private static bool ValidateDisplayName(InputField inputField, string name, bool autoFocus)
    {
        string status = "";

        status = inputField.ValidateEmpty(name, autoFocus);
        if (!string.IsNullOrEmpty(status))
        {
            OGUIM.Toast.Show(status, UIToast.ToastType.Warning, 3f);
            return false;
        }

        status = inputField.ValidateLength(name, 6, 18, autoFocus);
        if (!string.IsNullOrEmpty(status))
        {
            OGUIM.Toast.Show(status, UIToast.ToastType.Warning, 3f);
            return false;
        }

        status = inputField.ValidateDisplayName(name, autoFocus);
        if (!string.IsNullOrEmpty(status))
        {
            OGUIM.Toast.Show(status, UIToast.ToastType.Warning, 3f);
            return false;
        }
        return true;
    }

    public static bool ValidatePhoneNumber(InputField inputField, string name, bool autoFocus, bool allowEmpty = true)
    {
        string status = "";
        if (string.IsNullOrEmpty(inputField.text) && allowEmpty)
        {
            return true;
        }

        status = inputField.ValidateNumb(name, autoFocus);
        if (!string.IsNullOrEmpty(status))
        {
            OGUIM.Toast.Show(status, UIToast.ToastType.Warning, 3f);
            return false;
        }

        status = inputField.ValidateLength(name, 8, 12, autoFocus);
        if (!string.IsNullOrEmpty(status))
        {
            OGUIM.Toast.Show(status, UIToast.ToastType.Warning, 3f);
            return false;
        }
        return true;
    }
    #endregion

    #region ValidateGiftCode
    public static bool ValidateString(InputField inputField, string name, bool autoFocus, int minLength = 0, int maxLength = 0)
    {
        string status = "";

        status = inputField.ValidateEmpty(name, autoFocus);
        if (!string.IsNullOrEmpty(status))
        {
            OGUIM.Toast.Show(status, UIToast.ToastType.Warning, 3f);
            return false;
        }


        status = inputField.ValidateLength(name, minLength, maxLength, autoFocus);
        if (minLength > 0 && maxLength > 0 && !string.IsNullOrEmpty(status))
        {
            OGUIM.Toast.Show(status, UIToast.ToastType.Warning, 3f);
            return false;
        }

        OGUIM.Toast.Hide();
        return true;
    }
    #endregion
}
