using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

public class PotInfo : MonoBehaviour {

    public Transform[] chipsTransform;
    public Text txtAllBet;
    public Text txtOwnerBet;
	public Text txtLucky;
    public Animator winEfx;

    private float bgAlpha;
    void Start ()
    {
    }
    
    public void SetAllBet(int bet)
    {
		var betTemp = bet;
        txtAllBet.text = Ultility.CoinToString(betTemp).Replace("+","");
    }
    public void SetOwnerBet(int bet)
    {
        txtOwnerBet.gameObject.SetActive(bet >= 0);
		var betTemp = bet;
        txtOwnerBet.text = Ultility.CoinToString(betTemp).Replace("+", "");
    }
    public void Reset(bool isHost)
    {
        SetWin(false);
        SetOwnerBet(isHost ? -1 : 0);
		SetAllBet(0);
		SetLucky(false);
    }
    public void SetWin(bool win)
    {
        if (win)
        {
            winEfx.gameObject.SetActive(win);
            winEfx.transform.DOKill();
            winEfx.transform.localScale = Vector3.zero;
            winEfx.transform.DOScale(Vector3.one, 0.3f);
        }
        else
        {
            if (winEfx.gameObject.activeSelf)
            {
                winEfx.transform.DOKill();
                winEfx.transform.DOScale(Vector3.zero, 0.3f).SetDelay(2).OnComplete(()=> {
                    winEfx.gameObject.SetActive(false);
                });
            }
        }
    }

	public void SetLucky(bool isShow, int rate = 2, float delay = 0)
	{
		if (!isShow)
			txtLucky.gameObject.SetActive(false);
		else
		{
			txtLucky.gameObject.SetActive(true);
			txtLucky.text = "x" + rate;
			DOTween.Kill(this);
			if(delay != 0)
			{
				DOVirtual.DelayedCall(delay, () => { txtLucky.gameObject.SetActive(false); }).SetId(this);
			}
		}
	}
}
