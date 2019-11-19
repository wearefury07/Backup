using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class RootCardProvider
{
	public List<CardProvider> data ;
}

[Serializable]
public class CardProvider
{
	public string name;
	public string image ;
	public string provider ;
}

