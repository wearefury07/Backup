using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
internal class UILogView : MonoBehaviour
{
    [Range(10, 100)]
    public int eventsDebugShow = 20;
    public RectTransform content;
    public Text textLog;
    public Text iconShow, iconHide;
    public UIAnimation anim;

    public static UILogView uiLogView;
    private static string datePatt = @"hh:mm:ss tt";
    private static IList<string> events = new List<string>();

    private void Awake()
    {
        uiLogView = this;
    }

    private void Start()
    {
        if (uiLogView == null)
            uiLogView = this;
        Hide();
    }

    public void Show()
    {
        iconShow.color = new Color32(255, 255, 255, 255);
        iconHide.color = new Color32(255, 255, 255, 255);
        anim.Show();
    }

    public void Hide()
    {
        anim.Hide();
    }

    public static void Log(object log, bool isError = false)
    {
        var result = log.ToString();
#if (DEBUG)
        var stackFrame = new System.Diagnostics.StackFrame(1, true);
        result = string.Format("{0} \n at method '{1}' \n at line {2} ({3})", log, stackFrame.GetMethod().Name, stackFrame.GetFileLineNumber(), stackFrame.GetFileName());            

        if (isError)
            Debug.LogError(result);
        else
            Debug.Log(result);
#endif
        if (uiLogView != null)
        {
            if (isError)
            {
                uiLogView.iconShow.color = new Color32(180, 0, 0, 255);
                uiLogView.iconHide.color = new Color32(180, 0, 0, 255);
            }
            var timeString = "<color=#ffffff99>" + DateTime.Now.ToString(datePatt) + "</color>";
            var logString = isError ? "<color=#cc0000ff>" + result + "</color>" : result;
            if (events.Count > uiLogView.eventsDebugShow)
                events.Remove(events.LastOrDefault());
            events.Insert(0, timeString + "\n" + logString);
            uiLogView.textLog.text = string.Join("\n", events.ToArray());
            uiLogView.content.sizeDelta = new Vector2(uiLogView.content.sizeDelta.x, uiLogView.textLog.preferredHeight);
        }
    }

    public void ClearLog()
    {
        events = new List<string>();
        uiLogView.content.sizeDelta = Vector2.zero;
        uiLogView.textLog.text = "";
    }
}
