using System;
using System.Collections.Generic;
using System.Linq;
public class TurnData
{
    public List<CardData> cards;
    public List<CardData> playedCards;
    public List<UserData> users;
    public RoomInfo room;
    public UserData user;
    public int remainCardCount;
    public int userId;
    public int loserId;
    public int winnerId;
    public bool newTurn;
    public bool keyTurn;
    public int type;
    public Properties properties;

    #region LIENG
    public List<LIENGPot> pots;
    public int specialty;
    public int total_chips;
    public int call_chips;
    public int bet;
    public int maxBet;
    #endregion
}
public class CardEndMatchData
{
    public int reason;
    public int specialty;
    public List<UserData> users;
    public RoomInfo room;

}
[Serializable]
public class LIENGPot
{
    public int order;
    public int takenUserId;
    public int totalKoins;
}

