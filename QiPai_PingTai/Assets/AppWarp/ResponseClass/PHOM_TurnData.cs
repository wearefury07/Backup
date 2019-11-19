using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class PHOM_SuiteData
{
    public int type;
    public int userId;
    public int order;
    public List<CardData> cards;
}
[Serializable]
public class PHOM_TurnData
{
    public int type;
    public int userId;
    public CardData card;
    public PHOM_SuiteData suite;
    public List<List<CardData>> suites;
    public List<CardData> cards;
}
