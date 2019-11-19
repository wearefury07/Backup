using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class Mini_RootData
{
    public LobbyId id;
    public bool isSub;
    public int jackpot;

    public int type;
    public int chipType;
    public int betChips;
    public int status;
    public int chips;
    public int freeSpins;
    public bool isWinJackpot;
    public int koin;
    public int winChips;
    public int gold;
    public int winGolds;

    public int myMaxBet;

    public int myMinBet;

    public string desc;
    public List<CardData> cards;
    public HistoryBase vi;

    public List<HistoryBase> history;
    public RoomInfo room;

    // this below for tai xiu
    public int betMin;
    public int betMax;
    public int chip;
    public int chipChange;
    public int userId;
    public int bet;
    public int total;
    public int pot;
    public int totalPot;
    public int ResultCode;
    public int payback;

    // this below for caothap
    public int spinID;
    public int high;
    public int low;
    public int numA;
    public int timeCountDown;
}


