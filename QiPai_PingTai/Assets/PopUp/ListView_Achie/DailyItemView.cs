using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DailyItemView : MonoBehaviour
{
    public Text dayLabel;
    public Text rewardLabel;
	public Image originImage;
    public Image checkImage;
    public AchieListView achieListView;

	public DailyData achie;

	public bool FillData(DailyData _daily)
    {
        try
        {
			dayLabel.text = "Ngày " + _daily.day;
			rewardLabel.text = LongConverter.ToFull(_daily.koin);
			checkImage.gameObject.SetActive(_daily.check);
        }
        catch (Exception ex)
        {
            Debug.LogError("DailyItemView: FillData: " + ex.Message);
            return false;
        }
        return true;
    }
		
}
