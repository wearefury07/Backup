using System.Collections.Generic;
using System;

[Serializable]
public class XengResult
{
    public int betChips;
	public List<int> luckyPosition;
    public int pos;
    public int luckyNum;
    public int winChips;
	public int number;
}

[Serializable]
public class XengResponse
{
	public XengResult result;
	public int chips;
	public int winChips;
    public int type;
    public int spinID;
}

