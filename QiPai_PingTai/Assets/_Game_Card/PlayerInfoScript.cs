using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerInfoScript : MonoBehaviour {

    public Image avatarImg;
    public Image turnImg;
    public Image readyMask;
    public Image ownerImg;
    public Image rankImg;
    public Text nameTxt;
    public Text coinTxt;
    public Text remainCardTxt;
    public bool isTurn;
    public GameObject CardUI;
    public GameObject efxBackground;
    public Text efxText;
    public GameObject chipChangeTxt;
    public MessageToast userToast;

    public UserData userData;
    public Vector3 showCardDirection;
    
    void SetEnable(bool enable)
    {
        gameObject.SetActive(enable);
        gameObject.isStatic = !enable;
    }
    public void Reset()
    {
        SetRemainCard(0);
		SetTurn(false, 0);
        SetEffect("", false, false);
        SetChipChange(0, null, 0);
    }
    public void SetData(UserData userData)
    {
        if (userData == null || userData.id == 0)
        {
            Reset();
            SetEnable(false);
        }
        else
        {
            this.userData = userData;
            if(!gameObject.activeSelf)
                SetEnable(true);
            if (nameTxt != null && !string.IsNullOrEmpty(userData.displayName))
                nameTxt.text = userData.displayName;

            var avaKey = userData.avatar;
            if (!string.IsNullOrEmpty(userData.avatar) && ImageSheet.Instance.resourcesDics.ContainsKey(avaKey))
                avatarImg.sprite = ImageSheet.Instance.resourcesDics[avaKey];

            SetReady(false);
            SetOwner(userData.owner);
            SetRank(userData.allLevel);
        }
    }
    public void SetReady(bool unready)
    {
        if (readyMask != null)
            readyMask.gameObject.SetActive(!unready);
    }
    public void SetCoin(long coin)
    {
        if (coinTxt != null)
            coinTxt.text = Ultility.FormatNumber(coin);
    }
    public void SetRemainCard(int remainCardCount)
    {
        if (remainCardCount == 0)
        {
            if(CardUI != null)
                CardUI.SetActive(false);
        }
        else
        {
            if(CardUI != null && !CardUI.activeSelf)
                CardUI.SetActive(true);
            if (remainCardTxt != null)
                remainCardTxt.text = remainCardCount + "";
        }
    }
    public void SetStatus(string str)
    {
        userToast.SetData(str);
    }
    public void SetTurn(bool isTurn, float interval = 0, float maxInterval = 30)
    {
        this.isTurn = isTurn;
        if (turnImg == null)
            return;
        if (isTurn && interval != 0)
        {
            var currentFillAount = interval >= maxInterval ? 1 : (interval / maxInterval);
            turnImg.color = Color.green;
            turnImg.fillAmount = currentFillAount;
            DOVirtual.Float(currentFillAount, 0, interval, (amount) => {
                turnImg.fillAmount = amount;
                if (amount > 0.6f)
                    turnImg.color = Color.green;
                else if (amount > 0.3f)
                    turnImg.color = Color.yellow;
                else
                    turnImg.color = Color.red;
            }).SetEase(Ease.Linear).SetId(this);
        }
        else
        {
            DOTween.Kill(this);
            turnImg.fillAmount = 0;
        }
    }
    public void SetChipChange(int coin, UserData userData, int chipType, bool isAutoHide = true)
    {
        if(coin <= 0)
        {
            chipChangeTxt.SetActive(false);
        }
        if (coin != 0)
        {
            chipChangeTxt.SetActive(true);
            chipChangeTxt.transform.localScale = Vector3.one;
            chipChangeTxt.transform.DOKill();

            //var gradientComp = chipChangeTxt.GetComponent<Gradient>();
            //gradientComp.bottomVertex = coin > 0 ? new Color(1, 210f / 255, 0, 1) : Color.gray;
            var textComp = chipChangeTxt.GetComponent<Text>();
            textComp.text = Ultility.CoinToString(coin);
            if(isAutoHide)
                chipChangeTxt.transform.DOScale(Vector3.one * 0.3f, 0.3f).SetEase(Ease.OutCubic).SetDelay(3).OnComplete(()=> {
                    textComp.text = "";
                    chipChangeTxt.SetActive(false);
                });
        }

        if (userData != null)
            SetCoin(chipType == 0 ? userData.koin : userData.gold);
    }
    public void SetEffect(string content, bool hasEfxBg, bool win)
    {
        if (string.IsNullOrEmpty(content))
        {
            efxText.gameObject.SetActive(false);
            efxBackground.gameObject.SetActive(false);
        }
        else if(efxText != null)
        {
            efxText.gameObject.SetActive(true);
            efxText.text = content;
            //efxText.GetComponent<Gradient>().bottomVertex = win ? new Color(1, 210f / 255, 0, 1) : Color.gray;
            if (hasEfxBg && efxBackground != null)
            {
                efxBackground.gameObject.SetActive(true);
            }
        }
    }
    public void SetOwner(bool owner)
    {
        if(ownerImg != null)
            ownerImg.gameObject.SetActive(owner);
    }
    public void SetRank(int level)
    {
        rankImg.gameObject.SetActive(level > 20);
        if(level > 40)
        {
            rankImg.sprite = ImageSheet.Instance.resourcesDics["icon_thanbai"];
        }
        else if (level > 30)
        {
            rankImg.sprite = ImageSheet.Instance.resourcesDics["icon_vuabai"];
        }
        else if (level > 20)
        {
            rankImg.sprite = ImageSheet.Instance.resourcesDics["icon_caothu"];
        }
    }
}
