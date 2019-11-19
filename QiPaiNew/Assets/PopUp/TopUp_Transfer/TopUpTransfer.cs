using UnityEngine;
using UnityEngine.UI;

public class TopUpTransfer : MonoBehaviour
{
    public Text title;
    public Text note;
    public PlayerMoneyView moneyView;
    public InputField inputField_UserId;
    public InputField inputField_Value;
    public Text status;
    public InputField inputField_Content;
    public Button submitButton;
    public bool requireContent = true;
    public bool autoFocus = true;

    private string defaultEmpIdMes = "Vui lòng nhập id người chơi nhận chuyển khoản";
    private string defaultEmpValueMes = "Vui lòng nhập giá trị chuyển khoản";
    private string defaultMinValueMes = "Giá trị chuyển khoản nhỏ hơn mức yêu cầu";
    private string defaultEmpContentMes = "Vui lòng nhập nội dung chuyển khoản";
    private string defaultUserMes = "Tài khoản \"<color=#FFFFFF>{0}</color>\" sẽ nhận được <color=#FFFFFF>{1}</color> {2}";
    private string defaultTranferMes = "<size=24>Tôi đồng ý chuyển số <color=#FFC800FF>{0}</color></size>"
            + "\n" + "tới tài khoản"
            + "\n" + "<size=24><color=#FFC800FF>{1}</color>" + " " + "({2})</size>";

    private string defaultTransferNote = "Giá trị giao dịch tối thiếu <color=#FFFFFF>{0}</color> {1} với phí giao dịch <color=#FFFFFF>{2}%</color> giá trị chuyển khoản";
    //+ "\n" + "- Không giới hạn giá trị tối đa và số lần giao dịch trong ngày";
    //+ "\n" + "- ID nhận là ID của tài khoản được nhận (tra cứu trong phần thông tin người chơi)"
    //+ "\n" + "- Các giao dịch chuyển nhầm tài khoản được tính hợp lệ và không được hoàn trả";

    private UserData user;
    private bool canFind;
    private long lowValueTransfer;
    private double rateTransfer;

    public UIAnimation anim;

    public void Awake()
    {
        anim = transform.GetComponent<UIAnimation>();
    }

    private void OnEnable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnTransferGoldDone += Wc_OnTransferGoldDone;
            WarpClient.wc.OnGetConfigDone += Wc_OnGetConfigDone;

