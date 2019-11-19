using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class ListView_TopUp_IAP : MonoBehaviour
{
    public Text statusLabel;
    public GameObject listView;
    public GridLayoutGroup contentListView;
    public GameObject detailView;

    private List<GameObject> objectList = new List<GameObject>();

    public void Get_Data()
    {
        ClearList();

        UpdateStatus("Đang tải dữ liệu, vui lòng chờ giây lát...", true);

        //GetData form Server

        //Test
        DOVirtual.DelayedCall(1, () =>
        {
            var list = new List<SampleData>();
            for (int i = 1; i < 4; i++)
            {
                list.Add(new SampleData
                {
                    Label_1 = "IAP " + LongConverter.ToK(i * 10000) + " Xu",
                    Label_2 = LongConverter.ToK(i * 21000) + " VNĐ",
                });
            }
            FillData(list);
        });
    }

    public void FillData(List<SampleData> dataList)
    {
        if (dataList != null && dataList.Any())
        {
            UpdateStatus("Đang tải dữ liệu, vui lòng chờ giây lát...", false);

            foreach (var i in dataList)
            {
                var data = Instantiate(detailView);
                var ui = data.GetComponent<IAPView_Item>();

                if (ui.Description != null)
                    ui.Description.text = i.Label_1;
                if (ui.BuyButonContent != null)
                    ui.BuyButonContent.text = i.Label_2;

                data.transform.SetParent(contentListView.transform, false);
                data.SetActive(true);

                //if (objectList.Count % 2 != 0)
                //{
                //    var image = data.GetComponent<Image>();
                //    if (image != null)
                //        image.color = new Color32(0, 0, 0, 0);
                //}

                objectList.Add(data);
            }
            var size = contentListView.GetComponent<RectTransform>().sizeDelta;
            size.x = (dataList.Count) * contentListView.GetComponent<GridLayoutGroup>().cellSize.x;
            contentListView.GetComponent<RectTransform>().sizeDelta = size;
        }
        else
        {
            UpdateStatus("--", false);
        }
    }

    public void ClearList()
    {
        foreach (var i in objectList)
            Destroy(i);
        objectList.Clear();
    }

    public void UpdateStatus(string status, bool show)
    {
        if (statusLabel != null)
        {
            statusLabel.gameObject.SetActive(show);
            statusLabel.text = status;
        }
    }
}
