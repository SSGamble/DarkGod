/****************************************************
	文件：ServerRoot.cs
	作者：CaptainYun
	日期：2019/05/14 20:00   	
	功能：服务器初始化
*****************************************************/

public class ServerRoot {

    private static ServerRoot instance = null;
    public static ServerRoot Instance {
        get {
            if (instance == null) {
                instance = new ServerRoot();
            }
            return instance;
        }
    }

    /// <summary>
    /// 初始化各个层级
    /// </summary>
    public void Init() {
        // 数据层
        DBMgr.Instance.Init();

        // 服务层
        CfgSvc.Instance.Init(); // 配置文件的读取
        CacheSvc.Instance.Init(); // 缓存
        NetSvc.Instance.Init(); // 网络
        TimerSvc.Instance.Init(); // 定时

        // 业务系统层
        LoginSys.Instance.Init(); // 登录
        GuideSys.Instance.Init(); // 任务引导
        StrongSys.Instance.Init(); //强化
        ChatSys.Instance.Init(); // 聊天
        BuySys.Instance.Init(); // 交易

        //TimerSvc.Instance.AddTimeTask((int tid) => {
        //    PECommon.Log("XXX");
        //}, 1000, PETimeUnit.Millisecond, 0);
    }

    /// <summary>
    /// 驱动消息的检测
    /// </summary>
    public void Update() {
        NetSvc.Instance.Update(); // 网络消息
        TimerSvc.Instance.Update(); // 定时任务
    }

    private int SessionID = 0;
    public int GetSessionID() {
        if (SessionID == int.MaxValue) {
            SessionID = 0;
        }
        return SessionID += 1;
    }
}
