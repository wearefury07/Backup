using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class MiniPoker : MonoBehaviour
{

    public bool isShow;
    public bool isSpin;
    private bool autoLeave;

    public UIAnimation anim;
    public Transform gameTransform;

    public int currentBet = 100;
    public Toggle autoToggle;
    public UIToggleGroup betToggles;


    public NumberAddEffect jackpotValue;

    public List<CardData> allCards;
    public SlotMachine slotMachine;

    void Awake()
    {
    }

    void Start()
    {
        anim = GetComponent<UIAnimation>();
        allCards = new List<CardData>();

        for (int i = 0; i < 52; i++)
        {
            allCards.Add(new CardData(Mathf.FloorToInt(i / 4) + 1, i % 4 + 1));
        }
        slotMachine.UpdateUIByRoom(0);
    }

    public void Show()
    {
        if (!isShow)
        {
            Reset();
            anim.Show(() => isShow = true);
        }
    }

    public void Hide()
    {
        betToggles.IsOn(0);
        anim.Hide(() => isShow = false);
    }

    public void Reset()
    {
        betToggles.IsOn(0);
        autoLeave = false;
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
            OGUIM.Toast.ShowNotification("Bạn sẽ rời game sau khi vòng quay kết thúc.");
            autoLeave = true;
        }
        else
        {
            BuildWarpHelper.MINI_UnSub(LobbyId.MINI_POKER, () =>
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
            BuildWarpHelper.MINI_StartMatch(LobbyId.MINI_POKER, () =>
            {
                UILogView.Log("MINI_StartMatch is timeout!");
            });

            //StartMatch(new Mini_RootData {
            //    chips = 200,
            //    winChips = 200,
            //    isWinJackpot = true,
            //    cards = new List<CardData> { new CardData(10,1), new CardData(11, 1) , new CardData(12, 1) , new CardData(13, 1) , new CardData(1, 1) }
            //});
        }
    }

    public void SetCurrentBet(int bet)
    {
        if (currentBet != bet && isShow)
        {
            currentBet = bet;
            OGUIM.Toast.Show("");
            BuildWarpHelper.MINI_Sub(LobbyId.MINI_POKER, currentBet, 1, () =>
            {
                OGUIM.Toast.Hide();
                UILogView.Log("MINI_Sub  MINI_SPIN is time out");
            });
        }

        //currentToggle++;

        //if (currentToggle == chipToggles.Length)
        //    currentToggle = 0;
    }

    public void StartMatch(Mini_RootData data)
    {
        isSpin = true;
        allCards = allCards.OrderBy(x => System.Guid.NewGuid()).ToList();
        allCards[8] = data.cards[0];
        allCards[18] = data.cards[1];
        allCards[28] = data.cards[2];
        allCards[38] = data.cards[3];
        allCards[48] = data.cards[4];
        slotMachine.Spin(allCards);
        var gold = data.chips;
        var winChips = data.winChips;
        var isWinJackpot = data.isWinJackpot;
        var maubinhTurn = new MAUBINHTurn(data.cards);
        var turn = maubinhTurn.Name == "dach" ? "Đôi" : maubinhTurn.Name == "thu" ? "Hai Đôi" : maubinhTurn.FullName;


        DOVirtual.DelayedCall(3.5f, () =>
        {
            var toastStrs = new List<string>();
            if (isWinJackpot)
			{
				UIManager.PlaySound("white_win");
                MiniGames.instance.jackpotEfx.Active();
            }
            if (winChips > 0 && isShow)
            {
				UIManager.PlaySound("winchip");
                toastStrs.Add(turn);
                toastStrs.Add(Ultility.CoinToString(winChips) + " " + GameBase.moneyGold.name);

                DOVirtual.DelayedCall(2f, () => { SpinDone(gold); });
            }
            else
            {
                SpinDone(gold);
            }

            var pos = Vector3.zero;
            for (int i = 0; i < toastStrs.Count; i++)
            {
                MiniGames.SpawnTextEfx(toastStrs[i], pos);
                pos += Vector3.down;
            }
        }).SetId(MiniGames.miniGameTweenId);
    }

    private void SpinDone(int gold)
    {
        isSpin = false;
        if (autoToggle.isOn && isShow && !autoLeave)
            Spin_Click();
		
		OGUIM.me.gold = gold;
        OGUIM.instance.meView.FillData(OGUIM.me);

        if (autoLeave)
        {
            Close_Click();
            autoLeave = false;
        }
    }
}
