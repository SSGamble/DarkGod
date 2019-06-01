/****************************************************
	文件：PowerSys.cs
	作者：CaptainYun
	日期：2019/05/31 10:05   	
	功能：体力恢复系统
*****************************************************/

using System.Collections.Generic;
using PEProtocol;

public class PowerSys {
    private static PowerSys instance = null;
    public static PowerSys Instance {
        get {
            if (instance == null) {
                instance = new PowerSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    private TimerSvc timerSvc = null;

    public void Init() {
        cacheSvc = CacheSvc.Instance;
        timerSvc = TimerSvc.Instance;
        // 每隔 5 分钟增加一次体力
        TimerSvc.Instance.AddTimeTask(CalcPowerAdd, PECommon.PowerAddSpace, PETimeUnit.Minute, 0);
        PECommon.Log("PowerSys Init Done.");
    }

    /// <summary>
    /// 增加体力
    /// </summary>
    private void CalcPowerAdd(int tid) {
        // 计算体力增长
        PECommon.Log("所有在线玩家的体力增长计算....");
        GameMsg msg = new GameMsg {
            cmd = (int)CMD.PshPower
        };
        msg.pshPower = new PshPower();

        // 所有在线玩家获得实时的体力增长，推送数据
        Dictionary<ServerSession, PlayerData> onlineDic = cacheSvc.GetOnlineCache();
        foreach (var item in onlineDic) {
            PlayerData pd = item.Value;
            ServerSession session = item.Key;
            // 体力增长上限
            int powerMax = PECommon.GetPowerLimit(pd.lv);
            if (pd.power >= powerMax) { 
                continue;
            }
            else {
                pd.power += PECommon.PowerAddCount;
                pd.time = timerSvc.GetNowTime(); // 更新玩家在线时间
                if (pd.power > powerMax) { // 上限
                    pd.power = powerMax;
                }
            }
            // 将更新后的数据，更新到缓存
            if (!cacheSvc.UpdatePlayerData(pd.id, pd)) {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else {
                msg.pshPower.power = pd.power;
                session.SendMsg(msg);
            }
        }
    }
}