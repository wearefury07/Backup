using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsItemView : MonoBehaviour
{
    public Image iconGame;
    public Text lobbyName, totalPlay, winPercent, win, draw, loss;

    public bool FillData(Stat data)
    {
        try
        {
            int checkZoneId = data.zoneId;
            if (checkZoneId == (int)LobbyId.PHOM_SOLO)
                checkZoneId = (int)LobbyId.PHOM;
            else if (checkZoneId == (int)LobbyId.SAM_SOLO)
                checkZoneId = (int)LobbyId.SAM;
            else if (checkZoneId == (int)LobbyId.TLMNDL_SOLO)
                checkZoneId = (int)LobbyId.TLMNDL;

            if (checkZoneId != 0 && ImageSheet.Instance.resourcesDics.ContainsKey("icon_lobby_" + checkZoneId))
                iconGame.sprite = ImageSheet.Instance.resourcesDics["icon_lobby_" + checkZoneId];


            var checkLobby = LobbyViewListView.listData.FirstOrDefault(x => x.id == (int)checkZoneId);
            if (checkLobby != null)
                lobbyName.text = checkLobby.desc + " " + checkLobby.subname;
            else
                lobbyName.text = data.zoneDesc;

            data.play = data.win + data.draw + data.loss;
            if (data.play > 0)
                data.winPercent = (double)(data.win) / (double)data.play * 100;
            else
                data.winPercent = 0; 

            totalPlay.text = LongConverter.ToK(data.play);
            winPercent.text = (int)data.winPercent + "%";
            win.text = data.win + "";
            draw.text = data.draw + "";
            loss.text = data.loss + "";
        }
        catch (Exception ex)
        {
            Debug.Log("StatisticsItemView FillData: " + ex.Message);
            return false;
        }
        return true;
    }
}
