/****************************************************
	文件：ServerSession.cs
	作者：CaptainYun
	日期：2019/05/14 20:27   	
	功能：网络会话连接
*****************************************************/
using PENet;
using PEProtocol;

public class ServerSession :PESession<GameMsg>{

    protected override void OnConnected() {
        PECommon.Log("Client Connect");
        SendMsg(new GameMsg {
            text = "Welcome to connect"
        });
    }

    protected override void OnReciveMsg(GameMsg msg) {
        PECommon.Log("Client Req: " + msg.text);
        SendMsg(new GameMsg {
            text = "SrvRsp: " + msg.text
        });
    }

    protected override void OnDisConnected() {
        PECommon.Log("Client DisConnect");
    }
}
