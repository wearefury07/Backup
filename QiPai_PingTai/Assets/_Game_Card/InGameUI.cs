using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using DG.Tweening;

public abstract class InGameUI
{
    public PlayerView[] players;
    public Button[] buttonsInGame;
    public Dictionary<string, GameObject> buttons;
    public int maxPlayerInGame = 4;
    public abstract void UpdateRoom(RoomInfo roomData);
    public void SetAllButtons(bool active, params string[] excludes)
    {
        InitButtonsIfEmpty();
        foreach (var key in buttons.Keys)
        {
            if (active)
                buttons[key].SetActive(!excludes.Any(x => x == key));
            else
                buttons[key].SetActive(excludes.Any(x => x == key));


            // Set sibling index for reset position after setactive true from false
            int index = buttons[key].transform.GetSiblingIndex();
            buttons[key].transform.SetSiblingIndex(index - 1);
            buttons[key].transform.SetSiblingIndex(index);
        }
    }

    public void SetButtonActive(string btnName, bool active)
    {
        InitButtonsIfEmpty();
        if (buttons.ContainsKey(btnName))
        {
            buttons[btnName].SetActive(active);
            // Set sibling index for reset position after setactive true from false
            int index = buttons[btnName].transform.GetSiblingIndex();
            buttons[btnName].transform.SetSiblingIndex(index-1);
            buttons[btnName].transform.SetSiblingIndex(index);
        }
        else
        {
            UILogView.Log("SetButtonActive: " + btnName + " is not existed");
        }
    }

    public void SetButtonsActive(string[] btnNames, bool[] active)
    {
        for (int i = 0; i < btnNames.Length; i++)
        {
            SetButtonActive(btnNames[i], active[i]);
        }
    }

    private void InitButtonsIfEmpty()
    {
        if (buttons == null || !buttons.Any())
        {
            buttons = new Dictionary<string, GameObject>();
            foreach (var btn in buttonsInGame)
            {
                buttons.Add(btn.name, btn.gameObject);
            }
        }
    }
}

[System.Serializable]
public class CardGameUI : InGameUI
{
    public Text[] chiInfoTxts;
    public Transform[] MAUBINHCardsTransform;
    public void SetChiInfo(params string[] chis)
    {
        if (chis.Length >= 3)
        {
            for (int i = 0; i < chiInfoTxts.Length; i++)
            {
                chiInfoTxts[i].transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(chis[i]));
                chiInfoTxts[i].text = chis[i];
            }
        }
        else
        {
            for (int i = 0; i < chiInfoTxts.Length; i++)
            {
                chiInfoTxts[i].transform.parent.gameObject.SetActive(false);
                chiInfoTxts[i].text = "";
            }
        }
    }

    public override void UpdateRoom(RoomInfo roomData)
    {
    }
}

[System.Serializable]
public class CasinoGameUI : InGameUI
{
    public const string setBetTweenId = "CasinoGameUI-setbet";
    public Text valueCardTxt, gaMoneyTxt;
    public UISlider sliderBet;
    public GameObject chipTypePrefab;
    public int currentBet;

    public override void UpdateRoom(RoomInfo roomData)
    {
        var playersOnBoard = IGUIM.GetPlayersOnBoard();
        if (OGUIM.currentLobby.id == (int)LobbyId.BACAY_GA)
        {
            gaMoneyTxt.gameObject.transform.parent.gameObject.SetActive(true);
            sliderBet.gameObject.SetActive(false);

            var newKoinGa = Ultility.CoinToStringNoMark(roomData.koinGA) + " " + OGUIM.currentMoney.name;
            if (gaMoneyTxt.text != newKoinGa)
            {
                gaMoneyTxt.text = newKoinGa;
                if (roomData.koinGA != 0)
                {
                    foreach (var pKey in playersOnBoard.Keys)
                    {
                        GameObject go = IGUIM.SpawnChipEfx();
                        go.transform.SetParent(IGUIM.instance.transform, false);
                        if (pKey == OGUIM.me.id)
                        {
                            if (IGUIM.instance.gameManager.cardsOnHand.Any())
                                SetButtonActive("BACAY_submit_btn", true);
                            go.transform.position = IGUIM.instance.currentUser.avatarView.moneyView.moneyImage.transform.position;
                        }
                        else
                        {
                            go.transform.position = playersOnBoard[pKey].avatarView.imageAvatar.transform.position;
                            go.transform.localScale = Vector3.one * 0.5f;
                        }
                        go.transform.DOMove(gaMoneyTxt.gameObject.transform.parent.position, 0.5f)
                            .OnComplete(() => go.Recycle());
                    }
                }
            }
        }
        else
        {
            sliderBet.gameObject.SetActive(!roomData.started && !OGUIM.me.owner);
            gaMoneyTxt.gameObject.transform.parent.gameObject.SetActive(false);
            sliderBet.scaleValue = roomData.bet;
            sliderBet.nameValue = OGUIM.currentMoney.name;
            sliderBet.UpdateSliderValue();
        }
    }
}
