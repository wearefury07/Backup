using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class XOCDIA_HistoryListView : MonoBehaviour {
    private const string chanFormat = " <color=#FFC800FF>CHẴN:</color> ";
    private const string leFormat = "  <color=#FFFFFF> LẺ:</color> ";

    public Text HisChanTxt, HisLeTxt;
    public UIListView uiListView;
    public int items = 10;
    public List<CasinoVi> listData = new List<CasinoVi>();
    private List<XOCDIA_HistoryItemView> listView = new List<XOCDIA_HistoryItemView>();
    
    
    public void Get_Data(bool reload)
    {
        if (reload)
        {
            uiListView.ClearList();
            listData = new List<CasinoVi>();
            listView = new List<XOCDIA_HistoryItemView>();
        }
    }

    public void FillData()
    {
        HisChanTxt.text = chanFormat + listData.Count(x => x.mainPot % 2 == 1);
        HisLeTxt.text = leFormat + listData.Count(x => x.mainPot % 2 == 0);
        if (listData != null && listData.Any())
        {
            int count = 0;
            foreach (var i in listData)
            {
                try
                {
                    var ui = uiListView.GetUIView<XOCDIA_HistoryItemView>(uiListView.GetDetailView());
                    if (count < listData.Count - 1)
                        ui.image.SetAlpha(0.5f);
                    //if (count % 2 != 0)
                    //    ui.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

                    if (ui.FillData(i))
                        listView.Add(ui);

                    count++;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("FillData: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
        OGUIM.Toast.Hide();
    }
}
