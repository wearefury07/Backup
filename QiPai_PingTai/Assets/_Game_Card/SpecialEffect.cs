using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SpecialEffect : MonoBehaviour {
    public Image image;
    public int duration = 2;
    public GameObject chipPrefab;
	// Use this for initialization
	void Start () {
	    
	}
    void SetAlpha(float alpha, bool skipSprite)
    {
        if (!skipSprite)
        {
            var imgColor = image.color;
            imgColor.a = alpha;
            image.color = imgColor;
        }
    }
    public void SetData(Sprite sprite, string content, Vector3 rotation)
    {
        if (sprite != null)
        {
            image.sprite = sprite;
            image.transform.rotation = Quaternion.Euler(rotation);
            image.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic);
        }
        else
            image.gameObject.SetActive(false);
        DOVirtual.Float(0.5f, 1, 0.5f, (alpha) =>
        {
            SetAlpha(alpha, sprite == null);
        });
        DOVirtual.Float(1, 0, 1, (alpha) => {
            SetAlpha(alpha, sprite == null);
        }).SetDelay(duration + 0.5f).OnComplete(() => Destroy(gameObject));
    }
}
