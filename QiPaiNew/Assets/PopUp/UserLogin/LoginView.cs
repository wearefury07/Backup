using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : MonoBehaviour
{
    public InputField userName;
    public InputField password;

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if ((Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Return)) && userName.isFocused)
            {
                password.Select();
                password.ActivateInputField();
            }
            else if (Input.GetKeyDown(KeyCode.Return) && password.isFocused)
            {
                Login();
            }
        }
    }

    public void Login()
    {
        if (SubmitFormExtend.ValidateLogin(userName, password, false))
        {
            OGUIM.Toast.ShowLoading("Đang tiến hành đăng nhập...");
            OGUIM.instance.Login(userName.text, password.text);
        }
    }
}
