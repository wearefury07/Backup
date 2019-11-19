using UnityEngine;
using UnityEngine.UI;

public class TopUpSMSItemView : MonoBehaviour
{
    public Text description;
    public InputField formSMS;
    public Button sendButon;
    public Text sendButonContent;

    private string desSMS = "Nạp <color=#FFFFFF>{0} VNĐ</color> nhận <color=#FFFFFF>{1} 368vipEdited</color> soạn tin";
    private SMSData data;

    public void Awake()
    {
        gameObject.SetActive(false);
    }

    public void FillData(SMSData _data)
    {
        gameObject.SetActive(false);
        if (_data == null)
            return;

        data = _data;
        description.text = string.Format(desSMS, LongConverter.ToFull(data.money), LongConverter.ToFull(data.gold));
        formSMS.text = data.message;
        sendButonContent.text = "Gửi " + data.shortCode;
        gameObject.SetActive(true);
    }

    public void SendSMS()
    {
		#if !UNITY_IOS
        var smsForm = string.Format("sms:{0}?body={1}", data.shortCode, data.message);
		#else
		var smsForm = string.Format("sms:{0}?&body={1}", data.shortCode, System.Uri.EscapeDataString(data.message));
		#endif

        Debug.LogError("Show SMS: " + smsForm);
        Application.OpenURL(smsForm);        
    }
}
