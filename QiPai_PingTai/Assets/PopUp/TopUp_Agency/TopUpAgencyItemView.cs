using UnityEngine;
using UnityEngine.UI;

public class TopUpAgencyItemView : MonoBehaviour
{
    public Text Stt;
    public Text Name;
    public Text Phone;
    public Text Location;

    private UserData data;

    public bool FillData(UserData _data, int numb)
    {
        try
        {
            data = _data;
            if (Stt != null)
                Stt.text = numb.ToString();
            if (Name != null)
                Name.text = data.displayName;
            if (Phone != null)
                Phone.text = data.mobile;
            if (Location != null)
                Location.text = data.passport;
        }
        catch (System.Exception ex)
        {
            UILogView.Log("TopUpAgencyItemView: " + ex.Message, true);
            return false;
        }
        return true;
    }

    public void Call()
    {
        UIManager.Call(data.mobile);
    }

    public void FacebookPage()
    {
        //OGUIM.Toast.ShowNotification("FacebookPage to " + data.fb);
        if (data != null && !string.IsNullOrEmpty(data.faceBookId))
        {
            UIManager.OpenURL(data.faceBookId);
        }
    }

    public void Transfer()
    {
        //OGUIM.Toast.ShowNotification("Show Transfer " + data.id);
        OGUIM.instance.popupTransfer.Show(data, false);
    }
}
