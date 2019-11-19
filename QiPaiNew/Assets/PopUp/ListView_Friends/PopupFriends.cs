using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupFriends : MonoBehaviour {
    public UIAnimation anim;
    public FriendItemListView friendsList;
	
    public void Show()
    {
        anim.Show();
        friendsList.Get_Data(true);
    }

    public void Hide()
    {
        anim.Hide();
    }
}
