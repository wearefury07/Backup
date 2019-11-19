using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupAllMes : MonoBehaviour
{
    public UIAnimation anim;
    public UIToggleGroup uiToggleGroup;
    public Toggle showPromoAtStartToggle;

    public static List<Message> listPromo = new List<Message>();

    public CountView allCountObj;

    public CountView adminViewCountObj;
    public MesListView listAdminView;
    public static List<Message> listAdmin = new List<Message>();

    public CountView userViewCountObj;
    public MesListView listUserView;
    public static List<Message> listUser = new List<Message>();

    public CountView claimViewCountObj;
    public MesListView listClaimView;
    public static List<Message> listClaim = new List<Message>();

    public static Message currentMes;

    public static PopupAllMes instance { get; set; }

    public void Show(int tab)
    {
        instance = this;

        if (anim == null)
            anim = GetComponent<UIAnimation>();

        uiToggleGroup.IsOn(tab);

        anim.Show(() => FillData());
    }

    public void ReloadUserInfo()
    {
        WarpRequest.GetUserInfo(OGUIM.me.id);
    }

    public void Wc_OnGetSystemMessagesDone(WarpResponseResultCode status, List<Message> data)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            foreach (var i in data)
            {
                if (i.type != (int)SystemMessageType.HEADLINE_MESSAGE)
                {
                    i.type = (int)MesType.ADMIN;
                    i.sender = new UserData { displayName = i.title, avatar = "admin", id = i.senderId };
                    i.title = "";
                    i.createdDate = DateTimeConverter.ToRelativeTime(i.createdDate);
                    i.content = i.contentData;

                    listAdmin.Add(i);

                    if (i.typeMessage == (int)SystemMessageType.PROMOTION_MESSAGE)
                        listPromo.Add(i);
                }
            }

            if (gameObject.activeSelf)
            {
                FillData();
            }

            userViewCountObj.FillData(listUser.Count);
            claimViewCountObj.FillData(listClaim.Count);
            adminViewCountObj.FillData(listAdmin.Count);
            if(allCountObj != null)
            allCountObj.FillData(listAdmin.Count + listUser.Count + listClaim.Count);


			if (OGUIM.isTheFirst && showPromoAtStartToggle.isOn && listPromo.Any())
			{
				OGUIM.isTheFirst = false;
				Show(0);
			}
        }
    }

	public void Wc_OnGetMessagesDone(WarpResponseResultCode status, List<Message> data)
	{
		if (status == WarpResponseResultCode.SUCCESS)
		{
			foreach (var i in data)
			{
				if (i.type == (int)MesType.NORMAL)
				{
					i.type = (int)MesType.USER;
					if (i.senderName.ToLower() == "admin")
					{
						i.senderName = "Admin";
						i.senderAvatar = "admin";
					}

					i.sender = new UserData { displayName = i.senderName, avatar = i.senderAvatar, faceBookId = i.senderFacebookId, id = i.senderId };
					i.createdDate = DateTimeConverter.ToRelativeTime(i.createdDate);
					listUser.Add(i);
				}
				else
				{
					// Fix title + content
					if (i.title.ToLower().Contains("lên cấp"))
					{
						var tempContent = "";
						var tempTitle = "";
						var tempString = i.content.Split('-');
						if (tempString != null && tempString.Count() == 2)
						{
							tempTitle = "Quà " + i.title + " " + tempString.FirstOrDefault().Replace("game", "trò chơi");
							tempContent = tempString.LastOrDefault();
							i.title = tempTitle;
							i.content = tempContent;
						}
					}

					i.type = (int)MesType.CLAIMABLE;
					i.sender = new UserData { displayName = "Admin", avatar = "gift", id = i.senderId };
					i.createdDate = DateTimeConverter.ToRelativeTime(i.createdDate);
					listClaim.Add(i);
				}
			}

			if (gameObject.activeSelf)
			{
				FillData();
			}

			userViewCountObj.FillData(listUser.Count);
			claimViewCountObj.FillData(listClaim.Count);
			adminViewCountObj.FillData(listAdmin.Count);
            if(allCountObj != null)
			allCountObj.FillData(listAdmin.Count + listUser.Count + listClaim.Count);


			if (OGUIM.isTheFirst && showPromoAtStartToggle.isOn && listPromo.Any())
			{
				OGUIM.isTheFirst = false;
				Show(0);
			}
		}
	}

    public void FillData()
    {
        if (uiToggleGroup.currentIndex == 0)
        {
			if (!listAdmin.Any()){}
                //OGUIM.Toast.ShowNotification("Hiện tại không có tin nhắn nào từ hệ thống.");
            else
            {
                OGUIM.Toast.Hide();
                listAdminView.FillData(listAdmin);
            }
        }
        else if (uiToggleGroup.currentIndex == 1)
        {
			if (!listUser.Any()){}
                //OGUIM.Toast.ShowNotification("Hiện tại không có tin nhắn nào từ chiến hữu.");
            else
            {
                listUserView.FillData(listUser);
                OGUIM.Toast.Hide();
            }
        }
        else if (uiToggleGroup.currentIndex == 2)
        {
			if (!listClaim.Any()){}
                //OGUIM.Toast.ShowNotification("Hiện tại không có quà tặng nào.");
            else
            {
                listClaimView.FillData(listClaim);
                OGUIM.Toast.Hide();
            }
        }
    }

    public void CheckMesToRemove(Message mes)
    {
        if (mes.type == (int)MesType.ADMIN)
        {
            var checkMes = listAdmin.FirstOrDefault(x => x.id == mes.id);
            if (checkMes != null)
                listAdmin.Remove(checkMes);
            adminViewCountObj.FillData(listAdmin.Count);
            listAdminView.FillData(listAdmin);
        }
        else if (mes.type == (int)MesType.USER)
        {
            var checkMes = listUser.FirstOrDefault(x => x.id == mes.id);
            if (checkMes != null)
                listUser.Remove(checkMes);
            userViewCountObj.FillData(listUser.Count);
            listUserView.FillData(listUser);
        }
        else if (mes.type == (int)MesType.CLAIMABLE)
        {
            var checkMes = listClaim.FirstOrDefault(x => x.id == mes.id);
            if (checkMes != null)
                listClaim.Remove(checkMes);
            claimViewCountObj.FillData(listClaim.Count);
            listClaimView.FillData(listClaim);
        }
        if(allCountObj != null)
        allCountObj.FillData(listAdmin.Count + listUser.Count + listClaim.Count);
    }

    public static void GetPromoAndAdminMes()
    {
        listPromo = new List<Message>();
        listAdmin = new List<Message>();
        WarpRequest.GetSystemMessage(SystemMessageType.PROMOTION_MESSAGE);
        WarpRequest.GetSystemMessage(SystemMessageType.ADMIN_MESSAGE);
    }

    public static void GetFriendAndGiftMes()
    {
        listUser = new List<Message>();
        listClaim = new List<Message>();
        WarpRequest.GetMessages();
    }

    public void ToggleShowPromoAtStart()
    {
        if (showPromoAtStartToggle != null)
        {
            PlayerPrefs.SetInt("showPromoAtStart", showPromoAtStartToggle.isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public void ShowPromoAtStartCheck()
    {
        if (showPromoAtStartToggle != null)
            showPromoAtStartToggle.isOn = PlayerPrefs.GetInt("showPromoAtStart", 1) == 1 ? true : false;
    }

    public void GetAllMes()
    {
        if (OGUIM.isTheFirst)
		{
			ShowPromoAtStartCheck ();
            listPromo = new List<Message>();
            listAdmin = new List<Message>();
            listUser = new List<Message>();
            listClaim = new List<Message>();
            WarpRequest.GetSystemMessage(SystemMessageType.PROMOTION_MESSAGE);
            WarpRequest.GetSystemMessage(SystemMessageType.ADMIN_MESSAGE);
            WarpRequest.GetMessages();
        }
    }
}
