using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaintenanceConterller : MonoBehaviour
{
    public Text comingSoonText;
    public Text maintenanceTimeText;
    public Text DaoJiShiText;

    int TotalTime=100;
    void Start()
    {
        
    }

    void Update()
    {
        //复制文字
        //TextEditor text = new TextEditor();
        //text.text = DaoJiShiText.text;
        //text.Copy();
    }

    IEnumerator CountDown()
    {
        DaoJiShiText.text = TotalTime.ToString();
        yield return new WaitForSeconds(1);
        TotalTime--;
       
    }
}
