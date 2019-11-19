using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaintenanceConterller : MonoBehaviour
{
    public Text comingSoonText;
    public Text maintenanceTimeText;
    public Text DaoJiShiText;

    public Text startTime;
    public Text endTime;

    int TotalTime=100;
    void Start()
    {
        StartCoroutine(CountDown());
    }

    void Update()
    {
        
    }



    IEnumerator CountDown()
    {
        while (TotalTime>=0)
        {
            DaoJiShiText.text = TotalTime.ToString();

            yield return new WaitForSeconds(1);
            TotalTime--;
        }

    }
}
