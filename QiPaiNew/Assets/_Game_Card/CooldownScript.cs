using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class CooldownScript : MonoBehaviour {
    public Image leftImage;

	// Use this for initialization
	void Start () {
        //rightImage.transform.DORotate(Vector3.forward * -180, 15);
        //leftImage.transform.DORotate(Vector3.forward * -180, 15).SetDelay(15);
        DOVirtual.Float(1, 0, 30, (x) =>
        {
            leftImage.fillAmount = x;
        });
    }

}
