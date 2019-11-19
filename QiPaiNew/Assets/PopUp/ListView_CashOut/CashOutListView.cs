using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(UIListView))]
[DisallowMultipleComponent]
public class CashOutListView : MonoBehaviour
{
    public CashOutType cashOutType;
    public PlayerMoneyView playerMoneyViewOnTab_0;
    public PlayerMoneyView playerMoneyViewOnTab_1;
    public UIListView uiListView;
    public int maxItems = 10;

    private List<CashOutItemView> listView = new List<CashOutItemView>();
    public static List<CashoutProduct> listData = new List<CashoutProduct>();

    public static string cashOutVersion;
    public static string cashOutUrlImage;
    public static CashoutProduct curentCashOut;

    public static List<CashoutProduct> listCashOutCard = new List<CashoutProduct>();
    public static List<CashoutProduct> listCashOutItem = new List<CashoutProduct>();

    private void Awake()
    {
        if (uiListView == null)
            uiListView = transform.GetComponent<UIListView>();
    }

    public void Start()
    {
        WarpClient.wc.OnGetConfigDone += Wc_OnGetConfigDone;
        WarpClient.wc.OnGetCashOutListDone += Wc_OnGetCashOutListDone;
    }

    public void OnEnable()
    {
        if (playerMoneyViewOnTab_0 != null)
            playerMoneyViewOnTab_0.FillData(MoneyType.Gold, OGUIM.me.gold);
        if (playerMoneyViewOnTab_1 != null)
            playerMoneyViewOnTab_1.FillData(MoneyType.Gold, OGUIM.me.gold);
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnCashOutDone += Wc_OnCashOutDone;
            WarpClient.wc.OnUpdatedMoneyDone += Wc_OnUpdatedMoneyDone;
        }
    }

    public void OnDisable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnCashOutDone -= Wc_OnCashOutDone;
            WarpClient.wc.OnUpdatedMoneyDone -= Wc_OnUpdatedMoneyDone;
        }
    }

    private void Wc_OnUpdatedMoneyDone(UpdatedMoneyData data)
    {
        if (data.chipType == (int)MoneyType.Gold)
        {
            if (playerMoneyViewOnTab_0 != null)
                playerMoneyViewOnTab_0.FillData((MoneyType)data.chipType, data.total);
            if (playerMoneyViewOnTab_1 != null)
                playerMoneyViewOnTab_1.FillData((MoneyType)data.chipType, data.total);
        }
    }

    private void Wc_OnGetConfigDone(WarpResponseResultCode status, RootConfig data)
    {
        //// Nếu dữ liệu trả về có data.version --> gán data.link(link ảnh cashout), data.verson --> lưu tham số và đối chiếu phiên bản 
		if (status == WarpResponseResultCode.SUCCESS && data != null && data.data.version != null)
        {
            cashOutVersion = data.data.version;
            cashOutUrlImage = data.data.link.Replace(".zip", "/").Replace(".Zip", "/").Replace(".ZIP", "/");
            WarpRequest.GetCashOutList();
        }
    }

    private void Wc_OnGetCashOutListDone(WarpResponseResultCode status, List<CashoutProduct> data)
    {
        // List giải thưởng với type = 0 là card, type = 1 là vật phẩm --> Dựa vào link + tên ảnh để lấy ảnh cashout từ web về và vẽ
        if (status == WarpResponseResultCode.SUCCESS && data != null)
        {
            listCashOutCard = data.Where(x => x.type == 0).ToList();
            listCashOutItem = data.Where(x => x.type == 1).ToList();
            FillData();
        }
    }

    public void Get_Data(bool reload)
    {
        if (reload)
        {
            uiListView.ClearList();
            listCashOutCard = new List<CashoutProduct>();
            listCashOutItem = new List<CashoutProduct>();
            listView = new List<CashOutItemView>();
        }

        if (!listView.Any() || reload)
		{
			OGUIM.Toast.ShowLoading("");
			//OGUIM.Toast.ShowLoading("Đang tải dữ liệu, vui lòng chờ giây lát...");

            if (string.IsNullOrEmpty(cashOutVersion) || string.IsNullOrEmpty(cashOutUrlImage))
                WarpRequest.GetConfig(ConfigType.REWARD_CONFIG);
            else if (!listCashOutCard.Any() || !listCashOutItem.Any())
                WarpRequest.GetCashOutList();
            else
                FillData();

        }
    }

    public void FillData()
    {
        if (cashOutType == CashOutType.CARD)
            listData = listCashOutCard;
        else
            listData = listCashOutItem;

        if (listData != null && listData.Any())
        {
            //int count = 0;
            foreach (var i in listData)
            {
                if (listView.Count > maxItems)
                    return;

                try
                {
                    var ui = uiListView.GetUIView<CashOutItemView>(uiListView.GetDetailView());
                    //if (count % 2 != 0)
                    //    ui.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

                    if (ui.FillData(i))
                    {
                        listView.Add(ui);
                        //count++;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("FillData: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
        OGUIM.Toast.Hide();
    }

    private void Wc_OnCashOutDone(WarpResponseResultCode status, CashoutData data)
    {
        var title = "Thông báo đổi thưởng";
        var content = "Yêu cầu đổi ";

        if (status == WarpResponseResultCode.SUCCESS && data != null)
        {
            if (curentCashOut != null)
                content += curentCashOut.name;

            OGUIM.me.gold = data.gold;
            OGUIM.instance.meView.moneyView.FillData(GameBase.moneyGold.type, OGUIM.me.gold);
            if (playerMoneyViewOnTab_0 != null)
            playerMoneyViewOnTab_0.FillData(GameBase.moneyGold.type, OGUIM.me.gold);
            if (playerMoneyViewOnTab_1 != null)
                playerMoneyViewOnTab_1.FillData(GameBase.moneyGold.type, OGUIM.me.gold);

            if (data.status == (int)CastOutStatus.NEW)
            {
                OGUIM.MessengerBox.Show(title, "Gửi yêu cầu đổi thành công.Vui lòng đợi BQT liên lạc lại");
            }
            else if (data.status == (int)CastOutStatus.ACCEPT)
            {
                OGUIM.MessengerBox.Show(title, "Gửi yêu cầu đổi thành công.Vui lòng đợi BQT liên lạc lại");
            }
            else if (data.status == (int)CastOutStatus.RECEIVE)
            {
                if (data.card.Any())
                {
                    foreach (var i in data.card)
                    {
                        content += " trị giá " + LongConverter.ToFull(i.amount) + " VNĐ" + "\n";
                        content += "Mã thẻ: " + i.pin + "\n";
                        content += "Serial: " + i.serial + "\n";
                        content += "Hạn sử dụng: " + i.expire + "\n";
                    }
                    OGUIM.MessengerBox.Show(title, content);
                }
            }
            else if (data.status == (int)CastOutStatus.REJECT)
            {
                OGUIM.MessengerBox.Show(title, "Yêu cầu đổi bị từ chối hoặc có sự cố. Thử lại hoặc gọi Hotline để được hỗ trợ.");
            }
            else if (data.status == (int)CastOutStatus.ERROR)
            {
                OGUIM.MessengerBox.Show(title, "Yêu cầu đổi bị từ chối hoặc có sự cố. Thử lại hoặc gọi Hotline để được hỗ trợ.");
            }
        }
        else if (status != WarpResponseResultCode.SUCCESS)
        {
            switch (status)
            {
                case WarpResponseResultCode.INVALID_GOLD:
                    content = "Bạn cần có " + LongConverter.ToFull(data.min) + " " + GameBase.moneyGold.name + " để đổi thưởng";
                    break;
                case WarpResponseResultCode.CASHOUT_LOCKED:
                    content = "Hệ thống đổi thưởng tạm bảo trì. Vui lòng thử lại sau";
                    break;
                case WarpResponseResultCode.CASHOUT_ITEM_NOT_EXIST:
                    content = "Thẻ cào này tạm hết. Chọn thẻ khác hoặc thử lại sau";
                    break;
                case WarpResponseResultCode.CASHOUT_OVER_AMOUNT_DAY:
                    content = "Hệ thống đổi thưởng tạm bảo trì. Vui lòng thử lại sau";
                    break;
                case WarpResponseResultCode.CASHOUT_OVER_USER_AMOUNT_DAY:
                    content = "Rất tiếc, quá số lượng đổi trong ngày";
                    break;
                case WarpResponseResultCode.CASHOUT_SMALLER_MIN_VALUE:
                    content = "Số " + GameBase.moneyGold.name + " giao dịch nhỏ hơn quy định.";
                    break;
                case WarpResponseResultCode.CASHOUT_OVER_MAX_VALUE:
                    content = "Số " + GameBase.moneyGold.name + " giao dịch lớn hơn quy định.";
                    break;
                case WarpResponseResultCode.CASHOUT_OVER_AMOUNT_LIMIT:
                    content = "Loại thẻ tạm hết. Vui lòng chọn mệnh giá khác hoặc thử lại sau.";
                    break;
                case WarpResponseResultCode.CASHOUT_OVER_TIMES_LIMIT:
                    content = "Loại thẻ tạm hết. Vui lòng chọn mệnh giá khác hoặc thử lại sau.";
                    break;
                case WarpResponseResultCode.CASHOUT_OVER_TIME_DELAY:
                    content = "Thao tác quá nhanh. Thử lại sau " + data.time + " phút";
                    break;
                case WarpResponseResultCode.USER_NOT_VERIFY:
                    content = "Vui lòng xác thực SỐ ĐIỆN THOẠI trước";
                    break;
                case WarpResponseResultCode.CASHOUT_INVALID_BALANCE:
                    content = "Số dư tối thiều là x (x là số tiền và đơn vị)";
					break;
				case WarpResponseResultCode.TRANSFER_IN_PLAY_MOD:
					content = "Không thể đổi khi đang chơi game";
					break;
                default:
                    content = "Yêu cầu đổi thưởng thất bại, vui lòng thử lại/ liên hệ CSKH để biết thêm chi tiết";
                    break;
            }
            OGUIM.MessengerBox.Show(title, content);
        }
        curentCashOut = null;
        OGUIM.Toast.Hide();
    }

    #region Get reward config --> Get list reward --> Get cashout config
    //// Lấy thông tin giải thưởng
    //WarpRequest.GetConfig(ConfigType.REWARD_CONFIG);
    //// Nếu dữ liệu trả về có data.version --> gán data.link(link ảnh cashout), data.verson --> lưu tham số và đối chiếu phiên bản 
    //WarpClient.wc.OnGetConfigDone();

    //// Lấy danh sách giải thưởng
    //WarpRequest.GetListReward();
    //// List giải thưởng với type = 0 là card, type = 1 là vật phẩm --> Dựa vào link + tên ảnh để lấy ảnh cashout từ web về và vẽ
    //WarpClient.wc.OnRewardListDone();

    ////Cashout
    //int id; // Chỉ cần id vì hiện tại server chỉ cho đổi số lượng 1 với tất cả phần thưởng nên gán cứng luôn
    //WarpRequest.CashOut(id);
    //// Dựa vào result code, status để hiển thị thông báo
    //WarpClient.wc.OnCashOutDone();
    #endregion
}

public enum CashOutType
{
    CARD = 0,
    ITEM = 1,
}
