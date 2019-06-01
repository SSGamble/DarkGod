/****************************************************
	文件：CacheSvc.cs
	作者：CaptainYun
	日期：2019/05/19 13:36   	
	功能：缓存层
*****************************************************/
using System;
using System.Collections.Generic;
using PEProtocol;

public class CacheSvc {
    // 单例
    private static CacheSvc instance = null;
    public static CacheSvc Instance {
        get {
            if (instance == null) {
                instance = new CacheSvc();
            }
            return instance;
        }
    }

    // 数据库管理器
    private DBMgr dbMgr;

    public void Init() {
        dbMgr = DBMgr.Instance;
        PECommon.Log("CacheSvc Init Done.");
    }

    /// <summary>
    /// 当前已在线的账号
    /// </summary>
    private Dictionary<string, ServerSession> onLineAcctDic = new Dictionary<string, ServerSession>();
    /// <summary>
    /// 连接客户端和玩家数据
    /// </summary>
    private Dictionary<ServerSession, PlayerData> onLineSessionDic = new Dictionary<ServerSession, PlayerData>();

    /// <summary>
    /// 判断当前账号是否已在线
    /// </summary>
    public bool IsAcctOnLine(string acct) {
        return onLineAcctDic.ContainsKey(acct);
    }

    /// <summary>
    /// 获取所有在线的客户端，用于聊天广播
    /// </summary>
    /// <returns></returns>
    public List<ServerSession> GetOnlineServerSessions() {
        List<ServerSession> lst = new List<ServerSession>();
        foreach (var item in onLineSessionDic) {
            lst.Add(item.Key);
        }
        return lst;
    }

    /// <summary>
    /// 根据账号密码范湖对应的账号数据，密码错误，返回 null，账号不存在，则默认创建新的账号
    /// </summary>
    public PlayerData GetPlayerData(string acct, string pwd) {
        // 从数据库获取
        return dbMgr.QueryPlayerData(acct, pwd);
    }

    /// <summary>
    /// 玩家登录后添加信息到缓存
    /// </summary>
    public void AcctOnLine(string acct, ServerSession session, PlayerData playerData) {
        onLineAcctDic.Add(acct, session);
        onLineSessionDic.Add(session, playerData);
    }

    /// <summary>
    /// 名字是否已经存在
    /// </summary>
    public bool IsNameExist(string name) {
        return dbMgr.QueryNameData(name);
    }

    /// <summary>
    /// 得到缓存中的玩家数据
    /// </summary>
    public PlayerData GetPlayerDataBySession(ServerSession session) {
        if (onLineSessionDic.TryGetValue(session, out PlayerData playerData)) {
            return playerData;
        }
        else {
            return null;
        }
    }

    /// <summary>
    /// 更新玩家数据
    /// </summary>
    public bool UpdatePlayerData(int id, PlayerData playerData) {
        return dbMgr.UpdataPlayerData(id, playerData);
    }

    /// <summary>
    /// 用户下线
    /// </summary>
    public void AcctOffLine(ServerSession session) {
        // 清除缓存
        foreach (var item in onLineAcctDic) {
            if (item.Value == session) {
                onLineAcctDic.Remove(item.Key);
                break;
            }
        }
        bool succ = onLineSessionDic.Remove(session);
        PECommon.Log("玩家下线：" + session.sessionID + " " + succ);
    }

    /// <summary>
    /// 在线玩家
    /// </summary>
    public Dictionary<ServerSession, PlayerData> GetOnlineCache() {
        return onLineSessionDic;
    }

    /// <summary>
    /// 根据当前 id 获取连接的客户端
    /// </summary>
    /// <param name="id">玩家 id</param>
    public ServerSession GetOnlineServersession(int id) {
        ServerSession session = null;
        foreach (var item in onLineSessionDic) {
            if (item.Value.id == id) {
                session = item.Key;
                break;
            }
        }
        return session;
    }
}
