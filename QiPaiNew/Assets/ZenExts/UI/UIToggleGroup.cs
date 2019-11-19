using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
[DisallowMultipleComponent]
public class UIToggleGroup : MonoBehaviour
{
    public List<Toggle> toggles;
    public ToggleGroup toggleGroup;
    public int currentIndex = -1;

    private void Awake()
    {
        toggleGroup = gameObject.GetComponent<ToggleGroup>();
    }

    private void Start()
    {
        if (toggleGroup == null)
            toggleGroup = gameObject.GetComponent<ToggleGroup>();
        GetToggles();
    }

    public List<Toggle> GetToggles()
    {
        if (toggleGroup == null)
            toggleGroup = gameObject.GetComponent<ToggleGroup>();
        if (toggles == null || !toggles.Any())
        {
            toggles = new List<Toggle>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var a = transform.GetChild(i).gameObject.GetComponent<Toggle>();
                if (a != null && a.gameObject.activeSelf)
                    toggles.Add(a);
            }
        }
        return toggles;
    }

    public void IsOn(int index)
    {
        GetToggles();
        if (toggles != null && toggles.Any())
        {
            if (index != -1)
                currentIndex = index;
            if (currentIndex >= 0)
            {
                for (int i = 0; i < toggles.Count; i++)
                {
                    if (i == currentIndex)
                        toggles[i].isOn = true;
                    else
                        toggles[i].isOn = false;
                }
            }
        }
        else
        {
            Debug.LogError("UIToggleGroup: toggles not found!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
    }

    public void isShowOnly(int index)
    {
        GetToggles();
        if (toggles != null && toggles.Any())
        {
            if (index != -1)
                currentIndex = index;
            if (currentIndex >= 0)
            {
                for (int i = 0; i < toggles.Count; i++)
                {
                    if (i == currentIndex)
                        toggles[i].gameObject.SetActive(true);
                    else
                        toggles[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            Debug.LogError("UIToggleGroup: toggles not found!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }  
    }

    public void isShowAll()
    {
        GetToggles();
        if (toggles != null && toggles.Any())
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                toggles[i].gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("UIToggleGroup: toggles not found!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
    }

    public void Disable()
    {
        GetToggles();
        if (toggles != null && toggles.Any())
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                toggles[i].interactable = false;
            }
        }
        else
        {
            Debug.LogError("UIToggleGroup: toggles not found!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
    }

    public void Disable(int except)
    {
        GetToggles();
        if (toggles != null && toggles.Any())
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                if (i != except)
                    toggles[i].interactable = false;
                else
                    toggles[i].interactable = true;
            }
        }
        else
        {
            Debug.LogError("UIToggleGroup: toggles not found!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
    }

    public void Enable()
    {
        GetToggles();
        if (toggles != null && toggles.Any())
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                toggles[i].interactable = true;
            }
        }
        else
        {
            Debug.LogError("UIToggleGroup: toggles not found!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
    }

    public void SetCurrentTab(int _index)
    {
        if (toggles.Count >= _index && toggles[_index].isOn)
            currentIndex = _index;
    }

    public void UpdateTextContent(List<object> objs)
    {
        GetToggles();
        if (toggles != null && toggles.Any() && objs != null && objs.Count >= toggles.Count)
        {
            for (int i = 0; i < toggles.Count; i++)
                toggles[i].GetComponent<UIToggle>().UpdateTextContent(objs[i]);
        }
        else
        {
            Debug.LogError("toggles == null || && objs != null || objs.Count < toggles.Count");
        }
    }
}
