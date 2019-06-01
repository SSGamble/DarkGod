/****************************************************
	文件：BuySys.cs
	作者：CaptainYun
	日期：2019/05/26 21:07   	
	功能：交易购买系统
*****************************************************/

using PEProtocol;

public class BuySys {
    private static BuySys instance = null;
    public static BuySys Instance {
        get {
            if (instance == null) {
                instance = new BuySys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;

    public void Init() {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("BuySys Init Done.");
    }

    public void ReqBuy(MsgPack pack) {
        ReqBuy data = pack.msg.reqBuy;
        GameMsg msg = new GameMsg {
            cmd = (int)CMD.RspBuy
        };
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        if (pd.diamond < data.cost) {
            msg.err = (int)ErrorCode.LackDiamond;
        }
        else {
            pd.diamond -= data.cost;
            PshTaskPrgs pshTaskPrgs = null;
            switch (data.type) {
                case 0:
                    pd.power += 100;
                    //TaskSys.Instance.GetTaskPrgs(pd, 4); // 不让他发送了，网络并包流量优化
                    pshTaskPrgs = TaskSys.Instance.GetTaskPrgs(pd, 4); // 任务进度数据更新
                    break;
                case 1:
                    pd.coin += 1000;
                    pshTaskPrgs = TaskSys.Instance.GetTaskPrgs(pd, 5); // 任务进度数据更新
                    break;
            }
            if (!cacheSvc.UpdatePlayerData(pd.id, pd)) {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else {
                RspBuy rspBuy = new RspBuy {
                    type = data.type,
                    dimond = pd.diamond,
                    coin = pd.coin,
                    power = pd.power
                };
                msg.rspBuy = rspBuy;
                // 并包处理
                msg.pshTaskPrgs = pshTaskPrgs;
            }
        }
        pack.session.SendMsg(msg); // 发送的就是合并后的两个包
    }
}