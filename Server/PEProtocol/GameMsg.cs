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
        RspRename = 104
    }

    /// <summary>
    /// 错误码
    /// </summary>
    public enum ErrorCode {
        None = 0,
        AcctIsOnLine = 101, // 账号已在线
        WrongPwd=102, // 密码错误
        NameIsExist = 103, // 用户名已经存在
        UpdateDBError = 104, // 更新数据库出错
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
        public int hp;
        public int ad;
        public int ap;
        public int addef; // ad 防御
        public int apdef; // /ap 防御
        public int dodge;//闪避概率
        public int pierce;//穿透比率
        public int critical;//暴击概率
        public int guideid;//当前进行的引导id
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

}
