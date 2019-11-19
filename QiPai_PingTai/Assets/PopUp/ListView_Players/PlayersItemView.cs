using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersItemView : MonoBehaviour
{
    public AvatarView avatar;
    public Text rankLabel;
    public Text statusLabel;
    public List<Image> achiImages;

    public UserData userData;

    public bool FillData(UserData i)
    {
        try
        {
            userData = i;
            avatar.FillData(i);

            if (rankLabel != null)
                rankLabel.text = LongConverter.ToK(i.position);

            if (statusLabel != null)
                statusLabel.text = i.status;
        }
        catch (Exception ex)
        {
            Debug.LogError("PlayersItemView FillData: " + ex.Message);
            return false;
        }

        return true;
    }
}
