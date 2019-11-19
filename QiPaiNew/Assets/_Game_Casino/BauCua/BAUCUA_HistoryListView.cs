using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BAUCUA_HistoryListView : MonoBehaviour
{
    private const string chanFormat = " <color=#FFC800FF>HƯƠU:</color> {0} \n<color=#FFC800FF>BẦU:</color> {1} \n<color=#FFC800FF>GÀ:</color> {2}";
    private const string leFormat = " <color=#FFC800FF>CÁ:</color> {0} \n<color=#FFC800FF>CUA:</color> {1} \n<color=#FFC800FF>TÔM:</color> {2}";

    public Text HisChanTxt, HisLeTxt;
    public UIListView uiListView;
    public int items = 10;
    public List<CasinoVi> listData = new List<CasinoVi>();
    private List<BAUCUA_HistoryItemView> listView = new List<BAUCUA_HistoryItemView>();


    public void Get_Data(bool reload)
    {
        if (reload)
        {
            uiListView.ClearList();
            listData = new List<CasinoVi>();
            listView = new List<BAUCUA_HistoryItemView>();
        }
    }

    public void FillData()
    {
        var potCount = new int[] { 0, 0, 0, 0, 0, 0 };
        if (listData != null && listData.Any())
        {
            int count = 0;
            for(int i = 0; i < potCount.Length; i++)
            {
                potCount[i] = listData.Sum(x => x.faces.Count(xx => xx == i + 1));
            }
            foreach (var i in listData)
            {
                try
                {
                    var ui = uiListView.GetUIView<BAUCUA_HistoryItemView>(uiListView.GetDetailView());
                    //if (count % 2 != 0)
                    //    ui.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

                    if (ui.FillData(i, count < listData.Count - 1))
                        listView.Add(ui);

                    count++;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("FillData: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
        HisChanTxt.text = string.Format(chanFormat, potCount[0], potCount[1], potCount[2]);
        HisLeTxt.text = string.Format(leFormat, potCount[3], potCount[4], potCount[5]);

        OGUIM.Toast.Hide();
    }
}
