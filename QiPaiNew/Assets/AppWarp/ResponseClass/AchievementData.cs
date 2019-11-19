using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class RootAchievement
{
	public List<AchieData> data;
}

[Serializable]
public class AchieData
{
    public AchieType achieType;
    public int actual;
	public string desc;
	public int gold;
	public int id;
	public int koin;
	public string name;
	public int status;
	public int target;
	public int type;
	public int zoneId;
}

[Serializable]
public class DailyData
{
	public int type;
	public bool check;
	public int day;
	public int koin;
}

[Serializable]
public class Reward
{
	public int koin;
	public int gold;
}

public enum AchieType
{
    ACHIEVEMENT = 0,
    MISSION = 1,
	DAILY = 2,
}
