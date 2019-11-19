using System;
using System.Collections.Generic;


[Serializable]
public class RootTopup
{
    public List<ValueTopup> value;
}

[Serializable]
public class ValueTopup
{
    public string type;
    public string money;
    public string gold;
}

