using System.Collections.Generic;
using System;

[Serializable]
public class MAUBINH_Chi
{
    public List<MAUBINH_User> users;
    public int index;
}

[Serializable]
public class MAUBINH_User
{
    public int userId;
    public string username;
    public int numOfChiTaken;
    public bool binhlung;
    public bool maubinh;
    public List<CardData> cards;
}
