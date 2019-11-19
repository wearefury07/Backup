using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class RootMessage
{
    public List<Message> messages;
    public string desc;
    public Message message;
}

[Serializable]
public class Message
{
    public string content;
    public string createdDate;
    public int id;
    public int senderId;
    public string senderName;
    public string senderAvatar;
    public string senderFacebookId;
    public int status;
    public string title;
    public int type = -1;
    public int value;

    public int typeMessage;
    public string contentData;
    public int contentDisplayType;
    public UserData sender;
}

[Serializable]
public class UserMessageCount
{
    public int countMsg;
    public int countTask;
    public int countCons;
    public int countAchi;
}