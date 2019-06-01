/****************************************************
	文件：DungeonSys.cs
	作者：CaptainYun
	日期：2019/06/01 21:56   	
	功能：副本战斗业务
*****************************************************/

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
}
