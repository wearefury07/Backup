using UnityEngine;
using System.Collections;

public class ChipEffect : MonoBehaviour {
    public Vector3 endPosition = Vector3.one * 5;
    public Vector3 beginPosition;

    bool isRunning;
    Vector3 velocity = Vector3.zero;
    float smoothTime = 0.5f;
    float smoothTime2 = 0.5f;
    //float rotateVelocity;
    float startTime;
    float delayTime;
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (isRunning)
        {
            if (Time.time - startTime < delayTime)
                transform.position = Vector3.SmoothDamp(transform.position, beginPosition, ref velocity, smoothTime);
            else
                transform.position = Vector3.SmoothDamp(transform.position, endPosition, ref velocity, smoothTime2, 20, Time.deltaTime * 8f);


            //var rotation = transform.rotation.eulerAngles;
            //rotation.z += rotateVelocity;
            //transform.rotation = Quaternion.Euler(rotation);

            var scale = transform.localScale;
            scale += Vector3.one * 0.005f;
            transform.localScale = Vector3.Min(scale, Vector3.one);


            if (Vector2.Distance(transform.position, endPosition) <= 0.1)
            {
                gameObject.SetActive(false);
                isRunning = false;
                gameObject.Recycle();
            }
        }
    }
    public void Run(Vector3 end, float offsetRange = 0)
    {
        transform.localScale = Random.Range(0.5f, 1) * Vector3.one;
        var animator = GetComponent<Animator>();
        animator.speed = Random.Range(0.5f, 2.0f);
        animator.SetBool("isRunning", true);
        startTime = Time.time;
        delayTime = Random.Range(0.3f, 1.0f);

        beginPosition = transform.position;
        endPosition = end;
        isRunning = true;
        //rotateVelocity = Random.Range(1, 8);
        smoothTime = Random.Range(4 - offsetRange / 2, 10 - offsetRange) * 0.1f;
        smoothTime2 = Random.Range(3 - offsetRange / 2, 10 - offsetRange) * 0.25f;


        var angle = Random.Range(0, 360);
        var vx = Mathf.Cos(angle * Mathf.Deg2Rad) * 15;
        var vy = Mathf.Sin(angle * Mathf.Deg2Rad) * 15;
        velocity = new Vector3(vx, vy, 0);
    }
}
