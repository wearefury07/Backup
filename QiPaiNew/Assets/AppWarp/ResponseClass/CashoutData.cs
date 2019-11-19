using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class RootCashout
{
	public List<CashoutProduct> data ;
}

[Serializable]
public class CashoutProduct
{
	public int id ;
	public int hot ;
	public string image ;
	public string name ;
	public int status ;
	public int type ;
	public int money ;
	public int gold ;
	public string provider ;
}


[Serializable]
public class CashoutData
{
	public long gold;
    public long min;
    public long time;
    public long goldChange;
	public int status;
	public List<CardInfo> card;
}

[Serializable]
public class CashoutHistoryData
{
    public List<CardInfo> data;
}

[Serializable]
public class CardInfo
{
	public string serial;
	public string pin;
	public string expire;
    public string provider_code;
    public int amount;
}

