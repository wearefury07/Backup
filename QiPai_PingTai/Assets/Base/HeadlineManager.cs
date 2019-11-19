using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class HeadlineManager : MonoBehaviour
{
    public Text contentTxt;
    public RectTransform rect;
    public int defaultMaxTextLength;
    public float defaultTotalTime = 20;

    float originX = 1000;
    float totalTime = 20f;
    bool isRunning = false;
    float beginX, endX, currX, elapsedTime;
    int currentContentIndex = 0;

    List<Message> messages = new List<Message>();

    void OnEnable()
    {
        isRunning = true;
        #if UNITY_WEBGL
        originX = transform.GetChild(0).GetComponent<RectTransform>().rect.width;
        #endif
        Reset();
        Invoke("onLoaded", 1);
    }

    void OnDisable()
    {
        isRunning = false;
    }

    // Use this for initialization
    void Start()
    {
       
        //Debug.LogError("orgin x: " + originX);
        WarpClient.wc.OnGetSystemMessagesDone += Wc_OnGetSystemMessagesDone;
        WarpClient.wc.OnNewHeadLine += Wc_OnNewHeadLine;
    }
    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            endX = -rect.sizeDelta.x;
            var pos = rect.anchoredPosition;
            if (elapsedTime < totalTime)
            {
                elapsedTime += Time.deltaTime;
                var step = elapsedTime / totalTime;
                var x = Mathf.Lerp(beginX, endX, step);
                pos.x = x;
                #if UNITY_WEBGL
                if (pos.x <= 0)
                {
                    elapsedTime = totalTime;
                }
                #endif
            }
            else
            {
                NextText();
                elapsedTime = 0;
            #if UNITY_WEBGL
                beginX = originX - rect.rect.width;
             #else
                beginX=originX;
            #endif
                pos.x = originX;
            }
            rect.anchoredPosition = pos;
        }
    }

    private void Reset()
    {
        endX = 0;
        currX = 0;
        elapsedTime = 0;
    #if UNITY_WEBGL
        beginX = originX - rect.rect.width;
    #else
        beginX=originX;
    #endif
        rect.anchoredPosition = Vector2.right * originX;
    }

    private void NextText(int index = -1)
    {
        if (!messages.Any())
            return;
        if (messages[currentContentIndex].typeMessage == 1)
            messages.RemoveAt(currentContentIndex);

        if (index == -1)
        {
            var special = messages.FindIndex(x => x.typeMessage == 1);
            if (special == -1)
            {
                currentContentIndex = (currentContentIndex + 1) % messages.Count;
            }
            else
                currentContentIndex = special;
            contentTxt.text = messages[currentContentIndex].content;
        }
        else
        {
            currentContentIndex = index % messages.Count;
            contentTxt.text = messages[currentContentIndex].content;
        }
        if (messages[currentContentIndex].content != null)
        {
            totalTime = Mathf.Max(messages[currentContentIndex].content.Length, 100) * defaultTotalTime / defaultMaxTextLength;
        }
    }

    private void onLoaded()
    {
        WarpRequest.GetSystemMessage(SystemMessageType.HEADLINE_MESSAGE);
    }

	private void Wc_OnGetSystemMessagesDone(WarpResponseResultCode status, List<Message> data)
    {
        if (data != null)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].senderId == 0 && string.IsNullOrEmpty(data[i].senderName) && data[i].type == (int)SystemMessageType.HEADLINE_MESSAGE)
                {
                    var mes = data[i];
                    mes.typeMessage = 0;
                    messages.Add(mes);
                }
            }
            NextText(0);
        }
    }

    private void Wc_OnNewHeadLine(Message data)
    {
        data.typeMessage = 1;
        messages.Add(data);
    }
}
