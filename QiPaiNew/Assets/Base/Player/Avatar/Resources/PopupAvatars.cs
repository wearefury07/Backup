using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PopupAvatars : MonoBehaviour
{
    public List<string> listAvatar = new List<string> { "1", "f1", "f2", "f3", "f4", "f5", "m1", "m2", "m3", "m4" };
    public UIToggleGroup uiToggleGroup;
    public PopupUserInfo popupUserInfo;

    private string newAvatarName;

    public UIAnimation anim;

    public void Show(UserData user)
    {
        var checkIndex = listAvatar.IndexOf(user.avatar);
        if (checkIndex < 0)
            checkIndex = 0;
        uiToggleGroup.IsOn(checkIndex);

        anim.Show(() => UpdateUIAvatar());
    }

    public void OnEnable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnUpdateAvatarDone += Wc_OnUpdateAvatarDone;
        }
    }
    private void OnDisable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnUpdateAvatarDone -= Wc_OnUpdateAvatarDone;
        }
    }

    private void Wc_OnUpdateAvatarDone(WarpResponseResultCode status)
    {
        if (status == WarpResponseResultCode.SUCCESS)
        {
            OGUIM.Toast.ShowNotification("Cập nhật ảnh đại diện thành công");
            OGUIM.me.avatar = newAvatarName;
            OGUIM.instance.meView.FillImage(OGUIM.me);
            popupUserInfo.FillData(OGUIM.me);
            anim.Hide();
        }
    }

    private void UpdateUIAvatar()
    {
        if (listAvatar != null && uiToggleGroup.toggles != null)
        {
            for (int i = 0; i < listAvatar.Count; i++)
            {
                try
                {
                    if (i < uiToggleGroup.toggles.Count)
                    {
                        //Update Name
                        uiToggleGroup.toggles[i].name = listAvatar[i];
                        var sprite = ImageSheet.Instance.resourcesDics["avatar_" + listAvatar[i]];
                        uiToggleGroup.toggles[i].GetComponent<UIToggle>().UpdateImageContent(sprite);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("UpdateUIAvatar: " + ex.Message);
                }
            }
        }
    }

    public void UpdateAvatar()
    {
        var toggle = uiToggleGroup.toggleGroup.ActiveToggles().FirstOrDefault();
        if (toggle != null)
        {
            newAvatarName = toggle.name;
            Debug.Log("UpdateAvatar: " + newAvatarName);
            WarpRequest.UpdateAvatar(newAvatarName);
        }
    }
}
