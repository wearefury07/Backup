using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

[DisallowMultipleComponent]
public class UIListView : MonoBehaviour
{
    public Text title;
    public int sizeTitle = 36;
    public ScrollRect scrollRect;
    public GridLayoutGroup content;
    public VerticalLayoutGroup verticalContent;
    public HorizontalLayoutGroup horizontalContent;
    public GameObject detailView;
    public bool hideDetailViewAtStart = true;
    public bool autoFade = true;
    public bool autoFixCenter = true;
    [Range(0, 10)]
    public int itemPerScreen = 4;
    [Range(0f, 1000f)]
    public float distanceToFadeMin = 50f;
    [Range(0f, 1000f)]
    public float distanceToFadeMax = 50f;

    private Vector2 itemSize;
    private float posMin = 0f;
    private float posMax = 0f;

    private float paddingLeft;
    private float paddingRight;
    private float paddingTop;
    private float paddingBottom;

    private string titleFormat = "<size={0}>{1}</size>" + "\n" + "<color=#ffffff99>{2}</color>";


    public List<GameObject> objectList = new List<GameObject>();

    public void Awake()
    {
        if (hideDetailViewAtStart)
            detailView.SetActive(false);

        if (content != null)
        {
            itemSize = content.cellSize;
            paddingTop = content.padding.top;
            paddingBottom = content.padding.bottom;
            paddingLeft = content.padding.left;
            paddingRight = content.padding.right;
        }
        else if (verticalContent != null)
        {
            itemSize = detailView.GetComponent<RectTransform>().sizeDelta;
            paddingTop = verticalContent.padding.top;
            paddingBottom = verticalContent.padding.bottom;
            paddingLeft = verticalContent.padding.left;
            paddingRight = verticalContent.padding.right;
        }
        else if (horizontalContent != null)
        {
            itemSize = detailView.GetComponent<RectTransform>().sizeDelta;
            paddingTop = horizontalContent.padding.top;
            paddingBottom = horizontalContent.padding.bottom;
            paddingLeft = horizontalContent.padding.left;
            paddingRight = horizontalContent.padding.right;
        }
    }

    //public void Update()
    //{
    //    if (autoFade)
    //        FadeItem();
    //}

    private void FixedUpdate()
    {
        if (autoFade)
            FadeItem();
    }

    public void ClearList()
    {
        foreach (var i in objectList)
            Destroy(i);
        objectList.Clear();
    }

    public void Remove(GameObject ojb)
    {
        var checkObj = objectList.FirstOrDefault(x => x == ojb);
        if (checkObj != null)
            Destroy(ojb);
    }

    public void FillTitle(string _sizeTitle, string _title, string _subtitle)
    {
        title.text = string.Format(titleFormat, _sizeTitle, _title, _subtitle);
    }

    private void OnEnable()
    {
        if (Camera.main != null && UIManager.uiScreenRect != null)
        {
            //Fix item center screen
            if (scrollRect.horizontal && content != null && autoFixCenter && itemPerScreen > 0)
            {
                content.padding.left = (int)((UIManager.uiScreenRect.size.x - itemPerScreen * content.cellSize.x) / 2);
                content.padding.right = content.padding.left;
            }
        }
    }

