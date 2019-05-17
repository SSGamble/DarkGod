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
            if (instance ==null) {
                instance = new ServerRoot();
            }
            return instance;
        }
    }

    public void Init() {
        // 数据层
        // 服务层
        NetSvc.Instance.Init();
        // 业务系统层
        LoginSys.Instance.Init();
    }
}
