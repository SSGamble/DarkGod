/****************************************************
	文件：NetSvc.cs
	作者：CaptainYun
	日期：2019/05/14 20:06   	
	功能：网络服务
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using PENet;
using PEProtocol;

/// <summary>
/// 消息包，包含了 ServerSession 和 GameMsg 信息
/// </summary>
public class MsgPack {
    public ServerSession session;
    public GameMsg msg;
    public MsgPack(ServerSession session,GameMsg msg) {
        this.session = session;
        this.msg = msg;
    }
}

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

    // 客户端发来的消息包队列
    private Queue<MsgPack> msgPackQue = new Queue<MsgPack>();

    // 锁
    public static readonly string obj = "lock";

    /// <summary>
    /// 将客户端发来的消息加入到消息队列里面
    /// </summary>
    /// <param name="pack">消息包</param>
    public void AddMsgQue(MsgPack pack) {
        lock (obj) { // 因为是异步多线程的网络库，所以需要加锁
            msgPackQue.Enqueue(pack);
        }
    }

    /// <summary>
    /// 从消息队列里取出数据进行处理
    /// </summary>
    public void Update() {
        if (msgPackQue.Count>0) { // 当前队列里是否有数据
            //PECommon.Log("当前队列里要需要处理的数据包数量：" + msgPackQue.Count);
            lock (obj) {
                MsgPack pack = msgPackQue.Dequeue(); // 取出一条数据
                HandOutMsg(pack); // 处理数据
            }
        }
    }

    /// <summary>
    /// 处理数据，分发
    /// </summary>
    /// <param name="msg"></param>
    private void HandOutMsg(MsgPack pack) {
        // 将消息分发到各个业务系统里面
        switch ((CMD)pack.msg.cmd) {
            // 登录响应，交给登录系统  
            case CMD.ReqLogin:
                LoginSys.Instance.ReqLogin(pack);
                break;
            // 取名响应
            case CMD.ReqRename:
                LoginSys.Instance.ReqRename(pack);
                break;
            default:
                break;
        }
    }

}
