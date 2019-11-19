using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Room
{
    public int intervalPlay;
    public RoomInfo room;
    public List<UserData> users;
    public int userId;


    //auto kick user time count down
    public int countDown;
}

[Serializable]
public class RoomInfo
{
    public int koinGA;
    public int bet;
    public int chipType;
    public int curr_num_of_player;
    public int funds;
    public int id;
    public bool locked;
    public int maxUsers;
    public int max_player;
    public bool quickplay;
    public bool started;

    public int timeCountDown;
    public int state;
	public int timeCountDownNew;
	public int stateNew;

    public bool lucky;
    public CasinoLuckySlot luckySlot;
    public List<Slot> slots;

    // this below for tai xiu
    public int maxBet;
    public int minBet;
    public int maxRate;
    public int minRate;
    public int minCount;
    public int maxCount;
    public string name;
    public object type;
}

public class GetRoomInfoData
{
    public List<CardData> cards;
    public int lastTurnUser;
    public int currTurnUser;
    public RoomInfo room;
    public List<UserData> users;
    public bool isNewTurn;
    public long turnStartTime;
}
