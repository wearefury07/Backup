using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatView : MonoBehaviour
{
    public ChatMode chatMode = ChatMode.World;
    public int delayTimeSend = 3;
    public bool chatWorldInRoom = false;

    public InputField inputChat;
    public Button buttonSendChat;
    public UIToggleGroup toggleGroupChatType;
    public UIToggleGroup toggleGroupChatMode;
    public GameObject modeGroup;
    public List<Text> iconOnButtons;
    public UIAnimation anim;
	public Text chatTitle;
    public PlayerChatListView chatListView;

    public string[] quickChat = new string[]
    {
        "Nhanh nào mấy thím!",
        "Bà con ơi... hàng nóng hổi đây!",
        "Giúp tại hạ hồi máu nhanh cái!",
        "Chuẩn cơm mẹ nấu cmnr...!",
        "Cho tại hạ xin ít xiền mua sữa cho cháu!",
    };
    private DateTime lastTimeSend;

    public void Awake()
    {
    }

    public void Start()
    {
        WarpClient.wc.OnSenChatDone += Wc_OnSenChatDone;
    }

    private void Wc_OnSenChatDone(WarpResponseResultCode status)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            inputChat.text = "";
            buttonSendChat.interactable = true;
            lastTimeSend = DateTime.Now;
        }
        else
        {
            Debug.Log(status);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //Debug.LogError("Enter roir ne");
            SendChatText();
            inputChat.text = "";
        }
    }

    //public void Update()
    //{
    //    if (gameObject.activeSelf && inputChat.isFocused)
    //    {
    //        if (!string.IsNullOrEmpty(inputChat.text))
    //        {
    //            buttonSendChat.interactable = true;
    //            if (Input.GetKeyDown(KeyCode.Return))
    //                SendChatText();
    //        }
    //        else
    //        {
    //            buttonSendChat.interactable = false;
    //        }
    //    }
    //}

    public void FillData(RootChatData data)
    {
        try
        {
           // ShowAnimationChat();
            chatListView.FillData(data);
        }
        catch (Exception ex)
        {
            UILogView.Log("PlayerChatListView FillData: " + "\n" + ex.Message + "\n" + ex.StackTrace, true);
        }
    }

    public void FillData(List<RootChatData> listData)
    {
        try
        {
            //ShowAnimationChat();
            chatListView.FillData(listData);
        }
        catch (Exception ex)
        {
            UILogView.Log("PlayerChatListView FillData: " + "\n" + ex.Message + "\n" + ex.StackTrace, true);
        }
    }

    public void ChangeMode(int _chatMode)
    {
		
        chatMode = (ChatMode)_chatMode;
    }

    public void SendQuickChat(int quickIndex)
    {
#if DEBUG
        var ran = UnityEngine.Random.Range(0, quickChat.Length);
        quickIndex = ran;
#endif

        if (lastTimeSend.AddSeconds(delayTimeSend) < DateTime.Now && quickIndex < quickChat.Length && OGUIM.currentRoom != null && OGUIM.currentRoom.room != null && OGUIM.currentRoom.room.id != 0)
        {
            BuildWarpHelper.SendChat(OGUIM.currentRoom.room.id, quickChat[quickIndex], (int)ChatType.Text, null, null);
        }
        else
        {
            UILogView.Log("Next chat available in " + (delayTimeSend - (DateTime.Now - lastTimeSend).TotalSeconds) + " seconds");
        }
    }

    public void SendChatText()
    {
        if (lastTimeSend.AddSeconds(delayTimeSend) < DateTime.Now)
        {
            if (chatMode == ChatMode.Room && OGUIM.currentRoom != null && OGUIM.currentRoom.room != null && OGUIM.currentRoom.room.id != 0)
            {
                if (string.IsNullOrEmpty(inputChat.text))
                {
                    var ran = UnityEngine.Random.Range(0, quickChat.Length);
                    inputChat.text = quickChat[ran];
                }
                buttonSendChat.interactable = false;
                BuildWarpHelper.SendChat(OGUIM.currentRoom.room.id, inputChat.text, (int)ChatType.Text, null, () => buttonSendChat.interactable = true);
            }
            else if (chatMode == ChatMode.World)
            {
                if (string.IsNullOrEmpty(inputChat.text))
                {
                    var ran = UnityEngine.Random.Range(0, quickChat.Length);
                    inputChat.text = quickChat[ran];
                }
                buttonSendChat.interactable = false;
                BuildWarpHelper.SendChat(inputChat.text, () => buttonSendChat.interactable = true);
            }
        }
        else
        {
            UILogView.Log("Next chat available in" + (delayTimeSend - (DateTime.Now - lastTimeSend).TotalSeconds) + " seconds");
        }
    }

    public void SendChatEmotion(int emotionIndex)
    {
        if (lastTimeSend.AddSeconds(delayTimeSend) < DateTime.Now && OGUIM.currentRoom != null && OGUIM.currentRoom.room != null && OGUIM.currentRoom.room.id != 0)
        {
            inputChat.interactable = false;
            BuildWarpHelper.SendChat(OGUIM.currentRoom.room.id, emotionIndex.ToString(), (int)ChatType.Emotion, "1", () => inputChat.interactable = true);
        }
        else
        {
            UILogView.Log("Next chat available in" + (delayTimeSend - (DateTime.Now - lastTimeSend).TotalSeconds) + " seconds");
        }
    }

    public void Show()
    {
        if (anim == null)
            anim = GetComponent<UIAnimation>();
        if (OGUIM.currentRoom == null || OGUIM.currentRoom.room == null && OGUIM.currentRoom.room.id == 0)
            toggleGroupChatMode.IsOn((int)ChatMode.World);
        else
            toggleGroupChatMode.IsOn((int)ChatMode.Room);
        toggleGroupChatType.gameObject.SetActive(true);
        modeGroup.SetActive(false);
        anim.Show();
    }

    public void Show(int mode, bool showChatType, bool showChatMode)
    {
        toggleGroupChatType.gameObject.SetActive(showChatType);
        toggleGroupChatMode.gameObject.SetActive(showChatMode);
        toggleGroupChatMode.IsOn(mode);
        if (!showChatType && !showChatMode)
            modeGroup.SetActive(false);
        else
            modeGroup.SetActive(true);
        anim.Show();
    }

    public void ShowAnimationChat()
    {
        if (iconOnButtons != null)
        {
            for (int i = 0; i < iconOnButtons.Count; i++)
            {
                iconOnButtons[i].color = new Color32(255, 200, 0, 255);
                iconOnButtons[i].text = "";
                int ii = i;
                DOVirtual.DelayedCall(1, () =>
                {
                    iconOnButtons[ii].color = new Color32(255, 255, 255, 255);
                    iconOnButtons[ii].text = "";
                });
            }
        }
    }
}
