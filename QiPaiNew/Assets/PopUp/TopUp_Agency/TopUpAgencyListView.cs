using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;


[RequireComponent(typeof(UIListView))]
[DisallowMultipleComponent]
public class TopUpAgencyListView : MonoBehaviour
{
    public UIListView uiListView;
    public int maxItems = 10;
    private List<UserData> listData = new List<UserData>();
    private List<TopUpAgencyItemView> listView = new List<TopUpAgencyItemView>();

    private void Awake()
    {
        if (uiListView == null)
            uiListView = transform.GetComponent<UIListView>();
    }

    public void OnEnable()
    {
        HttpRequest.OnGetDealerConfigDone += HttpRequest_OnGetDealerConfigDone;
    }

    public void OnDisable()
    {
        HttpRequest.OnGetDealerConfigDone -= HttpRequest_OnGetDealerConfigDone;
    }

    private void HttpRequest_OnGetDealerConfigDone(WarpResponseResultCode status, List<UserData> data)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            OGUIM.Toast.Hide();
            listData = data;
            FillData();
        }
        else
        {
            OGUIM.Toast.Show("Có lỗi xảy ra. Vui lòng thử lại", UIToast.ToastType.Warning, 3f);
        }
    }

    public void Get_Data(bool reload)
    {
        if (reload)
        {
            uiListView.ClearList();
            listData = new List<UserData>();
            listView = new List<TopUpAgencyItemView>();
        }

        if (!listView.Any() || reload)
		{
			OGUIM.Toast.ShowLoading("");
			//OGUIM.Toast.ShowLoading("Đang tải dữ liệu, vui lòng chờ giây lát...");
            StartCoroutine(HttpRequest.Instance.GetDealerConfig());
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
                    var ui = uiListView.GetUIView<TopUpAgencyItemView>(uiListView.GetDetailView());
                    if (count % 2 != 0)
                        ui.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

                    if (ui.FillData(i, count + 1))
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
