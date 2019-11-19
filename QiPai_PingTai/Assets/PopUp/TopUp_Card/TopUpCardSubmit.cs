using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopUpCardSubmit : MonoBehaviour
{
    public InputField seriInputField, pinInputField;
    public Text statusLabel;
    public Button buttonSubmit;
    public ScrollRect scrollRect;
    private PayUrlModel QRPayUrl;

    UserData UserData = new UserData();

    public ToggleGroup toggleCardGroup;
    public List<Toggle> toggleCards;

    private string[] listCardImage = new string[] { "viettel", "mobi", "vina", "mega", "zing", "vcard" };
    private string[] listCardName = new string[] { "Viettel", "Mobiphone", "Vinaphone", "Mega", "Zing", "Vcard" };
	private string[] listProvider = new string[] { "VT", "MOBI", "VINA", "MGC", "ZING", "VCARD" };
    private string[] listCardAvaiable = new string[] { "VT", "MOBI", "VINA", "MGC", "ZING", "VCARD" };

    public Image VPBankImage;

    public static TopUpToggleCard currentCard { get; set; }

    public void Start()
    {
        scrollRect.horizontalNormalizedPosition = 0f;
        FillCardData();

    }

    private void OnEnable()
    {
        WarpClient.wc.OnCardTopupDone += Wc_OnCardTopupDone;
    }

    private void OnDisable()
    {
        WarpClient.wc.OnCardTopupDone -= Wc_OnCardTopupDone;
    }

    public void FillCardData()
    {
        for (int i = 0; i < toggleCards.Count; i++)
        {
            try
            {
                var cardData = toggleCards[i].GetComponent<TopUpToggleCard>();
                cardData.name = listCardName[i];
                cardData.provider = listProvider[i];
				cardData.image.sprite = ImageSheet.Instance.resourcesDics["iap-card-" + listCardImage[i]];
                cardData.isAvaiable = listCardAvaiable.Any(x => x == listProvider[i]);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("FillCardData: " + ex.Message);
            }
        }

        scrollRect.horizontalNormalizedPosition = 0f;
    }

    private void Wc_OnCardTopupDone(WarpResponseResultCode status)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            OGUIM.Toast.Show("Đã gửi thông tin thẻ " + currentCard.name + ". Vui lòng chờ hệ thống xử lý trong giây lát.");
            seriInputField.text = "";
            pinInputField.text = "";
        }
        else
        {
            OGUIM.MessengerBox.Show("Gửi thông tin thẻ " + currentCard.name + " thất bại", "Vui lòng kiểm tra lại.");
        }
        buttonSubmit.interactable = true;
    }

    public void GetCurrentCardData()
    {
        var checkToggle = toggleCardGroup.ActiveToggles().FirstOrDefault();
        if (checkToggle != null)
        {
            currentCard = checkToggle.GetComponent<TopUpToggleCard>();
            if (currentCard != null && currentCard.isAvaiable)
            {
                statusLabel.text = "Thẻ " + currentCard.name;
                buttonSubmit.interactable = true;
            }
            else
            {
                OGUIM.Toast.ShowLoading("Hệ thống thẻ " + currentCard.name + " đang bảo trì");
                buttonSubmit.interactable = false;
            }
        }
    }

    public void SubmitCard()
    {
        GetCurrentCardData();
        if (currentCard != null)
        {
            seriInputField.text = seriInputField.text.Trim();
            pinInputField.text = pinInputField.text.Trim();

            //if (SubmitFormExtend.ValidateCard(seriInputField, pinInputField, statusLabel))
            //{
            //    OGUIM.Toast.ShowLoading("Đang kiểm tra giao dịch...");
            //    WarpRequest.QRCodePay(UserData.id.ToString(),seriInputField.text, "upacp_pc", int.Parse(pinInputField.text), "VP");
            //}
            WarpRequest.QRCodePay(UserData.id.ToString(), seriInputField.text, "upacp_pc", int.Parse(pinInputField.text), "VP");
            StartCoroutine(OpenURL());

        }
        else
        {
            OGUIM.Toast.Show("Vui lòng chọn loại thẻ!", UIToast.ToastType.Warning);
        }
    }

    IEnumerator OpenURL()
    {
        yield return new WaitForSeconds(0.5f);
        var QRPayUrlString = WarpClient.wc.PayloadUrlTemp;
      
        QRPayUrl = JsonUtility.FromJson<PayUrlModel>(QRPayUrlString);


        Debug.LogError(QRPayUrl.resultUrl);

        Application.OpenURL(QRPayUrl.resultUrl);
    }

    public void SetToggleCard(int indexToggle)
    {
        for (int i = 0; i < toggleCards.Count; i++)
        {
            if (i == indexToggle)
                toggleCards[i].isOn = true;
            else
                toggleCards[i].isOn = false;
        }
    }
}
