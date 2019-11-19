using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupLikeRate : MonoBehaviour {
	
	public Text likeLabel;
	public Text rateLabel;
	public Button likeButton;
	public Button rateButton;
	public Text likeButtonText;
	public Text rateButtonText;


	public void Show()
	{
		rateLabel.text = "Bình chọn 5 sao trên store \n <color=#FFC800> Mức thưởng: " + GameBase.likeReward + " " + GameBase.moneyGold.name + " </color>"; 
		likeLabel.text = "Like fanpage trên facebook \n <color=#FFC800> Mức thưởng: " + GameBase.rateReward + " " + GameBase.moneyGold.name + " </color>";

		HandleButton ();
	}

	private void OnEnable()
	{
		if (WarpClient.wc != null)
		{
			WarpClient.wc.OnGetRateRewardDone += Wc_OnGetRateRewardDone;
			WarpClient.wc.OnGetLikeRewardDone += Wc_OnGetLikeRewardDone;
		}
	}

	private void OnDisable()
	{
		if (WarpClient.wc != null)
		{
			WarpClient.wc.OnGetRateRewardDone -= Wc_OnGetRateRewardDone;
			WarpClient.wc.OnGetLikeRewardDone -= Wc_OnGetLikeRewardDone;
		}
	}

	private void Wc_OnGetRateRewardDone(WarpResponseResultCode status)
	{
		if (status == WarpResponseResultCode.SUCCESS) {
			OGUIM.me.isRateReward = true;
			OGUIM.instance.SetGiftButton ();
		} else if (status == WarpResponseResultCode.ALREADY_CLAIMED) {
			OGUIM.me.isRateReward = true;
			OGUIM.instance.SetGiftButton ();
			OGUIM.Toast.ShowNotification ("Bạn đã nhận phần thưởng đánh giá ứng dụng rồi!");
		}
		HandleButton ();
	}

	private void Wc_OnGetLikeRewardDone(WarpResponseResultCode status)
	{
		if (status == WarpResponseResultCode.SUCCESS)
		{
			OGUIM.me.isLikeReward = true;
			OGUIM.instance.SetGiftButton ();
		}
		else if (status == WarpResponseResultCode.ALREADY_CLAIMED) {
			OGUIM.me.isLikeReward = true;
			OGUIM.instance.SetGiftButton ();
			OGUIM.Toast.ShowNotification ("Bạn đã nhận phần thưởng thích fanpage rồi!");
		}
		HandleButton ();
	}

	private void HandleButton()
	{
		if (OGUIM.me.isLikeReward || string.IsNullOrEmpty (OGUIM.me.faceBookId)) 
		{
			likeButton.enabled = false;
			likeButton.image.color = Color.gray;
			if (string.IsNullOrEmpty (OGUIM.me.faceBookId))
				likeButtonText.text = "Cần đăng nhập FB";
			if (OGUIM.me.isLikeReward )
				likeButtonText.text = "Đã nhận";
				
		}
		else {
			likeButton.enabled = true;
			likeButton.image.color = Color.white;
			likeButtonText.text = "Like";
			likeButton.onClick.AddListener (() => {
				DOVirtual.DelayedCall(3f, () =>
					{
						WarpRequest.GetLikeReward ();
					});
				Application.OpenURL(GameBase.fbFanpage);
			});
		}

		if (OGUIM.me.isRateReward) 
		{
			rateButton.enabled = false;
			rateButton.image.color = Color.gray;
			rateButtonText.text = "Đã nhận";
		}
		else 
		{
			rateButton.enabled = true;
			rateButton.image.color = Color.white;
			rateButtonText.text = "Rate";
			rateButton.onClick.AddListener(() => 
				{
					DOVirtual.DelayedCall(3f, () =>
						{
							WarpRequest.GetRateReward ();
					});

					Application.OpenURL(GameBase.downloadURL);
				});
		}
	}
}
