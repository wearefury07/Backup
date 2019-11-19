using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class ZenLoopingList : MonoBehaviour
{
    public GameObject resultPanel;
    public GameObject itemPrefab;
    public List<GameObject> allItems;

    [HideInInspector]
    public RectTransform rect;

    [HideInInspector]
    public Vector2 cellSize;
    public bool IsSpinning
    {
        get { return gameObject.activeSelf; }
    }

    public virtual void Start()
    {
        cellSize = GetComponent<GridLayoutGroup>().cellSize;
        rect = GetComponent<RectTransform>();
    }
    public virtual void Awake()
    {
        itemPrefab.CreatePool(80);
    }
    public virtual void UpdateUI(int roomId)
    {
    }
    public void Spin(float time, params int[] results)
    {
        gameObject.SetActive(true);
        resultPanel.gameObject.SetActive(false);
        if (results.Any())
            StartCoroutine(DoSpin(time, results));
    }

    public abstract IEnumerator DoSpin(float time, int[] results);
}