using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonExtend : MonoBehaviour
{
    public void GotoSence(string sence)
    {
        try
        {
            SceneManager.LoadScene(sence);
        }
        catch (UnityException ex)
        {
            Debug.LogError("GotoSence: " + ex.Message);
        }
    }
}
