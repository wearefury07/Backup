using System;
using UnityEngine;

public class UIPopupTabs : MonoBehaviour
{
    public UIAnimation anim;
    public UIToggleGroup uiToggleGroup;

    public void Show(int _tab)
    {
        uiToggleGroup.IsOn(_tab);
        anim.Show();
    }
    
    public void Hide(Action action)
    {
        uiToggleGroup.isShowAll();
        anim.Hide(()=> action());
    }

    public void ShowOnly(int _tab)
    {
        uiToggleGroup.isShowOnly(_tab);
    }


    public void ShowAll()
    {
        uiToggleGroup.isShowAll();
    }
}
