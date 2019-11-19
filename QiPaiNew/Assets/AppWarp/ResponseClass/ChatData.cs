using UnityEngine;
using System.Collections;

[SerializeField]
public class RootChatData
{
    public int userId;
    public string userName;
    public string icon;
    public string message;
    public string pack;
    public int type;
}

[SerializeField]
public class EmoticonData
{
    public string pack;
    public string path;
    public string message;
}

[SerializeField]
public class RootWorldChat
{
    public string message;
    public UserData user;
}