    public void FadeItem()
    {
        try
        {
            if (UIManager.uiScreenRect == null)
                UIManager.UpdateUIScreenRect();

            if (UIManager.uiScreenRect == null)
                return;

            for (int i = 0; i < objectList.Count; i++)
            {
                var obj = objectList[i];
                if (obj != null)
                {
                    CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                        canvasGroup = obj.gameObject.AddComponent<CanvasGroup>();


                    var alpha = 1f;

                    if (scrollRect.horizontal)
                    {
                        var currentPost = Camera.main.WorldToScreenPoint(obj.transform.position);
                        var scale = Camera.main.pixelWidth / UIManager.uiScreenRect.size.x;
                        posMin = (itemSize.x / 2 + paddingLeft) * scale + distanceToFadeMin;
                        posMax = (itemSize.x * itemPerScreen + paddingRight + posMin) * scale - distanceToFadeMax;

                        if (currentPost.x < posMin)
                        {
                            alpha = 1f - (posMin - currentPost.x) / itemSize.x;
                        }
                        else if (currentPost.x > posMax)
                        {
                            alpha = 1f - (currentPost.x - posMax) / itemSize.x;
                        }
                    }
                    else
                    {
                        var currentPost = Camera.main.WorldToScreenPoint(obj.transform.position);
                        var scale = Camera.main.pixelHeight / UIManager.uiScreenRect.size.y;
                        posMin = (itemSize.y / 2 + paddingBottom) * scale + distanceToFadeMin;
                        posMax = (itemSize.y * itemPerScreen + paddingTop + posMin) * scale - distanceToFadeMax;


                        if (currentPost.y < posMin)
                        {
                            alpha = 1f - (posMin - currentPost.y) / itemSize.y;
                        }
                        else if (currentPost.y > posMax)
                        {
                            alpha = 1f - (currentPost.y - posMax) / itemSize.y;
                        }
                    }

                    if (canvasGroup.alpha != alpha)
                    {
                        canvasGroup.alpha = alpha;

                        for (int ii = 0; ii < obj.transform.childCount; ii++)
                        {
                            if (alpha <= 0)
                            {
                                obj.transform.GetChild(ii).gameObject.SetActive(false);
                            }
                            else
                            {
                                if (obj.transform.GetChild(ii).GetComponent<ParticleSystem>() != null)
                                {
                                    if (UIManager.instance != null && UIManager.instance.particleToggle != null)
                                        obj.transform.GetChild(ii).gameObject.SetActive(UIManager.instance.particleToggle.isOn);
                                }
                                else
                                    obj.transform.GetChild(ii).gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            UILogView.Log("FadeItem: " + ex.Message + "\n" + ex.StackTrace);
        }
    }

    public void SnapTo(int index)
    {
        if (index < objectList.Count)
        {
            var obj = objectList[index];
            if (obj != null)
                SnapTo(obj);
        }
    }

    public void SnapTo(GameObject obj)
    {
        var target = obj.GetComponent<RectTransform>();
        Canvas.ForceUpdateCanvases();
        var rec = content.GetComponent<RectTransform>();
        rec.anchoredPosition = (Vector2)scrollRect.transform.InverseTransformPoint(rec.position) - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
    }

    public void SnapTo(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();
        var rec = content.GetComponent<RectTransform>();
        rec.anchoredPosition = (Vector2)scrollRect.transform.InverseTransformPoint(rec.position) - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
    }

    public GameObject GetDetailView(Func<bool> isValidItem = null)
    {
        if (isValidItem != null && !isValidItem())
            return null;
        var obj = Instantiate(detailView);
        obj.SetActive(true);
        if (content != null)
            obj.transform.SetParent(content.transform, false);
        else if (verticalContent != null)
            obj.transform.SetParent(verticalContent.transform, false);
        else if (horizontalContent != null)
            obj.transform.SetParent(horizontalContent.transform, false);
        objectList.Add(obj);
        return obj;
    }

    public T GetUIView<T>(GameObject obj) where T : Component
    {
        if (obj == null)
            return null;
        var comp = obj.GetComponent(typeof(T));
        return comp as T;
    }

    public void ScrollVerticalToTop()
    {
        ScrollTo(ScrollPosision.Top);
    }

    public void ScrollVerticalToBottom()
    {
        ScrollTo(ScrollPosision.Bottom);
    }

    public void ScrollVerticalToLeft()
    {
        ScrollTo(ScrollPosision.Left);
    }

    public void ScrollVerticalToRight()
    {
        ScrollTo(ScrollPosision.Right);
    }

    public void ScrollTo(ScrollPosision pos)
    {
        switch (pos)
        {
            case ScrollPosision.Left:
                scrollRect.horizontalNormalizedPosition = 0f;
                break;
            case ScrollPosision.Right:
                scrollRect.horizontalNormalizedPosition = 1f;
                break;
            case ScrollPosision.Center:
                scrollRect.verticalNormalizedPosition = 0.5f;
                break;
            case ScrollPosision.Top:
                scrollRect.verticalNormalizedPosition = 1f;
                break;
            case ScrollPosision.Bottom:
                scrollRect.verticalNormalizedPosition = 0f;
                break;
        }
    }

    public enum ScrollPosision
    {
        Left,
        Right,
        Center,
        Top,
        Bottom,
    }
}