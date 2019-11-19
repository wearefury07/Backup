public enum LobbyId
{
    NONE = 0,
    TLMNDL = 7,
    TLMNDL_SOLO = 25,
    PHOM = 9,
    PHOM_SOLO = 26,
    SAM = 16,
    SAM_SOLO = 27,
    LIENG = 15,
    MAUBINH = 13,
    BACAY = 11,
    BACAY_GA = 17,


    XOCDIA = 28,
    BAUCUA = 29,
    XOCDIA_OLD = 19,
    BAUCUA_OLD = 20,
    //XOCDIA_NEW = 28,
    //BAUCUA_NEW = 29,

    SLOT = 18,
    XENG_HOAQUA = 31,

    MINI_TAIXIU = 96,
    MINI_SPIN = 97,
    MINI_POKER = 98,
    MINI_CAOTHAP = 110,

    TOPLEVEL = 999,
    TOPGOLD = 9999,
}

public enum GameId
{
    NONE = 0,
    CARD = 1,
    CASINO = 2,
    SLOT = 3
}

public enum PlayMode
{
    NORMAL = 0,
    QUICK = 1,
}

public enum LobbyMode
{
    CLASSIC = 0,
    MULTI = 1,
    SOLO = 2,
}

public enum MINIGAME_ID
{
    MINI_TAIXIU = 96,
    MINI_SPIN = 97,
    MINI_POKER = 98,
    MINI_CAOTHAP = 110,
    NONE = 0
}

public enum CHATTYPE
{
    CHAT = 1,
    EMOTION = 2,
}

#region GameNotifyTypesCode
public enum GameNotifyTypesCode_PHOM
{
    END_MATCH = 10,
    SUBMIT_TURN = 11,
    SET_TURN = 13,
    WHITE_WIN = 22,
    USER_GET_CARD = 29,
    USER_TAKE_CARD = 30,
    ATTACH_CARD_TO_SUITE = 31,
    USER_SUBMIT_SUITE = 32,
    CARDS_CHANGE = 33,
    CANCEL_KEY_TURN = 34,
    SHOW_HAND = 35,
    SET_SUBMIT_SUITE = 36,
    SET_KEY_TURN = 37,
    U_K = 48,
}
public enum GameNotifyTypesCode_SAM
{
    END_MATCH = 10,
    SUBMIT_TURN = 11,
    PASS_TURN = 12,
    SET_TURN = 13,
    WHITE_WIN = 22,
    REQUEST_TURN = 26,
    CANCEL_TURN = 27,
    RED_ALERT = 28,
}
public enum GameNotifyTypesCode_TLMN
{
    END_MATCH = 10,
    SUBMIT_TURN = 11,
    PASS_TURN = 12,
    SET_TURN = 13,
    WHITE_WIN = 22,
}
public enum GameNotifyTypesCode_TLMNDL
{
    END_MATCH = 10,
    SUBMIT_TURN = 11,
    PASS_TURN = 12,
    SET_TURN = 13,
    WHITE_WIN = 22,
    DUT_3_BICH = 55,
    THOI_3_BICH = 56,
    NOTIFY_ROOM_STATE_CHANGE = 71,
}
public enum GameNotifyTypesCode_BACAY
{
    END_MATCH = 10,
    SUBMIT_TURN = 11,
    //PASS_TURN = 12,
    //SET_TURN = 13,
    //WHITE_WIN=22,
    NOTIFY_USER_SET_BET = 38,
}
public enum GameNotifyTypesCode_BACAY_GA
{
    END_MATCH = 10,
    SUBMIT_TURN = 11,
    //PASS_TURN = 12,
    //SET_TURN = 13,
    //WHITE_WIN=22,
    NOTIFY_USER_SET_BET = 38,
    NOTIFY_ON_CHICKEN_BET = 39,
}
public enum GameNotifyTypesCode_LIENG
{
    END_MATCH = 10,
    SET_TURN = 13,
    USER_RAISE = 40,
    USER_CALL = 41,
    USER_FOLD = 42,
    NOTIFY_ON_CHICKEN_BET = 39,
    CALCULATE_POT = 43,
}
public enum GameNotifyTypesCode_MAUBINH
{
    END_MATCH = 10,
    USER_SUBMIT_SUITE = 32,
    NOTIFY_CANCEL_SUBMIT_SUITE = 44,
    NOTIFY_COMPARE_CHI = 45,

}
public enum GameNotifyTypesCode_XOCDIA
{
    END_MATCH = 10,
    START_MATCH = 6,
    NOTIFY_USER_SET_BET = 38,
    NOTIFY_USER_CLEAR_BET = 58,
	ROOM_STATE_CHANGED = 14,
    NOTIFY_USER_SIT_CHANGED = 63,
	NOTIFY_USER_TAKE_OWNER = 64,
	NOTIFY_USER_PASS_OWNER = 65,
	NOTIFY_OWNER_SELL_POT = 66,
	NOTIFY_USER_BUY_POT = 67,
	NOTIFY_OWNER_TAKE_ALL = 68,
	NOTIFY_RETURN_BET = 69,
	NOTIFY_USER_STAND_UP = 70,
}
public enum GameNotifyTypesCode_BAUCUA
{
    END_MATCH = 10,
    START_MATCH = 6,
    NOTIFY_USER_SET_BET = 38,
    NOTIFY_USER_CLEAR_BET = 58,
	ROOM_STATE_CHANGED = 14,
	NOTIFY_USER_SIT_CHANGED = 63,
	NOTIFY_USER_TAKE_OWNER = 64,
	NOTIFY_USER_PASS_OWNER = 65,
	NOTIFY_OWNER_SELL_POT = 66,
	NOTIFY_USER_BUY_POT = 67,
	NOTIFY_OWNER_TAKE_ALL = 68,
	NOTIFY_RETURN_BET = 69,
	NOTIFY_USER_STAND_UP = 70,
}
public enum GameNotifyTypesCode_TAIXIU
{
    UPDATE_INFO = 1,
    START_MATCH = 2,
    END_MATCH = 3,
    UNSUBSCRIBE = 4,
}
#endregion

