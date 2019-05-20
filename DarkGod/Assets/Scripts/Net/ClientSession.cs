/****************************************************
    文件：ClientSession.cs
	作者：CaptainYun
    日期：2019/5/14 20:48:27
	功能：会话连接
*****************************************************/

using PENet;
using PEProtocol;
using UnityEngine;

public class ClientSession : PESession<GameMsg> {

    protected override void OnConnected() {
        PETool.LogMsg("连接服务器成功");
    }

    protected override void OnReciveMsg(GameMsg msg) {
        PETool.LogMsg("接收到服务器的数据包，操作码为：" + ((CMD)msg.cmd).ToString());
        NetSvc.Instance.AddNetPkg(msg); // 将接收到的数据包加到消息队列里
    }

    protected override void OnDisConnected() {
        PETool.LogMsg("断开服务器连接");
    }
}