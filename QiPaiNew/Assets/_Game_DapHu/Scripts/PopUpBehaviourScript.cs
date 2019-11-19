using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using DG.Tweening;

public class PopUpBehaviourScript : MonoBehaviour
{
    public GameObject popUp;

    public System.Collections.Generic.List<string> headers;

    public System.Collections.Generic.List<Toggle> tabs;

    public System.Collections.Generic.List<GameObject> contents;

    private Animator popUpAnimator;
    public Image[] howToImages;
    
    void Awake()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            var title = tabs[i].GetComponentInChildren<Text>();
            if (title != null && i < headers.Count)
            {
                title.text = headers[i];
                tabs[i].gameObject.SetActive(!string.IsNullOrEmpty(headers[i]));
            }
            else
            {
                tabs[i].gameObject.SetActive(false);
            }
        }

        foreach (var i in contents)
        {
            if(i != null)
                i.SetActive(false);
        }

        popUpAnimator = GetComponent<Animator>();

        popUp.SetActive(false);
    }
    public void SetRoomId(int id)
    {
        for (int i = 0; i < howToImages.Length; i++)
            howToImages[i].sprite = ImageSheet.Instance.resourcesDics[string.Format("slot_{0}_{1}", id, i + 1)];
    }
    public void SetCellSize(int count)
    {
        if (tabs.Any())
        {
            var parentGrid = tabs.FirstOrDefault().transform.parent.gameObject.GetComponent<GridLayoutGroup>();
            if (parentGrid != null)
            {
                var cellsize = parentGrid.cellSize;
                cellsize.x = (1020 - 108 * 2) / count;
                parentGrid.cellSize = cellsize;
            }
        }

    }
	public void Show(int indexTab)
    {
        if (tabs.Count != contents.Count)
        {
            Debug.LogError("Show: tabs.Count != contents.Count");
            return;
        }

        if (!popUp.activeSelf)
            popUp.SetActive(true);

        CheckTabsIsOff(indexTab);

        if (popUpAnimator == null)
            popUpAnimator = GetComponent<Animator>();

        if (popUpAnimator != null)
            popUpAnimator.SetTrigger("Show");
    }

	public void Show(int indexTab, Action action)
	{
		Show (indexTab);

		if (action != null) {
			DOVirtual.DelayedCall (1f, () => {
				action();
			});
		}
	}

    public void CheckTabsIsOff(int indexTab)
    {
        if (tabs.Count != contents.Count)
        {
            Debug.LogError("CheckTabsIsOff: tabs.Count != contents.Count");
            return;
        }

        for (int i = 0; i < tabs.Count; i++)
        {
            if (i == indexTab)
            {
                //var ani = tabs[i].animator;
                tabs[i].isOn = true;
            }
            else
            {
                //var ani = tabs[i].animator;
                tabs[i].isOn = false;
            }

            if (contents[i] != null)
                contents[i].SetActive(tabs[i].isOn);
        }
    }

    public void CheckContentIsOff()
    {
        if (tabs.Count != contents.Count)
        {
            Debug.LogError("CheckTabsIsOff: tabs.Count != contents.Count");
            return;
        }

        for (int i = 0; i < tabs.Count; i++)
        {
            if(contents[i] != null)
                contents[i].SetActive(tabs[i].isOn);
        }
    }

    public void Hide()
    {
        if (popUpAnimator != null)
            popUpAnimator.SetTrigger("Hide");
    }

    public void SetActive(int isActive)
    {
        popUp.SetActive(isActive == 1);
    }
}
