using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class TopUpCardItemListView : MonoBehaviour
{
    public UIListView uiListView;
    public int maxItems = 10;
    private List<ValueTopup> listData = new List<ValueTopup>();
    private List<TopUpCardItemView> listView = new List<TopUpCardItemView>();

    public void Start()
    {
        WarpClient.wc.OnGetKoinExchangeDone += Wc_OnGetKoinExchangeDone;
    }

    private void Wc_OnGetKoinExchangeDone(WarpResponseResultCode status, List<ValueTopup> data)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            foreach (var i in data.Where(x => x.type == "card").OrderBy(x => long.Parse(x.money)))
            {
                listData.Add(i);
            }
            FillData();
        }
        OGUIM.Toast.Hide();
    }

    public void Get_Data(bool reload)
    {
        if (reload)
        {
            uiListView.ClearList();
            listData = new List<ValueTopup>();
            listView = new List<TopUpCardItemView>();
        }

        if (!listView.Any() || reload)
		{
			OGUIM.Toast.ShowLoading("");
			//OGUIM.Toast.ShowLoading("Đang tải dữ liệu, vui lòng chờ giây lát...");
            WarpRequest.GetKoinExchange();
        }
    }

    public void FillData()
    {
        if (listData != null && listData.Any())
        {
            int count = 0;
            foreach (var i in listData)
            {
                try
                {
                    var ui = uiListView.GetUIView<TopUpCardItemView>(uiListView.GetDetailView());
                    if (count % 2 != 0)
                        ui.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

                    if (ui.FillData(i))
                    {
                        listView.Add(ui);
                        count++;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("FillData: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
    }
}
