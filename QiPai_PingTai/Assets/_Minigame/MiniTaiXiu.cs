using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public enum TaiXiuNotifyType
{
    UPDATE_INFO = 1,
    START_MATCH = 2,
    END_MATCH = 3,
    UNSUBSCRIBE = 6,
    PAYBACK = 7,
}

public class MiniTaiXiu : MonoBehaviour
{
    public bool isShow;
    public bool isSub;
    public bool isAutoSub;
    private bool autoLeave;
    public UIAnimation anim;
    public Transform gameTransform;
    public BatDiaTaiXiu batdia;

    public NumberAddEffect userTaiLabel;
    public NumberAddEffect allBetTaiLabel;
    public NumberAddEffect myBetTaiLabel;

    public NumberAddEffect userXiuLabel;
    public NumberAddEffect allBetXiuLabel;
    public NumberAddEffect myBetXiuLabel;

    public CanvasGroup TaiGroup;
    public CanvasGroup XiuGroup;
    public GameObject taiWinEfx, xiuWinEfx;

    public GameObject ThanhVi;
    public List<Sprite> vi;
    public Image ImageVi;

    private Image[] listVi;
    private int[] listChipValue;
    private int chipType = 1;
    private int state;
    private int currentBet;

    private int allBetXiu;
    private int allBetTai;
    private int myBetXiu;
    private int myBetTai;
    private int userTai;
    private int userXiu;

    private List<HistoryBase> history;
    private HistoryBase result;

    void Awake()
    {
        currentBet = 500;
    }

    void Start()
    {
        anim = GetComponent<UIAnimation>();
    }

    private void OnEnable()
    {
        TaiGroup.alpha = 1f;
        XiuGroup.alpha = 1f;

        taiWinEfx.SetActive(false);
        xiuWinEfx.SetActive(false);
    }

    public void Show()
    {
        //OGUIM.Toast.ShowNotification("Trò chơi đang được hoàn thiện...!");
        anim.Show(() => isShow = true);
    }

    public void Hide()
    {
        anim.Hide(() => isShow = false);
    }

    public void Close_Click()
    {
        if (isShow)
            Hide();
		MiniGames.instance.currentGame = LobbyId.NONE;
    }

    public void OnTaiXiuNotifyDone(Mini_RootData data)
    {
        int notifyType = data.type;
        bool changeState = false;

        if (chipType == data.room.chipType)
        {
            if (state != data.room.state)
            {
                batdia.SetTime(data.room.timeCountDown);
                MiniGames.instance.SetTime(data.room.timeCountDown);
            }
            if (state != data.room.state)
                changeState = true;

            state = data.room.state;
        }

        if (notifyType == (int)TaiXiuNotifyType.UPDATE_INFO)
        {
            allBetXiu = data.room.minBet;
            allBetTai = data.room.maxBet;

            userTai = data.room.maxCount;
            userXiu = data.room.minCount;

            ChangeText();
            if (state == 1 && changeState == true)
            {
                //Debug.Log ("CHANGE STATE");
                if (isShow)
                    OGUIM.Toast.ShowNotification("Ngừng nhận cược và cân cửa");
            }
        }
        else if (notifyType == (int)TaiXiuNotifyType.START_MATCH)
        {
            TaiGroup.alpha = 1f;
            XiuGroup.alpha = 1f;

            allBetTai = 0;
            allBetXiu = 0;

            ChangeText();

            batdia.SetTime(0);
            MiniGames.instance.SetTime(data.room.timeCountDown);
            batdia.StartShake();
            DOVirtual.DelayedCall(3f, () =>
            {
                batdia.StopShake();
                batdia.SetTime(data.room.timeCountDown - 3);
            });

            taiWinEfx.SetActive(false);
            xiuWinEfx.SetActive(false);
        }
        else if (notifyType == (int)TaiXiuNotifyType.END_MATCH)
        {
            myBetTai = 0;
            myBetXiu = 0;
            userTai = 0;
            userXiu = 0;

            while (history.Count > 19)
                history.RemoveAt(0);
            result = data.vi;
            history.Add(result);

            SetHistory();

            batdia.SetVis(data.vi.faces);
            batdia.Open();

            OGUIM.me.gold = data.chip;
            EndMatch(data.chipChange, data.vi.point);
        }
        else if (notifyType == (int)TaiXiuNotifyType.UNSUBSCRIBE)
        {
            if (isShow)
                Hide();
            MiniGames.instance.SetTime(0);
			MiniGames.instance.currentGame = LobbyId.NONE;
            isSub = false;
        }
        else if (data.type == (int)TaiXiuNotifyType.PAYBACK)
        {
            string _pot = "";
            if (data.pot == 0)
            {
                myBetXiu -= data.payback;
                _pot = " cửa XỈU";
            }
            else
            {
                myBetTai -= data.payback;
                _pot = " cửa TÀI";
            }
            ChangeText();

            if (isShow)
                OGUIM.Toast.ShowNotification("Hoàn trả " + LongConverter.ToFull(data.payback) + " " + GameBase.moneyGold.name + " " + _pot);


        }
    }

