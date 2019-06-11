/****************************************************
	文件：DungeonSys.cs
	作者：CaptainYun
	日期：2019/06/01 21:56   	
	功能：副本战斗业务
*****************************************************/

using System;
using PEProtocol;

public class DungeonSys {
    private static DungeonSys instance = null;
    public static DungeonSys Instance {
        get {
            if (instance == null) {
                instance = new DungeonSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;

    public void Init() {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("DungeonSys Init Done.");
    }

    public void ReqDungeonFight(MsgPack pack) {
        ReqDungeonFight data = pack.msg.reqDungeonFight;

        GameMsg msg = new GameMsg {
            cmd = (int)CMD.RspDungeonFight
        };

        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        int power = cfgSvc.GetMapCfg(data.dungeonId).power;

        if (pd.dungeon < data.dungeonId) { // 关卡进度是否合法
            msg.err = (int)ErrorCode.ClientDataError;
        }
        else if (pd.power < power) {
            msg.err = (int)ErrorCode.LackPower;
        }
        else {
            pd.power -= power;
            if (cacheSvc.UpdatePlayerData(pd.id, pd)) {
                RspDungeonFight rspDungeonFight = new RspDungeonFight {
                    dungeonId = data.dungeonId,
                    power = pd.power
                };
                msg.rspDungeonFight = rspDungeonFight;
            }
            else {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
        }
        pack.session.SendMsg(msg);
    }

    public void ReqDungeonFightEnd(MsgPack pack) {
        ReqDungeonFightEnd data = pack.msg.reqDungeonFightEnd;
        GameMsg msg = new GameMsg {
            cmd = (int)CMD.RspDungeonFightEnd
        };
        // 校验战斗是否合法
        if (data.win) {
            if (data.costTime > 0 && data.restHp > 0) {
                // 根据 副本 ID 获取相应奖励
                MapCfg rd = cfgSvc.GetMapCfg(data.dungeonId);
                PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
                // 任务进度数据更新
                TaskSys.Instance.CalcTaskPrgs(pd, 2);
                pd.coin += rd.coin;
                pd.crystal += rd.crystal;
                PECommon.CalcExp(pd, rd.exp);
                if (pd.dungeon == data.dungeonId) {
                    pd.dungeon += 1;
                }
                if (!cacheSvc.UpdatePlayerData(pd.id, pd)) {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else {
                    RspDungeonFightEnd rspDungeonFight = new RspDungeonFightEnd {
                        win = data.win,
                        dungeonId = data.dungeonId,
                        restHp = data.restHp,
                        costTime = data.costTime,
                        coin = pd.coin,
                        lv = pd.lv,
                        exp = pd.exp,
                        crystal = pd.crystal,
                        dungeon = pd.dungeon
                    };
                    msg.rspDungeonFightEnd = rspDungeonFight;
                }
            }
        }
        else {
            msg.err = (int)ErrorCode.ClientDataError;
        }
        pack.session.SendMsg(msg);
    }
}
