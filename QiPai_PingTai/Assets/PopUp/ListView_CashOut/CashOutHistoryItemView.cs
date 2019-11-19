using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CashOutHistoryItemView : MonoBehaviour
{
    public Text codeLabel, typeLabel, valueLabel, requestDateLabel, processDateLabel, statusLabel;
    public Button buttonDetail;
    public CashoutHistory cashoutHistoryData;

    public bool FillData(CashoutHistory _data)
    {
        if (_data != null)
        {
            cashoutHistoryData = _data;
            codeLabel.text = cashoutHistoryData.code;
            typeLabel.text = cashoutHistoryData.item;
            valueLabel.text = LongConverter.ToFull(cashoutHistoryData.gold);

            DateTime time;
            if (DateTime.TryParse(cashoutHistoryData.requestDate, out time))
                requestDateLabel.text = DateTimeConverter.ToRelativeTime(time);
            else
                requestDateLabel.text = cashoutHistoryData.requestDate;

            if (DateTime.TryParse(cashoutHistoryData.processDate, out time))
                processDateLabel.text = DateTimeConverter.ToRelativeTime(time);
            else
                processDateLabel.text = cashoutHistoryData.processDate;

            if (cashoutHistoryData.status == 0)
                cashoutHistoryData.desc = "Chờ";
            else if (cashoutHistoryData.status == 1)
                cashoutHistoryData.desc = "Chấp nhận";
            else if (cashoutHistoryData.status == 2)
                cashoutHistoryData.desc = "Đã nhận";
            else if (cashoutHistoryData.status == 3)
                cashoutHistoryData.desc = "Từ chối";
            else if (cashoutHistoryData.status == 4)
                cashoutHistoryData.desc = "Lỗi";
            statusLabel.text = cashoutHistoryData.desc;

            if (cashoutHistoryData.status == 2 && buttonDetail != null)
            {
                statusLabel.gameObject.SetActive(false);
                buttonDetail.gameObject.SetActive(true);
            }
            else
            {
                statusLabel.gameObject.SetActive(true);
                buttonDetail.gameObject.SetActive(false);
            }

            return true;
        }
        else
            return false;
    }

    private void OnEnable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnGetCashOutDetailDone += Wc_OnGetCashOutDetailDone;
        }
    }

    private void OnDisable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnGetCashOutDetailDone -= Wc_OnGetCashOutDetailDone;
        }
    }

    private void Wc_OnGetCashOutDetailDone(WarpResponseResultCode status, List<CardInfo> cards)
    {
        if (CashOutHistoryListView.currentCashoutHistory.id == cashoutHistoryData.id)
        {
            string content = CashOutHistoryListView.currentCashoutHistory.item;
            if (cards != null && cards.Any())
            {
                var card = cards.FirstOrDefault();
                DateTime time;
                content += "\n";
                content += "Trị giá: " + LongConverter.ToFull( card.amount) + "VNĐ";
                content += "\n";
                content += "Serial thẻ: " + card.serial;
                content += "\n";
                content += "Mã thẻ: " + card.pin;
                content += "\n";
                if (DateTime.TryParse(card.expire, out time))
                    content += "Hạn dùng: " + time.ToString("HH:mm dd/MM/yyyy");
                else
                    content += "Hạn dùng: " + card.expire;
            }

            OGUIM.MessengerBox.Show("Thông tin đổi thưởng", content);
        }
    }

    public void ShowDetail()
    {
        CashOutHistoryListView.currentCashoutHistory = cashoutHistoryData;
        WarpRequest.GetCashOutDetail(cashoutHistoryData.id);
    }
}