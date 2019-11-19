using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using MsgPack;

public class PayLoadHistoryItemView : MonoBehaviour
{
    public Text payType, OderID, Time, Amount;
   // public Button buttonDetail;
    public PayLoadHistory PayloadHistoryData;

    public string payloadHistroy;
    byte[] payLoad;
    public bool FillData(PayLoadHistory _data)
    {
        Debug.LogError(_data);
        if (_data != null)
        {
            PayloadHistoryData = _data;
            payType.text = PayloadHistoryData.payType;
            OderID.text = PayloadHistoryData.orderId;
            Amount.text = PayloadHistoryData.confirmAmount;
            Time.text = PayloadHistoryData.createDate;
            return true;
        }
        else
            return false;
    }

    private void OnEnable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnGetPayloadDetailDone += Wc_OnGetPayloadDetailDone;
        }
    }

    private void OnDisable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnGetPayloadDetailDone -= Wc_OnGetPayloadDetailDone;
        }
    }

    private void Wc_OnGetPayloadDetailDone(WarpResponseResultCode status,List<PayLoadHistory> payLoads)
    {
        //if (PayloadHistoryListView.currentPayloadHistory.id == PayloadHistoryData.id)
        //{
        //    //string content = PayloadHistoryListView.currentPayloadHistory.item;

        //    //OGUIM.MessengerBox.Show("Thông tin đổi thưởng", content);
        //}
    }

    public void ShowPayloadDetail()
    {
        //WarpRequest.PayloadHistoryView(PayloadHistoryData.id);
    }
}