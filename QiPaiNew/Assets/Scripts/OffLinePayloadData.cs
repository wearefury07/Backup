using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;



[Serializable]
public class OffLinePayloadData
{
    public string pictureCode;
    public string remark;
    public string amount;
    public string phone;
    public string bankNo;
    public string bankName;
    public string branchName;
    public string accountOwner;
}


[Serializable]
public class OffLinePayloadDataRoot
{
    public OffLinePayloadData patamMap;
}


