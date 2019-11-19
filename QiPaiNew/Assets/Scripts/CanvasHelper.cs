using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasHelper : MonoBehaviour {


#if UNITY_EDITOR
    int preWidth, preHeight;
#endif

    void Awake()
    {
        var size3_2 = 16f / 9f;
        var sizeScene = GetScreenDimension();
        var isMatchHeight = sizeScene >= size3_2;
        var canvas = GetComponent<CanvasScaler>();
        canvas.matchWidthOrHeight = isMatchHeight ? 1 : 0;
    }

    double GetScreenDimension()
    {
        return Math.Truncate(((float)Screen.width / Screen.height) * 100.0) / 100.0;
    }


#if UNITY_EDITOR
    void Update()
    {
        
        if (preWidth != Screen.width || preHeight != Screen.height)
        {
            preWidth = Screen.width;
            preHeight = Screen.height;
            Awake();
        }
    }
#endif
}
