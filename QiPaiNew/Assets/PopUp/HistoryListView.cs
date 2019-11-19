using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(UIListView))]
[DisallowMultipleComponent]
public class HistoryListView : MonoBehaviour
{
    public LobbyId lobbyId;
    public HistoryType historyType;
    public UIListView uiListView;
    public int maxItems = 10;
    private List<HistoryData> listData = new List<HistoryData>();
    private List<HistoryItemView> listView = new List<HistoryItemView>();

    public List<Text> labels;
    public List<string> Label_MINI_TAIXIU;
    public List<string> Label_MINI_SLOT;
    public List<string> Label_MINI_POKER;
    public List<string> Label_MINI_SPIN;
    public List<string> Label_MINI_CAOTHAP;

    private void Awake()
    {
        if (uiListView == null)
            uiListView = transform.GetComponent<UIListView>();
    }

    private void OnEnable()
    {
        if (WarpClient.wc != null)
            AddListener();
    }

    private void OnDisable()
    {
        if (WarpClient.wc != null)
            RemoveListener();
    }

    public void SetType(int _type)
    {
        historyType = (HistoryType)_type;
    }

    public void SetData(LobbyId _id, HistoryType _type)
    {
        lobbyId = _id;
        historyType = _type;
    }

    public void AddListener()
    {
        if (historyType == HistoryType.USER)
            WarpClient.wc.OnUserHistory += Wc_OnUserHistory;
        else if (historyType == HistoryType.JACKPOT)
            WarpClient.wc.OnJackpotHistory += Wc_OnJackpotHistory;
    }

    public void RemoveListener()
    {
        if (historyType == HistoryType.USER)
            WarpClient.wc.OnUserHistory -= Wc_OnUserHistory;
        else if (historyType == HistoryType.JACKPOT)
            WarpClient.wc.OnJackpotHistory -= Wc_OnJackpotHistory;
    }

