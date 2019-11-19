using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class CashoutHistory
{
    public int id;
    public int userId;
	public string requestDate;
	public string processDate;
	public string code;
	public string item;
	public int gold;
	public int status;
    public string desc;
}

[Serializable]
public class CashoutDetail
{
    public string data;
}

[Serializable]
public class RootCashoutHistory
{
	public List<CashoutHistory> data;
}