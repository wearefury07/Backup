using UnityEngine;

public class Select_Line : MonoBehaviour
{
    public GameObject LineDots;
    private Animator animator;

    void Start()
    {

    }
    
    void Update()
    {

    }

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void ShowLine(float speed = 1)
    {
        gameObject.SetActive(true);
        if (animator != null)
        {
            animator.speed = speed;
            animator.SetTrigger("Show");
        }
    }

    public void ActiveLineDots()
    {
        LineDots.SetActive(true);
    }

    public void DeActiveLineDots()
    {
        LineDots.SetActive(false);
    }
}
