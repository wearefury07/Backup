using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BAUCUAGameManager : CasinoGameManager
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
                if (data.vi.faces != null && data.vi.faces.Any())
                    IGUIM_Casino.SetVis(data.vi.faces);
                //add history
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
        IGUIM_Casino.TakeChipToHost(data.pots.Select(x=>x.pot).ToArray());
        IGUIM_Casino.SetResult(data.vi.faces.ToArray());

        if (data.pots.Any(x=>x.bet > 0))
        {
            yield return new WaitForSeconds(2);

            //tra tien cho cua thang
            foreach (var p in data.pots)
            {
                IGUIM_Casino.AddChipFromHost(p.pot);
            }

        }

        yield return new WaitForSeconds(2);
        if (data.users != null && OGUIM.currentLobby.id == (int)LobbyId.BAUCUA)
        {
            IGUIM_Casino.SetUsers(data.users);
        }

        if (data.users != null && OGUIM.currentLobby.id == (int)LobbyId.BAUCUA_OLD)
        {
            var listUsers = new List<UserData>();
            listUsers.Add(data.user);
            IGUIM_Casino.SetUsers(listUsers);
        }

        if (data.users != null && OGUIM.currentLobby.id == (int)LobbyId.BAUCUA )
        {
            IGUIM_Casino.SetUsers(data.users);
            for (int i = 0; i < data.users.Count; i++)
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

		if (data.users != null && OGUIM.currentLobby.id == (int)LobbyId.BAUCUA_OLD )
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
        IGUIM_Casino.TakeChipToUser(true, data.pots.Select(x => x.pot).ToArray());
    }
}
