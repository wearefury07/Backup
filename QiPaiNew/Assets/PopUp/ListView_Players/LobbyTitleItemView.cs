using System.Linq;
using UnityEngine;

public class LobbyTitleItemView : MonoBehaviour
{
    public UIToggle uiToggle;
    public Lobby lobbyData;
    public PlayersInLeaderBoard leaderBoard;

    public bool FillData(Lobby _lobbyData)
    {
        lobbyData = _lobbyData;

        Debug.Log("LobbyTitleItemView: " + lobbyData.shotname + "-" + lobbyData.desc);
        if (!string.IsNullOrEmpty(lobbyData.shotname))
        {
            uiToggle.UpdateTextContent(lobbyData.shotname.ToUpper());
            return true;
        }
        return false;
    }

    //public void GetLeaderBoard()
    //{
    //    var checkLobbyData = LobbyViewListView.listData.FirstOrDefault(x => x.id == lobbyData.id);
    //    if (checkLobbyData != null)
    //        lobbyData = checkLobbyData;

    //    leaderBoard.subTitle.text = "Đang tải danh sách cao thủ...";

    //    if (OGUIM.instance.lobbyViewInLeader != null)
    //        OGUIM.instance.lobbyViewInLeader.FillData(lobbyData);

    //    OGUIM.Toast.ShowLoading("");
    //    leaderBoard.Get_Data(lobbyData);
    //}
}
