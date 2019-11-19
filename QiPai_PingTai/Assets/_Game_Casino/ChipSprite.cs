using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ChipSprite : MonoBehaviour {
    public static readonly string chipTweenId = "chipTweenId";
    RectTransform rect;
    public int userId;
    public int pot;

    public void SetPosition(Vector3 position, bool isAnimate, float delay = 0, bool destroyOnComplete = false)
    {
        rect = GetComponent<RectTransform>();

        var time = 0.5f;
        rect.DOKill();
        if (isAnimate)
        {
            rect.DOMove(position, time).SetDelay(delay).OnComplete(() =>
            {
                if (destroyOnComplete)
                    gameObject.Recycle();
            }).SetEase(Ease.Linear).SetId(chipTweenId);
        }
        else
        {
            transform.position = position;
        }
    }
}