    private void Wc_OnUserHistory(List<HistoryBase> data, LobbyId _loby)
    {
        if (data != null && data.Any() && _loby == lobbyId)
        {
            int count = 0;
            var time = DateTime.Now;
            foreach (var i in data)
            {
                try
                {
                    count++;
                    var listString = new HistoryData { data = new List<string>() };

                    if (lobbyId == LobbyId.SLOT || lobbyId == LobbyId.XENG_HOAQUA)
                    {
                        //{ "Mã", "Thời gian", "Tài", "Xỉu", "Thắng", "Mô tả" };

                        listString.data.Add(i.id.ToString());
                        if (DateTime.TryParse(i.time, out time))
                            listString.data.Add(DateTimeConverter.ToRelativeTime(time));
                        else
                            listString.data.Add(i.time);
                        listString.data.Add(LongConverter.ToFull(i.bet));
                        listString.data.Add(LongConverter.ToFull(i.sub));
                        listString.data.Add(LongConverter.ToFull(i.add));

                        if (i.code == 0)
                            i.desc = "Thua";
                        else if (i.code == 1)
                            i.desc = "Đập hũ";
                        else if (i.code == 2)
                            i.desc = "Miễn phí";
                        else if (i.code == 3)
                            i.desc = "Thắng";
                        listString.data.Add(i.desc);

                        listData.Add(listString);
                    }
                    else if (lobbyId == LobbyId.MINI_POKER)
                    {
                        // { "Mã", "Thời gian", " ", "Mức cược", "Thắng", "Mô tả" };

                        listString.data.Add(i.id.ToString());
                        if (DateTime.TryParse(i.time, out time))
                            listString.data.Add(DateTimeConverter.ToRelativeTime(time));
                        else
                            listString.data.Add(i.time);
                        listString.data.Add(" ");
                        listString.data.Add(LongConverter.ToFull(i.bet));
                        listString.data.Add(LongConverter.ToFull(i.add));
                        if (i.total == 0)
                            i.desc = "Hòa";
                        else if (i.total > 0)
                            i.desc = "Thắng";
                        else
                            i.desc = "Thua";
                        listString.data.Add(i.desc);

                        listData.Add(listString);
                    }
                    else if (lobbyId == LobbyId.MINI_SPIN)
                    {
                        // { "Mã", "Thời gian", " ", "Mức cược", GameBase.moneyGold.name, GameBase.moneyKoin.name };

                        listString.data.Add(i.id.ToString());
                        if (DateTime.TryParse(i.time, out time))
                            listString.data.Add(DateTimeConverter.ToRelativeTime(time));
                        else
                            listString.data.Add(i.time);
                        listString.data.Add(" ");
                        listString.data.Add(LongConverter.ToFull(i.bet));
                        listString.data.Add(LongConverter.ToFull(i.gold));
                        listString.data.Add(LongConverter.ToFull(i.koin));

                        listData.Add(listString);
                    }
					else if (lobbyId == LobbyId.MINI_TAIXIU)
					{
						// { "Mã", "Thời gian", "Tài", "Xỉu", "Nhận", "Mô tả" };
						listString.data.Add(i.id.ToString());
						if (DateTime.TryParse(i.time, out time))
							listString.data.Add(DateTimeConverter.ToRelativeTime(time));
						else
							listString.data.Add(i.time);
						
						listString.data.Add(LongConverter.ToFull(i.betMax));
						listString.data.Add(LongConverter.ToFull(i.betMin));
						listString.data.Add(LongConverter.ToFull(i.win));

						if (i.payback > 0) 
							i.desc = "Hoàn trả " + LongConverter.ToFull(i.win);
						else
						{
							if (i.win == 0)
								i.desc = "Hòa";
							else if (i.win > 0)
								i.desc = "Thắng";
							else
								i.desc = "Thua";
						}
						listString.data.Add(i.desc);
						
						listData.Add(listString);
                    }
                    else if (lobbyId == LobbyId.MINI_CAOTHAP)
                    {
                        // { "Mã", "Thời gian", "Mức cược", "Bước - Kết Quả", "Cửa đặt", "Nhận" };
                        listString.data.Add(i.id.ToString());
                        if (DateTime.TryParse(i.time, out time))
                            listString.data.Add(DateTimeConverter.ToRelativeTime(time));
                        else
                            listString.data.Add(i.time);

                        listString.data.Add(LongConverter.ToFull(i.bet));
                        listString.data.Add(i.turn + " - " + FaceConverter.ToString(i.result));

                        var isHighText = "";
                        if (i.isHigh == 0)
                            isHighText = "Dưới";
                        else if (i.isHigh == 1)
                            isHighText = "Trên";

                        listString.data.Add(isHighText);
                        listString.data.Add(LongConverter.ToFull(i.win));
                     
                        listData.Add(listString);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.Log("Wc_OnSlotUserHistory: " + ex.Message);
                }
            }

            FillData();
        }
        else
            OGUIM.Toast.ShowNotification("Hiện tại không có lịch sử chơi");
    }

    private void Wc_OnJackpotHistory(List<HistoryBase> data, LobbyId _loby)
    {
        if (data != null && data.Any() && lobbyId == _loby)
        {
            int count = 0;
            foreach (var i in data)
            {
                try
                {
                    count++;
                    var listString = new HistoryData { data = new List<string>() };

                    listString.data.Add(count.ToString());
                    listString.data.Add(i.displayName);
                    listString.data.Add(i.time);
                    listString.data.Add(LongConverter.ToFull(i.bet));
                    listString.data.Add(LongConverter.ToFull(i.win));

                    listData.Add(listString);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("Wc_OnSlotHuHistory: " + ex.Message);
                }
            }

            FillData();
        }
        else
            OGUIM.Toast.ShowNotification("Hiện tại không có lịch sử đập hũ");
    }

    public void Get_Data(bool reload)
    {
        if (reload)
        {
            uiListView.ClearList();
            listData = new List<HistoryData>();
            listView = new List<HistoryItemView>();
        }



        if (MiniGames.instance != null && MiniGames.instance.currentGame != LobbyId.NONE)
        {
            lobbyId = MiniGames.instance.currentGame;
        }
        else if (OGUIM.currentLobby != null && OGUIM.currentLobby.id != (int)LobbyId.NONE)
        {
            lobbyId = (LobbyId)OGUIM.currentLobby.id;
        }

        if (historyType == HistoryType.USER)
        {
            UpdateLabels();
        }

        if (!listView.Any() || reload)
		{
			OGUIM.Toast.ShowLoading("");
            //OGUIM.Toast.ShowLoading("Đang tải dữ liệu, vui lòng chờ giây lát...");
            if (historyType == HistoryType.JACKPOT)
                GetJackpotHistory();
            else
                GetUserHistory();
        }
    }

    public void FillData()
    {
        if (listData != null && listData.Any())
        {
            int count = 0;
            foreach (var i in listData)
            {
                try
                {
                    var ui = uiListView.GetUIView<HistoryItemView>(uiListView.GetDetailView());
                    if (count % 2 != 0)
                        ui.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

                    if (ui.FillData(i.data))
                    {
                        listView.Add(ui);
                        count++;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("FillData: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
        OGUIM.Toast.Hide();
    }

    private void UpdateLabels()
    {
		Label_MINI_TAIXIU = new List<string> { "Mã", "Thời gian", "Tài", "Xỉu", "Nhận", "Mô tả" };
        Label_MINI_SLOT = new List<string> { "Mã", "Thời gian", "Mức cược", "Trừ", "Cộng", "Mô tả" };
        Label_MINI_POKER = new List<string> { "Mã", "Thời gian", " ", "Mức cược", "Thắng", "Mô tả" };
        Label_MINI_SPIN = new List<string> { "Mã", "Thời gian", " ", "Mức cược", GameBase.moneyGold.name, GameBase.moneyKoin.name };
        Label_MINI_CAOTHAP = new List<string> { "Mã", "Thời gian", "Mức cược", "Bước - Kết quả", "Cửa đặt", "Nhận" };

        if (lobbyId == LobbyId.SLOT || lobbyId == LobbyId.XENG_HOAQUA)
        {
            for (int i = 0; i < labels.Count; i++)
                labels[i].text = Label_MINI_SLOT[i];
        }
        else if (lobbyId == LobbyId.MINI_TAIXIU)
        {
            for (int i = 0; i < labels.Count; i++)
                labels[i].text = Label_MINI_TAIXIU[i];
        }
        else if (lobbyId == LobbyId.MINI_POKER)
        {
            for (int i = 0; i < labels.Count; i++)
                labels[i].text = Label_MINI_POKER[i];
        }
        else if (lobbyId == LobbyId.MINI_SPIN)
        {
            for (int i = 0; i < labels.Count; i++)
                labels[i].text = Label_MINI_SPIN[i];
        }
        else if (lobbyId == LobbyId.MINI_CAOTHAP)
        {
            for (int i = 0; i < labels.Count; i++)
                labels[i].text = Label_MINI_CAOTHAP[i];
        }
    }

    public void GetJackpotHistory()
    {
        Debug.Log("MINI_GetJackpotHis " + lobbyId);
        if (lobbyId != LobbyId.SLOT)
        {
            BuildWarpHelper.MINI_GetJackpotHis((int)lobbyId, () =>
            {
                UILogView.Log("MINI_GetJackpotHis " + lobbyId + " is time out!");
            });
        }
        else
        {
            BuildWarpHelper.SLOT_GetSlotHistory(() =>
            {
                UILogView.Log("MINI_GetJackpotHis " + lobbyId + " is time out!");
            });
        }
    }

    public void GetUserHistory()
    {
        Debug.Log("GetUserHistory " + lobbyId);
        if (lobbyId != LobbyId.SLOT && lobbyId != LobbyId.XENG_HOAQUA)
        {
            BuildWarpHelper.MINI_GetUserHis((int)lobbyId, () =>
            {
                UILogView.Log("MINI_GetUserHis " + (int)lobbyId + " is time out!");
            });
        }
        else
        {
            BuildWarpHelper.SLOT_GetUserHistory(() =>
            {
                UILogView.Log("SLOT_XENG_GetUserHistory " + lobbyId + " is time out!");
            });
        }
    }

    public void GetPayloadHistory()
    {
       
    }
}