    public void OnSubDone(Mini_RootData data)
    {
        RoomInfo room = data.room;
		history = data.history;

        userTai = room.maxCount;
        userXiu = room.minCount;
        batdia.SetTime(room.timeCountDown);
        MiniGames.instance.SetTime(room.timeCountDown);
        state = room.state;
        result = history.LastOrDefault();
        SetHistory();
        ChangeText();
        batdia.StopShake();
        isSub = true;
    }

    public void OnAddBetDone(Mini_RootData data)
    {
        if (data.pot == 1)
        {
            myBetTai = data.total;
            allBetTai = data.totalPot;
        }
        else if (data.pot == 0)
        {
            myBetXiu = data.total;
            allBetXiu = data.totalPot;
        }
        ChangeText();
    }

    public void OnClearBetDone()
    {
        myBetTai = 0;
        myBetXiu = 0;
        ChangeText();
    }

    public void SetCurrentBet(int bet)
    {
        currentBet = bet;
    }

    public void AddBet(int pot)
    {
        taixiuRequest _params = new taixiuRequest();
        _params.type = (int)MINIGAME.ADD_BET;
        _params.bet = currentBet;
        _params.pot = pot;
        WarpRequest.TaiXiuRequest(_params);
    }

    public void ClearBet()
    {
        taixiuRequest _params = new taixiuRequest();
        _params.type = (int)MINIGAME.CLEAR_BET;
        WarpRequest.TaiXiuRequest(_params);
    }

    private void ChangeText()
    {
        allBetTaiLabel.FillData(allBetTai);
        myBetTaiLabel.FillData(myBetTai, "", 1, true);
        userTaiLabel.FillData(userTai);

        allBetXiuLabel.FillData(allBetXiu);
        myBetXiuLabel.FillData(myBetXiu, "", 1, true);
        userXiuLabel.FillData(userXiu);
    }

    private void SetHistory()
    {
        if (listVi != null && listVi.Length > 0)
        {
            foreach (Transform child in ThanhVi.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            listVi = null;
        }

        int numberOfVi = history.Count;

        listVi = new Image[numberOfVi];
        int indexOfCell = 0;

        foreach (var _value in history)
        {
            listVi[indexOfCell] = Instantiate(ImageVi);
            if (_value.point > 10)
                listVi[indexOfCell].sprite = vi[0];
            else
                listVi[indexOfCell].sprite = vi[1];

            listVi[indexOfCell].transform.SetParent(ThanhVi.transform, false);
            listVi[indexOfCell].SetAlpha(indexOfCell != numberOfVi - 1 ? 0.5f : 1);
            indexOfCell++;
        }
    }

    private void EndMatch(long winChips, int result)
    {
        var toastStrs = new List<string>();
        string potName;
        if (result > 10)
        {
            TaiGroup.alpha = 1f;
            XiuGroup.alpha = 0.5f;
            potName = "TÀI";
            taiWinEfx.SetActive(true);
            xiuWinEfx.SetActive(false);
        }
        else
        {
            TaiGroup.alpha = 0.5f;
            XiuGroup.alpha = 1f;
            potName = "XỈU";
            taiWinEfx.SetActive(false);
            xiuWinEfx.SetActive(true);
        }

        if (isShow)
        {
            toastStrs.Add(potName);
            toastStrs.Add(Ultility.CoinToString(winChips) + " " + GameBase.moneyGold.name);
        }
        var pos = Vector3.zero;
        for (int i = 0; i < toastStrs.Count; i++)
        {
			MiniGames.SpawnTextEfx(toastStrs[i], pos, winChips > 0);
            pos += Vector3.down;
        }

		if (winChips > 0)
			UIManager.PlaySound ("winchip");
        OGUIM.instance.meView.FillData(OGUIM.me);
    }
}