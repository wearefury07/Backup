using System;
using System.Collections.Generic;

[Serializable]
public class RootLobby
{
    public List<Lobby> lobbies;
}

[Serializable]
public class RootGame
{
    public List<Game> games;
}

[Serializable]
public class RootRoom
{
    public string desc;
    public ListBet listBet;
    public ListBetAvailable listBetAvailable;
}

[Serializable]
public class RootRoomSlot
{
    public List<RoomSlot> rooms;
}


[Serializable]
public class Lobby
{
    public int id;
    public int gameId;
    public int playmode;
    public LobbyMode lobbymode;
    public bool onlygold;
    public string name;
    public string desc;
    public string scenename;
    public int order;
    public string releaseDate;
    public int status;

    public long level;
    public long win;
    public long play;

    public string shotname;
    public string subname;
    public long tables;
    public long players;
}

[Serializable]
public class Game
{
    public string desc;
    public int id;
    public string name;
    public int status;
    public int type;
}

[Serializable]
public class ListBet
{
    public List<int> gold;
    public List<int> koin;
}

[Serializable]
public class ListBetAvailable
{
    public List<int> gold;
    public List<int> koin;
}


[Serializable]
public class RoomSlot
{
    public int timeCountDown;
    public int id;
    public object name;
    public object type;
    public int chipType;
    public int bet;
    public int maxUsers;
    public int max_player;
    public int curr_num_of_player;
    public bool locked;
    public bool started;
    public bool quickplay;
    public long funds;
}

[Serializable]
public class LobiesStatus
{
	public List<LobbyStatus> list;
}

[Serializable]
public class LobbyStatus
{
	public int zoneId;
	//slot -> tables = jackpot hu to nhat
	public long tables;
	public long player;
}
