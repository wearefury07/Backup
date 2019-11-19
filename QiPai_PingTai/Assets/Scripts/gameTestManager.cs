using UnityEngine;
using System.Collections;
using DG.Tweening;

public class gameTestManager : MonoBehaviour {
    public GameObject coinEfxPrefab, cardPrefab;
	// Use this for initialization
	void Start () {
        DOTween.Init();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            var pos = Vector3.zero;
            for (int i = 0; i < 40; i++)
            {
                var go = Instantiate(coinEfxPrefab) as GameObject;
                go.transform.SetParent(transform,false);
                go.transform.position = pos;
                go.GetComponent<ChipEffect>().Run(new Vector3(7,5,0));
            }
        }
        if (Input.GetButtonDown("Jump"))
        {
            var go = Instantiate(cardPrefab, new Vector3(Random.Range(-2f,2), Random.Range(-2f, 2),0), Quaternion.identity) as GameObject;
            go.transform.SetParent(transform, false);
            var card = go.GetComponent<Card>();
            card.SetData(new CardData(Random.Range(1, 14), Random.Range(1, 5)));
        }
	}
}
