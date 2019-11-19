using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[DisallowMultipleComponent]
public class NumberAddEffect : MonoBehaviour
{
    public FormatType type;
    public Text number;

    private Outline outline;
    private string subname;
    private long current;
    private long next;
    private long step;

    private void Start()
    {
        if (number == null)
            number = GetComponent<Text>();
        if (outline == null)
            outline = GetComponent<Outline>();
    }

	public void FillData(long data, string str = "", float speed = 1, bool instant = false)
    {
       
        next = data;
        subname = str;
		if (current == 0 || instant == true)
            current = data;
        step = (long)(Mathf.Max(Mathf.FloorToInt((next - current) * Time.deltaTime / 2), 1) * Mathf.Abs(speed));
    }


    void FixedUpdate()
    {
        if (number != null && number.gameObject.activeSelf)
        {
            if (current < next)
                current += step;
            else
                current = next;

            if (outline != null)
                outline.effectDistance = new Vector2(1f, -1f);

            if (type == FormatType.ToK)
                number.text = (LongConverter.ToK(current) + " " + subname).Trim();
            else if (type == FormatType.ToM)
                number.text = (LongConverter.ToM(current) + " " + subname).Trim();
            else
                number.text = (LongConverter.ToFull(current) + " " + subname).Trim();
        }
        else
        {
            if (outline != null)
                outline.effectDistance = new Vector2(0.5f, -0.5f);
        }
    }

    public enum FormatType
    {
        ToM,
        ToK,
        ToFull
    }
}
