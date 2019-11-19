using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextEffext : MonoBehaviour {

    public float totalTimeAnimation;
    public Text contentTxt;
    public Image backgroundImg;
	void Awake () {
    }
    void OnEnable()
    {
        contentTxt.SetAlpha(0);
        backgroundImg.SetAlpha(0);
    }
    private void OnDisable()
    {
        contentTxt.DOKill();
        backgroundImg.DOKill();
    }
    private void OnDestroy()
    {
        contentTxt.DOKill();
        backgroundImg.DOKill();
    }

    public void SetData(string str, float delay = 0, float time = 0)
    {
        contentTxt.text = str;
        if (time > 0)
            totalTimeAnimation = time;
        contentTxt.DOFade(1, totalTimeAnimation * 0.3f);
        backgroundImg.DOFade(1, totalTimeAnimation * 0.3f);

        contentTxt.DOFade(0, totalTimeAnimation * 0.3f).SetDelay(totalTimeAnimation * 0.7f);
        backgroundImg.DOFade(0, totalTimeAnimation * 0.3f).SetDelay(totalTimeAnimation * 0.7f).OnComplete(
            () =>
            {
                gameObject.Recycle();
            }
        );
    }
    public void SetData(string str, Color color, float delay = 0, float time = 0)
    {
        contentTxt.text = str;
        if (time > 0)
            totalTimeAnimation = time;
        contentTxt.DOFade(1, totalTimeAnimation * 0.3f);
        backgroundImg.DOFade(1, totalTimeAnimation * 0.3f);
        backgroundImg.SetColor(color);

        contentTxt.DOFade(0, totalTimeAnimation * 0.3f).SetDelay(totalTimeAnimation * 0.7f);
        backgroundImg.DOFade(0, totalTimeAnimation * 0.3f).SetDelay(totalTimeAnimation * 0.7f).OnComplete(
            () =>
            {
                gameObject.Recycle();
            }
        );
    }
}
