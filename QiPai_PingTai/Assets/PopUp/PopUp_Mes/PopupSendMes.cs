using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PopupSendMes : MonoBehaviour
{
    public InputField inputFieldSearchId;
    public Text status;
    public InputField inputFieldTitle;
    public InputField inputFieldContent;

    public UIToggleGroup toggleGroup;

    public UIAnimation anim;
    public UserData userData;



    private string statusDefault = "Người nhận: <color=#FFFFFF>{0}</color>";

    private void OnEnable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnSentMessageDone += Wc_OnSentMessageDone;
            WarpClient.wc.OnFeedbackDone += Wc_OnFeedbackDone;
        }
    }

    private void OnDisable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnSentMessageDone -= Wc_OnSentMessageDone;
            WarpClient.wc.OnFeedbackDone -= Wc_OnFeedbackDone;
        }
    }

    private void Wc_OnSentMessageDone(WarpResponseResultCode status)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            inputFieldTitle.text = "";
            inputFieldContent.text = "";
            anim.Hide();
            OGUIM.Toast.ShowNotification("Đã gửi tin nhắn tới " + userData.displayName);
        }
    }

    public void Show(UserData _user)
    {
        if (_user == null)
        {
            inputFieldSearchId.gameObject.SetActive(true);
            status.text = "Vui lòng nhập id người nhận";
        }
        else
        {
            userData = _user;
            inputFieldSearchId.gameObject.SetActive(false);
            status.text = string.Format(statusDefault, userData.displayName);
        }
        anim.Show();
    }

    public void Send()
    {
        if (SubmitFormExtend.ValidateString(inputFieldTitle, "Tiêu đề tin nhắn", false, 2, 40) && SubmitFormExtend.ValidateString(inputFieldContent, "Nội dung tin nhắn", false, 2, 120))
        {
            OGUIM.Toast.ShowLoading("Đang gửi tin nhắn...");
            WarpRequest.SendMessage(userData.id, inputFieldTitle.text, inputFieldContent.text);
        }
    }

    public void ShowFeedback()
    {
        inputFieldContent.text = "";
        anim.Show();
    }

    public void SendFeedback()
    {
        if (inputFieldContent == null || SubmitFormExtend.ValidateString(inputFieldContent, "Nội dung phản hồi góp ý", false, 12, 360))
        {
            var checkToggle = (FeedbackType)toggleGroup.currentIndex;
            Debug.Log(checkToggle + " " + inputFieldContent.text);
            WarpRequest.Feedback(checkToggle, inputFieldContent.text);
        }
    }

    private void Wc_OnFeedbackDone(WarpResponseResultCode status)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            OGUIM.Toast.ShowNotification("Cảm ơn bạn đã gửi góp ý, chúng tôi sẽ xem xét và phản hồi sớm nhất!");
            inputFieldContent.text = "";
            anim.Hide();
        }
    }
}
