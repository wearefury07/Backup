using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[RequireComponent(typeof(Collider2D))]
public class ObjectDraggingDetech : MonoBehaviour {
    public float maxY;
    float maxX;
    Vector3 mouseBegin, originPos;
    bool isTouch = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(point, Vector2.zero, 1000);
            if (hits.Any())
            {
                var col = hits.Where(x => x.transform == transform).FirstOrDefault();
                if (col.transform != null && col.transform.gameObject != null && col.transform.gameObject.name == gameObject.name)
                {
                    mouseBegin = point;
                    originPos = transform.position;
                    isTouch = true;
                    if (maxX == 0)
                        maxX = originPos.x;
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (isTouch)
            {
                var newPos = originPos + point - mouseBegin;
                transform.SetPosition(Mathf.Clamp(newPos.x, -maxX + 2, maxX), Mathf.Clamp(newPos.y, -maxY, maxY));
                transform.SetZ(0, Space.Self);
            }
        }

        if (Input.GetMouseButtonUp(0))
            isTouch = false;
	}
}
