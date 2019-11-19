using System.Collections.Generic;

public class SMSData
{
    public string money;
    public string gold;
    public string shortCode;
    public string message;
    public string provider;
}

public class RootSMSValue
{
    public List<SMSData> value50;
    public List<SMSData> value20;
    public List<SMSData> value10;
}