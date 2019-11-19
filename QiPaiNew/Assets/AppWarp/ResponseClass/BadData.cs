using System.Collections.Generic;
using System;


[Serializable]
public class BaseData
{
    public int type;
    public string desc;
}

[Serializable]
public class UpdatePeerData
{
    public int type;
    public List<UserData> users;
}