using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
[DisallowMultipleComponent]
public class UIButton : MonoBehaviour
{
    public bool isBackButton;
    public string soundName;

    private static Button button;


    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => UIManager.PlaySound(soundName));
        if (isBackButton)
            button.onClick.AddListener(action);
    }

    private static UnityAction action = () => OGUIM.GoBack();
}