            //if (lowValueTransfer == 0 || rateTransfer == 0)
            WarpRequest.GetConfig(ConfigType.TRANSFER_GOLD);
        }
        title.text = "QUY ĐỊNH CHUYỂN " + GameBase.moneyGold.name.ToUpper();
        note.text = "Loading...";
    }

    private void Wc_OnGetConfigDone(WarpResponseResultCode status, RootConfig data)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            lowValueTransfer = data.data.min;
            rateTransfer = data.data.rate;
            note.text = string.Format(defaultTransferNote, data.data.min, GameBase.moneyGold.name, (100 - data.data.rate * 100));
            OnInputChanged();
        }
    }

    private void OnDisable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnTransferGoldDone -= Wc_OnTransferGoldDone;
        }
    }

    private void Wc_OnTransferGoldDone(WarpResponseResultCode status, UserData data)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            FillData(data, canFind);
            OGUIM.instance.meView.moneyView.FillData(data);
            //string defaultOnTransferGoldDone = "Đã chuyển khoản thành công!" + "\n" + "<size=24>Bạn hiện có <color=#FFC800FF>{0}</color> " + GameBase.moneyGold.name + "</size>";
            //OGUIM.MessengerBox.Show("Chuyển khoản thành công", string.Format(defaultOnTransferGoldDone, LongConverter.ToFull(data.gold)));
        }
        else
        {
            //string message = "";
            //if (status == WarpResponseResultCode.TRANSFER_IN_PLAY_MOD)
            //    message = "Không thể giao dịch " + GameBase.moneyGold.name + " trong bàn chơi.";
            //else if (status == WarpResponseResultCode.TRANSFER_SMALLER_MIN_VALUE)
            //    message = "Số " + GameBase.moneyGold.name + " giao dịch nhỏ hơn quy định. ";
            //else if (status == WarpResponseResultCode.TRANSFER_OVER_MAX_VALUE)
            //    message = "Số " + GameBase.moneyGold.name + " giao dịch lớn hơn quy định.";
            //else if (status == WarpResponseResultCode.INVALID_REG_USERNAME)
            //    message = "Không tìm thấy người chơi.";
            //else
            //    message = "Giao dịch " + GameBase.moneyGold.name + " thất bại. Vui lòng thử lại.";

            //OGUIM.Toast.Show(message, UIToast.ToastType.Warning, 3f);
        }
    }

    public void Update()
    {

    }

    public void FillData(UserData _user, bool _canFind)
    {
        canFind = _canFind;
        user = _user;

        inputField_UserId.interactable = canFind;
        if (user != null)
        {
            inputField_Value.text = "0";
            inputField_UserId.text = "ID: " + user.id.ToString();
        }

        moneyView.FillData(MoneyType.Gold, OGUIM.me.gold);

        ValidateSubmitForm();
    }

    public void OnInputChanged()
    {
        long valueTransfer;
        if (long.TryParse(inputField_Value.text, out valueTransfer))
        {
            valueTransfer = (long)(valueTransfer * rateTransfer);
        }
        status.text = string.Format(defaultUserMes, user.displayName, LongConverter.ToFull(valueTransfer), GameBase.moneyGold.name);
    }

    public void ValidateSubmitForm()
    {
        if (string.IsNullOrEmpty(inputField_UserId.text))
        {
            submitButton.interactable = false;
            status.text = defaultEmpIdMes;
            OGUIM.Toast.Show(defaultEmpIdMes, UIToast.ToastType.Warning, 3f);
            if (autoFocus)
                inputField_UserId.FocusInputField();
        }
        else if (string.IsNullOrEmpty(inputField_Value.text))
        {
            submitButton.interactable = false;
            OGUIM.Toast.Show(defaultEmpValueMes, UIToast.ToastType.Warning, 3f);
            if (autoFocus)
                inputField_Value.FocusInputField();
        }
        else if (!string.IsNullOrEmpty(inputField_Value.text) && long.Parse(inputField_Value.text) < lowValueTransfer)
        {
            submitButton.interactable = false;
            OGUIM.Toast.Show(defaultMinValueMes, UIToast.ToastType.Warning, 3f);
            if (autoFocus)
                inputField_Value.FocusInputField();
        }
        else if (!string.IsNullOrEmpty(inputField_Value.text) && long.Parse(inputField_Value.text) > OGUIM.me.gold)
        {
            inputField_Value.text = OGUIM.me.gold.ToString();
        }
        else if (requireContent && string.IsNullOrEmpty(inputField_Content.text))
        {
            submitButton.interactable = false;
            OGUIM.Toast.Show(defaultEmpContentMes, UIToast.ToastType.Warning, 3f);
            if (autoFocus)
                inputField_Content.FocusInputField();
        }
        else
            submitButton.interactable = true;
    }

    public void Show(UserData _user, bool _canFind)
    {
        FillData(_user, _canFind);
        if (anim == null)
            anim = transform.GetComponent<UIAnimation>();
        anim.Show();
    }

    public void Confirm()
    {
        submitButton.interactable = false;
        anim.Hide();
        OGUIM.MessengerBox.Show("Xác nhận", string.Format(defaultTranferMes, LongConverter.ToFull(inputField_Value.text) + " " + GameBase.moneyGold.name, user.displayName, user.id),
            "Để sau", () =>
            {
                Show(user, canFind);
            },
            "Xác nhận", () =>
            {
                WarpRequest.TransferGold(user.id, int.Parse(inputField_Value.text), inputField_Content.text);
            });
    }
}
