using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class QRCodePay : MonoBehaviour
{
    public InputField AmountInput;
    public InputField AccountInput;
    public Text AccountText;
    public Image MOMOImage;
    public Image ZaloImage;
    public SubmitFormExtend RechargeFrom;
    string RechargeType = "";
    public Image QRCodeImage;

    public UserData data = new UserData();
    private PayUrlModel QRPayUrl ;
    void Start()
    {
        OnClickMomo();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 切换二维码类型为MOMO
    /// </summary>
    public void OnClickMomo()
    {
        ZaloImage.GetComponent<Image>().SetAlpha(0);
        MOMOImage.GetComponent<Image>().SetAlpha(0.5f);
        RechargeType = "pay_qr";
        AccountText.text = "MOMO Số tài khoản";
    }

    /// <summary>
    /// 切换二维码类型为zalo
    /// </summary>
    public void OnClickZalo()
    {
        MOMOImage.GetComponent<Image>().SetAlpha(0);
        ZaloImage.GetComponent<Image>().SetAlpha(0.5f);
        RechargeType = "pay_qr";
        AccountText.text = "ZALO Số tài khoản";
    }

    /// <summary>
    /// 点击跳转
    /// </summary>
    public void OnClickCreatORCode()
    {
        WarpRequest.QRCodePay(data.id.ToString(),data.username, "pay_qr", int.Parse(AmountInput.text),null);
        StartCoroutine(OpenURL());
    }

    /// <summary>
    /// 跳转二维码界面
    /// </summary>
    /// <returns></returns>
    IEnumerator OpenURL()
    {
        yield return new WaitForSeconds(0.5f);
        var QRPayUrlString = WarpClient.wc.PayloadUrlTemp;
        //string QRPayJson = QRPayUrlString.Substring(QRPayUrlString.IndexOf("{"), QRPayUrlString.LastIndexOf("}") + 1);

        //QRPayUrl = JsonMapper.ToObject<PayUrlModel>(QRPayUrlString);

        QRPayUrl  = JsonUtility.FromJson<PayUrlModel>(QRPayUrlString);

       // Debug.LogError(QRPayUrl.resultUrl);

        Application.OpenURL(QRPayUrl.resultUrl);
    }
}
