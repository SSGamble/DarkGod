/****************************************************
	文件：GuideSys.cs
	作者：CaptainYun
	日期：2019/05/24 19:35   	
	功能：引导业务系统
*****************************************************/
using PEProtocol;

public class GuideSys {
    private static GuideSys instance = null;
    public static GuideSys Instance {
        get {
            if (instance == null) {
                instance = new GuideSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;

    public void Init() {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("GuideSys Init Done.");
    }

    public void ReqGuide(MsgPack pack) {
        ReqGuide data = pack.msg.reqGuide;

        GameMsg msg = new GameMsg {
            cmd = (int)CMD.RspGuide
        };

        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        GuideCfg gc = cfgSvc.GetGuideData(data.guideid);

        //更新引导 ID
        if (pd.guideid == data.guideid) {
            pd.guideid += 1; // 如果是连环任务，可以在后面加字段然后回传回去

            //更新玩家数据
            pd.coin += gc.coin;
            CalcExp(pd, gc.exp);

            // 更新到数据库
            if (!cacheSvc.UpdatePlayerData(pd.id, pd)) {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else {
                msg.rspGuide = new RspGuide {
                    guideid = pd.guideid,
                    coin = pd.coin,
                    lv = pd.lv,
                    exp = pd.exp
                };
            }
        }
        else {
            msg.err = (int)ErrorCode.ServerDataError;
        }
        pack.session.SendMsg(msg);
    }

    /// <summary>
    /// 计算经验值，因为经验奖励可能会影响到级别
    /// </summary>
    private void CalcExp(PlayerData pd, int addExp) {
        int curtLv = pd.lv;
        int curtExp = pd.exp;
        int addRestExp = addExp;
        while (true) {
            // 升级所需的经验
            int upNeedExp = PECommon.GetExpUpValByLv(curtLv) - curtExp;
            // 升级
            if (addRestExp >= upNeedExp) { 
                curtLv += 1;
                curtExp = 0;
                addRestExp -= upNeedExp;
            }
            // 不够升级了
            else {
                pd.lv = curtLv;
                pd.exp = curtExp + addRestExp;
                break;
            }
        }
    }
}