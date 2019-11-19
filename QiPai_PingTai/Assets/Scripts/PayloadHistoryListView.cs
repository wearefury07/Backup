using LitJson;
using MsgPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIListView))]
[DisallowMultipleComponent]
public class PayloadHistoryListView : MonoBehaviour
{
    public HistoryType historyType;
    public UIListView uiListView;
    public int maxItems = 20;
    private List<PayLoadHistory> listData;
    private List<PayLoadHistoryItemView> listView = new List<PayLoadHistoryItemView>();
    public List<Text> listLabels;
    byte[] payLoad;
    public UserData data = new UserData();
    public static TimeOutHelper timeOutHelper = new TimeOutHelper();

    public static PayLoadHistory currentPayloadHistory { get; set; }


    public RootPayloadHistory PayloadHistory;


    public Text payType, OderID, Time, Amount;


    private void Awake()
    {
        if (uiListView == null)
            uiListView = transform.GetComponent<UIListView>();
    }

    private void OnEnable()
    {
        if (WarpClient.wc != null)
            AddListener();
    }

    private void OnDisable()
    {
        if (WarpClient.wc != null)
            RemoveListener();
    }

    public void AddListener()
    {
        if (historyType == HistoryType.PAYLOAD)
            WarpClient.wc.OnPayloadHistoryDone += Wc_OnPayloadHistoryDone;
    }

    public void RemoveListener()
    {
        if (historyType == HistoryType.CASHOUTUSER)
            WarpClient.wc.OnPayloadHistoryDone -= Wc_OnPayloadHistoryDone;
    }

    public void SetType(int _type)
    {
        historyType = (HistoryType)_type;
    }

    private void Wc_OnPayloadHistoryDone(WarpResponseResultCode status, List<PayLoadHistory> data)
    {
        if (status == WarpResponseResultCode.SUCCESS && data != null)
        {
            if (data != null && data.Any())
            {
                listData = data;
                FillData();
            }
            else
            {
                //OGUIM.Toast.ShowNotification("Hiện tại chưa có giao dịch nào!");
            }
        }
        else
            OGUIM.Toast.Show("Có lỗi xảy ra. Vui lòng thử lại", UIToast.ToastType.Warning, 3f);
        Debug.LogError("data:"+data);

    }

    public void FillData()
    {
        if (listData != null && listData.Any())
        {
            int count = 0;
            foreach (var i in listData)
            {
                try
                {
                    var ui = uiListView.GetUIView<PayLoadHistoryItemView>(uiListView.GetDetailView());
                    if (count % 2 != 0)
                        ui.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

                    if (ui.FillData(i))
                    {
                        listView.Add(ui);
                        count++;
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

    public void GetData(bool reload)
    {
        if (reload)
        {
            uiListView.ClearList();
            //listData = new PayLoadHistory[listData.Length];
            listView = new List<PayLoadHistoryItemView>();
        }

        UpdateLabels();

        if (!listView.Any() || reload)
        {
            OGUIM.Toast.ShowLoading("");
            //OGUIM.Toast.ShowLoading("Đang tải dữ liệu, vui lòng chờ giây lát...");
            GetHistory(historyType);
        }
    }

    public void UpdateLabels()
    {
        var listTitles = new List<string>();

        if (historyType == HistoryType.PAYLOAD)
        {
            listTitles = new List<string> { "支付类型", "订单号", "时间", "金额"};
        }

        for (int i = 0; i < listLabels.Count; i++)
        {
            if (i < listTitles.Count)
                listLabels[i].text = listTitles[i];
        }
    }

    //发送获取记录请求
    public void GetHistory(HistoryType _type)
    {
        historyType = _type;
        if (historyType == HistoryType.PAYLOAD)
        {
            WarpRequest.PayloadHistoryView(data.id.ToString());
           // OldWarpChannel.Channel.DataReceive();
            if (WarpClient.wc.PayloadHisTemp != null)
            {
                StartCoroutine(GetHisString());
            }
        }
    }

    //获取并解析json字符串
    IEnumerator GetHisString()
    {
        yield return new WaitForSeconds(0.2f);

        string hisJsonString = WarpClient.wc.PayloadHisTemp;
        PayloadHistory = JsonMapper.ToObject<RootPayloadHistory>(hisJsonString);

        //PayloadHistory = JsonUtility.FromJson<RootPayloadHistory>(hisJsonString);

        if (PayloadHistory.rechargeRecordList != null)
        {
            foreach (var item in PayloadHistory.rechargeRecordList)
            {
                //Debug.LogError(item.createDate);
            }
        }
       
    }

}
