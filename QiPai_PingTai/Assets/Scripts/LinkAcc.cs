using UnityEngine;

public class LinkAcc : MonoBehaviour
{
    public void OnLinkAccDone(WarpResponseResultCode status)
    {
        string message = "";
        if (status == WarpResponseResultCode.SUCCESS)
        {
            message = "Đăng kí thành công. Vào hòm thư nhận quà";
            OGUIM.me.link_acc = true;

            OGUIM.MessengerBox.Show("Thông báo", message);
        }
        else
        {
            if (status == WarpResponseResultCode.REG_USER_EXIST)
                message = "Đăng ký tài khoản thất bại. Tài khoản đã tồn tại.";
            else if (status == WarpResponseResultCode.REG_PASS_LEN_WRONG)
                message = "Đăng ký tài khoản thất bại. Độ dài mật khẩu không hợp lệ.";
            else if (status == WarpResponseResultCode.REG_USER_EQ_PASS)
                message = "Đăng ký tài khoản thất bại. Tài khoản và mật khẩu giống nhau.";
            else if (status == WarpResponseResultCode.INVALID_REG_USERNAME)
                message = "Đăng ký tài khoản thất bại. Tài khoản không hợp lệ.";
            else if (status == WarpResponseResultCode.INVALID_USERNAME)
                message = "Đăng ký tài khoản thất bại. Tài khoản không hợp lệ.";

            OGUIM.MessengerBox.Show("Thông báo", message);
            // Trường hợp này phải hiện lại popup nhập username/password để Link Acc lại
        }
    }

    public void OnLinkFBDone(WarpResponseResultCode status)
    {
        string message = "";
        if (status == WarpResponseResultCode.SUCCESS)
        {
            message = "Đăng nhập thành công. Vào hòm thư nhận quà";
            OGUIM.me.link_fb = true;

            OGUIM.MessengerBox.Show("Thông báo", message);
        }
        else if (status == WarpResponseResultCode.REG_USER_EXIST)
        {
            message = "Rất tiếc, Facebook này đã kết nối với một tài khoản khác";

            OGUIM.MessengerBox.Show("Thông báo", message);
            // Trường hợp này phải hiện lại popup đăng nhập fb khác để Link FB lại
        }
    }
}
