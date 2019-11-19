using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class CasinoTurnData
{
    public int userId;
    public int bet;
    public int pot;
    public int smallPot;
    public int mainPot;
    public List<CasinoVi> history;
    public UserData user;
    public List<UserData> users;
    public RoomInfo room;
    public CasinoVi vi;
    public int chip;
    public int type;
    public List<Slot> pots;
    public bool lucky;
    public CasinoLuckySlot luckySlot;
}

[Serializable]
public class CasinoVi
{
    public List<int> face;
    public List<int> faces;
    public int mainPot;
    public int smallPot;
    public int point;
}
[Serializable]
public class CasinoLuckySlot
{
    public int pot;
    public int rate;
}
[Serializable]
public class Slot
{
    public int pot;
    public int bet;
}


