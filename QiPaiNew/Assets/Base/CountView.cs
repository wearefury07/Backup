using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CountView : MonoBehaviour
{
    public GameObject circle;
    public NumberAddEffect countNumb;

    public void FillData(long obj = 0)
    {
        if (obj == 0)
        {
            circle.gameObject.SetActive(false);
        }
        else
        {
            circle.gameObject.SetActive(true);
            DOVirtual.DelayedCall(0.1f, () =>
            {
                countNumb.FillData(obj);
            });
        }
    }
}
