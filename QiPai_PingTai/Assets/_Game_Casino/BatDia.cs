using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class BatDia : MonoBehaviour
{

    public GameObject bat;
    public Image[] vis;
    public Sprite[] baucuaVisSpite;
    public Sprite[] xocdiaVisSpite;
    public Text txtCoolDown;
    public Image imgCoolDown;

    Animator batAnim;
    public Vector3 originPos;
    bool isOpen;
    long DOTweenId;

    void OnEnable()
    {
        batAnim = GetComponent<Animator>();
        originPos = transform.position;
        isOpen = false;
        DOTweenId = System.DateTime.Now.Ticks;
        if (IGUIM_Casino.instance != null)
            vis[3].gameObject.SetActive(IGUIM_Casino.instance.casinoMode == CasinoMode.XOCDIA);
    }
    private void OnDisable()
    {
        DOTween.Kill(DOTweenId);
        transform.DOKill();
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
        if (originPos == Vector3.zero)
            originPos = transform.position;
        transform.position = originPos;
        transform.DOKill();
        transform.DOShakePosition(30, 30, 15, 0, true).SetEase(Ease.Linear).SetDelay(isOpen ? 1 : 0);
        Close();
    }
    public void StopShake()
    {
        if (originPos == Vector3.zero)
            originPos = transform.position;
        transform.DOKill();
        transform.DOMove(originPos, 0.5f);
    }
    public void Open()
    {
        StopShake();
        if (!isOpen && batAnim != null)
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
            if (i >= visData.Count)
                return;
            int index = visData[i];
            if (IGUIM_Casino.instance.casinoMode == CasinoMode.XOCDIA)
            {
                vis[i].sprite = xocdiaVisSpite[index];
            }
            else
            {
                vis[i].sprite = baucuaVisSpite[index - 1];
            }
        }
    }
}
