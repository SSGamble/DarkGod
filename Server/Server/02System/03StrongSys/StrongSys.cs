/****************************************************
	文件：StrongSys.cs
	作者：CaptainYun
	日期：2019/05/25 22:09   	
	功能：强化升级系统
*****************************************************/
using PEProtocol;

public class StrongSys {

    private static StrongSys instance = null;
    public static StrongSys Instance {
        get {
            if (instance == null) {
                instance = new StrongSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;

    public void Init() {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("StrongSys Init Done.");
    }

    public void ReqStrong(MsgPack pack) {
        ReqStrong data = pack.msg.reqStrong;

        GameMsg msg = new GameMsg {
            cmd = (int)CMD.RspStrong
        };

        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        int curtStartLv = pd.strongArr[data.pos];
        StrongCfg nextSd = CfgSvc.Instance.GetStrongCfg(data.pos, curtStartLv + 1);

        // 强化条件判断，不满足，返回错误码
        if (pd.lv < nextSd.minlv) {
            msg.err = (int)ErrorCode.LackLevel;
        }
        else if (pd.coin < nextSd.coin) {
            msg.err = (int)ErrorCode.LackCoin;
        }
        else if (pd.crystal < nextSd.crystal) {
            msg.err = (int)ErrorCode.LackCrystal;
        }
        // 符合条件
        else {
            // 资源扣除
            pd.coin -= nextSd.coin;
            pd.crystal -= nextSd.crystal;
            pd.strongArr[data.pos] += 1;
            // 增加属性
            pd.hp += nextSd.addhp;
            pd.ad += nextSd.addhurt;
            pd.ap += nextSd.addhurt;
            pd.addef += nextSd.adddef;
            pd.apdef += nextSd.adddef;
        }
        // 更新数据库
        if (!cacheSvc.UpdatePlayerData(pd.id, pd)) {
            msg.err = (int)ErrorCode.UpdateDBError;
        }
        else {
            // 回应的强化消息
            msg.rspStrong = new RspStrong {
                coin = pd.coin,
                crystal = pd.crystal,
                hp = pd.hp,
                ad = pd.ad,
                ap = pd.ap,
                addef = pd.addef,
                apdef = pd.apdef,
                strongArr = pd.strongArr
            };
        }
        // 返回消息给客户端
        pack.session.SendMsg(msg);
    }
}
