using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class MiniSpin : MonoBehaviour
{
    public bool isShow;
    public bool isSpin;
    public Button spinButton;
    private bool autoLeave;
    public UIAnimation anim;
    public Transform gameTransform;

    public GameObject largeCircle, smallCircle;

    public NumberAddEffect jackpotValue;

    void Awake()
    {
        anim = GetComponent<UIAnimation>();
    }

    public void Show()
    {
        autoLeave = false;
        anim.Show(() => isShow = true);
    }

    public void Hide()
    {
        anim.Hide(() => isShow = false);
    }

    public void SetJackpot(int jackpot)
    {
        if (jackpot != 0)
            jackpotValue.FillData(jackpot);
    }

    public void Close_Click()
    {
        if (isSpin)
        {
            OGUIM.Toast.ShowNotification("Bạn sẽ rời trò chơi sau khi vòng quay kết thúc.");
            autoLeave = true;
        }
        else
        {
            BuildWarpHelper.MINI_UnSub(LobbyId.MINI_SPIN, () =>
            {
                UILogView.Log("MINI_UnSub is timeout!");
            });
        }
    }

    public void Spin_Click()
    {
        if (isSpin)
        {
            OGUIM.Toast.Show("Vui lòng chờ ván kết thúc", UIToast.ToastType.Warning);
            return;
        }
        else
        {
            BuildWarpHelper.MINI_StartMatch(LobbyId.MINI_SPIN, () =>
            {
                UILogView.Log("MINI_StartMatch is timeout!");
            });
        }
    }

    public void StartMatch(Mini_RootData data)
    {
        SetJackpot(data.jackpot);
        isSpin = true;
        var gold = data.gold;
        var koin = data.koin;
        var winChips = data.winChips;
        var winGolds = data.winGolds;
        var isWinJackpot = data.isWinJackpot;

        var large = data.vi == null ? 0 : data.vi.largFace;
        var small = data.vi == null ? 0 : data.vi.smallFace;

        largeCircle.transform.DOKill();
        smallCircle.transform.DOKill();

        if (isShow)
        {
            var largeTurnNumb = Random.Range(3, 8);
            var smallTurnNumb = Random.Range(3, 8);

            largeCircle.transform.DORotate(Vector3.forward * ((large - 1) * 30 + 360 * largeTurnNumb), 3, RotateMode.FastBeyond360).SetEase(Ease.InOutCubic).SetId(MiniGames.miniGameTweenId);
            smallCircle.transform.DORotate(Vector3.back * ((1 - small) * 45 + 360 * smallTurnNumb), 3, RotateMode.FastBeyond360).SetEase(Ease.InOutCubic).SetId(MiniGames.miniGameTweenId);

            DOVirtual.DelayedCall(2.5f, () =>
            {
                var toastStrs = new List<string>();

                //efx hiển thị tiền thắng

                if (isWinJackpot)
				{
					UIManager.PlaySound("white_win");
                    MiniGames.instance.jackpotEfx.Active();
                }

                if (winGolds > 0)
				{
                    toastStrs.Add(Ultility.CoinToString(winGolds) + " " + GameBase.moneyGold.name);
                }
                if (winChips > 0)
                {
                    toastStrs.Add(Ultility.CoinToString(winChips) + " " + GameBase.moneyKoin.name);
                }

                if (winGolds > 0 || winChips > 0)
				{
					UIManager.PlaySound("winchip");
                    DOVirtual.DelayedCall(2f, () => { SpinDone(gold, koin); });
                }
                else
                {
                    SpinDone(gold, koin);
                }

                var pos = Vector3.up;
                for (int i = toastStrs.Count - 1; i >= 0; i--)
                {
                    pos += Vector3.up * 0.8f;
                    MiniGames.SpawnTextEfx(toastStrs[i], pos);
                }
            }).SetId(MiniGames.miniGameTweenId);
        }
        else
        {
            largeCircle.transform.rotation = Quaternion.Euler(Vector3.forward * (large - 1) * 30);
            smallCircle.transform.rotation = Quaternion.Euler(Vector3.back * (1 - small) * 45);
            SpinDone(gold, koin);
        }
    }

    private void SpinDone(int gold, int koin)
    {
        isSpin = false;

        OGUIM.me.gold = gold;
        OGUIM.me.koin = koin;
        OGUIM.instance.meView.FillData(OGUIM.me);

        if (autoLeave)
        {
            Close_Click();
            autoLeave = false;
        }
    }
}
