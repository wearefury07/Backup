using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class BatDiaTaiXiu : MonoBehaviour
{

    public GameObject bat;
    public Image[] vis;
    public Sprite[] taixiuVisSprite;
    public Text txtCoolDown;
    public Image imgCoolDown;

    Animator batAnim;
    public Vector3 originPos;
    bool isOpen;
    long DOTweenId;

    private void Awake()
    {
        originPos = transform.localPosition;
    }

    void OnEnable()
    {
        batAnim = GetComponent<Animator>();
        isOpen = false;
        DOTweenId = System.DateTime.Now.Ticks;
        transform.localPosition = originPos;
    }

    public void SetTime(float time)
    {
        if (time <= 0)
        {
            txtCoolDown.gameObject.SetActive(false);
            imgCoolDown.gameObject.SetActive(false);
        }
        else
        {
            txtCoolDown.gameObject.SetActive(true);
            imgCoolDown.gameObject.SetActive(true);
            DOTween.Kill(DOTweenId);
            DOVirtual.Float(time, 0, time, (x) =>
                {
                    imgCoolDown.fillAmount = x / time;
                    txtCoolDown.text = x.ToString("0");
                }).SetEase(Ease.Linear).SetId(DOTweenId);
        }
    }
    public void StartShake()
    {
        transform.localPosition = originPos;
        transform.DOKill();
        transform.DOShakePosition(30, 30, 15, 0, true).SetEase(Ease.Linear).SetDelay(isOpen ? 1 : 0);
        isOpen = true;
        Close();
    }

    public void StopShake()
    {
        transform.DOKill();
        transform.DOLocalMove(originPos, 0.5f);
    }

    public void Open()
    {
        StopShake();
        if (!isOpen)
            batAnim.SetBool("isOpen", true);
        isOpen = true;
    }

    public void Close()
    {
        if (isOpen)
            batAnim.SetBool("isOpen", false);
        isOpen = false;
    }

    public void SetVis(List<int> visData)
    {
        for (int i = 0; i < vis.Length; i++)
        {
            int index = visData[i];
            vis[i].sprite = taixiuVisSprite[index - 1];
        }
    }
}
