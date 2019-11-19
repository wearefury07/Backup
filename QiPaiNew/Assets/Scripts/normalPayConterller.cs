using LitJson;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
public class normalPayConterller : MonoBehaviour
{
    public Text PayAmountText;
    public Text PayAccountText;
    public Text PayTipsText;
    public RawImage QRImage;
    Texture2D encoded;

    public GameObject SubmitPanel;
    public GameObject CardTooglesPanel;
    public GameObject NextMomoZaloPanel;
    public GameObject NextBankPanel;
    public InputField PayAmountInput;
    public InputField PayAccountInput;

    public InputField BankNoInput;
    public InputField BankNameInput;
    public InputField branchNameInput;
    public Text BankAmountText;
    public Text BankTrueNameText;

    public UserData userData;
    private string channel;

    TextEditor CopyText = new TextEditor();


    public OffLinePayloadDataRoot OffLinePayInfo;

    void Awake()
    {
       
    }

   public void Start()
    {
        encoded = new Texture2D(256, 256);

        if (NextMomoZaloPanel.activeSelf==true||NextBankPanel.activeSelf==true)
        {
            NextMomoZaloPanel.SetActive(false);
            NextBankPanel.SetActive(false);
            SubmitPanel.SetActive(true);
            CardTooglesPanel.SetActive(true);
        }

        OnClickMomo();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 定义方法生成二维码 
    /// </summary>
    /// <param name="textForEncoding">需要生产二维码的字符串</param>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    /// <returns></returns>
    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
               Height = height,     
               Width = width
            }
        };
       return writer.Write(textForEncoding);
     }

    public void Btn_CreatQr()
     {
        
         if (OffLinePayInfo.patamMap.pictureCode!=null)
         {
             //二维码写入图片    
             var color32 = Encode(OffLinePayInfo.patamMap.pictureCode, encoded.width, encoded.height);
             encoded.SetPixels32(color32);
             encoded.Apply();
            //生成的二维码图片附给RawImage    
            QRImage.texture = encoded;
         }
         else
         {
             
         }
     }

    /// <summary>
    /// 选择momo支付
    /// </summary>
    public void OnClickMomo()
    {
        if (PayAmountInput!=null)
        {
            PayAmountInput.text = "";
        }
        if (PayAccountInput!=null)
        {
            PayAccountInput.text = "";
        }
       
        PayAccountInput.gameObject.SetActive(true);
       
        PayAccountText.text = "充值金额";
        PayAmountText.text = "付款人账号";
         channel = "momo";
    }
    /// <summary>
    /// 选择zalo支付
    /// </summary>
    public void OnClickZalo()
    {
        if (PayAmountInput != null)
        {
            PayAmountInput.text = "";
        }
        if (PayAccountInput != null)
        {
            PayAccountInput.text = "";
        }

        PayAccountInput.gameObject.SetActive(false);
        PayAmountText.text = "充值金额";
        channel = "zalo";
    }
    /// <summary>
    /// 选择银行支付
    /// </summary>
    public void OnClickBank()
    {
        if (PayAmountInput != null)
        {
            PayAmountInput.text = "";
        }
        if (PayAccountInput != null)
        {
            PayAccountInput.text = "";
        }

        PayAccountInput.gameObject.SetActive(true);
        PayAccountText.text = "充值金额";
        PayAmountText.text = "付款人姓名"; 
         channel = "bank_card";
    }

    /// <summary>
    /// 点击确认按钮
    /// </summary>
    public void OnClickSubmit()
    {
        switch (channel)
        {
            case "momo":
                if (PayAccountInput.text!=""&&PayAmountInput.text!="")
                {
                    WarpRequest.OffLinePay(userData.id.ToString(), PayAccountInput.text, channel, int.Parse(PayAmountInput.text), "", "");
                }
                break;

            case "zalo":
                if (PayAmountInput.text!="")
                {
                    WarpRequest.OffLinePay(userData.id.ToString(), "", channel, int.Parse(PayAmountInput.text), PayAccountInput.text, "");
                }
                break;

            case "bank_card":
                if (PayAccountInput.text != "" && PayAmountInput.text != "")
                {
                    WarpRequest.OffLinePay(userData.id.ToString(), "", channel, int.Parse(PayAmountInput.text), "", PayAccountInput.text);
                }
                break;

            default:
                break;
        }

        OldWarpChannel.Channel.DataReceive();
        StartCoroutine(OnClickSubmitDone());
    }



    IEnumerator OnClickSubmitDone()
    {
        yield return new WaitForSeconds(0.2f);

        string OffLinePayString = WarpClient.wc.OffLinePayloadTemp;

        OffLinePayInfo = JsonMapper.ToObject<OffLinePayloadDataRoot>(OffLinePayString);


        CardTooglesPanel.SetActive(false);
        SubmitPanel.SetActive(false);
        if (channel=="momo"||channel=="zalo")
        {
            if (OffLinePayInfo.patamMap.pictureCode != null)
            {
                Btn_CreatQr();
            }
            NextBankPanel.SetActive(false);
            NextMomoZaloPanel.SetActive(true);

            Debug.LogError(OffLinePayInfo.patamMap.pictureCode);

            if (channel=="momo")
            {
                PayTipsText.text = "充值金额:" + OffLinePayInfo.patamMap.amount + " " + "付款人手机号:" + PayAccountInput.text;
            }
            else
            {
                PayTipsText.text = "充值金额:" + OffLinePayInfo.patamMap.amount + " " + "备注:" + OffLinePayInfo.patamMap.remark;
            }
        }

        else if (channel== "bank_card")
        {
            NextMomoZaloPanel.SetActive(false);
            NextBankPanel.SetActive(true);

          //  BankTrueNameText.text=
            BankNameInput.text = OffLinePayInfo.patamMap.bankName;
            BankNoInput.text = OffLinePayInfo.patamMap.bankNo;
            branchNameInput.text = OffLinePayInfo.patamMap.branchName;
        }
       
    }

    public void CopyAmount()
    {
        CopyText.text = OffLinePayInfo.patamMap.amount;
        CopyText.Copy();
    }

    public void CopyName()
    {
        CopyText.text = OffLinePayInfo.patamMap.accountOwner;
        CopyText.Copy();
    }
    public void CopyBankName()
    {
        CopyText.text = OffLinePayInfo.patamMap.bankName;
        CopyText.Copy();
    }
    public void CopyBranchName()
    {
        CopyText.text = OffLinePayInfo.patamMap.branchName;
        CopyText.Copy();
    }
}
