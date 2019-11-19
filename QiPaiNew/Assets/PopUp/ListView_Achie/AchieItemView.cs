using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AchieItemView : MonoBehaviour
{
    public Image iconGame;
    public Text statusLabel;
    public UISlider statusSlider;
    public Text rewardLabel;
    public Image rewardImage;
    public Button buttonGo;
    public Button buttonClaim;
    public AchieListView achieListView;

    public AchieData achie;

    public bool FillData(AchieData _achie)
    {
        try
        {
            achie = _achie;

            int checkZoneId = achie.zoneId;
            if (checkZoneId == (int)LobbyId.PHOM_SOLO)
                checkZoneId = (int)LobbyId.PHOM;
            else if (checkZoneId == (int)LobbyId.SAM_SOLO)
                checkZoneId = (int)LobbyId.SAM;
            else if (checkZoneId == (int)LobbyId.TLMNDL_SOLO)
                checkZoneId = (int)LobbyId.TLMNDL;

            if (checkZoneId != 0 && ImageSheet.Instance.resourcesDics.ContainsKey("icon_lobby_" + checkZoneId))
                iconGame.sprite = ImageSheet.Instance.resourcesDics["icon_lobby_" + checkZoneId];
            statusLabel.text = achie.desc.Replace("game", "trò chơi").Trim();

            statusSlider.slider.maxValue = achie.target;
            statusSlider.slider.value = achie.actual;
            statusSlider.sliderValue.text = achie.actual + "/" + achie.target;

            if (achie.gold > 0)
            {
                rewardLabel.text = LongConverter.ToFull(achie.gold);
                rewardImage.sprite = GameBase.moneyGold.image;
            }
            else if (achie.koin > 0)
            {
                rewardLabel.text = LongConverter.ToFull(achie.koin);
                rewardImage.sprite = GameBase.moneyKoin.image;
            }

            if (achie.status == 0)
            {
                buttonGo.gameObject.SetActive(true);
                buttonClaim.gameObject.SetActive(false);
            }
            else
            {
                buttonGo.gameObject.SetActive(false);
                buttonClaim.gameObject.SetActive(true);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("AchieItemView: FillData: " + ex.Message);
            return false;
        }
        return true;
    }

    private void OnEnable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnClaimRewardDone += Wc_OnClaimRewardDone;
        }
    }

    private void OnDisable()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnClaimRewardDone -= Wc_OnClaimRewardDone;
        }
    }


    private void Wc_OnClaimRewardDone(WarpResponseResultCode status, Reward data)
    {
        if (achieListView.currentData != null && achie.id == achieListView.currentData.id)
        {
            achieListView.CheckMesToRemove();
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
            OGUIM.Toast.ShowNotification(message);
        }
    }

	public void Claim(bool isAchie = true)
    {
        achieListView.currentData = achie;
        OGUIM.Toast.ShowLoading("");

		if (isAchie)
        	WarpRequest.ClaimAchievementBonus(achie.id);
		else
			WarpRequest.ClaimDailyQuestBonus(achie.id);
    }

    public void GoTo()
    {
//		#if DEBUG
//        achieListView.currentData = achie;
//        Wc_OnClaimRewardDone(WarpResponseResultCode.SUCCESS, null);
//        return;
//		#endif

        var checkLobby = LobbyViewListView.listData.FirstOrDefault(x => x.id == achie.zoneId);
        if (checkLobby != null)
        {
            if (OGUIM.instance != null)
            {
                if ((LobbyId)checkLobby.id == LobbyId.XENG_HOAQUA)
                {
                    OGUIM.Toast.ShowNotification(checkLobby.desc + " sẽ sớm phát hành trong thời gian gần nhất");
                }
                else
                {
                    OGUIM.instance.popupAchieMissiDaily.Hide(() =>
                    {
                        if ((LobbyId)checkLobby.id == LobbyId.SLOT)
                        {
                            checkLobby.playmode = (int)PlayMode.QUICK;
                            if (OGUIM.instance.lobbyViewInRooms != null)
                                OGUIM.instance.lobbyViewInRooms.FillData(checkLobby);
                            OGUIM.instance.SubLobby(checkLobby);
                        }
                        else
                        {
                            checkLobby.playmode = (int)PlayMode.NORMAL;
                            if (OGUIM.instance.lobbyViewInRooms != null)
                                OGUIM.instance.lobbyViewInRooms.FillData(checkLobby);
                            OGUIM.instance.SubLobby(checkLobby);
                        }
                    });
                }
            }
            else
            {
                UILogView.Log("OGUIM.instance: NULL!???????????");
            }
        }
    }
}
