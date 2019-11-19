using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIListView))]
[DisallowMultipleComponent]
public class CashOutHistoryListView : MonoBehaviour
{
    public HistoryType historyType;
    public UIListView uiListView;
    public int maxItems = 20;
    private List<CashoutHistory> listData = new List<CashoutHistory>();
    private List<CashOutHistoryItemView> listView = new List<CashOutHistoryItemView>();
    public List<Text> listLabels;

    public static CashoutHistory currentCashoutHistory { get; set; }

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
        if (historyType == HistoryType.CASHOUTUSER)
            WarpClient.wc.OnCashoutHistoryDone += Wc_OnCashoutHistoryDone;
    }

    public void RemoveListener()
    {
        if (historyType == HistoryType.CASHOUTUSER)
            WarpClient.wc.OnCashoutHistoryDone -= Wc_OnCashoutHistoryDone;
    }

    public void SetType(int _type)
    {
        historyType = (HistoryType)_type;
    }

    private void Wc_OnCashoutHistoryDone(WarpResponseResultCode status, List<CashoutHistory> data)
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
                    var ui = uiListView.GetUIView<CashOutHistoryItemView>(uiListView.GetDetailView());
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
            listData = new List<CashoutHistory>();
            listView = new List<CashOutHistoryItemView>();
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
        if (historyType == HistoryType.CASHOUTUSER)
            listTitles = new List<string> { "Mã", "Thể loại", "Giá trị", "Yêu cầu", "Xủ lý", "Trạng thái" };
        else if (historyType == HistoryType.CASHOUTHISTORY)
            listTitles = new List<string> { "Stt", "Người chơi", "Thời gian", "", "Thể loại", "Giá trị" };

        for (int i = 0; i < listLabels.Count; i++)
        {
            if (i < listTitles.Count)
                listLabels[i].text = listTitles[i];
        }
    }

    public void GetHistory(HistoryType _type)
    {
        historyType = _type;
        if (historyType == HistoryType.CASHOUTUSER)
        {
            WarpRequest.GetCashoutHistory();
        }
        else if (historyType == HistoryType.CASHOUTHISTORY)
        {
            OGUIM.Toast.ShowNotification("Tính năng đang được hoàn thiện, vui lòng quay lại sau");
        }
    }
}
