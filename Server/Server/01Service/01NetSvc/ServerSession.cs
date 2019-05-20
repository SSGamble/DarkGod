﻿/****************************************************
	文件：ServerSession.cs
	作者：CaptainYun
	日期：2019/05/14 20:27   	
	功能：网络会话连接
*****************************************************/
using PENet;
using PEProtocol;

public class ServerSession :PESession<GameMsg>{

    /// <summary>
    /// 客户端连接
    /// </summary>
    protected override void OnConnected() {
        PECommon.Log("客户端连接成功");
    }

    /// <summary>
    /// 接收到客户端发来的信息
    /// </summary>
    protected override void OnReciveMsg(GameMsg msg) {
        PECommon.Log("接收到客户端发来的数据，操作码为： " + ((CMD)msg.cmd).ToString());
        NetSvc.Instance.AddMsgQue(new MsgPack(this, msg)); // 把消息交给 网络服务 解决，需要把 session 也传过去，因为处理完业务后需要给客户端响应
    }

    /// <summary>
    /// 客户端断开连接
    /// </summary>
    protected override void OnDisConnected() {
        PECommon.Log("客户端断开连接");
    }
}
