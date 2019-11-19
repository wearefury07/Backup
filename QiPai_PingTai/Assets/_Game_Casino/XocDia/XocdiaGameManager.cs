using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class XocdiaGameManager : CasinoGameManager
{
    public override void _wc_OnEndMatch(byte[] payLoadBytes)
    {
        base._wc_OnEndMatch(payLoadBytes);
        IGUIM_Casino.SetTime(0);
        var data = ZenMessagePack.DeserializeObject<CasinoTurnData>(payLoadBytes, WarpContentTypeCode.MESSAGE_PACK);
        if (data != null)
        {
            if (data.vi != null)
            {
                //set vi
                if (data.vi.face != null && data.vi.face.Any())
                    IGUIM_Casino.SetVis(data.vi.face);

                IGUIM_Casino.AddHistory(data.vi);

                //mo bat
                IGUIM_Casino.Open();
                StartCoroutine(EndedMatch(data));
            }
        }
    }

    IEnumerator EndedMatch(CasinoTurnData data)
    {
        yield return new WaitForSeconds(2);
        //lay tien tu cua thua
        IGUIM_Casino.TakeChipToHost(data.vi.mainPot, data.vi.smallPot);
        IGUIM_Casino.SetResult(data.vi.mainPot, data.vi.smallPot);

        yield return new WaitForSeconds(2);
        if (data.users != null && OGUIM.currentLobby.id == (int)LobbyId.XOCDIA)
        {
            IGUIM_Casino.SetUsers(data.users);
        }
        if (data.users != null && OGUIM.currentLobby.id == (int)LobbyId.XOCDIA_OLD)
        {
            var listUsers = new List<UserData>();
            listUsers.Add(data.user);
            IGUIM_Casino.SetUsers(listUsers);
        }

        //tra tien cho cua thang
        if (data.mainPot != 0)
        {
            IGUIM_Casino.AddChipFromHost(data.vi.mainPot);
        }
        if (data.smallPot != 0)
        {
            IGUIM_Casino.AddChipFromHost(data.vi.smallPot);
        }

        if (data.user != null && data.user.properties != null)
        {
            if (data.vi.mainPot == 1)
            {
                IGUIM_Casino.AddChipFromHost(1);
            }
            else
            {
                IGUIM_Casino.AddChipFromHost(2);
            }
            IGUIM_Casino.AddChipFromHost(data.vi.smallPot);
        }

        yield return new WaitForSeconds(2);

		if (data.users != null && OGUIM.currentLobby.id == (int)LobbyId.XOCDIA )
        {
            for(int i = 0; i < data.users.Count; i++)
            {
                var playersOnBoard = IGUIM_Casino.GetPlayersOnBoard();
                if (playersOnBoard.ContainsKey(data.users[i].id))
                {
                    var chipchange = data.users[i].chipChange;
                    if (chipchange != 0)
                    {
                        var pos = playersOnBoard[data.users[i].id].avatarView.imageAvatar.transform.position;
                        IGUIM_Casino.SpawnTextEfx(chipchange > 0 ? "Thắng" : "Thua", pos, chipchange > 0);

                        if (chipchange != 0)
                        {
                            var str = Ultility.CoinToString(playersOnBoard[data.users[i].id].userData.chipChange) + " " + OGUIM.currentMoney.name;
                            IGUIM_Casino.SpawnTextEfx(str, playersOnBoard[data.users[i].id].avatarView.imageAvatar.transform.position + Vector3.down * 0.75f, chipchange > 0);
                        }

                    }
                }
            }
        }

		if (data.user != null && OGUIM.currentLobby.id == (int)LobbyId.XOCDIA_OLD )
		{
			var playersOnBoard = IGUIM_Casino.GetPlayersOnBoard();
			if (playersOnBoard.ContainsKey(data.user.id))
			{
				var chipchange = data.user.chipChange;
				if (chipchange != 0)
				{
					var pos = playersOnBoard[data.user.id].avatarView.imageAvatar.transform.position;
					IGUIM_Casino.SpawnTextEfx(chipchange > 0 ? "Thắng" : "Thua", pos, chipchange > 0);

					if (chipchange != 0)
					{
						var str = Ultility.CoinToString(data.user.chipChange) + " " + OGUIM.currentMoney.name;
						IGUIM_Casino.SpawnTextEfx(str, playersOnBoard[data.user.id].avatarView.imageAvatar.transform.position + Vector3.down * 0.75f, chipchange > 0);
					}

				}
			}
		}
        //tien bay ve user
        IGUIM_Casino.TakeChipToUser(true, data.vi.mainPot, data.vi.smallPot);
    }
}
