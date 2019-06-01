/****************************************************
	文件：GameMsg.cs
	作者：CaptainYun
	日期：2019/05/19 10:42   	
	功能：游戏中需要传递的信息
*****************************************************/
using System;
using PENet;

namespace PEProtocol {

    /// <summary>
    /// 服务器连接信息
    /// </summary>
    public class SrvCfg {
        public const string srvIP = "127.0.0.1";
        public const int srvPort = 17666;
    }

    /// <summary>
    /// 操作码
    /// </summary>
    public enum CMD {
        None = 0,

        // 登录
        ReqLogin = 101, // 请求
        RspLogin = 102, // 回应
        // 改名
        ReqRename = 103,
        RspRename = 104,
        // 主城相关 200
        ReqGuide = 200,
        RspGuide = 201,
        // 强化
        ReqStrong = 203,
        RspStrong = 204,
        // 聊天
        SndChat = 205,
        PshChat = 206,
        // 交易
        ReqBuy = 207,
        RspBuy = 208,
        // 体力
        PshPower = 209,
        // 任务奖励
        ReqTakeTaskReward = 210,
        RspTakeTaskReward = 211,
        // 任务进度
        PshTaskPrgs = 212,
        // 副本
        ReqDungeonFight = 301,
        RspDungeonFight = 302,
    }

    /// <summary>
    /// 错误码
    /// </summary>
    public enum ErrorCode {
        None = 0,
        AcctIsOnLine = 101, // 账号已在线
        WrongPwd = 102, // 密码错误
        NameIsExist = 103, // 用户名已经存在
        UpdateDBError = 104, // 更新数据库出错
        ServerDataError = 105, // 数据异常，客户端和服务器端不一致
        LackLevel, // 等级不够
        LackCoin, // 金币不够
        LackCrystal, // 水晶不够
        LackDiamond, // 钻石不够
        ClientDataError, // 客户端数据异常
        LackPower, // 体力不足
    }

    /// <summary>
    /// 游戏中需要传递的信息
    /// </summary>
    [Serializable]
    public class GameMsg : PEMsg {
        public ReqLogin reqLogin;
        public RspLogin rspLogin;

        public ReqRename reqRename;
        public RspRename rspRename;

        public ReqGuide reqGuide;
        public RspGuide rspGuide;

        public ReqStrong reqStrong;
        public RspStrong rspStrong;

        public SndChat sndChat;
        public PshChat pshChat;

        public ReqBuy reqBuy;
        public RspBuy rspBuy;

        public PshPower pshPower;

        public ReqTakeTaskReward reqTakeTaskReward;
        public RspTakeTaskReward rspTakeTaskReward;

        public PshTaskPrgs pshTaskPrgs;

        public ReqDungeonFight reqDungeonFight;
        public RspDungeonFight rspDungeonFight;
    }

    /// <summary>
    /// 玩家信息
    /// </summary>
    [Serializable]
    public class PlayerData {
        public int id;
        public string name;
        public int lv; // 级别
        public int exp; // 经验
        public int power; // 体力
        public int coin; // 金币
        public int diamond; // 钻石
        public int crystal;
        public int hp;
        public int ad;
        public int ap;
        public int addef; // ad 防御
        public int apdef; // ap 防御
        public int dodge; // 闪避概率
        public int pierce; // 穿透比率
        public int critical; // 暴击概率
        public int guideid; // 当前进行的引导 id
        public int[] strongArr; // 索引号：第一个位置，值：星级
        public long time; // 玩家最后一次在线的时间
        public string[] taskArr;
        public int dungeon; // 打到了哪一关
    }

    #region 登录相关
    /// <summary>
    /// 客户端请求登录
    /// </summary>
    [Serializable]
    public class ReqLogin {
        public string acct;
        public string pwd;
    }
    /// <summary>
    /// 登录的回应
    /// </summary>
    [Serializable]
    public class RspLogin {
        public PlayerData playerData;
    }

    /// <summary>
    /// 客户端请求改名
    /// </summary>
    [Serializable]
    public class ReqRename {
        public string name;
    }
    /// <summary>
    /// 改名的响应
    /// </summary>
    [Serializable]
    public class RspRename {
        public string name;
    }

    #endregion

    #region 引导相关
    [Serializable]
    public class ReqGuide {
        public int guideid; // 当前已完成的任务 ID
    }

    [Serializable]
    public class RspGuide {
        public int guideid;
        public int coin;
        public int lv; // 经验值奖励可能会导致等级的变化
        public int exp;
    }

    #endregion

    #region 强化相关
    [Serializable]
    public class ReqStrong {
        public int pos;
    }
    [Serializable]
    public class RspStrong {
        public int coin;
        public int crystal;
        public int hp;
        public int ad;
        public int ap;
        public int addef;
        public int apdef;
        public int[] strongArr;
    }
    #endregion

    #region 聊天相关
    /// <summary>
    /// 客户端发送
    /// </summary>
    [Serializable]
    public class SndChat {
        public string chat;
    }

    /// <summary>
    /// 服务器广播
    /// </summary>
    [Serializable]
    public class PshChat {
        public string name;
        public string chat;
    }
    #endregion

    #region 资源交易相关
    [Serializable]
    public class ReqBuy {
        public int type;
        public int cost;
    }

    [Serializable]
    public class RspBuy {
        public int type;
        public int dimond;
        public int coin;
        public int power;
    }
    #endregion

    #region 体力系统
    [Serializable]
    public class PshPower {
        public int power;
    }
    #endregion

    #region 任务奖励相关
    [Serializable]
    public class ReqTakeTaskReward {
        public int rid;
    }

    [Serializable]
    public class RspTakeTaskReward {
        public int coin;
        public int lv; // 获取经验奖励后，可能会有等级的变化
        public int exp;
        public string[] taskArr;
    }

    /// <summary>
    /// 推送任务进度
    /// </summary>
    [Serializable]
    public class PshTaskPrgs {
        public string[] taskArr;
    }
    #endregion

    #region 副本战斗相关
    [Serializable]
    public class ReqDungeonFight {
        public int dungeonId;
    }
    [Serializable]
    public class RspDungeonFight {
        public int dungeonId;
        public int power;
    }
    #endregion
}
