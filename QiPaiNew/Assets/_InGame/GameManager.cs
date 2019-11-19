using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class GameManager : MonoBehaviour
{
    public virtual void Awake()
    {
        Debug.Log("-------------------GameManager Awake");
        AddListener();
        Invoke("LoadScene", 0.5f);
    }
    public virtual void LoadScene()
    {
        Debug.Log("-------------------GameManager Start");
        if (this is CardGameManager)
        {
            if (OGUIM.currentRoom.users != null && OGUIM.currentRoom.users.Any())
                IGUIM.SetUsers(OGUIM.currentRoom.users);
        }

        if (OGUIM.currentRoom != null)
        {
            BuildWarpHelper.GetRoomInfo(OGUIM.currentRoom, () =>
            {
                Debug.LogError("GetRoomInfo is time out.");
            });
        }
    }
    private void OnDestroy()
    {
        try
        {
            OnUnloadScene();
        }
        catch(Exception ex)
        {
            Debug.LogError("----------GameManager / OnDestroy: " + ex.Message);
        }
    }
    
    public virtual void Update() { }
    public abstract void AddListener();
    public abstract void RemoveListener();
    public abstract void OnUnloadScene();
}
