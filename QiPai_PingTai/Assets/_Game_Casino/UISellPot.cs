using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UISellPot : MonoBehaviour
{
    public List<UIToggle> toggles;

    public void Show(bool isBuy, params int[] list)
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            if (!list.Contains(i + 1))
                continue;
            if (toggles[i].toggle == null)
                toggles[i].toggle.GetComponent<Toggle>();

            toggles[i].toggle.isOn = isBuy;
            toggles[i].transform.parent.gameObject.SetActive(true);
        }
    }
    public void ShowAll(bool isBuy)
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            if (toggles[i].toggle == null)
                toggles[i].toggle.GetComponent<Toggle>();

            toggles[i].toggle.isOn = isBuy;
            toggles[i].transform.parent.gameObject.SetActive(true);
        }
    }
    public void HideAll()
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            if(toggles[i].transform.parent.gameObject != null)
                toggles[i].transform.parent.gameObject.SetActive(false);
        }
    }
    public void Hide(int i)
    {
        toggles[i].transform.parent.gameObject.SetActive(false);
    }
}
