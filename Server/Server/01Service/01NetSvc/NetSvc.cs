/****************************************************
	文件：NetSvc.cs
	作者：CaptainYun
	日期：2019/05/14 20:06   	
	功能：网络服务
*****************************************************/
using PENet;
using PEProtocol;

public class NetSvc {
    // 单例
    private static NetSvc instance = null;
    public static NetSvc Instance {
        get {
            if (instance == null) {
                instance = new NetSvc();
            }
            return instance;
        }
    }

    public void Init() {
        //创建 Socket 服务器
        PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
        server.StartAsServer(SrvCfg.srvIP, SrvCfg.srvPort);

        PECommon.Log("NetSvc Init Done.");
    }
}
