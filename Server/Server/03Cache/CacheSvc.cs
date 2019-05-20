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

    // 当前已在线的账号
    private Dictionary<string, ServerSession> onLineAcctDic = new Dictionary<string, ServerSession>();
    // 连接客户端和玩家数据
    private Dictionary<ServerSession, PlayerData> onLineSessionDic = new Dictionary<ServerSession, PlayerData>();

    /// <summary>
    /// 判断当前账号是否已在线
    /// </summary>
    public bool IsAcctOnLine(string acct) {
        return onLineAcctDic.ContainsKey(acct);
    }

    /// <summary>
    /// 根据账号密码范湖对应的账号数据，密码错误，返回 null，账号不存在，则默认创建新的账号
    /// </summary>
    public PlayerData GetPlayerData(string acct, string pwd) {
        // 从数据库获取
        return dbMgr.QueryPlayerData(acct,pwd);
    }


    /// <summary>
    /// 玩家登录后添加信息到缓存
    /// </summary>
    public void AcctOnLine(string acct,ServerSession session,PlayerData playerData) {
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
    /// <param name="session"></param>
    /// <returns></returns>
    public PlayerData GetPlayerDataBySession(ServerSession session) {
        if (onLineSessionDic.TryGetValue(session,out PlayerData playerData)) {
            return playerData;
        }
        else {
            return null;
        }
    }

    /// <summary>
    /// 更新玩家数据
    /// </summary>
    /// <param name="id"></param>
    /// <param name="playerData"></param>
    /// <returns></returns>
    public bool UpdatePlayerData(int id,PlayerData playerData) {
        return dbMgr.UpdataPlayerData(id,playerData);
    }
}
