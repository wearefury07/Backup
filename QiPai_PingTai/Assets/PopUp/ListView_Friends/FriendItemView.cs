using System;
using UnityEngine;
using UnityEngine.UI;

public class FriendItemView : MonoBehaviour
{
    public Image avatar;
    public Text displayName;
    public Image statusImage;
    public Text statusText;
    public Text gold;
    public Text coin;

    public static UserData userData;

    public bool FillData(UserData i)
    {
        try
        {
            userData = i;

            displayName.text = userData.displayName;

            var resAvatar = ImageSheet.Instance.resourcesDics["avatar_" + userData.avatar];
            if (resAvatar == null)
                Debug.LogError("FriendItemView FillData: " + "avatar_" + userData.status + "not found!!!");
            else
                avatar.sprite = resAvatar;

            if (!string.IsNullOrEmpty(userData.status))
                statusText.text = userData.status;

            var resStatus = ImageSheet.Instance.resourcesDics["icon_lobby_" + userData.status];
            if (resStatus == null)
                Debug.LogError("FriendItemView FillData: " + "icon_lobby_" + userData.status + "not found!!!");
            else
                statusImage.sprite = resStatus;
        }
        catch (Exception ex)
        {
            Debug.LogError("FriendItemView FillData: " + ex.Message);
            return false;
        }

        return true;
    }
}
