using System;

[Serializable]
public class DataConfig
{
	public int min ;
	public int max ;
	public double rate ;
	public string version;
	public string link;	
	public int userMin;	

	public int rateReward;
	public int likeReward;

}
	
[Serializable]
public class RootConfig
{
	public DataConfig data ;
	public int type ;
}