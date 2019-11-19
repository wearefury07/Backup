using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    public bool isMe;
    public LobbyId playing;
    public AvatarView avatarView;
    public PlayerMoneyView betMoneyView;
    public PlayerChatView chatView;
    public CardView cardView;
    public GameObject owner;

    public bool isTurn;
    public int suiteCount;
    public Vector3 showCardDirection;
    public Transform PHOM_submitCardsTransform;
    public Transform[] PHOM_suitesCardsTransform;

    public UserData userData;

    public void FillData(UserData _userData)
    {
        if (_userData == null)
        {
            gameObject.SetActive(false);
            return;
        }
        _userData.CopyDataIfEmpty(userData);
        if (_userData.id == OGUIM.me.id)
            OGUIM.instance.meView.moneyView.FillData(_userData);
        gameObject.SetActive(true);

        playing = OGUIM.instance.currentLobbyId;


        if (avatarView != null && userData != _userData)
            avatarView.FillData(_userData);
        userData = _userData;

        owner.SetActive(userData.owner);
        cardView.gameObject.SetActive(false);
        betMoneyView.gameObject.SetActive(false);

        if (playing == LobbyId.BACAY || playing == LobbyId.BACAY_GA || playing == LobbyId.LIENG)
        {
            betMoneyView.gameObject.SetActive(true);
        }
        else if (playing == LobbyId.SAM || playing == LobbyId.TLMNDL || playing == LobbyId.TLMNDL_SOLO)
        {
            if (cardView != null && !isMe && userData != null && userData.isPlayer 
                && OGUIM.currentRoom != null
                && OGUIM.currentRoom.room != null
                && OGUIM.currentRoom.room.started)
                cardView.gameObject.SetActive(true);
        }
    }

    public void Reset()
    {
        betMoneyView.UpdateBet(0);
        SetStatus();
    }

    public void SetRemainCard(int value)
    {
        if (userData.id != OGUIM.me.id)
            cardView.UpdateView(value);
        else
            cardView.UpdateView(0);
    }

    public void PlusRemainCard()
    {
        if (userData.id != OGUIM.me.id)
            cardView.PlusCard();
        else
            cardView.UpdateView(0);
    }

    public void SetReady(bool isready)
    {
		// Hidden in new version
        if (playing != LobbyId.NONE)
            avatarView.SetReady(isready);
        else
            avatarView.SetReady(true);
    }

    public void SetTurn(bool turn, float interval = 0, float maxInterval = 30)
    {
        isTurn = turn;
        avatarView.SetTurn(turn, interval, maxInterval);
    }

    public void SetStatus(string _status = null)
    {
        if (!string.IsNullOrEmpty(_status))
            avatarView.displayName.text = _status;
        else
            avatarView.displayName.text = userData.displayName;
    }

    public void SetOwner(bool _owner)
    {
        owner.SetActive(_owner);
    }

    public void SetBet(long betValue)
    {
        betMoneyView.UpdateBet(betValue);
    }
    

    #region Set Rank Star
    private Image rankImg;
    private List<Sprite> rankImgs;
    public void SetRank(int level)
    {
        int index = Mathf.Min(level, 40) / 10;
        rankImg.sprite = rankImgs[index];
    }

    private Image starImg;
    private List<Sprite> starImgs;
    public void SetStar(int level)
    {
        if (level == 0)
        {
            starImg.sprite = starImgs[0];
        }
        else
        {
            var index = (level % 10) + 1;
            starImg.sprite = starImgs[index];
        }
    }
    #endregion

    public Vector3 Position
    {
        get
        {
            var offset = Vector3.zero;
            var name = gameObject.name;
            if (name.Contains("Left"))
                offset = Vector3.left * 2;
            else if (name.Contains("Right"))
                offset = Vector3.right * 2;
            else if (name.Contains("Top"))
                offset = Vector3.up * 2;
            return gameObject.transform.position + offset;
        }
    }
}
