using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MesItemView : MonoBehaviour
{
    public AvatarView avatar;
    public Text dateTime;
    public Text contentMes;

    public Button buttonDel;
    public Button buttonReply;
    public Button buttonGo;
    public Button buttonClaim;
    

    public Message mes;

    public bool FillData(Message _mes)
    {
        try
        {
            mes = _mes;
            if (mes.type == (int)MesType.ADMIN)
            {
                buttonDel.gameObject.SetActive(false);
                buttonReply.gameObject.SetActive(false);
                buttonClaim.gameObject.SetActive(false);
                buttonGo.gameObject.SetActive(false);
                
            }
            else if (mes.type == (int)MesType.USER)
            {
                buttonDel.gameObject.SetActive(true);
                buttonReply.gameObject.SetActive(true);
                buttonClaim.gameObject.SetActive(false);
                buttonGo.gameObject.SetActive(false);
              
            }
            else if (mes.type == (int)MesType.CLAIMABLE)
            {
                buttonDel.gameObject.SetActive(false);
                buttonReply.gameObject.SetActive(false);
                buttonClaim.gameObject.SetActive(true);
                buttonGo.gameObject.SetActive(false);
                
                avatar.FillData(mes.sender);
            }
            

            var content = "";
            if (!string.IsNullOrEmpty(mes.title))
                content = ("<size=20>" + mes.title + "</size>\n" + "   " + mes.content);
            else
                content = mes.content;
            contentMes.text = content;

            avatar.FillData(mes.sender);
            dateTime.text = mes.createdDate;
        }
        catch (Exception ex)
        {
            Debug.LogError("MesItemView FillData: " + ex.Message);
            return false;
        }
        return true;
    }

    public void Delete()
    {
        PopupAllMes.currentMes = mes;
        WarpRequest.DeleteMessages(new List<string> { mes.id.ToString() });
    }

    public void Reply()
    {
        OGUIM.instance.popupSendMes.Show(mes.sender);
    }

    public void Claim()
    {
        PopupAllMes.currentMes = mes;
        WarpRequest.ClaimReward(mes.type, mes.id.ToString());
    }

    public void acceptFriend()
    {

    }

    public void Go()
    {

    }

    public void ShowDetail()
    {
        if (mes != null)
        {
            var content = "";
            if (!string.IsNullOrEmpty(mes.title))
                content = mes.title + "\n" + mes.content;
            else
                content = mes.content;

            if (mes.type == (int)MesType.ADMIN)
            {
                OGUIM.MessengerBox.Show(mes.sender.displayName, content);
            }
            else if (mes.type == (int)MesType.USER)
            {

                OGUIM.MessengerBox.Show(mes.sender.displayName, content,
                    "Trả lời", () =>
                    {
                        Reply();
                    }, "Lần sau", null);
            }
            else if (mes.type == (int)MesType.CLAIMABLE)
            {
                OGUIM.MessengerBox.Show(mes.sender.displayName, content,
                    "Nhận thưởng", () =>
                    {
                        Claim();
                    }, "Lần sau", null);
            }
        }
    }

    private void OnEnable()
    {
        WarpClient.wc.OnClaimRewardDone += Wc_OnClaimRewardDone;
        WarpClient.wc.OnDeleteMessagesDone += Wc_OnDeleteMessagesDone;
    }

    private void OnDisable()
    {
        WarpClient.wc.OnClaimRewardDone -= Wc_OnClaimRewardDone;
        WarpClient.wc.OnDeleteMessagesDone -= Wc_OnDeleteMessagesDone;
    }

    private void Wc_OnClaimRewardDone(WarpResponseResultCode status, Reward data = null)
    {
        if (PopupAllMes.currentMes != null && mes.id == PopupAllMes.currentMes.id)
        {
            PopupAllMes.currentMes = null;

            if (buttonClaim != null && mes.type == (int)MesType.CLAIMABLE)
                buttonClaim.gameObject.SetActive(false);

            var message = "";
			if (status == WarpResponseResultCode.SUCCESS)
			{
				message = "Nhận thưởng thành công...!";
				WarpRequest.GetUserInfo (OGUIM.me.id);
			}
            else if (status == WarpResponseResultCode.INVALID_CLAIM_VALUE)
                message = "Nhận thưởng thất bại...!";
            else if (status == WarpResponseResultCode.ALREADY_CLAIMED)
                message = "Bạn đã nhận thưởng...!";

            PopupAllMes.instance.CheckMesToRemove(mes);
            OGUIM.Toast.ShowNotification(message);
        }
    }


    private void Wc_OnDeleteMessagesDone(WarpResponseResultCode status)
    {
        if (PopupAllMes.currentMes != null && mes.id == PopupAllMes.currentMes.id)
        {
            PopupAllMes.currentMes = null;
            if (status == WarpResponseResultCode.SUCCESS)
            {
                PopupAllMes.instance.CheckMesToRemove(mes);
                OGUIM.Toast.ShowNotification("Đã xóa tin nhắn...");
            }
        }
    }
}