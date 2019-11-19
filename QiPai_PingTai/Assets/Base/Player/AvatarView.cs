using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AvatarView : MonoBehaviour
{
    public bool isInGame = false;
    public bool isCanShowInfo = true;
    public Image imageAvatar;
    public Image imageCoolDown;
    public Text displayName;
    public PlayerMoneyView moneyView;

    [Tooltip("Show User Id 	nearby DisplayName")]
    public bool showId;

    private UserData data;

    private void OnDestroy()
    {
        transform.DOKill();
        DOTween.Kill(this);
    }

    public void FillData(UserData _data)
    {
        if (_data == null)
            return;

        if (_data == null || data == null || data.id != _data.id)
            imageCoolDown.fillAmount = 0;
        data = _data;

        FillImage(data);
        FillDisplayName(data);
        FillMoney(data);
    }

    public void FillImage(UserData _data)
    {
        if (imageAvatar == null)
            return;
        try
        {
            if (string.IsNullOrEmpty(_data.faceBookId) || _data.faceBookId == "0")
                imageAvatar.sprite = ImageSheet.Instance.resourcesDics["avatar_" + _data.avatar];
            else if (string.IsNullOrEmpty(_data.avatar))
                imageAvatar.sprite = ImageSheet.Instance.resourcesDics["avatar_1"];
            else
                ImageHelper.GetFBProfilePicture(_data.faceBookId, imageAvatar);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
            UILogView.Log("AvatarView: " + ex.Message);
        }
    }

    public void FillDisplayName(UserData _data)
    {
        if (displayName == null)
            return;
        displayName.text = _data.displayName + (showId ? " <color=#ffffff99><i>" + data.id + "</i></color>" : "");
    }

    public void FillMoney(UserData _data)
    {
        if (moneyView == null)
            return;
        moneyView.FillData(_data);
    }

    public void SetReady(bool isReady)
    {
        if (imageAvatar == null)
            return;
        if (isReady)
            imageAvatar.color = new Color32(255, 255, 255, 255);
        else
            imageAvatar.color = new Color32(255, 255, 255, 50);
    }

    public void SetTurn(bool turn, float interval = 0, float maxInterval = 30)
    {
        if (imageCoolDown == null)
            return;

        if (turn && interval != 0)
        {
            imageCoolDown.gameObject.SetActive(true);
            var currentFillAount = (interval >= maxInterval ? 1 : (interval / maxInterval));
            imageCoolDown.fillAmount = currentFillAount;
            imageCoolDown.DOFillAmount(0, interval).SetId(this);
        }
        else
        {
            imageCoolDown.DOKill();
            imageCoolDown.fillAmount = 0;
            imageCoolDown.gameObject.SetActive(false);
        }
    }

    public void ShowUserInfo()
    {
        if (isCanShowInfo)
        {
			#if UNITY_ANDROID || UNITY_IOS
			if (GameBase.underReview)
				return;
			#endif
            OGUIM.instance.popupUserInfo.Show(data);
        }
    }
}
