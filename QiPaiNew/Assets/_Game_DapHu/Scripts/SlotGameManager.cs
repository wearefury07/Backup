using UnityEngine;
using DG.Tweening;
using System.Linq;
using System.Collections.Generic;

public class SlotGameManager : GameManager
{

    public override void OnUnloadScene()
    {
        RemoveListener();
    }
    public override void AddListener()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnRoomStateChanged += _wc_OnRoomStateChanged;
            WarpClient.wc.OnSlotStart += _wc_OnSlotStart;
            WarpClient.wc.OnSlotStartFailed += _wc_OnSlotStartFailed;
        }
    }

    public override void RemoveListener()
    {
        if (WarpClient.wc != null)
        {
            WarpClient.wc.OnRoomStateChanged -= _wc_OnRoomStateChanged;
            WarpClient.wc.OnSlotStart -= _wc_OnSlotStart;
            WarpClient.wc.OnSlotStartFailed -= _wc_OnSlotStartFailed;
        }
    }

    private void _wc_OnRoomStateChanged(byte[] payloads, WarpContentTypeCode payloadType)
    {
        var slotData = ZenMessagePack.DeserializeObject<SLOT_ResultData>(payloads, WarpContentTypeCode.MESSAGE_PACK);
        UISlot.instance.UpdateJackpotValue(slotData.roomId, slotData.jackpot);
    }

    private void _wc_OnSlotStart(JSONObject data)
    {
        UISlot.instance.ResetSpin();
        var faces = data["result"]["slots"]["faces"].list.Select(x =>
        {
            var f = new int[5];
            for (int i = 0; i < f.Length; i++)
                f[i] = (int)x[i].n;
            return f;
        }).ToList();
        if (faces != null && faces.Any())
            UISlot.instance.DoSpin(faces);

        var paylines = data["result"]["payLines"].list.Select(x =>
        {
            var pl = new PayLine();
            pl.line = (int)x["line"].n;
            pl.matched = x["matched"].list.Select(xx => (int)xx.n).ToList();
            return pl;
        }).ToList();

        var toastStrs = new List<string>();
        var chip = (int)data["result"]["winChips"].n;
        var bet = (int)data["result"]["betChips"].n;
        var userCoin = (int)data["chips"].n;
        var freeSpin = (int)data["result"]["freeSpins"].n;
        if (paylines != null && paylines.Any())
        {
            DOVirtual.DelayedCall(3.8f, () =>
            {
                UISlot.instance.ShowLine(paylines);
            }).SetId(UISlot.instance.slotMachine.doTweenId);
        }
        else
        {
            UISlot.instance.SetWinCoinAndSpinAndUserCoin(chip, freeSpin, userCoin);
            UISlot.instance.OnSpinCompleted();
            return;
        }
        DOVirtual.DelayedCall(3.3f, () =>
        {
            UISlot.instance.SetWinCoinAndSpinAndUserCoin(chip, freeSpin, userCoin);
            if (data["result"]["isWinJackpot"].b)
			{
				UIManager.PlaySound("white_win");
                UISlot.instance.CreateSpecialEfx(1);
                UISlot.instance.CreateCoinRainEfx();
            }
            else if (chip > OGUIM.currentRoom.room.bet * 80)
			{
				UIManager.PlaySound("white_win");
                UISlot.instance.CreateSpecialEfx(2);
                UISlot.instance.CreateCoinRainEfx();
            }

            if (chip > 0 && bet > 0)
            {
				UIManager.PlaySound("winchip");
                toastStrs.Add(Ultility.CoinToString(chip) + " Gold");
                var mutiply = chip / bet / 2;
                mutiply = Mathf.Clamp(mutiply, 1, 4);
                for (int i = 1; i <= mutiply; i++)
                    DOVirtual.DelayedCall(0.7f * i, () => UISlot.instance.CreateCoinRainEfx()).SetId(UISlot.instance.slotMachine.doTweenId);
            }

            if (freeSpin > 0)
            {
                toastStrs.Add("Bạn nhận được miễn phí " + freeSpin + " dòng cược!");
            }

            var pos = new Vector3(0, UISlot.instance.listToggleLineOnGame[17].transform.position.y);
            for (int i = 0;i<toastStrs.Count; i++)
            {
                UISlot.SpawnTextEfx(toastStrs[i], pos);
                pos += Vector3.down * 0.8f;
            }
        }).SetId(UISlot.instance.slotMachine.doTweenId);
    }

    private void _wc_OnSlotStartFailed()
    {
        UISlot.instance.isSpining = false;
    }
}
