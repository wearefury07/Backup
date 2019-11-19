using UnityEngine;
using UnityEngine.UI;

public class TopUpCardItemView : MonoBehaviour
{
    public Text Label_1;
    public Text Label_2;

    public bool FillData(ValueTopup data)
    {
        try
        {
            Label_1.text = LongConverter.ToFull(long.Parse(data.money)) + "VNĐ";
            Label_2.text = LongConverter.ToFull(long.Parse(data.gold));
        }
        catch (System.Exception ex)
        {
            Debug.LogError("TopUpCardItemView FillData: " + ex.Message);
            return false;
        }
        return true;
    }
}
