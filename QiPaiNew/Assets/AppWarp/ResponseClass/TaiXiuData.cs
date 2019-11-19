using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class RoomTX
{
    public int maxBet;
    public int minBet;
    public int maxRate;
    public int minRate;
    public int state;
    public int timeCountDown;
    public int minCount;
    public int maxCount;
    public int id;
    public string name;
    public object type;
    public int chipType;
    public int bet;
    public int maxUsers;
    public int max_player;
    public int curr_num_of_player;
    public bool locked;
    public bool started;
    public bool quickplay;
    public int funds;
}

[Serializable]
public class TaiXiuData
{
    public int type;
    public RoomTX room;
    public int betMin;
    public int betMax;
    public List<Vi> history;
    public int chipType;

    public Vi vi;
    public int chip;
    public int chipChange;
    public int userId;

    public int bet;
    public int total;
    public int pot;
    public int totalPot;
    public int ResultCode;
    public int payback;
}

[Serializable]
public class Vi
{
    public List<int> faces;
    public int point;
}

[Serializable]
public class RootTXHistory
{
    public List<TXHistory> history;
    public int type;
}

[Serializable]
public class TXHistory
{
    public int id;
    public string time;
    public int betMax;
    public int betMin;
    public string result;
    public int win;
    public int chipType;
    public int payback;
}
