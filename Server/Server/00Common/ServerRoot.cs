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

        // 业务系统层
        LoginSys.Instance.Init(); // 登录
        GuideSys.Instance.Init();
    }

    /// <summary>
    /// 驱动网络消息的检测
    /// </summary>
    public void Update() {
        NetSvc.Instance.Update();
    }

    private int SessionID = 0;
    public int GetSessionID() {
        if (SessionID == int.MaxValue) {
            SessionID = 0;
        }
        return SessionID += 1;
    }
}
