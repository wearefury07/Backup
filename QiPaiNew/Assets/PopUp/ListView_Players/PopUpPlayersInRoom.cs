using UnityEngine;

public class PopUpPlayersInRoom : MonoBehaviour
{
    public PlayersInRoomListView playersList;
    public UIAnimation anim;

    public void Show()
    {
        anim.Show();
    }

    public void Hide()
    {
        anim.Hide();
    }

    public void FillData()
    {
        if (playersList.gameObject.activeSelf)
            playersList.Get_Data();
    }
}
