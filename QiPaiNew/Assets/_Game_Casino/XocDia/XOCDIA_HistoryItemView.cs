using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class XOCDIA_HistoryItemView : MonoBehaviour {
    public Sprite[] spriteVis;
    public Image image;
    public bool FillData(CasinoVi i)
    {
        try
        {
            image.sprite = spriteVis[i.face.Count(x => x == 1)];
        }
        catch (System.Exception ex)
        {
            Debug.LogError("XOCDIA_HistoryItemView FillData: " + ex.Message + " " + ex.StackTrace);
            return false;
        }

        return true;
    }
}
