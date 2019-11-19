using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupUserInfo : MonoBehaviour
{

    public AvatarView avatarView;
    public Text userId;
    private string userIdFormat = "<color=#FFC800FF>ID:</color> {0}";
    public Text displayName;
    private string displayNameFormat = "<color=#FFC800FF>Tên:</color> {0}";
    public Text sex;
    private string sexFormat = "<color=#FFC800FF>Giới tính:</color> {0}";
    public Text mobile;
    private string mobileFormat = "<color=#FFC800FF>Điện thoại:</color> {0}";

    public Text displayNameUpper;

    public PlayerMoneyView goldView;
    public PlayerMoneyView koinView;

    public NumberAddEffect timePlayLabel;
    public NumberAddEffect numbPlayLabel;
    public NumberAddEffect winpercentLabel;
    public NumberAddEffect cashOutLabel;

    public Button verifyPhoneButton;
    public Button changeAvatarButton;
    public Button updateInfoButton;

    public Button historyButton;
    public Button changPassButton;
    public Button flowButton;
    public Button unFlowButton;
    public Button sendMesButton;
    public Button linkAccButton;
    public Button linkFacebookButton;

    public UserData userData;
    private RootStat rootStat;

    public PopupVerifyPhone popupVerifyPhone;
    public PopupAvatars popupAvatars;
    public RegisterView popupLinkAcc;
    public PopupUpdatePass popupUpdatePass;
    public StatisticsListView popupStatistics;

    //修改个人信息界面
    public PopupUpdateInfo PopupUpdateInfo;

    public UIAnimation anim;

    private void Awake()
    {
        if (anim == null)
            anim = GetComponent<UIAnimation>();
    }

    public void Show(UserData _userData)
    {
        userData = _userData;
        rootStat = null;
        FillData(userData);
        FillDataStatic(false);
        anim.Show(() =>
        {
            OGUIM.Toast.ShowLoading("");
            WarpRequest.GetUserInfo(_userData.id);
        });
    }

    public void OnEnable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnGetUserInfoDone += Wc_OnGetUserInfoDone;
            WarpClient.wc.OnGetUserStatDone += Wc_OnGetUserStatDone;
            WarpClient.wc.OnAddFriendsDone += Wc_OnAddFriendsDone;
            WarpClient.wc.OnDeleteFriendDone += Wc_OnDeleteFriendDone;
            WarpClient.wc.OnUpdatedMoneyDone += Wc_OnUpdatedMoneyDone;
            WarpClient.wc.OnLinkFBDone += Wc_OnLinkFBDone;
        }
    }

    public void OnDisable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnGetUserInfoDone -= Wc_OnGetUserInfoDone;
            WarpClient.wc.OnGetUserStatDone -= Wc_OnGetUserStatDone;
            WarpClient.wc.OnAddFriendsDone -= Wc_OnAddFriendsDone;
            WarpClient.wc.OnDeleteFriendDone -= Wc_OnDeleteFriendDone;
            WarpClient.wc.OnLinkFBDone -= Wc_OnLinkFBDone;
            WarpClient.wc.OnUpdatedMoneyDone -= Wc_OnUpdatedMoneyDone;
        }
    }

    private void Wc_OnGetUserInfoDone(WarpResponseResultCode status, RootUserInfo data)
    {
        if (status == WarpResponseResultCode.SUCCESS && data != null)
        {
            if (!string.IsNullOrEmpty(data.faceBookId))
                userData.faceBookId = data.faceBookId;
            if (!string.IsNullOrEmpty(data.mobile))
                userData.mobile = data.mobile;
            userData.isFriend = data.isFriend;
            userData.gold = data.user.gold;
            userData.koin = data.user.koin;

            FillData(userData);
            //OGUIM.isListen = false;
            //WarpRequest.GetUserStat(userData.id);

            if (userData.id == OGUIM.me.id)
            {
                if (!string.IsNullOrEmpty(data.mobile))
                    OGUIM.me.mobile = data.mobile;
                if (!string.IsNullOrEmpty(data.faceBookId))
                    OGUIM.me.faceBookId = data.faceBookId;
                OGUIM.me.verified = data.user.verified;
                OGUIM.instance.meView.FillData(data.user);
            }
        }
        OGUIM.Toast.Hide();
    }

    private void Wc_OnGetUserStatDone(WarpResponseResultCode status, RootStat data)
    {
        if (status == WarpResponseResultCode.SUCCESS && data != null)
        {
            rootStat = data;
            FillDataStatic(false);
        }
        //OGUIM.isListen = true;
        OGUIM.Toast.Hide();
    }

    private void Wc_OnAddFriendsDone(WarpResponseResultCode status)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            userData.isFriend = true;
            flowButton.gameObject.SetActive(!userData.isFriend);
            unFlowButton.gameObject.SetActive(userData.isFriend);
            OGUIM.Toast.ShowNotification("Kết bạn với " + userData.displayName + " thành công!");
            ResetBottomBtnPos();
        }
    }

    private void Wc_OnDeleteFriendDone(WarpResponseResultCode status)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            userData.isFriend = false;
            flowButton.gameObject.SetActive(!userData.isFriend);
            unFlowButton.gameObject.SetActive(userData.isFriend);
            OGUIM.Toast.ShowNotification("Hủy kết bạn với " + userData.displayName + " thành công!");
            ResetBottomBtnPos();
        }
    }

    public void Wc_OnLinkFBDone(WarpResponseResultCode status)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            OGUIM.me.link_fb = true;
            FillData(OGUIM.me);
            OGUIM.Toast.ShowNotification("Liên kết tài khoản Facebook thành công!");
        }
        else if (status == WarpResponseResultCode.REG_USER_EXIST)
        {
            OGUIM.MessengerBox.Show("Thông báo", "Rất tiếc, Facebook này đã kết nối với một tài khoản khác");
        }
    }

    private void Wc_OnUpdatedMoneyDone(UpdatedMoneyData data)
    {
        if (data.chipType == (int)MoneyType.Gold && goldView != null)
            goldView.FillData(MoneyType.Gold, data.total);
        else if (data.chipType == (int)MoneyType.Koin && koinView != null)
            koinView.FillData(MoneyType.Koin, data.total);
    }

    public void FillData(UserData _userData)
    {
        userData = _userData;
        avatarView.FillData(userData);

        string tempDisplayName = userData.displayName;
        if (tempDisplayName.Length > 15)
            tempDisplayName = tempDisplayName.Substring(0, 15) + "...";

        displayName.text = string.Format(displayNameFormat, tempDisplayName);
        displayNameUpper.text = userData.displayName.ToUpper();
        sex.text = string.Format(sexFormat, sexConverter(userData.gender));



        goldView.FillData(GameBase.moneyGold.type, userData.gold);
        koinView.FillData(GameBase.moneyKoin.type, userData.koin);

        var mobileNumb = userData.mobile;

        if (IsMe(userData))
		{
			userId.text = string.Format(userIdFormat, userData.id);
            verifyPhoneButton.gameObject.SetActive(OGUIM.isVerified != 1);
			//verifyPhoneButton.gameObject.SetActive (false); // Hidden verify
            if (OGUIM.instance != null && !string.IsNullOrEmpty(OGUIM.instance.currentGameScene))
                changeAvatarButton.gameObject.SetActive(false);
            updateInfoButton.gameObject.SetActive(true);
            changPassButton.gameObject.SetActive(true);

            flowButton.gameObject.SetActive(false);
            unFlowButton.gameObject.SetActive(false);
            sendMesButton.gameObject.SetActive(false);

            if (string.IsNullOrEmpty(mobileNumb) || mobileNumb == "0" || mobileNumb.Length < 8)
				mobile.text = string.Format(mobileFormat, "chưa có");
            else
                mobile.text = string.Format(mobileFormat, mobileNumb);

            //Nếu chưa Linkacc //link_acc; -> Show popup Register
            linkAccButton.gameObject.SetActive(!userData.link_acc);

            //Nếu chưa LinkFb //link_fb; - > Show facebook login
            linkFacebookButton.gameObject.SetActive(!userData.link_fb && userData.faceBookId.Length < 2);
            changPassButton.gameObject.SetActive(userData.faceBookId.Length < 2);



        }
        else
		{
			// Hidden userId of others
			userId.text = string.Format(userIdFormat, "******");
            verifyPhoneButton.gameObject.SetActive(false);
            changeAvatarButton.gameObject.SetActive(false);
            changPassButton.gameObject.SetActive(false);
            updateInfoButton.gameObject.SetActive(false);

            flowButton.gameObject.SetActive(!userData.isFriend);
            unFlowButton.gameObject.SetActive(userData.isFriend);
            sendMesButton.gameObject.SetActive(true);

			// Hidden mobile of others
            mobile.text = string.Format(mobileFormat, "**********");

            linkAccButton.gameObject.SetActive(false);
            linkFacebookButton.gameObject.SetActive(false);
        }

        linkAccButton.gameObject.SetActive(false);
        updateInfoButton.gameObject.SetActive(false);
        ResetBottomBtnPos();
    }

    public void ResetBottomBtnPos()
    {
        int index;
        var listBtn = new List<Button>();
        listBtn.Add(historyButton);
        listBtn.Add(flowButton);
        listBtn.Add(unFlowButton);
        listBtn.Add(sendMesButton);
        listBtn.Add(changPassButton);
        listBtn.Add(linkAccButton);
        listBtn.Add(linkFacebookButton);

        foreach (var _btn in listBtn)
        {
            index = _btn.transform.GetSiblingIndex();
            _btn.transform.SetSiblingIndex(index - 1);
            _btn.transform.SetSiblingIndex(index);
        }
    }

    public void FillDataStatic(bool reload)
    {
        //card 90s 
        //casino 60s

        if (reload && rootStat == null)
        {
            //OGUIM.isListen = false;
            //WarpRequest.GetUserStat(userData.id);
        }

        if (rootStat == null)
        {
            timePlayLabel.FillData(0, "phút");
            numbPlayLabel.FillData(0, "ván");
            winpercentLabel.FillData(0, "%");
            cashOutLabel.FillData(0, GameBase.moneyGold.name);
            return;
        }

        foreach (var i in rootStat.data)
        {
            rootStat.winTotal += i.win;

            i.play = i.loss + i.win + i.draw;
            rootStat.playTotal += i.play;

            if (i.play > 0)
                i.winPercent = (double)(i.win) / (double)i.play * 100;
            else
                i.winPercent = 0;

            if (i.zoneId != (int)LobbyId.BAUCUA
                && i.zoneId != (int)LobbyId.XOCDIA)
            {
                i.playTime = i.play * 89;
            }
            else
            {
                i.playTime = i.play * 54;
            }
            rootStat.playTimeTotal += i.playTime;
        }

        if (rootStat.playTotal > 0)
            rootStat.winPercentTotal = ((double)rootStat.winTotal / (double)rootStat.playTotal * 100);
        else
            rootStat.winPercentTotal = 0;

        var timePlaySpan = (long)(System.TimeSpan.FromSeconds(rootStat.playTimeTotal).TotalMinutes);

        timePlayLabel.FillData(timePlaySpan, "phút");
        numbPlayLabel.FillData(rootStat.playTotal, "ván");
        winpercentLabel.FillData((long)rootStat.winPercentTotal, "%");
        cashOutLabel.FillData(0, GameBase.moneyGold.name);
    }

    /// <summary>
    /// 验证手机号
    /// </summary>
    public void VerifyPhoneShow()
    {
        popupVerifyPhone.Show();
    }

 

    /// <summary>
    /// 修改密码
    /// </summary>
    public void UpdatePassShow()
    {
		if (OGUIM.me.link_acc)
            popupUpdatePass.Show();
        else
            OGUIM.MessengerBox.Show("Oops...!", "Tài khoản của bạn đang ở chế độ CHƠI NHANH"
                + "\n\n"
                + "Vui lòng tạo tài khoản để thay đổi mật khẩu",
                "Tạo tài khoản", () => { LinkAccShow(); },
                "Lần sau", null);
    }

    /// <summary>
    /// 修改信息
    /// </summary>
    public void UpdateInfoShow()
    {
        //OGUIM.Toast.ShowNotification("Tính năng \"cập nhật\" đang được cập nhật");
        PopupUpdateInfo.Show(userData);
    }

    public void StaticShow()
    {
        if (rootStat != null && rootStat.data != null)
            popupStatistics.Show(rootStat.data);
    }

    public void HistoryShow()
    {
        OGUIM.Toast.ShowNotification("Tính năng \"lịch sử chơi\" đang được cập nhật");
    }

    public void Flow()
    {
        WarpRequest.AddFriends(userData.id);
    }

    public void UnFlow()
    {
        WarpRequest.DeleteFriend(userData.id);
    }

    public void SendMesShow()
    {
        OGUIM.instance.popupSendMes.Show(userData);
    }

    public void ChangeAvatarShow()
    {
        if (string.IsNullOrEmpty(userData.faceBookId) || userData.faceBookId == "0")
            popupAvatars.Show(userData);
        else
            OGUIM.Toast.ShowNotification("Không thể thay ảnh đại diện facebook");
    }

    public void LinkAccShow()
    {
        popupLinkAcc.Show(() =>
        {
            FillData(OGUIM.me);
        });
    }

    public void LinkFacebookShow()
    {
        OGUIM.instance.LoginWithFB(() =>
        {
            WarpRequest.LinkFB(OGUIM.me.faceBookId, OGUIM.me.facebookAccessToken);
        });
    }

    public string sexConverter(int gender)
    {
        switch (gender)
        {
            case 1:
                return "Nam";
            case 0:
                return "Nữ";
            default:
                return "Không xác định";
        }
    }

    public bool IsMe(UserData userData)
    {
        if (OGUIM.me.id == userData.id)
            return true;
        return false;
    }
}
