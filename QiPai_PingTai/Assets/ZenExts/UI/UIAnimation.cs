using DG.Tweening;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Events;
using System.Collections;

[DisallowMultipleComponent]
public class UIAnimation : MonoBehaviour
{
    public State state;
    public NavigationType navigation;
    public bool hideAtStart = true;
    public bool hideAtEnd = true;

    public bool animationAtStart;

    public string soundOnStart;

    public Type animationIn;
    public MovePosition startPos;

    [Range(0, 10)]
    public float timeAnimationIn = 0.3f;
    [Range(0, 10)]
    public float timeDelayIn = 0.0f;

    [SerializeField]
    public myFloatEvent eventOnStart;


    [SerializeField]
    public myFloatEvent eventOnShowCompleted;

    public Type animationOut;
    public MovePosition endPos;

    [Range(0, 10)]
    public float timeAnimationOut = 0.3f;
    [Range(0, 10)]
    public float timeDelayOut = 0.0f;

    [SerializeField]
    public myFloatEvent eventOnHideCompleted;

    [Serializable]
    public class myFloatEvent : UnityEvent { }

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        gameObject.SetActive(!hideAtStart);
    }

    void Start()
    {
        if (animationAtStart)
            Show();
    }

    public void Show()
    {
        Show(() => eventOnStart.Invoke(), () => eventOnShowCompleted.Invoke());
    }

    public void Show(TweenCallback actionOnStart = null, TweenCallback actionOnShowCompleted = null)
    {
        if (UIManager.instance == null)
        {
            UILogView.Log("UIManager.instance NULL", true);
            return;
        }

        if (!string.IsNullOrEmpty(soundOnStart))
            UIManager.PlaySound(soundOnStart);

        //rectTransform.SetPosition(GetPosition(MovePosition.BottomScreenEdge), Space.Self);
        gameObject.SetActive(true);

        if (navigation != NavigationType.Toast)
        {
            UIManager.instance.StopCoroutine("ShowAsync");
            UIManager.instance.StartCoroutine(ShowAsync(actionOnStart, actionOnShowCompleted));
        }
        else
            Play(animationIn, actionOnStart, actionOnShowCompleted, null);
    }

    IEnumerator ShowAsync(TweenCallback actionOnStart = null, TweenCallback actionOnShowCompleted = null)
    {
        while (state == State.IsAnimation)
        {
            yield return new WaitForSeconds(0.1f);
        }
        Play(animationIn, actionOnStart, actionOnShowCompleted, null);
    }

    public void Hide()
    {
        Hide(() => eventOnHideCompleted.Invoke());
    }

    public void Hide(TweenCallback actionOnHideCompleted = null)
    {
        if (navigation != NavigationType.Toast)
        {
            UIManager.instance.StopCoroutine("HideAsync");
            UIManager.instance.StartCoroutine(HideAsync(actionOnHideCompleted));
        }
        else
            Play(animationOut, null, null, actionOnHideCompleted);
    }

    IEnumerator HideAsync(TweenCallback actionOnHideCompleted = null)
    {
        while (state == State.IsAnimation)
        {
            yield return new WaitForSeconds(0.1f);
        }
        Play(animationOut, null, null, actionOnHideCompleted);
    }

    public void Play(Type type)
    {
        if (state == State.IsAnimation && navigation != NavigationType.Toast)
        {
            return;
        }
        switch (type)
        {
            case Type.SlideIn:
                SlideIn(startPos, () => eventOnStart.Invoke(), () => eventOnShowCompleted.Invoke());
                break;
            case Type.SlideOut:
                SlideOut(endPos, () => eventOnHideCompleted.Invoke());
                break;
            case Type.FadeIn:
                FadeIn(startPos, () => eventOnStart.Invoke(), () => eventOnShowCompleted.Invoke());
                break;
            case Type.FadeOut:
                FadeOut(endPos, () => eventOnHideCompleted.Invoke());
                break;
            default:
                Debug.LogError("[UIAnimaion] " + startPos + " not implemented!");
                break;
        }
    }

    public void Play(Type type, TweenCallback actionOnStart = null, TweenCallback actionOnShowCompleted = null, TweenCallback actionOnHideCompleted = null)
    {
        if (state == State.IsAnimation && navigation != NavigationType.Toast)
        {
            Debug.Log(state + " " + navigation);
            return;
        }

        switch (type)
        {
            case Type.SlideIn:
                SlideIn(startPos, actionOnStart, actionOnShowCompleted);
                break;
            case Type.SlideOut:
                SlideOut(endPos, actionOnHideCompleted);
                break;
            case Type.FadeIn:
                FadeIn(startPos, actionOnStart, actionOnShowCompleted);
                break;
            case Type.FadeOut:
                FadeOut(endPos, actionOnHideCompleted);
                break;
            default:
                Debug.LogError("[UIAnimaion] " + startPos + " not implemented!");
                break;
        }
    }

    private void SlideIn(MovePosition movePosition, TweenCallback actionOnStart, TweenCallback actionOnCompleted)
    {

        try
        {
            state = State.IsAnimation;

			if (navigation == NavigationType.Navigation && !OGUIM.listNavigation.ContainsKey(name))
                OGUIM.listNavigation.Add(name,this);
			else if (navigation == NavigationType.Menu && !OGUIM.listMenu.ContainsKey(name))
                OGUIM.listMenu.Add(name,this);
            else if (navigation == NavigationType.Popup && !OGUIM.listPopup.ContainsKey(name))
                OGUIM.listPopup.Add(name, this);
			else if (navigation == NavigationType.Toast && !OGUIM.listToast.ContainsKey(name))
                OGUIM.listToast.Add(name,this);

            SlideIn(GetRectTransform, GetPosition(movePosition), timeAnimationIn, timeDelayIn,
                () =>
                {
                    gameObject.SetActive(true);
                    if (actionOnStart != null)
                        actionOnStart.Invoke();
                },
                () =>
                {
                    state = State.IsShow;
                    if (actionOnCompleted != null)
                        actionOnCompleted.Invoke();
                });

        }
        catch (Exception ex)
        {
            Debug.Log("current: " + (gameObject == null) + "  " + name + " | " + ex.StackTrace + " " + ex.Message);
        }
    }

    private void SlideOut(MovePosition movePosition, TweenCallback actionOnCompleted)
    {
        state = State.IsAnimation;

        SlideOut(GetRectTransform, GetPosition(movePosition), timeAnimationOut, timeDelayOut,
            () =>
            {
                state = State.IsHide;

                gameObject.SetActive(!hideAtEnd);

                if (actionOnCompleted != null)
                    actionOnCompleted.Invoke();
            });
    }

    private void FadeIn(MovePosition movePosition, TweenCallback actionOnStart, TweenCallback actionOnCompleted)
    {
        state = State.IsAnimation;

		if (navigation == NavigationType.Navigation && !OGUIM.listNavigation.ContainsKey(name))
            OGUIM.listNavigation.Add(name,this);
        else if (navigation == NavigationType.Popup && !OGUIM.listPopup.ContainsKey(name))
            OGUIM.listPopup.Add(name, this);
		else if (navigation == NavigationType.Toast && !OGUIM.listToast.ContainsKey(name))
            OGUIM.listToast.Add(name,this);

        FaceIn(GetRectTransform, timeAnimationIn, timeDelayIn, 1f,
            () =>
            {
                gameObject.SetActive(true);
                if (actionOnStart != null)
                    actionOnStart.Invoke();
            },
            () =>
            {
                if (actionOnCompleted != null)
                    actionOnCompleted.Invoke();
                state = State.IsShow;
            });
    }

    private void FadeOut(MovePosition movePosition, TweenCallback actionOnCompleted)
    {
        state = State.IsAnimation;

        FaceOut(GetRectTransform, timeAnimationOut, timeDelayOut, 0f, () =>
        {
            if (actionOnCompleted != null)
                actionOnCompleted.Invoke();

            state = State.IsHide;

            gameObject.SetActive(!hideAtEnd);
        });
    }

    #region Slide Animation
    public void SlideIn(RectTransform rectTransform, Vector2 fromPosition, float timeAnimation, float timeDelay, TweenCallback actionOnStart = null, TweenCallback actionOnComplete = null, Ease ease = Ease.OutCubic)
    {
        rectTransform.DOComplete();
        rectTransform.anchoredPosition = fromPosition;
        rectTransform
            .DOAnchorPos(UIManager.startAnchoredPosition2D, timeAnimation, false)
                .SetDelay(timeDelay)
                .SetEase(ease)
                .SetUpdate(UpdateType.Normal, true)
                .OnStart(() =>
                {
                    if (actionOnStart != null)
                        actionOnStart.Invoke();
                })
                .OnComplete(() =>
                {
                    if (actionOnComplete != null)
                        actionOnComplete.Invoke();
                })
                .Play();
        ShowElements();
    }

    public void SlideOut(RectTransform rectTransform, Vector2 toPosition, float timeAnimation, float timeDelay, TweenCallback actionOnComplete = null, Ease ease = Ease.OutCubic)
    {
        rectTransform.DOComplete();
        rectTransform
            .DOAnchorPos(toPosition, timeAnimation, false)
                .SetDelay(timeDelay)
                .SetEase(ease)
                .SetUpdate(UpdateType.Normal, true)
                .OnComplete(() =>
                {
                    if (actionOnComplete != null)
                        actionOnComplete.Invoke();
                })
                .Play();
        HideElements();
    }
    #endregion

    #region Face Animation
    public void FaceIn(RectTransform rectTransform, float timeAnimation, float timeDelay, float end, TweenCallback actionOnStart = null, TweenCallback actionOnComplete = null, Ease ease = Ease.InCubic)
    {
        CanvasGroup canvasGroup = rectTransform.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = rectTransform.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.DOKill();
        canvasGroup.DOFade(end, timeAnimation)
            .SetDelay(timeDelay)
            .SetEase(ease)
            .OnStart(() =>
            {
                if (actionOnStart != null)
                    actionOnStart.Invoke();
            })
            .OnComplete(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 1f;
                if (actionOnComplete != null)
                    actionOnComplete.Invoke();
            })
            .Play();

        ShowElements();
    }

    public void FaceOut(RectTransform rectTransform, float timeAnimation, float timeDelay, float end, TweenCallback actionOnComplete = null, Ease ease = Ease.OutCubic)
    {
        CanvasGroup canvasGroup = rectTransform.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = rectTransform.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        canvasGroup.DOKill();
        canvasGroup.DOFade(end, timeAnimation)
            .SetDelay(timeDelay)
            .SetEase(ease)
            .OnComplete(() =>
            {
                if (actionOnComplete != null)
                    actionOnComplete.Invoke();
            })
            .Play();

        HideElements();
    }
    #endregion

    #region Helper
    public void ShowElements()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var a = transform.GetChild(i).gameObject.GetComponent<UIAnimation>();
            if (a != null)
                a.Play(a.animationIn);
        }
    }

    public void HideElements()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var a = transform.GetChild(i).gameObject.GetComponent<UIAnimation>();
            if (a != null)
                a.Play(a.animationOut);
        }
    }
    #endregion

    public Vector2 GetPosition(MovePosition movePosition)
    {
        RectTransform parent = rectTransform.parent.GetComponent<RectTransform>();  //We need to do this check because when we Instantiate a notification we need to use the uiContainer if the parent is null.
        if (parent == null)
        {
            parent = UIManager.GetUiContainer.GetComponent<RectTransform>();
        }

        Vector3 targetPosition = UIManager.startAnchoredPosition2D;

        Canvas tempCanvas = rectTransform.GetComponent<Canvas>();
        Canvas rootCanvas = null;

        if (tempCanvas == null) //this might be a button or an UIElement that does not have a Canvas component (this should not happen)
        {
            rootCanvas = rectTransform.root.GetComponentInChildren<Canvas>();
        }
        else
        {
            rootCanvas = tempCanvas.rootCanvas;
        }

        Rect rootCanvasRect = rootCanvas.GetComponent<RectTransform>().rect;
        float xOffset = rootCanvasRect.width / 2 + rectTransform.rect.width * rectTransform.pivot.x;
        float yOffset = rootCanvasRect.height / 2 + rectTransform.rect.height * rectTransform.pivot.y;

        var positionAdjustment = Vector3.zero;
        var positionFrom = Vector3.zero;

        switch (movePosition)
        {
            case MovePosition.ParentPosition:
                if (parent == null)
                    return targetPosition;

                targetPosition = new Vector2(parent.anchoredPosition.x + positionAdjustment.x,
                                             parent.anchoredPosition.y + positionAdjustment.y);
                break;

            case MovePosition.LocalPosition:
                if (parent == null)
                    return targetPosition;

                targetPosition = new Vector2(positionFrom.x + positionAdjustment.x,
                                             positionFrom.y + positionAdjustment.y);
                break;

            case MovePosition.TopScreenEdge:
                targetPosition = new Vector2(positionAdjustment.x + UIManager.startAnchoredPosition2D.x,
                                             positionAdjustment.y + yOffset);
                break;

            case MovePosition.RightScreenEdge:
                targetPosition = new Vector2(positionAdjustment.x + xOffset,
                                             positionAdjustment.y + UIManager.startAnchoredPosition2D.y);
                break;

            case MovePosition.BottomScreenEdge:
                targetPosition = new Vector2(positionAdjustment.x + UIManager.startAnchoredPosition2D.x,
                                             positionAdjustment.y - yOffset);
                break;

            case MovePosition.LeftScreenEdge:
                targetPosition = new Vector2(positionAdjustment.x - xOffset,
                                             positionAdjustment.y + UIManager.startAnchoredPosition2D.y);
                break;

            //case MovePosition.TopLeft:
            //    targetPosition = new Vector2(positionAdjustment.x - xOffset,
            //                                 positionAdjustment.y + yOffset);
            //    break;

            //case MovePosition.TopCenter:
            //    targetPosition = new Vector2(positionAdjustment.x,
            //                                 positionAdjustment.y + yOffset);
            //    break;

            //case MovePosition.TopRight:
            //    targetPosition = new Vector2(positionAdjustment.x + xOffset,
            //                                 positionAdjustment.y + yOffset);
            //    break;

            //case MovePosition.MiddleLeft:
            //    targetPosition = new Vector2(positionAdjustment.x - xOffset,
            //                                 positionAdjustment.y);
            //    break;

            //case MovePosition.MiddleCenter:
            //    targetPosition = new Vector2(positionAdjustment.x,
            //                                 positionAdjustment.y);
            //    break;

            //case MovePosition.MiddleRight:
            //    targetPosition = new Vector2(positionAdjustment.x + xOffset,
            //                                 positionAdjustment.y);
            //    break;

            //case MovePosition.BottomLeft:
            //    targetPosition = new Vector2(positionAdjustment.x - xOffset,
            //                                 positionAdjustment.y - yOffset);
            //    break;

            //case MovePosition.BottomCenter:
            //    targetPosition = new Vector2(positionAdjustment.x,
            //                                 positionAdjustment.y - yOffset);
            //    break;

            //case MovePosition.BottomRight:
            //    targetPosition = new Vector2(positionAdjustment.x + xOffset,
            //                                 positionAdjustment.y - yOffset);
            //break;

            default:
                Debug.LogWarning("[UIAnimaion] This should not happen! DoMoveIn in UIAnimator went to the default setting!");
                break;
        }

        //Debug.Log("[UIAnimaion] GetPosition: " + targetPosition);
        return targetPosition;
    }

    public RectTransform GetRectTransform
    {
        get
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            return rectTransform;
        }
    }

    #region Enums - MovePosition, Type, Stage
    public enum MovePosition
    {
        ParentPosition,
        LocalPosition,
        TopScreenEdge,
        RightScreenEdge,
        BottomScreenEdge,
        LeftScreenEdge,
        //TopLeft,
        //TopCenter,
        //TopRight,
        //MiddleLeft,
        //MiddleCenter,
        //MiddleRight,
        //BottomLeft,
        //BottomCenter,
        //BottomRight
    }

    public enum Type
    {
        None,
        SlideIn,
        SlideOut,
        FadeIn,
        FadeOut,
    }

    public enum State
    {
        IsHide,
        IsAnimation,
        IsShow
    }

    public enum NavigationType
    {
        None,
        Navigation,
        Menu,
        Popup,
        Toast,
    }
    #endregion
}
