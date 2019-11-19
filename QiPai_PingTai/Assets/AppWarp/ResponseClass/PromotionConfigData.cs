using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class LoginValue
{
	public int koin ;
	public int gold ;
}

[Serializable]
public class ConsecutiveDaysLogin
{
	public LoginValue login2Day ;
	public LoginValue login5Day ;
	public LoginValue login10Day ;
	public LoginValue login20Day ;
	public LoginValue loginAllDay ;
}

[Serializable]
public class Day
{
	public int day ;
	public bool special ;
	public string type ;
	public int amount ;
}

[Serializable]
public class DaysLogin
{
	public int year ;
	public int month ;
	public List<Day> days ;
}

[Serializable]
public class DailyLogin
{
	public ConsecutiveDaysLogin consecutive_days_login ;
	public DaysLogin days_login ;
}

[Serializable]
public class PromotionConfig
{
	public DailyLogin data ;
	public string message ;
	public int status ;
}