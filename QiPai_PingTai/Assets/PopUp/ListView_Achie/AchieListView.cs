using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using DG.Tweening;

public class AchieListView : MonoBehaviour
{
    public AchieType type;
    public UIListView uiListView;
    public List<AchieData> listData = new List<AchieData>();
	public List<AchieItemView> listView = new List<AchieItemView>();
	public List<DailyItemView> listDailyView = new List<DailyItemView>();
    public AchieData currentData;
    public AchieListView achieListView;
	public Text dayLogin;

	public List<Button> open_chest;
	public List<Button> close_chest;

	public GameObject progressBar;
	private string[] list_consecutive ;
	private string[] list_consecutive_1 ;
	private LoginValue currLValue;
	private Button currShow;
	private Button currHide;
	private double xScale = 1;
    public void Awake()
    {
		achieListView = this;
		list_consecutive = new string[]{ "2", "5", "10", "20", "All" };
		list_consecutive_1 = new string[]{ "2", "5", "10", "20", "all" };
    }

    public void OnEnable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnGetUserAchievementDone += Wc_OnGetUserAchievementDone;
			WarpClient.wc.OnGetCurrentDailyLoginDone += Wc_OnGetCurrentDailyLoginDone;
			WarpClient.wc.OnClaimRewardDone += Wc_OnClaimRewardDone;
            Get_Data(false);
        }
    }

    public void OnDisable()
    {
		if (WarpClient.wc != null) 
		{
			WarpClient.wc.OnGetUserAchievementDone -= Wc_OnGetUserAchievementDone;
			WarpClient.wc.OnGetCurrentDailyLoginDone -= Wc_OnGetCurrentDailyLoginDone;
			WarpClient.wc.OnClaimRewardDone -= Wc_OnClaimRewardDone;
		}
    }

    private void Wc_OnGetUserAchievementDone(WarpResponseResultCode status, List<AchieData> data)
    {
        if (status == WarpResponseResultCode.SUCCESS && data != null)
        {
            listData = new List<AchieData>();
            foreach (var i in data)
            {
                i.achieType = type;
                if (!LobbyViewListView.listSoloData.Any(x => x.id == i.zoneId))
                    listData.Add(i);
            }
            FillData(false);
        }
        else
        {
            OGUIM.Toast.Hide();
        }
    }

	private void Wc_OnGetCurrentDailyLoginDone(WarpResponseResultCode status, JSONObject data)
	{
		if (GameBase.days_login == null || GameBase.consecutive_days_login == null)
		{
			OGUIM.Toast.ShowNotification ("Vui lòng khởi động lại game để làm mới dữ liệu");
			return;
		}

		if (status == WarpResponseResultCode.SUCCESS && data != null) {
			JSONObject login_stats = data ["login_stats"];
			JSONObject claimed = data ["claimed"];

			DateTime now = DateTime.Now;
			int month = now.Month;
			int year = now.Year;
			int dayInMonth = DateTime.DaysInMonth (year, month);
			int consecutiveDays = 0;
			uiListView.ClearList ();
			listDailyView = new List<DailyItemView> ();

			if (data != null) {
				for (int i = 0; i < dayInMonth; i++) 
				{
					string index = (i+1).ToString();
					string strMonth = month.ToString();
					if (i + 1 < 10)
						index = "0" + index;

					if (month < 10)
						strMonth = "0" + month;
					string value = year + "-" + strMonth + "-" + index;

					var ui = uiListView.GetUIView<DailyItemView>(uiListView.GetDetailView());

					var dataI = new DailyData();
					dataI.day = i+1;
					dataI.koin = GameBase.days_login.days [i].amount;

					if (login_stats [value].str == "1") {
						dataI.check = true;
						consecutiveDays += 1;
					} else {
						dataI.check = false;
					}
					if (ui != null && ui.FillData(dataI))
						listDailyView.Add(ui);
				}

				RectTransform rt = progressBar.GetComponent(typeof (RectTransform)) as RectTransform;
				rt.sizeDelta = new Vector2 (rt.sizeDelta.x / (float)xScale, rt.sizeDelta.y);
				xScale = 1;
				if (consecutiveDays > 2)
				{
					progressBar.SetActive (true);
					if (consecutiveDays > 2 && consecutiveDays < 5)
						xScale = 1d / 4 * (consecutiveDays - 2) * 1d / 3;
					else if (consecutiveDays >= 5 && consecutiveDays < 10)
						xScale = 1d / 4 + 1d / 4 * (consecutiveDays - 5) * 1d / 5; 
					else if (consecutiveDays >= 10 && consecutiveDays < 20)
						xScale = 1d/2 + 1d/4 * (consecutiveDays - 10) * 1d/ 10;
					else if (consecutiveDays >= 20 && consecutiveDays < dayInMonth) 
						xScale = 3d/4 + 1d/4 * (consecutiveDays-20)*1d/(dayInMonth - 20);
					rt.sizeDelta = new Vector2 (rt.sizeDelta.x * (float)xScale, rt.sizeDelta.y);
				}
				else
					progressBar.SetActive (false);

				if (consecutiveDays < 10)
					dayLogin.text = "0" + consecutiveDays;
				else
					dayLogin.text = consecutiveDays.ToString();

				int indexGO = 0;
				// reset before go again
				foreach (var go in open_chest)
				{
					go.gameObject.SetActive (true);
					var currColor = go.image.color;
					currColor.a = 1f;
					go.image.color = currColor;
					go.enabled = true;
					go.onClick.RemoveAllListeners ();
				}
				foreach (var go in close_chest)
				{
					go.gameObject.SetActive (true);
				}

				foreach (var go in open_chest) {

					int value = Convert.ToInt32 (claimed [list_consecutive_1 [indexGO]].ToString ().Trim ('"'));
					if (value == 1)
					{
						go.gameObject.SetActive (false);
					}
					else
					{
						var lValue = GetLData (indexGO);
						close_chest[indexGO].gameObject.SetActive(false);
						if ((list_consecutive [indexGO] != "All" && Convert.ToInt32 (list_consecutive [indexGO]) > consecutiveDays) || (list_consecutive [indexGO] == "All" && consecutiveDays < GameBase.days_login.days.Count)) {
							var currColor = close_chest[indexGO].image.color;
							currColor.a = 0.5f;
							go.image.color = currColor;
							go.enabled = false;
						} else {
							int index = indexGO;
							go.onClick.AddListener (delegate {
								ChestHandle (index, lValue, go, close_chest[index]);
							});
						}

					}
					indexGO++;
				}


			}
		} 
		else {
			OGUIM.Toast.Hide ();
		}
	}

	private void Wc_OnClaimRewardDone(WarpResponseResultCode status, Reward  reward = null)
	{
		string text;
		if (status == WarpResponseResultCode.SUCCESS)
		{
			text = "";

			currShow.gameObject.SetActive (false);
			currHide.gameObject.SetActive (true);
			OGUIM.me.koin += currLValue.koin;
			OGUIM.me.gold += currLValue.gold;

			string content = "Bạn đã nhận được ";
			if (currLValue.gold > 0)
				content += Ultility.CoinToStringNoMark (currLValue.gold) + " " + GameBase.moneyGold.name;

			if (currLValue.koin > 0)
				content += Ultility.CoinToStringNoMark (currLValue.koin) + " " + GameBase.moneyKoin.name;
			
			OGUIM.MessengerBox.Show("Phần thưởng",content);
		}
		else if (status == WarpResponseResultCode.INVALID_CLAIM_VALUE)
		{
			text = "Nhận thưởng thất bại";
		}
		else if (status == WarpResponseResultCode.ALREADY_CLAIMED)
		{
			text = "Bạn đã nhận thưởng";
		}
		else
		{
			text = "Nhận thưởng thất bại (" + status + ")";
		}

		if (text != "") {
			OGUIM.Toast.ShowNotification (text);
		}
	}

    public void Get_Data(bool reload)
    {
        if (!listView.Any() || reload)
		{
			OGUIM.Toast.ShowLoading("");
			//OGUIM.Toast.ShowLoading("Đang tải dữ liệu, vui lòng chờ giây lát...");
            uiListView.ClearList();
            listData = new List<AchieData>();
            listView = new List<AchieItemView>();
			listDailyView = new List<DailyItemView> ();

            if (type == AchieType.ACHIEVEMENT)
                WarpRequest.GetUserAchievement();
            else if (type == AchieType.MISSION)
                WarpRequest.GetDailyMission();
			else if (type == AchieType.DAILY)
				WarpRequest.GetCurrentDailyLogin();
				
        }
    }

    public void FillData(bool refill)
    {
        if (refill)
        {
            uiListView.ClearList();
            listView = new List<AchieItemView>();
        }

        if (listData != null)
        {
            int count = 0;
            foreach (var i in listData)
            {
                try
                {
                    var ui = uiListView.GetUIView<AchieItemView>(uiListView.GetDetailView());
                    if (count % 2 != 0)
                        ui.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

                    if (ui.FillData(i))
                        listView.Add(ui);

                    count++;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("FillData: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
            OGUIM.Toast.Hide();
        }
        else
            OGUIM.Toast.ShowNotification("Không có dữ liệu, vui lòng thử lại!");
    }

    public void CheckMesToRemove()
    {
        if (listData.Any(x => x == currentData))
        {
            var temp = listData.FirstOrDefault(x => x == currentData);
            listData.Remove(temp);
            currentData = null;
            FillData(true);
        }
    }

	public void OnHintTouch(int i)
	{
		LoginValue lValue = GetLData (i);
		
		string content = "Bạn sẽ nhận được ";
		if (lValue.gold > 0)
			content += LongConverter.ToFull (lValue.gold) + " " + GameBase.moneyGold.name + " ";

		if (lValue.koin > 0)
			content += LongConverter.ToFull (lValue.koin) + " " + GameBase.moneyKoin.name + " ";
		OGUIM.MessengerBox.Show("Thông báo", content);
	}

	private void ChestHandle(int index, LoginValue lValue, Button btnShow, Button btnHide)
	{
		currLValue = lValue;
		currShow = btnShow;
		currHide = btnHide;
		WarpRequest.ClaimReward((int)ClaimType.CONSECUTIVE_LOGIN, list_consecutive_1 [index]);
	}

	public LoginValue GetLData(int i)
	{
		LoginValue lValue = new LoginValue();
		if (i == 0)
			lValue = GameBase.consecutive_days_login.login2Day;
		else if (i == 1)
			lValue = GameBase.consecutive_days_login.login5Day;
		else if (i == 2)
			lValue = GameBase.consecutive_days_login.login10Day;
		else if (i == 3)
			lValue = GameBase.consecutive_days_login.login20Day;
		else if (i == 4)
			lValue = GameBase.consecutive_days_login.loginAllDay;

		return lValue;
	}
}
