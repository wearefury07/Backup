using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


[Serializable]
public class PayLoadHistory
{
    public string confirmAmount;
    public string createDate;
    public string orderId;
    public string payType;
   
}

[Serializable]
public class RootPayloadHistory
{
    public List<PayLoadHistory> rechargeRecordList;
}


[Serializable]
public class PayloadDetail
{
    public string data;
}


