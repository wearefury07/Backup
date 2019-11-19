using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BAUCUA_HistoryItemView : MonoBehaviour {
    public Sprite[] spriteVis;
    public Image[] image;

    public bool FillData(CasinoVi vi, bool isFade)
    {
        try
        {
            for (int i = 0; i < vi.faces.Count; i++)
            {
                var s = vi.faces[i] - 1;
                image[i].sprite = spriteVis[s];
                image[i].SetAlpha(isFade ? 0.5f : 1);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(image.Length + "   " + vi.faces.Count);
            Debug.LogError("BAUCUA_HistoryItemView FillData: " + ex.Message + " " + ex.StackTrace);
            return false;
        }

        return true;
    }
}