#region GameRequestTypesCode
public enum GameRequestTypesCode_PHOM
{
    SUBMIT_TURN = 204,
    GET_CARD = 208,
    TAKE_CARD = 209,
    ATTACH_CARD_TO_SUITE = 210,
    SUBMIT_SUITE = 211,
    //FINISH_ATTACH_CARD_TO_SUITE = 212,
    //FINISH_SUBMIT_SUITE = 213
}
public enum GameRequestTypesCode_SAM
{
    PASS_TURN = 203,
    SUBMIT_TURN = 204,
    REQUEST_TURN = 206,
    CANCEL_TURN = 207
}
public enum GameRequestTypesCode_TLMN
{
    PASS_TURN = 203,
    SUBMIT_TURN = 204
}
public enum GameRequestTypesCode_TLMNDL
{
    PASS_TURN = 203,
    SUBMIT_TURN = 204,
}
public enum GameRequestTypesCode_BACAY
{
    SUBMIT_TURN = 204,
    USER_SET_BET = 214,
}
public enum GameRequestTypesCode_BACAY_GA
{
    //PASS_TURN=203,
    SUBMIT_TURN = 204,
    USER_SET_BET = 214,
}
public enum GameRequestTypesCode_LIENG
{
    USER_RAISE = 215, // Tố
    USER_CALL = 216, // Theo
    USER_FOLD = 217, // Bỏ
    NOTIFY_ON_CHICKEN_BET = 39,
    CALCULATE_POT = 43,
}
public enum GameRequestTypesCode_MAUBINH
{
    SUBMIT_SUITE = 211,
    CANCEL_SUBMIT_SUITE = 218,
}
public enum GameRequestTypesCode_XOCDIA
{
    USER_SET_BET = 214,
    USER_CLEAR_BET = 219,
	USER_SIT_DOWN = 224,  
	USER_TAKE_OWNER = 225,
	USER_PASS_OWNER = 226,
	OWNER_SELL_POT = 227,
	USER_BUY_POT = 228,
	OWNER_TAKE_ALL = 229,
	USER_STAND_UP = 230,
}
public enum GameRequestTypesCode_BAUCUA
{
    USER_SET_BET = 214,
	USER_CLEAR_BET = 219,
	USER_SIT_DOWN = 224,  
	USER_TAKE_OWNER = 225,
	USER_PASS_OWNER = 226,
	OWNER_SELL_POT = 227,
	USER_BUY_POT = 228,
	OWNER_TAKE_ALL = 229,
	USER_STAND_UP = 230,
}
public enum GameRequestTypesCode_SLOT
{
    START_MATCH = 202,
    GET_ROOM_USERS = 220,
    GET_JACKPOT_HISTORY = 221,
    GET_USER_HISTORY = 222,
    GET_TOP_USERS = 223,
}

public enum GameRequestTypesCode_XENG
{
	START_MATCH = 202,
	RECEIVE_MONEY = 231,
	BET_TAI_XIU = 232,
}
#endregion

#region GameWarpResponseResultCode
public enum GameWarpResponseResultCode_PHOM
{
    TAKE_CARD_FIRST = 20,
    CANNOT_TAKE_CARD = 209
}
public enum GameWarpResponseResultCode_LIENG
{
    INVALID_MAX_BET = 22
}
public enum GameWarpResponseResultCode_XOCDIA
{
    INVALID_CHIP = 15,
    ROOM_STARTED = 12,
}
#endregion