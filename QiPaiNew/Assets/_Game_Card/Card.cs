using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;

public class Card : MonoBehaviour
{
    public static readonly string[] listText = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    public CardData cardData;
    public Vector2 coordinate;
    public bool canTouch = false;
    public bool isSelected = false;
    public bool isHighlight = false;
    public Image contentImg;
    public Image highlightImg;
    public Image headerImg;
    public Text headerWinTxt;
    public Text headerLoseTxt;
    
    public int userId;

    [HideInInspector]
    public int orderOnCanvas;

    Animator anim;
    Canvas canvas;
    public bool isUp;
    public bool isCommunityCard;
    float offsetY;
    float tweenTime = 0.25f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        canvas = GetComponent<Canvas>();
    }
    private void OnEnable()
    {
    }
    public void TouchHandle()
    {
        SetSelected(!isSelected);
    }
    public void SetSelected(bool selected)
    {
        if (!canTouch)
            return;
        isSelected = selected;
        Vector3 pos = transform.position;
        offsetY = (selected ? 0.2f : 0);
        pos.y = IGUIM.instance.cardOnHandTransform.position.y + offsetY;
        transform.position = pos;
    }
    public void SetHighLight(bool highlight)
    {
        isHighlight = highlight;
        highlightImg.gameObject.SetActive(highlight);
    }
    public void SetFlip(bool up, float speed = 4)
    {
        isUp = up;
        anim.speed = speed;
        anim.SetBool("isUp", up);
    }
    public void Reset()
    {
        if(highlightImg != null)
            highlightImg.gameObject.SetActive(false);
        if (headerImg != null)
            SetHeader("", false);
        isSelected = false;
        isUp = false;
        SetSortingOrder(0);
        canTouch = false;
        isHighlight = false;
        contentImg.color = Color.white;
    }
    public void SetData(CardData cardData)
    {
        gameObject.transform.DOKill();
        Reset();
        this.cardData = cardData;
        if(cardData.card == 0)
        {
            UILogView.Log("Card/SetCard cardData is 0", false);
            return;
        }
        
        contentImg.sprite = ImageSheet.Instance.resourcesDics["card_" + cardData.ToString()];
    }
    public void SetCoord(float x, float y)
    {
        if (coordinate.x == x && coordinate.y == y)
            return;
        coordinate = new Vector2(x, y);
    }
    public void SetSortingOrder(int order, string layer = "")
    {
        if (canvas == null)
            return;
        if(layer != "")
            canvas.sortingLayerName = layer;
        canvas.sortingOrder = order;
        orderOnCanvas = order;
    }
    public void SetHeader(string str, bool win)
    {
        if (!string.IsNullOrEmpty(str))
        {
            headerImg.gameObject.SetActive(true);
            headerWinTxt.text = win ? str : "" ;
            headerLoseTxt.text = !win ? str : "";
        }
        else
        {
            headerImg.gameObject.SetActive(false);
        }
    }
    public void SetDisable()
    {
        contentImg.color = new Color(0.5f, 0.5f, 0.5f, 1);
    }

    public Tweener DoAnimate(float time, float delay, Vector3 pos, float rotate = 0, float scale = 0, Space space = Space.World)
    {
        Tweener tweener;
        if(space == Space.Self)
        {
            tweener = transform.DOLocalMove(pos, time).SetDelay(delay);
            transform.DOLocalRotate(Vector3.forward * rotate, time, RotateMode.FastBeyond360).SetDelay(delay);
        }
        else
        {
            tweener = transform.DOMove(pos, time).SetDelay(delay);
            transform.DORotate(Vector3.forward * rotate, time, RotateMode.FastBeyond360).SetDelay(delay);
        }

        transform.DOScale(scale, time).SetDelay(delay);
        return tweener;
    }

    public void SetCard(CardData cardData, Transform startPos = null, float delay = 0, System.Action<Card> onComplete = null)
    {
        gameObject.transform.DOKill();
        Reset();
        this.cardData = cardData;

        if (startPos != null)
        {
            transform.DOComplete();
            transform.DOMove(startPos.position, tweenTime).From().SetDelay(delay).OnStart(() =>
            {
                gameObject.SetActive(true);
            }).OnComplete(() =>
            {
                if (onComplete != null)
                    onComplete(this);
            });
        }
        else
        {
            gameObject.SetActive(true);
            if (onComplete != null)
                onComplete(this);
        }
    }

    public void UnActiveCard()
    {
        gameObject.SetActive(false);
    }
}
