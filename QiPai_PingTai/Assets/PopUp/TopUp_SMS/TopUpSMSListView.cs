using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TopUpSMSListView : MonoBehaviour
{
    public List<TopUpSMSItemView> smsItemViews;
    public UIToggleGroup uiToggleGroup;
    public static List<SMSData> listData =  new List<SMSData>();
    private bool isTried;


    private void Start()
    {
        WarpClient.wc.OnGetSMSConfig += Wc_OnGetSMSConfig;
    }

    private void Wc_OnGetSMSConfig(WarpResponseResultCode status, JSONObject _data)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            var data = new JSONObject(_data["value"].ToString().Replace("\\", ""));
            for (int i = 0; i < data.Count; i++)
            {
                var temp = new JSONObject(data[i].ToString());
                if (temp != null)
                {
                    for (int ii = 0; ii < temp.Count; ii++)
                    {
                        var tempSMS = new JSONObject(temp[ii].ToString());
                        try
                        {
                            string money = tempSMS["money"].str;
                            string gold = tempSMS["gold"].str;
                            string shortCode = tempSMS["shortCode"].str;
                            string message = tempSMS["message"].str;
                            string provider = tempSMS["provider"].str;
                            listData.Add(new SMSData { money = money, gold = gold, message = message, provider = provider, shortCode = shortCode });
                        }
                        catch (System.Exception ex)
                        {
                            Debug.Log("Wc_OnGetSMSConfig Passer: " + ex.Message);
                        }
                    }
                }
            }

            uiToggleGroup.IsOn(-1);
            var toggle = uiToggleGroup.toggleGroup.ActiveToggles().FirstOrDefault();
            if (toggle != null && listData != null)
                FillData(toggle);
        }
    }


    public void Get_Data(bool reload)
    {
        if (reload)
        {
            listData = new List<SMSData>();
            isTried = false;
        }

        if (!listData.Any() || reload)
		{
			OGUIM.Toast.ShowLoading("");
			//OGUIM.Toast.ShowLoading("Đang tải dữ liệu, vui lòng chờ giây lát...");
            WarpRequest.GetSMSConfig();
        }
    }

    public void FillData(Toggle toggle)
    {
        if (!listData.Any() && !isTried)
        {
            Get_Data(true);
            isTried = true;
            return;
        }

        if (listData.Any())
        {
            var checkSMS = listData.Where(x => x.provider == toggle.name).OrderBy(x => long.Parse(x.money)).ToList();
            if (checkSMS.Any())
            {
                for (int i = 0; i < smsItemViews.Count && i < checkSMS.Count; i++)
                {
                    if (i < checkSMS.Count)
                        smsItemViews[i].FillData(checkSMS[i]);
                    else
                        smsItemViews[i].FillData(null);
                }
            }
        }

        OGUIM.Toast.Hide();
    }
}
