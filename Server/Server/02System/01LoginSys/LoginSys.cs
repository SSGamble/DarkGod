/****************************************************
	文件：LoginSys.cs
	作者：CaptainYun
	日期：2019/05/14 20:07   	
	功能：登录业务系统
*****************************************************/

using System;
using PEProtocol;

public class LoginSys {

    private static LoginSys instance = null;
    public static LoginSys Instance {
        get {
            if (instance == null) {
                instance = new LoginSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private TimerSvc timerSvc = null;

    public void Init() {
        cacheSvc = CacheSvc.Instance;
        timerSvc = TimerSvc.Instance;
        PECommon.Log("LoginSys Init Done.");
    }


    /// <summary>
    /// 响应登录的请求
    /// </summary>
    /// <param name="pack">消息包</param>
    public void ReqLogin(MsgPack pack) {
        ReqLogin data = pack.msg.reqLogin;
        // 回应客户端
        GameMsg msg = new GameMsg {
            cmd = (int)CMD.RspLogin // 操作码
        };

        // 判断是否在线，并做相应的处理
        if (cacheSvc.IsAcctOnLine(data.acct)) { // 已在线，返回错误信息
            msg.err = (int)ErrorCode.AcctIsOnLine;
        }
        else { // 不在线，尝试登录
            PlayerData pd = cacheSvc.GetPlayerData(data.acct, data.pwd); // 获取玩家数据
            if (pd == null) { // 账号和密码不匹配
                msg.err = (int)ErrorCode.WrongPwd;
            }
            else { // 登录成功
                // 计算离线体力增长
                int power = pd.power; // 当前体力
                long now = timerSvc.GetNowTime();
                long milliseconds = now - pd.time; // 距离上一次下线的时间间隔
                int addPower = (int)(milliseconds / (1000 * 60 * PECommon.PowerAddSpace)) * PECommon.PowerAddCount; // 需要增加的体力值
                if (addPower > 0) {
                    int powerMax = PECommon.GetPowerLimit(pd.lv);
                    if (pd.power < powerMax) {
                        pd.power += addPower;
                        if (pd.power > powerMax) {
                            pd.power = powerMax;
                        }
                    }
                }

                if (power != pd.power) {
                    cacheSvc.UpdatePlayerData(pd.id, pd);
                }
                // 响应客户端的数据
                msg.rspLogin = new RspLogin {
                    playerData = pd // 返回玩家数据
                };
                // 缓存账号数据
                cacheSvc.AcctOnLine(data.acct, pack.session, pd);
            }
        }
        // 回应客户端
        pack.session.SendMsg(msg);
    }

    /// <summary>
    /// 响应改名请求
    /// </summary>
    /// <param name="pack"></param>
    public void ReqRename(MsgPack pack) {
        ReqRename data = pack.msg.reqRename;
        GameMsg msg = new GameMsg {
            cmd = (int)CMD.RspRename
        };

        // 名字是否存在
        if (cacheSvc.IsNameExist(data.name)) { // 存在，返回错误码
            msg.err = (int)ErrorCode.NameIsExist;
        }
        else { // 不存在，更新缓存和数据库，再返回给客户端
            PlayerData playerData = cacheSvc.GetPlayerDataBySession(pack.session);
            playerData.name = data.name;

            // 更新失败
            if (!cacheSvc.UpdatePlayerData(playerData.id, playerData)) {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            // 更新成功
            else {
                // 直接把更新后的名字返回给客户端
                msg.rspRename = new RspRename {
                    name = data.name
                };
            }
        }
        // 发送回应给客户端
        pack.session.SendMsg(msg);
    }

    /// <summary>
    /// 玩家线下
    /// </summary>
    public void ClearOfflineData(ServerSession session) {
        // 更新下线时间
        PlayerData pd = cacheSvc.GetPlayerDataBySession(session);
        if (pd != null) {
            pd.time = timerSvc.GetNowTime();
            if (!cacheSvc.UpdatePlayerData(pd.id, pd)) {
                PECommon.Log("更新下线时间错误..", LogType.Error);
            }
            cacheSvc.AcctOffLine(session); // 清除缓存
        }
    }
}
