using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class ListView_Metal : MonoBehaviour
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
            //FillData(MAINUI_Controler.listGame);
        });
    }

    public void FillData(List<Lobby> dataList)
    {
        if (dataList != null && dataList.Any())
        {
            UpdateStatus("Đang tải dữ liệu, vui lòng chờ giây lát...", false);

            foreach (var i in dataList)
            {
                var data = Instantiate(detailView);
                var ui = data.GetComponent<MetalView_Item>();

                var win = Random.Range(1, 99);
                var loss = 100 - win;
                if (ui.IconGame != null)
                    ui.IconGame.sprite = ImageSheet.Instance.resourcesDics["icon_lobby_" + i.id];
                if (ui.NameGame != null)
                    ui.NameGame.text = i.name;
                if (ui.Win != null)
                    ui.Win.text = "<size=24><color=#FFFFFF>" + win + "% </color></size> (" + win + ")";
                if (ui.Lose != null)
                    ui.Lose.text = "<size=24><color=#FFFFFF>" + loss + "% </color></size> (" + loss + ")";
                //if (ui.Archi_1 != null)
                //    ui.Archi_1.sprite = new Sprite();
                //if (ui.Archi_2 != null)
                //    ui.Archi_2.sprite = new Sprite();
                //if (ui.Archi_3 != null)
                //    ui.Archi_3.sprite = new Sprite();

                data.transform.SetParent(contentListView.transform, false);
                data.SetActive(true);

                if (objectList.Count % 2 != 0)
                {
                    var image = data.GetComponent<Image>();
                    if (image != null)
                        image.color = new Color32(0, 0, 0, 0);
                }

                objectList.Add(data);
            }
            var size = contentListView.GetComponent<RectTransform>().sizeDelta;
            size.y = (dataList.Count) * contentListView.GetComponent<GridLayoutGroup>().cellSize.y;
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
