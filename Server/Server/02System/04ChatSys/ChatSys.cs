/****************************************************
	文件：ChatSys.cs
	作者：CaptainYun
	日期：2019/05/26 16:12   	
	功能：聊天业务系统
*****************************************************/
using System.Collections.Generic;
using PEProtocol;

public class ChatSys {
    private static ChatSys instance = null;
    public static ChatSys Instance {
        get {
            if (instance == null) {
                instance = new ChatSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;

    public void Init() {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("ChatSys Init Done.");
    }

    /// <summary>
    /// 接收客户端的请求
    /// </summary>
    /// <param name="pack"></param>
    public void SndChat(MsgPack pack) {
        SndChat data = pack.msg.sndChat;
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        TaskSys.Instance.CalcTaskPrgs(pd, 6); // 任务进度数据更新
        GameMsg msg = new GameMsg {
            cmd = (int)CMD.PshChat,
            pshChat = new PshChat {
                name = pd.name,
                chat = data.chat // 聊天数据
            }
        };

        // 广播给所有在线客户端
        List<ServerSession> lst = cacheSvc.GetOnlineServerSessions();
        byte[] bytes = PENet.PETool.PackNetMsg(msg);
        for (int i = 0; i < lst.Count; i++) {
            lst[i].SendMsg(bytes); // 发送二进制数据，而不是直接发送 msg，可以节省 序列化 操作
        }
    }
}